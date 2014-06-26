using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            return Regex.IsMatch(candidate, @"(?<vertical>(\d{2}|\d{4})[N,S])(?<horizontal>((\d{3}|\d{5})[E,W]))");
        }
        public static bool isNavaidPoint(string candidate)
        {
            return Regex.IsMatch(candidate, @"(?<navaid>\w{2,3})(?<bearing>\d{3})(?<distance>\d{3})");
        }
        public static bool isSpeedLevel(string candidate)
        {
            return Regex.IsMatch(candidate, @"(?<speed>[K,N]\d{4}|M\d{3})(?<clevel>([A,F]\d{3})|[S,M]\d{4})");
        }
        public static bool isAirway(string candidate)
        {
            return Regex.IsMatch(candidate, @"(\D{1,4}\d{1,4})");
        }
        public static bool isChangeOfSpeedLevelPoint(string candidate)
            {
                try{
                string [] candidateparts = candidate.Split('/');
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
        public static bool isDirect(string candidate)
        {
            return candidate.Equals("DCT");
        }
        public interface IRouteElement
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
            private string _verticalcoord;
            private string _horizontalcoord;

            public CoordinatePoint(string verticalcoord, string horizontalcoord)
            {
                this._verticalcoord = verticalcoord;
                this._horizontalcoord = horizontalcoord;
            }
            public CoordinatePoint(string verticalcoordhorizontalcoord)
            {
                Match m = Regex.Match(verticalcoordhorizontalcoord, @"(?<vertical>(\d{2}|\d{4})[N,S])(?<horizontal>((\d{3}|\d{5})[E,W]))");
                this._verticalcoord = m.Groups["vertical"].Value;
                this._horizontalcoord = m.Groups["horizontal"].Value;
            }
            public string getRepresentation()
            {
                return string.Format("{0}{1}", this._verticalcoord, this._horizontalcoord);
            }
            //NOT YET IMPLEMENTED
            public string latitude { get { return string.Empty; } }
            //NOT YET IMPLEMENTED
            public string longitude { get { return string.Empty; } }
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
            public NavaidPoint(string navaidnamebearingdistance)
            {
                Match m = Regex.Match(navaidnamebearingdistance, @"(?<navaid>\w{2,3})(?<bearing>\d{3})(?<distance>\d{3})");
                this._navaidname = m.Groups["navaid"].Value;
                this._bearing = m.Groups["bearing"].Value;
                this._distance = m.Groups["distance"].Value;

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

            enum SpeedUnit { Kilometer="K",Knot="N",Mach="M"}
            enum LevelUnit { FlightLevel="F", StandardMetricLevel="S", AltitudeInFeet = "A", AltitudeInMeter="M"}
            
            public SpeedLevel(string cruisingspeed, string flightlevel)
            {
                this._cruisingspeed = cruisingspeed;
                this._flightlevel = flightlevel;
            }
            public SpeedLevel(string speedlevel)
            {
                Match m =  Regex.Match(speedlevel, @"(?<speed>[K,N]\d{4}|M\d{3})(?<clevel>([A,F]\d{3})|[S,M]\d{4})");
                this._cruisingspeed = m.Groups["speed"].Value;
                this._flightlevel = m.Groups["clevel"].Value;
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
            public ChangeOfSpeedLevelPoint(string pointlevel)
            {

                    string[] pointlevelparts = pointlevel.Split('/');
                    this._point = pointlevelparts[0];
                    this._speedlevel = pointlevelparts[1];
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
        class Direct : IRouteElement
        {
            public string getRepresentation()
            {
                return "DCT";
            }
        }
        private List<IRouteElement> _path;
        public Route()
        {
            this._path = new List<IRouteElement>();
        }
        public List<IRouteElement> path
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }

        public static Route StringToRoute(string routestring)
        {
            Route result = new Route();
            String[] routeelements = routestring.Split();
            for (int i = 0; i < routeelements.Length; i++ )
            {
                if (isDirect(routeelements[i]))
                {
                    result.path.Add(new Direct());
                    continue;
                }
                if (isChangeOfFlightRule(routeelements[i]))
                {
                    result.path.Add(new ChangeOfFlightRule(routeelements[i]));
                    continue;
                }
                if (isChangeOfSpeedLevelPoint(routeelements[i]))
                {
                    result.path.Add(new ChangeOfSpeedLevelPoint(routeelements[i]));
                    continue;
                }
            }
            return result;
        }
        public static IRouteElement StringToRouteElement(string elementstring)
        {
            if (isChangeOfFlightRule(elementstring)) { return new ChangeOfFlightRule(elementstring); }
            if (isChangeOfSpeedLevelPoint(elementstring)) { return new ChangeOfSpeedLevelPoint(elementstring); }
            if (isSpeedLevel(elementstring)) { return new SpeedLevel(elementstring); }
            if (isCoordinatePoint(elementstring)) { return new CoordinatePoint(elementstring); }
            if (isNavaidPoint(elementstring)) { return new NavaidPoint(elementstring); }
            if (isAirway(elementstring)) { return new Airway(elementstring); }
            if (isNamedPoint(elementstring)) { return new NamedPoint(elementstring); }
            else
            {
                throw new Exception("YO IT FELL THROUGH!");
            }
        }
    }
}
