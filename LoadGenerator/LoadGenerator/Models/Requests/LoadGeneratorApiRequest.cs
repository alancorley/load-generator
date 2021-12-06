using System;

namespace LoadGenerator.Models.Requests
{
    public class LoadGeneratorApiRequest
    {
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public int Requests_Sent { get; set; }
    }
}
