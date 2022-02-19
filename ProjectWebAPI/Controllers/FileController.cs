using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectWebAPI.ActionFilter;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjectWebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)] // hide controller
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    [Route("onlywhitelist/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpGet]
        public string GetPath()
        {
            return Directory.GetCurrentDirectory() + "\r\n" +
                   Environment.CurrentDirectory;
        }

        /// <summary>
        /// 上传安装包文件包
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <response code="403">禁止访问</response>
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync([FromForm] IFormFile file)
        {
            if (file == null)
                return BadRequest("No files data in the request.");

            if (file.Length > 0)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (ext is not (".apk" or ".wgt"))
                    return BadRequest("The extension is invalid ... discontinue processing the file");

                string path = Directory.GetCurrentDirectory();

                var dayfile = DateTime.Now.ToString("yyyyMMdd");

                var versionPath = Path.Combine(path, "MyStaticFiles", dayfile);

                if (!Directory.Exists(versionPath))
                {
                    Directory.CreateDirectory(versionPath);
                }

                var str = DateTime.Now.ToString("yyyyMMddHHmmss");

                var curFile = Path.Combine(versionPath, str + file.FileName);

                using (var stream = System.IO.File.Create(curFile))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(ext[1..].ToUpper());
            }
            else
            {
                return BadRequest("formfile length is zero");
            }
        }
    }
}
