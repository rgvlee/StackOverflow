using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Question60650516.Models;

namespace Question60650516.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyClassController : ControllerBase
    {
        // POST: api/MyClass
        [HttpPost]
        public string Post([FromBody] List<MyClass> model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}