using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Powerumc.AspNetCore.Core;
using SampleWeb.Services;

namespace SampleWeb.Controllers
{
    public class TestController : Controller
    {
        private readonly TraceId _traceId;
        private readonly ILogger<TestController> _logger;
        private readonly ITestService _testService;

        public TestController(TraceId traceId,
            ILogger<TestController> logger,
            ITestService testService)
        {
            _traceId = traceId;
            _logger = logger;
            _testService = testService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(await _testService.GetStringsAsync());
        }
    }
}