using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
///TODO IMPLEMENT isElementOf in all methods ex: NamedPoint.isElementOf("CARPE") --> true
namespace RouteParser
{
    class Route
    {
        interface IRouteElement
        {
            string getRepresentation();
        }
        interface IPoint: IRouteElement{
            string latitude { get; }
            string longitude { get; }
        }
        class NamedPoint : IPoint
        {
            private string _name;

            public NamedPoint(string name)
            {
                this._name = name;
            }

            public string name
            {
                get
                {
                    return this._name;
                }
                set
                {
                    this._name = value;
                }
            }


            public static bool includes(string candidate)
            {
                return Regex.IsMatch(candidate, @"\w{3,5}");
            }
            public string getRepresentation()
            {
                
                    return this._name;
            }
            //NOT YET IMPLEMENTED
            public string latitude { get { return string.Empty; } }
            //NOT YET IMPLEMENTED
            public string longitude { get { return string.Empty; } }
        }  
        class CoordinatePoint : IPoint
        {
            private string _latitude;
            private string _longitude;

            public CoordinatePoint(string latitude, string longitude)
            {
                this._latitude = latitude;
                this._longitude = longitude;
            }
            public string latitude
            {
                get
                {
                    return this._latitude;
                }
                set
                {
                    this._latitude = value;
                }
            }
            public string longitude
            {
                get
                {
                    return this._longitude;
                }
                set
                {
                    this._longitude = value;
                }
            }
            public string getRepresentation()
            {
                return string.Format("{0}{1}", this._latitude, this._longitude);
            }
            public static bool includes(string candidate)
            {
                return Regex.IsMatch(candidate, @"\d{2}[N,S]\d{3}[W,E]");
            }
        }
        class NavaidPoint : IPoint
        {
            private string _navaidname;
            private string _bearing;
            private string _distance;

            public NavaidPoint(string navaidname, string bearing, string distance)
            {
                this._navaidname = navaidname;
                this._bearing = bearing;
                this._distance = distance;
            }
            public string navaidname
            {
                get
                {
                    return this._navaidname;
                }
                set
                {
                    this._navaidname = value;
                }
            }
            public string bearing
            {
                get
                {
                    return this._bearing;
                }
                set
                {
                    this._bearing = value;
                }
            }
            public string distance
            {
                get
                {
                    return this._distance;
                }
                set
                {
                    this._distance = value;
                }
            }
            //NOT YET IMPLEMENTED
            public string latitude { get { return string.Empty; } }
            //NOT YET IMPLEMENTED
            public string longitude { get { return string.Empty; } }
            public string getRepresentation()
            {
                return string.Format("{0}{1}{2}", this._navaidname, this._bearing,this._distance);
            }
            public static bool includes(string candidate)
            {
                return Regex.IsMatch(candidate, @"\w{2,3}\d{3}\d{3}");
            }
        }
        class SpeedLevel : IRouteElement
        {
            private string _cruisingspeed;
            private string _flightlevel;

            public SpeedLevel(string cruisingspeed, string flightlevel)
            {
                this._cruisingspeed = cruisingspeed;
                this._flightlevel = flightlevel;
            }
            public string getRepresentation()
            {
                return string.Format("{0}{1}", this._cruisingspeed, this._flightlevel);
            }
            public static bool includes(string candidate)
            {
                return Regex.IsMatch(candidate, @"(?<speed>[K,N]\d{4}|M\d{3})(?<clevel>([A,F]\d{3})|[S,M]\d{4})");
            }
        }
        class Airway : IRouteElement
        {
            private string _designator;
            public Airway(string designator)
            {
                this._designator = designator;
            }
            public string getRepresentation()
            {
                return this._designator;
            }
            public static bool includes(string candidate)
            {
                return Regex.IsMatch(candidate, @"\w{2,7}");
            }
        }
        private List<IRouteElement> _path;        
        public Route()
        {
            this._path = new List<IRouteElement>();
        }

        
        public static Route parseString(string routestring)
        {
            Route result = new Route();
            String [] routeelements = Regex.Split(routestring, @"\s|/");
            Console.WriteLine(string.Join(Environment.NewLine, routeelements));
            return result;
        }
    }
}
