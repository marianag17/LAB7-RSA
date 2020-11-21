using System;
using System.Numerics;
using System.Collections.Generic;
using Ionic.Zip;
using System.Text;
using System.IO;

namespace RSA
{
    public class rsa
    {
        int p, q;
        BigInteger n, fi, e, d, a, b;
        string cifrado;

        public void generarLlaves(int a, int b)
        {
            try
            {
                p = a;
                q = b;
                n = p * q;
                fi = (p - 1) * (q - 1);
                Random rnd = new Random();
                bool correcto = false;
                while (!correcto)
                {
                    e = rnd.Next(1, (int)fi);
                    if ((esPrimo((int)e)) && (e < fi) && (fi % e > 0))
                    {
                        correcto = true;
                    }
                }
                d = Euclides(fi, e);
                Console.WriteLine(Convert.ToString(e) + "," + Convert.ToString(n));
                Console.WriteLine(Convert.ToString(d) + "," + Convert.ToString(n));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
            }
        }
        public FileStream Cifrar(string ruta1, string ruta2, string nombre)
        {
            string llave = File.ReadAllText(ruta2);
            string[] split = llave.Split(',');
            a = BigInteger.Parse(split[0]);
            b = BigInteger.Parse(split[1]);
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            FileInfo archivo = new FileInfo(ruta1);
            long tamano = archivo.Length;
            using (var Leer = new FileStream(ruta1, FileMode.Open))
            {
                using (var Reader = new BinaryReader(Leer))
                {
                    using (var Escribir = new FileStream($"{nombre}.rsa", FileMode.OpenOrCreate))
                    {
                        int buffer = 5000;
                        var bytes = new byte[buffer];
                        while (Reader.BaseStream.Position != Reader.BaseStream.Length)
                        {
                            bytes = Reader.ReadBytes(buffer);
                            foreach (var item in bytes)
                            {
                                int m = Convert.ToInt32(item);
                                BigInteger numCif = BigInteger.ModPow(m, a, b);
                                cifrado = numCif.ToString() + " ";
                                byte[] cif = iso.GetBytes(cifrado);
                                Escribir.Write(cif, 0, cif.Length);
                            }
                        }
                        return Escribir;
                    }
                }
            }
        }
        public FileStream Descifrar(string ruta1, string ruta2, string nombre)
        {
            string llave = File.ReadAllText(ruta2);
            string[] split = llave.Split(',');
            a = BigInteger.Parse(split[0]);
            b = BigInteger.Parse(split[1]);
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            FileInfo archivo = new FileInfo(ruta1);
            long tamano = archivo.Length;
            using (var Leer = new FileStream(ruta1, FileMode.Open))
            {
                using (var Reader = new BinaryReader(Leer))
                {
                    using (var Escribir = new FileStream($"{nombre}.txt", FileMode.OpenOrCreate))
                    {
                        int buffer = 5000;
                        var bytes = new byte[buffer];
                        while (Reader.BaseStream.Position != Reader.BaseStream.Length)
                        {
                            bytes = Reader.ReadBytes(buffer);
                            int tamaño = bytes.Length;
                            while (bytes[tamaño - 1] != ' ')
                            {
                                buffer++;
                                Array.Resize(ref bytes, buffer);
                                bytes[buffer-1] = Reader.ReadByte();
                                tamaño = bytes.Length;
                            }
                            BigInteger num;
                            List<byte> lista1 = new List<byte>();
                            byte[] numero;
                            BigInteger numCif;
                            char c;
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                if (bytes[i] != ' ')
                                {
                                    lista1.Add(bytes[i]);
                                }
                                else if (bytes[i] == ' ')
                                {
                                    numero = lista1.ToArray();
                                    num = BigInteger.Parse(iso.GetString(numero));
                                    numCif = BigInteger.ModPow(num, a, b);
                                    c = (char)numCif;
                                    Escribir.WriteByte(Convert.ToByte(c));
                                    lista1.Clear();
                                }
                            }
                        }
                        return Escribir;
                    } 
                }
            }
        }
        private BigInteger Euclides(BigInteger a, BigInteger b)
        {
            List<BigInteger> l1 = new List<BigInteger>();
            List<BigInteger> l2 = new List<BigInteger>();
            int num = 0;
            int aux = 0;
            int multi1 = 0;
            int multi2 = 0;
            int resta1 = 0;
            int resta2 = 0;
            l1.Add(a);
            l2.Add(a);
            l1.Add(b);
            l2.Add(1);
            while (num != 1)
            {
                aux = (int)l1[0] / (int)l1[1];
                multi1 = aux * (int)l1[1];
                multi2 = aux * (int)l2[1];
                resta1 = (int)l1[0] - multi1;
                resta2 = (int)l2[0] - multi2;
                if (resta1 < 0)
                {
                    resta1 = resta1 + (int)fi;
                }
                if (resta2 < 0)
                {
                    while (resta2 < 0)
                    {
                        resta2 = resta2 + (int)fi;
                    }
                }
                l1[0] = l1[1];
                l2[0] = l2[1];
                l1[1] = resta1;
                l2[1] = resta2;
                num = (int)l1[1];
            }
            return l2[1];
        }
        public bool esPrimo(int numero)
        {
            bool primo = true;
            int i = 2;
            while (primo && i != numero)
            {
                if (numero % i == 0)
                {
                    primo = false;
                }
                i++;
            }
            return primo;
        }
    }
}
