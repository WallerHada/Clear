using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace ProjectWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region some directory
            string currDir = Directory.GetCurrentDirectory();
            string[] arr = new string[] { "MyStaticFiles", "Log" };
            for (int i = 0; i < arr.Length; i++)
            {
                var myStaticFiles = Path.Combine(currDir, arr[i]);
                if (!Directory.Exists(myStaticFiles))
                {
                    Directory.CreateDirectory(myStaticFiles);
                }
            }
            #endregion



            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //.UseUrls("http://0.0.0.0:5000");
                });
    }
}
