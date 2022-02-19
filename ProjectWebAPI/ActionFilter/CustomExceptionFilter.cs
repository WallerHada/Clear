using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;

namespace ProjectWebAPI.ActionFilter
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public CustomExceptionFilter()
        {

        }

        public void OnException(ExceptionContext context)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Log", DateTime.Now.ToString("yyyyMMdd") + "log.txt");
            using (StreamWriter w = File.AppendText(path))
            {
                IOTasks.RecordLog.ErrorLog(context.Exception.ToString(), w);
            }
        }
    }
}
