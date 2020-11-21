using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Ionic.Zip;
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
                Descripcion = "En el parámetro {name} se envía el nombre en la url, para el archivo cifrado, " +
                "para mandar el archivo .txt desde form-data es {archivo} y para mandar el archivo de tipo .key es {llave} ",
                Metodo = "POST"
            };
            descriptor.Descompresion = new DescriptorRuta()
            {
                Ruta = "/api/rsa/{name}",
                Descripcion = "En el parámetro {name} se envía el nombre en la url para el archivo cifrado, " +
                "para mandar el archivo .rsa desde form-data es {archivo} y para mandar el archivo de tipo .key es {llave} ",
                Metodo = "POST"
            };

            descriptor.generarLlaves = new DescriptorRuta()
            {
                Ruta = "/api/rsa/keys/{p}/{q}",
                Descripcion = "Recibe dos valores int primos p y q para generar las llaves. Esta ruta devuelve un archivo zip con dos files de tipo .key con las llaves necesarias.",
                Metodo = "GET"
            };

            return descriptor;
        }



        [Route("{name}")]
        [HttpPost]
        public async Task<ActionResult> Post([FromRoute] string name, [FromForm] IFormFile archivo, [FromForm] IFormFile llave)
        {
            /*Crear Carpeta 'Uploads' para guardar los archivos subidos*/

            string workingDirectory = Environment.CurrentDirectory;
            string pathFolderActual = Directory.GetParent(workingDirectory).FullName;
            string pathFilesDirectory = pathFolderActual + "\\Uploads\\";

            string rutaArchivo = pathFilesDirectory + archivo.FileName;
            string rutaLlave = pathFilesDirectory + llave.FileName;

            string extensionArchivo = "."  + archivo.FileName.Split(".")[1];

            // Ser verifica si existe el directorio y se elimina para evitar duplicidad
            if (Directory.Exists(pathFilesDirectory))
            {
                Directory.Delete(pathFilesDirectory, true);
            }

            Directory.CreateDirectory(pathFilesDirectory);

            using (var fileStream = new FileStream((rutaArchivo), FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            using (var fileStream = new FileStream((rutaLlave), FileMode.Create))
            {
                await llave.CopyToAsync(fileStream);
            }


            string pathCifrados = pathFolderActual + "\\Cifrados\\";
            // Ser verifica si existe el directorio y se elimina para evitar duplicidad
            if (Directory.Exists(pathCifrados))
            {
                Directory.Delete(pathCifrados, true);
            }

            Directory.CreateDirectory(pathCifrados);

            FileStream files;

            switch (extensionArchivo)
            {
                case ".txt": {

                    rsa cifradoRsa = new rsa();
                    cifradoRsa.Cifrar(rutaArchivo, rutaLlave, name);
                    var streamCompress = System.IO.File.OpenRead($"{name}.rsa");
                        return new FileStreamResult(streamCompress, "application/" + ".rsa")
                        {
                            FileDownloadName = name + ".rsa"
                        };
                    }
                break;

                case ".rsa":
                    {

                        rsa descifradoRsa = new rsa();
                        descifradoRsa.Descifrar(rutaArchivo, rutaLlave, name);
                        var streamCompress = System.IO.File.OpenRead($"{name}.txt");
                        return new FileStreamResult(streamCompress, "application/" + ".txt")
                        {
                            FileDownloadName = name + ".txt"
                        };
                    }
                    break;
                default: {
                        return BadRequest();
                    }
                    break;

            }

            

        }


        [Route("keys/{p}/{q}")]
        [HttpGet]
        public  ActionResult generarLlaves([FromRoute] string p, [FromRoute] string q)
        {
            /*Crear Carpeta 'Keys' para guardar las llaves*/

            string workingDirectory = Environment.CurrentDirectory;
            string pathFolderActual = Directory.GetParent(workingDirectory).FullName;
            string pathDirectoryKeys = pathFolderActual + "\\Keys\\";

            string rutaKeyPublica = "";
            string rutaKeyPrivada = "";
            // Ser verifica si existe el directorio y se elimina para evitar duplicidad
            if (Directory.Exists(pathDirectoryKeys))
            {
                Directory.Delete(pathDirectoryKeys, true);
            }

            Directory.CreateDirectory(pathDirectoryKeys);



            try
            {
                rsa generarKeys = new rsa();
                // Se verifica si son primos
                if (generarKeys.esPrimo(int.Parse(p)) && generarKeys.esPrimo(int.Parse(q)))
                {
                    // Se verifican si estan entre 50 y 32000
                    if ((int.Parse(p) > 50 && int.Parse(p) < 32000) && (int.Parse(q) > 50 && int.Parse(q) < 32000))
                    {
                        generarKeys.generarLlaves(int.Parse(p), int.Parse(q));

                        //Escribir archivo para public key
                        rutaKeyPublica = pathDirectoryKeys + "public.key";
                        System.IO.File.WriteAllBytes(rutaKeyPublica, System.Text.Encoding.Default.GetBytes(generarKeys.public_key));

                        //Escribir archivo para private key
                        rutaKeyPrivada = pathDirectoryKeys + "private.key";
                        System.IO.File.WriteAllBytes(rutaKeyPrivada, System.Text.Encoding.Default.GetBytes(generarKeys.private_key));


                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AddEntry("public.key", System.IO.File.ReadAllBytes(rutaKeyPublica));
                            zip.AddEntry("private.key", System.IO.File.ReadAllBytes(rutaKeyPrivada));

                            using (MemoryStream output = new MemoryStream())
                            {
                                zip.Save(output);
                                return File(output.ToArray(), "application/zip", "Keys.zip");
                            }
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }

                }
                else {
                    return BadRequest();
                }

                

            }
            catch (Exception ex) {
                return BadRequest();
            }

        }

       
        
    }
}
