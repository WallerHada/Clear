using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MimeKit;
using ProjectWebAPI.DTO;
using ProjectWebAPI.TestClass;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ProjectWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Todo
        ///     {
        ///         "key":"value"
        ///     }
        ///     
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public string SendEmail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Doctor!", "1193473658@qq.com"));
            message.To.Add(new MailboxAddress("Peter Parker!", "xxx@example.com"));
            message.Subject = "人生经验";

            var body = new TextPart("plain")
            {
                Text = @"Hey peter parker,

I can't believe I'm able to work with a great team like yours, and I have to complete the mission to prove myself.

在一个人刚入社会时，确实是非常重要的一个问题。

技术当然重要，没有技术根本做不了事。技术高，还可以炫耀，出去找工作轻轻松松，是不是很 cool ？

至于业务逻辑，换个公司可能就完全不同了，弄那么熟悉做什么？

业务的繁杂，确实很烧脑，但并不是一无是处。

技术的发展，很多时候是靠业务来推动的，比如大数据，没有业务，哪来的数据？小规模的企业，搞大数据就是杀蚂蚁用牛刀，派不上用场。

你去找工作，人家问你在项目中是否做了什么？你说做了增删改查，基本上就没戏了，人家希望看到的是你在项目中承担什么样的角色，发挥了什么作用？能解决什么的问题？

再好的技术，必须在业务中才能沉淀和成熟。不和业务结合起来的技术，或者说没有实战，没有真正用起来的技术，只是屠龙之术，最多只能算是你写的个 demo 而已。

产品经理，必须精通业务。项目经理，至少要熟悉业务。

那对于一个普通的程序员来说，到底哪个重要？

个人看法是都重要。我学的东西比较杂，前端、后端甚至PS、Excel都会一点，这样很多时候可以不依赖人家迅速去解决问题。技术的全面性还是很重要。业务方面，比较繁杂的可以使用流程图、脑图、写注释等方式让自己的思路变得清晰，再往后面做就容易了。


-- Doctor"
            };

            string path = "C:\\Users\\pjguang\\Desktop\\附件代码测试.txt";
            // create an image attachment for the file located at path
            var attachment = new MimePart("image", "gif")
            {
                Content = new MimeContent(System.IO.File.OpenRead(path), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(path)
            };

            // now create the multipart/mixed container to hold the message text and the
            // image attachment
            var multipart = new Multipart("mixed")
            {
                body,
                attachment
            };

            // now set the multipart/mixed as the message body
            message.Body = multipart;

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.qq.com", 25, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("1193473658@qq.com", "***");

                client.Send(message);
                client.Disconnect(true);
            }
            return "已发送";
        }

        /// <summary>
        /// AD验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public string ActiveDirectory(LoginInput input)
        {
            string msg = "success";
            string ADPath = "LDAP://trio.com";
            try
            {
#pragma warning disable CA1416
                DirectoryEntry entry = new(ADPath, input.Account, input.Password);
                DirectorySearcher searcher = new(entry);
                searcher.Filter = $"(SamAccountName={input.Account})";
                SearchResult result = searcher.FindOne();
                if (null == result)
                {
                    msg = "找不到";
                }
                else
                {
                    msg = "账号密码正确";
                    // msg = System.Text.Json.JsonSerializer.Serialize(result);
                }
#pragma warning restore CA1416
            }
            catch (DirectoryServicesCOMException ex)
            {

                if (ex.ErrorCode == -2147023570)
                {
                    msg = ex.Message.ToString();
                }
            }

            return msg;
        }

        [HttpGet]
        public string GetDateTime()
        {
            var newTime = new DateTime(1970, 1, 1, 0, 0, 0);
            Console.WriteLine("The date and time are {0} UTC.", TimeZoneInfo.ConvertTimeToUtc(newTime));
            // out The date and time are 1969/12/31 16:00:00 UTC.

            // ISO 8601
            string ISO8601time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
            Console.WriteLine(ISO8601time);
            // out 2021-12-31T12:04:08+08:00

            return ISO8601time;
        }

        [HttpGet]
        public void LearnDelegate()
        {
            var test = new LearnDelegate();
            test.Start();
        }

        [HttpPost]
        public string Translate(EmailInput input)
        {
            // var example = "You can splice strings together and return them";
            // return _localizer["Empty Account"] + example;

            // you can try to format strings
            //LocalizedString localizedString = _localizer["{0} try between {1} and {2}", "密码", 0, 9];

            LocalizedString localizedString = _localizer["You email is {0}, password is {1}", input.Email, input.Password];
            return localizedString;
        }

#if DEBUG
        [HttpGet]
        public string TestErrorLog()
        {
            string str = "";
            string[] arr = new string[] { "1", "2" };
            for (int i = 0; i < 3; i++)
            {
                str += arr[i] + "\r\n";
            }
            return str;
        }
#endif
    }
}
