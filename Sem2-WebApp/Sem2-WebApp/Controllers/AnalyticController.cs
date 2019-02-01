using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem2_WebApp.Models;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticController : ControllerBase
    {
        /*
        [HttpGet("{id}")]
        public HttpResponseMessage getModel([FromRoute] string id)
        {
            var path = @"./wwwroot/resources/kerasModel/" + id;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }
        */
        [HttpGet("{id}")]
        public FileResult getModel([FromRoute] string id)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"./wwwroot/resources/kerasModel/" + id);
            string fileName = id;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}