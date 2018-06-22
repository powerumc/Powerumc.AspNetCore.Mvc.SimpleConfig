using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Powerumc.AspNetCore.Core;

namespace Powerumc.AspNetCore.Mvc.SimpleConfig.Middlewares
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items["TRACE_ID"] = TraceId.New();
            
            await _next(context);
        }
    }
}