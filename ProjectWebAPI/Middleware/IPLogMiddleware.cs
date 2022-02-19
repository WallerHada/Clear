using Microsoft.AspNetCore.Http;
using ProjectWebAPI.Helper;
using ProjectWebAPI.IOTasks;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectWebAPI.Middleware
{
    public class IPLogMiddleware
    {
        // https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.builder.useextensions.use?view=aspnetcore-6.0
        // https://www.cnblogs.com/RainingNight/p/middleware-in-asp-net-core.html#map
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public IPLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            // 过滤，只有接口
            // context.Request.Path.Value.Contains("api")
            if (true)
            {
                context.Request.EnableBuffering();

                try
                {
                    var request = context.Request;
                    var requestInfo = JsonSerializer.Serialize(new RequestInfo()
                    {
                        Ip = GetClientIP(context),
                        Url = request.Path.ObjToString().TrimEnd('/').ToLower(),
                        ISO8601 = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", System.Globalization.DateTimeFormatInfo.InvariantInfo),
                        //Week = GetWeek(),
                    });

                    if (!string.IsNullOrEmpty(requestInfo))
                    {
                        Parallel.For(0, 1, e =>
                        {
                            RecordLog.HighfrequencyLog("RequestIpInfoLogs", new string[] { requestInfo + ",\r\n" }, false);
                        });

                        request.Body.Position = 0;
                    }

                    await _next(context);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                // await _next(context);
            }
        }

        private string GetWeek()
        {
            string week = string.Empty;
            week = DateTime.Now.DayOfWeek switch
            {
                DayOfWeek.Monday => "周一",
                DayOfWeek.Tuesday => "周二",
                DayOfWeek.Wednesday => "周三",
                DayOfWeek.Thursday => "周四",
                DayOfWeek.Friday => "周五",
                DayOfWeek.Saturday => "周六",
                DayOfWeek.Sunday => "周日",
                _ => "N/A",
            };
            return week;
        }

        public static string GetClientIP(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].ObjToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ObjToString();
            }
            return ip;
        }
    }

    public class RequestInfo
    {
        public string Ip { get; set; }
        public string Url { get; set; }
        public string ISO8601 { get; set; }
        public string Week { get; set; }
    }
}
