using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProjectWebAPI.Helper;
using ProjectWebAPI.IOTasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ProjectWebAPI.Middleware
{
    public class RecordAccessLogMiddleware
    {
        // https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.builder.useextensions.use?view=aspnetcore-6.0
        // https://www.cnblogs.com/RainingNight/p/middleware-in-asp-net-core.html#map
        private readonly RequestDelegate _next;
        private Stopwatch _stopwatch;

        public RecordAccessLogMiddleware(RequestDelegate next)
        {
            _next = next;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var api = context.Request.Path.ObjToString().TrimEnd('/').ToLower();

            // 过滤，只有接口          
            // 后续个人猜测，app.map执行后会直接返回结果，不再继续执行后续操作。故在中间件中对路径进行区分知否执行该次操作
            // https://stackoverflow.com/questions/63098272/net-core-can-applicationbuilder-map-path-be-case-sensitive/63099495#63099495

            // if (api.Contains("api")) // && !ignoreApis.Contains(api)
            // 
            if (context.Request.Path.StartsWithSegments("/WeatherForecast/Translate"))
            {
                _stopwatch.Restart();
                var userAccessModel = new UserAccessModel();

                HttpRequest request = context.Request;

                userAccessModel.API = api;
                userAccessModel.IP = IPLogMiddleware.GetClientIP(context);
                userAccessModel.BeginTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                userAccessModel.RequestMethod = request.Method;
                userAccessModel.AcceptLanguage = request.Headers["Accept-Language"].ObjToString();


                // 获取请求body内容
                if (request.Method.ToLower().Equals("post") || request.Method.ToLower().Equals("put"))
                {
                    // 启用倒带功能，就可以让 Request.Body 可以再次读取
                    request.EnableBuffering();

                    Stream stream = request.Body;
                    byte[] buffer = new byte[request.ContentLength.Value];
                    await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                    userAccessModel.RequestData = Encoding.UTF8.GetString(buffer);

                    request.Body.Position = 0;
                }
                else if (request.Method.ToLower().Equals("get") || request.Method.ToLower().Equals("delete"))
                {
                    userAccessModel.RequestData = HttpUtility.UrlDecode(request.QueryString.ObjToString(), Encoding.UTF8);
                }

#if true
                // 获取Response.Body内容
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    var responseBodyData = await GetResponse(context.Response);

                    await responseBody.CopyToAsync(originalBodyStream);
                }
#endif

                // 响应完成记录时间和存入日志
                context.Response.OnCompleted(() =>
                {
                    _stopwatch.Stop();

                    userAccessModel.OPTime = _stopwatch.ElapsedMilliseconds + "ms";

                    // 自定义log输出
                    var requestInfo = JsonSerializer.Serialize(userAccessModel);
                    Parallel.For(0, 1, e =>
                    {
                        RecordLog.HighfrequencyLog("RecordAccessLogs", new string[] { requestInfo + ",\r\n" }, false);
                    });

                    // var logFileName = FileHelper.GetAvailableFileNameWithPrefixOrderSize(_environment.ContentRootPath, "RecordAccessLogs");
                    // SerilogServer.WriteLog(logFileName, new string[] { requestInfo + "," }, false);

                    return Task.CompletedTask;
                });
            }
            else
            {
                await _next(context);
            }
        }


        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<string> GetResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }

    public class UserAccessModel
    {
        public string User { get; set; }
        public string IP { get; set; }
        public string API { get; set; }
        public string BeginTime { get; set; }
        public string OPTime { get; set; }
        public string RequestMethod { get; set; }
        public string RequestData { get; set; }
        public string Agent { get; set; }
        public string AcceptLanguage { get; set; }
    }
}
