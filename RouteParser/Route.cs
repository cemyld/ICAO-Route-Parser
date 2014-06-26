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
        public static bool isPoint(string candidate)
        {
            return isCoordinatePoint(candidate)|isNavaidPoint(candidate)|isNamedPoint(candidate);
        }
        public static bool isNamedPoint(string candidate)
        {
            //Check name length
            return Regex.IsMatch(candidate, @"\w{2,5}");
        }
        public static bool isCoordinatePoint(string candidate)
        {
            //Latitude and Longitude OR Degrees and Minutes
            return Regex.IsMatch(candidate, @"\d{2}[N,S]\d{3}[E,W]") | Regex.IsMatch(candidate, @"\d{4}[N,S]\d{5}[E,W]");
        }
        public static bool isNavaidPoint(string candidate)
        {
            return Regex.IsMatch(candidate, @"\w{2,3}\d{3}\d{3}");
        }
        public static bool isSpeedLevel(string candidate)
        {
            return Regex.IsMatch(candidate, @"(?<speed>[K,N]\d{4}|M\d{3})(?<clevel>([A,F]\d{3})|[S,M]\d{4})");
        }
        public static bool isAirway(string candidate)
        {
            return Regex.IsMatch(candidate, @"\w{2,7}");
        }
        public static bool isChangeOfSpeedLevelPoint(string candidate)
            {
                try{
                string [] candidateparts = candidate.Split(new char['/']);
                string point = candidateparts[0];
                string speedlevel = candidateparts[1];
                return isPoint(point) & isSpeedLevel(speedlevel);
                }
                catch (Exception e){
                    return false;
                }
            }
        public static bool isChangeOfFlightRule(string candidate)
        {
            return Regex.IsMatch(candidate, @"IFR|VFR");
        }
        interface IRouteElement
        {
            string getRepresentation();
        }
        interface IPoint : IRouteElement
        {
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
                return string.Format("{0}{1}{2}", this._navaidname, this._bearing, this._distance);
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

        }
        class ChangeOfSpeedLevelPoint : IRouteElement
        {
            private string _point;
            private string _speedlevel;

            public ChangeOfSpeedLevelPoint(string point, string speedlevel)
            {
                this._point = point;
                this._speedlevel = speedlevel;
            }
            public string getRepresentation()
            {
                return string.Format("{0}{1}", this._point, this._speedlevel);
            }

        }
        class ChangeOfFlightRule : IRouteElement
        {
            private string _flightrule;
            public ChangeOfFlightRule(string flightrule)
            {
                this._flightrule = flightrule;
            }
            public string getRepresentation()
            {
                return this._flightrule;
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
            String[] routeelements = routestring.Split();
            Console.WriteLine(string.Join(Environment.NewLine, routeelements));
            return result;
        }
    }
}
