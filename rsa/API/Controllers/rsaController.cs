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
            descriptor.Compresion = new DescriptorRuta()
            {
                Ruta = "/api/rsa/{name}",
                Descripcion = "En el parámetro name se envía el nombre para el archivo cifrado, " +
                "para mandar el archivo .txt desde form-data es {archivo} y para mandar el archivo de tipo .key es {llave} ",
                Metodo = "POST"
            };
            descriptor.Descompresion = new DescriptorRuta()
            {
                Ruta = "/api/rsa/{name}",
                Descripcion = "En el parámetro name se envía el nombre para el archivo cifrado, " +
                "para mandar el archivo .rsa desde form-data es {archivo} y para mandar el archivo de tipo .key es {llave} ",
                Metodo = "POST"
            };

            descriptor.generarLlaves = new DescriptorRuta()
            {
                Ruta = "/api/rsa/{p}/{q}",
                Descripcion = "Recibe dos valores int primos p y q para generar las llaves. Esta ruta devuelve un archivo zip con dos files de tipo .key con las llaves necesarias.",
                Metodo = "GET"
            };

            return descriptor;
        }
        [Route("api/rsa/{nombre}")]
        [HttpPost]
        public async Task<ActionResult> desCifrar([FromRoute] string nombre, [FromForm] IFormFile archivo, [FromForm] IFormFile llave)
        {
           
        }
    }
}
