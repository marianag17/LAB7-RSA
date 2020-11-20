using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSA;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class rsaController : ControllerBase
    {
        [HttpGet]
        public DescriptorAPI getAPIDescriptor()
        {
            DescriptorAPI descriptor = new DescriptorAPI();
            return descriptor;
        }

    }
}
