using Microsoft.AspNetCore.Builder;

namespace ProjectWebAPI.Middleware
{
    public static class MiddlewareHelper
    {
        // https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#branch-the-middleware-pipeline-1
        // https://dotnetcorecentral.com/blog/middleware-in-asp-net-core/
        // 通过IApplicationBuilder暴露中间件

        public static IApplicationBuilder UseIPLogMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IPLogMiddleware>();
        }

        public static IApplicationBuilder UseRecordAccessLogMiddleware(this IApplicationBuilder app)
        {
           //return app.Map("/WeatherForecast/Translate", true, b => b.UseMiddleware<RecordAccessLogMiddleware>());
           return app.UseMiddleware<RecordAccessLogMiddleware>();
        }
    }
}
