using System;
using RSA;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Singleton
    {
        static public  Singleton singleton = null;
        public rsa cifradoRSA = new rsa();
        public static Singleton instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new Singleton();
                }
                return singleton;
            }
        }
    }
}
