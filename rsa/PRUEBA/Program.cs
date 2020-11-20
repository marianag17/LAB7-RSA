using System;
using RSA;
namespace PRUEBA
{
    class Program
    {
        static void Main(string[] args)
        {
            rsa cifrado = new rsa();
            Console.WriteLine("Ingrese p");
            int a = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Ingrese q");
            int b = Convert.ToInt32(Console.ReadLine());
            cifrado.generarLlaves(a, b);
            Console.WriteLine("Texto");
            string texto = Console.ReadLine();
            Console.WriteLine("Llave");
            string llave = Console.ReadLine(); 
            Console.WriteLine("Nombre");
            string nombre = Console.ReadLine();
            cifrado.Cifrar(texto, llave,nombre);
            Console.WriteLine("Descifrar texto");
            string des = Console.ReadLine();
            Console.WriteLine("Llave para descifrar");
            string llave2 = Console.ReadLine();
            Console.WriteLine("Nombre para descifrar");
            string nombre2 = Console.ReadLine();
            cifrado.Descifrar(des, llave2, nombre2);

        }
    }
}
