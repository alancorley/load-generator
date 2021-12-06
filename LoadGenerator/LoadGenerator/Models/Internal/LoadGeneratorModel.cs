using System.Net;

namespace LoadGenerator.Models.Internal
{
    public class LoadGeneratorModel
    {
        public int AttemptNumber { get; set; }
        public long EllapsedTimeMs { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? ErrorMsg { get; set; } = default!;
    }
}
