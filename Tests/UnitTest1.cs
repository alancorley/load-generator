using LoadGenerator.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace Tests
{
    public class UnitTest1: TestFixture
    {
        private readonly ILoadGeneratorService _loadGeneratorService;
        
        public UnitTest1(TestFixture fixture)
        {
            _loadGeneratorService = fixture.ServiceProvider.GetService<ILoadGeneratorService>()!;
        }

        [Fact(Skip="DI not working here")]
        public void LoadGeneratorSuccess()
        {
            //TODO: need to figure out why DI not working here 
            var response = _loadGeneratorService.Run();


            Assert.All(response.Result, item => Assert.True(item.StatusCode == HttpStatusCode.OK));
            //TODO: check for average and max to be within certain bounds
           
        }
    }
}