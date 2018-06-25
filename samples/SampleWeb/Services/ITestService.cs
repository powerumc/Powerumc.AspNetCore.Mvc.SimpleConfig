using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleWeb.Services
{
    public interface ITestService
    {
        Task<List<string>> GetStringsAsync();
    }
}