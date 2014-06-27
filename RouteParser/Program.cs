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
            Route a = new Route("N0290F280 EKI G12 GOLDO UM603 PEREN UN128 RAXAD/N0455F370 N128 NANER DCT BOSNA");
            //Route b = new Route("DCT SWL J174 HTO J55 PQI J564 CL J549 YNA DCT REDBY DCT CARPE/M080F410 DCT 54N050W 55N040W 55N030W 55N020W/N0465F410 DCT DOGAL/M080F410 UN533 BURAK/N0465F410 UN535 SHA UG1 DVR W71 VABIK W70 KOK");
            //Route c = new Route("TOLEY1 DUB180040/N0350F330 Z12 SUSLUS/N0406F370 UL984 KOROP/N0455F370 UL984 NM UA277 RABEN/N0455F370 UA277 BAROR UB246 NL UL981 SOBLO/K0841S1130 B143 VEKAT/N0455F370 UB143 BADIR UR315 ADEKI UN644 RODDAR/K0843S1130 N644 BABUM B143 POGON A477 UTSB A107 YU DCT");
            Console.WriteLine(a.ToString());
            Console.Read();
            
        }
    }
}
