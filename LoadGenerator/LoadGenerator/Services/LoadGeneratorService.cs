using ConsoleTables;
using Microsoft.Extensions.Configuration;
using LoadGenerator.Interfaces;
using LoadGenerator.Models.Internal;
using LoadGenerator.Models.Requests;
using LoadGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LoadGenerator.Services
{
    public class LoadGeneratorService : ILoadGeneratorService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public int TargetRps { get; set; }

        public LoadGeneratorService(IConfiguration config, IHttpClientFactory clientFactory)
        {
            _config = config;
            _clientFactory = clientFactory;
            TargetRps = config.GetValue<int>("TargetRPS");
        }

        public async ValueTask<LoadGeneratorModel[]> Run()
        {
            var watch = Stopwatch.StartNew();

            ConsoleUtil.DisplayMessage(ConsoleColor.Magenta, "---- Process Starting ----");
            var requestData = new LoadGeneratorApiRequest { Name = "Alan Corley", Date = DateTime.UtcNow, Requests_Sent = TargetRps };

            var tasks = new List<Task<LoadGeneratorModel>>();
            const int maxThreadsToUse = 8;
            var semaphoreSlim = new SemaphoreSlim(maxThreadsToUse);
 
            for (int i = 0; i < TargetRps; ++i)
            {
                tasks.Add(ExecuteApiRequest(semaphoreSlim, i+1, requestData));
            }

            var response = await Task.WhenAll(tasks.ToArray());

            var totalElapsedTime = watch.ElapsedMilliseconds;
            OutputReportAnalysis(response, totalElapsedTime);

            return response;
        }

        private async Task<LoadGeneratorModel> ExecuteApiRequest(SemaphoreSlim semaphoreSlim, int curPosition, LoadGeneratorApiRequest requestData)
        {
            LoadGeneratorModel analysis = new LoadGeneratorModel() { AttemptNumber = curPosition };

            var watch = Stopwatch.StartNew();

            try
            {
                await semaphoreSlim.WaitAsync();

                using var client = _clientFactory.CreateClient("generator");

                var response = await client.PostAsJsonAsync<LoadGeneratorApiRequest>("Live", requestData);

                analysis.EllapsedTimeMs = watch.ElapsedMilliseconds;
                analysis.StatusCode = response.StatusCode;

                return analysis;
            }
            catch (Exception ex)
            {
                analysis.EllapsedTimeMs = watch.ElapsedMilliseconds;
                analysis.ErrorMsg = ex.Message;
                return analysis;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private void OutputReportAnalysis(LoadGeneratorModel[] data, long totalElapsedTime)
        {
            ConsoleUtil.DisplayMessage(ConsoleColor.Magenta, "---- Results Summary ----");

            var avgRespTimeMs = data.Select(x => Convert.ToDouble(x.EllapsedTimeMs)).Average(t => t);

            // results table
            var table = new ConsoleTable("Attempt", "Response Time", "Status Code", "Error", "+/- Avg (ms)");
            foreach (var item in data.OrderBy(x => x.AttemptNumber))
            {
                if (item.StatusCode == HttpStatusCode.OK) {
                    ConsoleUtil.DisplayMessage(ConsoleColor.Green, $"{DateTime.Now:HH:mm:ss.fff}: API Post Attempt {item.AttemptNumber} of {TargetRps} - Response was successful!");
                }
                else {
                    ConsoleUtil.DisplayMessage(ConsoleColor.Red, $"{DateTime.Now:HH:mm:ss.fff}: API Post Attempt {item.AttemptNumber} of {TargetRps} - Unsuccessful response encountered");
                }

                var overUnderAvgRespMs = item.EllapsedTimeMs - avgRespTimeMs;
                
                table.AddRow(item.AttemptNumber, item.EllapsedTimeMs, item.StatusCode, GetErrorMessage(item), overUnderAvgRespMs);
            }

            // aggregate data table
            var minRespTimeMs = data.Select(x => Convert.ToDouble(x.EllapsedTimeMs)).Min(t => t);
            var maxRespTimeMs = data.Select(x => Convert.ToDouble(x.EllapsedTimeMs)).Max(t => t);

            var totalErrors = data.Count(x => x.StatusCode != HttpStatusCode.OK);
            var actualRps = TimeSpan.FromMilliseconds(TargetRps / (totalElapsedTime + 0.000001)).TotalSeconds;
            var targetRps = TimeSpan.FromMilliseconds(TargetRps / (1.000001)).TotalSeconds;
            table.Write();

            Console.WriteLine();

            var tableSummary = new ConsoleTable("Aggregate Summary","");
            tableSummary.AddRow("Avg Response Time (ms)", avgRespTimeMs);
            tableSummary.AddRow("Min Response Time (ms)", minRespTimeMs);
            tableSummary.AddRow("Max Response Time (ms)", maxRespTimeMs);
            tableSummary.AddRow("Total Errors Encountered", totalErrors);
            tableSummary.AddRow("Target RPS", targetRps);
            tableSummary.AddRow("Actual RPS", actualRps);
            tableSummary.AddRow("Total Ellapsed Time (ms)", totalElapsedTime);

            tableSummary.Write();
            
        }

        private static string? GetErrorMessage(LoadGeneratorModel data)
        {
            return !string.IsNullOrWhiteSpace(data.ErrorMsg) ? data.ErrorMsg : "";
        }

    }
}
