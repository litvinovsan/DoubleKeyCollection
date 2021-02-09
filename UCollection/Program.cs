using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace UCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            DoubleKeyCollection<int, string, string> ucCollection =new DoubleKeyCollection<int,string,string>();

            ucCollection.Add(1, "", "");

         //   ucCollection.Add(1, "", "");

            Dictionary<int,string> s=new Dictionary<int, string>();
            s.Add(1,"asas");
            var a = s[5];

        }
    }
}
