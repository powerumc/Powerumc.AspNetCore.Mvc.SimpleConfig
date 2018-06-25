using System.Collections.Generic;
using System.Threading.Tasks;
using Powerumc.AspNetCore.Core;

namespace SampleWeb.Services
{
    [Register(typeof(ITestService))]
    public class TestService : ITestService
    {
        public async Task<List<string>> GetStringsAsync()
        {
            return await Task.FromResult(new List<string>()
            {
                "Junil Um",
                "Hanji Jung"
            });
        }
    }
}