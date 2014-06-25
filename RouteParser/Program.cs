using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteParser;

namespace RouteParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Route a = Route.parseString("N0290F280 EKI G12 GOLDO UM603 PEREN UN128 RAXAD/NO455F370 N128 NANER L615 BOSNA");
            Console.Read();
        }
    }
}
