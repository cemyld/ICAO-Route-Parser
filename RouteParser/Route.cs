using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace RouteParser
{
    public class Route
    {
       

        /// <summary>
        /// Base class for Enum-like strings
        /// 
        /// Includes equality plumbing in case you're dealing in copies of StringEnums.
        /// </summary>
        [Serializable]
        public abstract class StringEnum : Object
        {
            protected readonly string Name;
            protected readonly int Value;

            protected StringEnum(int value, string name)
            {
                Name = name;
                Value = value;
            }

            public override string ToString()
            {
                return Name;
            }

            public static implicit operator string(StringEnum x)
            {
                return x.Name;
            }

            public static implicit operator int(StringEnum x)
            {
                return x.Value;
            }

            public static bool operator ==(StringEnum x, StringEnum y)
            {
                if (((object)x == null) || ((object)y == null))
                {
                    return false;
                }

                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                return (x.Value == y.Value);
            }

            public static bool operator !=(StringEnum x, StringEnum y)
            {
                return !(x.Value == y.Value);
            }

            public override bool Equals(object obj)
            {
                StringEnum p = obj as StringEnum;

                if ((object)p == null)
                {
                    return false;
                }

                return (Value == p.Value);
            }

            public bool Equals(StringEnum flag)
            {
                return (Value == flag.Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }
        //String Enums begin
        /// <summary>
        /// Class for holding cruising speed units
        /// 
        /// Includes Kilometer, Knot and Mach
        /// </summary>
        [Serializable]
        public sealed class SpeedUnit : StringEnum
        {
            private static readonly SpeedUnit _Kilometer = new SpeedUnit(1, "K");
            private static readonly SpeedUnit _Knot = new SpeedUnit(2, "N");
            private static readonly SpeedUnit _Mach = new SpeedUnit(3, "M");

            public static SpeedUnit Kilometer { get { return _Kilometer; } }
            public static SpeedUnit Knot { get { return _Knot; } }
            public static SpeedUnit Mach { get { return _Mach; } }

            private SpeedUnit(int value, string name)
                : base(value, name)
            { }

            public static implicit operator SpeedUnit(string x)
            {
                switch (x)
                {
                    case "K":
                        return Kilometer;
                    case "N":
                        return Knot;
                    case "M":
                        return Mach;
                    default:
                        return default(SpeedUnit);
                }
            }

            public static implicit operator SpeedUnit(int x)
            {
                switch (x)
                {
                    case 1:
                        return Kilometer;
                    case 2:
                        return Knot;
                    case 3:
                        return Mach;
                    default:
                        return default(SpeedUnit);
                }
            }
        }
        /// <summary>
        /// Class for holding flight level units
        /// 
        /// Includes Standard Flight Level, Standard Metric, Altitude in Feet and Altitude in Meters
        /// </summary>
        [Serializable]
        public sealed class LevelUnit : StringEnum
        {
            private static readonly LevelUnit _FlightLevel = new LevelUnit(1, "F");
            private static readonly LevelUnit _StandardMetric = new LevelUnit(2, "S");
            private static readonly LevelUnit _AltitudeInFeet = new LevelUnit(3, "A");
            private static readonly LevelUnit _AltitudeInMeter = new LevelUnit(4, "M");

            public static LevelUnit FlightLevel { get { return _FlightLevel; } }
            public static LevelUnit StandardMetric { get { return _StandardMetric; } }
            public static LevelUnit AltitudeInFeet { get { return _AltitudeInFeet; } }
            public static LevelUnit AltitudeInMeter { get { return _AltitudeInMeter; } }

            private LevelUnit(int value, string name)
                : base(value, name)
            { }

            public static implicit operator LevelUnit(string x)
            {
                switch (x)
                {
                    case "F":
                        return FlightLevel;
                    case "S":
                        return StandardMetric;
                    case "A":
                        return AltitudeInFeet;
                    case "M":
                        return AltitudeInMeter;
                    default:
                        return default(LevelUnit);
                }
            }

            public static implicit operator LevelUnit(int x)
            {
                switch (x)
                {
                    case 1:
                        return FlightLevel;
                    case 2:
                        return StandardMetric;
                    case 3:
                        return AltitudeInFeet;
                    case 4:
                        return AltitudeInMeter;
                    default:
                        return default(LevelUnit);
                }
            }
        }
        /// <summary>
        /// Classes for representing route elements
        /// 
        /// Includes significant points, airways, directs, change of flight speeds and rules
        /// </summary>
        abstract class RouteElement
        {
            private string _representation;
            public string representation
            {
                get
                {
                    return _representation;
                }
            }
            public RouteElement(string representation)
            {

                _representation = representation;
            }
        }
        abstract class SignificantPoint : RouteElement
        {
            public SignificantPoint(string representation)
            :base(representation)
            {
            }
        }
        class NamedPoint : SignificantPoint
        {
            public static bool IsValid(string candidate)
            {
                return Regex.IsMatch(candidate, @"\D{2,5}");
            }
            private string _name;

            public NamedPoint(string name)
                : base(name)
                
            {
                _name = name;
            }
        }
        class CoordinatePoint : SignificantPoint
        {
            private string _verticalcoordinate;
            private string _horizontalcoordinate;

            public static bool IsValid(string candidate)
            {
                return Regex.IsMatch(candidate, @"(?<vertical>(\d{2}|\d{4})[N,S])(?<horizontal>((\d{3}|\d{5})[E,W]))");
            }

            public CoordinatePoint(string verticalcoordinate, string horizontalcoordinate)
                :base(verticalcoordinate+horizontalcoordinate)
            {
                _verticalcoordinate = verticalcoordinate;
                _horizontalcoordinate = horizontalcoordinate;
            }

            public CoordinatePoint(string representation)
            :base(representation)
            {
                Match m = Regex.Match(representation, @"(?<vertical>(\d{2}|\d{4})[N,S])(?<horizontal>((\d{3}|\d{5})[E,W]))");
                string vertical = m.Groups["vertical"].Value;
                string horizontal = m.Groups["horizontal"].Value;
                this._verticalcoordinate = vertical;
                this._horizontalcoordinate = horizontal;
            }
        }
        class NavaidPoint:SignificantPoint
        {
            private string _navaidname;
            private string _bearing;
            private string _distance;

            public static bool IsValid(string candidate){
                return Regex.IsMatch(candidate, @"(?<navaid>\w{2,3})(?<bearing>\d{3})(?<distance>\d{3})");
            }
            public NavaidPoint(string navaidname, string bearing, string distance)
                :base(navaidname+bearing+distance)
            {
                _navaidname = navaidname;
                _bearing = bearing;
                _distance = distance;                 
            }

            public NavaidPoint(string representation)
            :base(representation)
            {
                //Create NavaidPoint from string
                Match m = Regex.Match(representation, @"(?<navaid>\w{2,3})(?<bearing>\d{3})(?<distance>\d{3})");
                this._navaidname = m.Groups["navaid"].Value;
                this._bearing = m.Groups["bearing"].Value;
                this._distance = m.Groups["distance"].Value;
            }
            
        }
        class Direct:RouteElement{
            public static bool IsValid(string candidate){
                return Regex.IsMatch(candidate, @"DCT");
            }
            public Direct()
            :base("DCT"){}

        }
        class ChangeOfFlightRule:RouteElement{
            private string _flightrule;
            public static bool IsValid(string candidate){
                return Regex.IsMatch(candidate, @"IFR|VFR");
            }
            public ChangeOfFlightRule(string flightrule)
             
            :base(flightrule)
            {
                _flightrule = flightrule;
            }
        }
        class SpeedLevel:RouteElement{
            private SpeedUnit _speedunit;
            private string _speed;
            private LevelUnit _levelunit;
            private string _level;

            public static bool IsValid(string candidate){
                return Regex.IsMatch(candidate, @"(?<speed>[K,N]\d{4}|M\d{3})(?<clevel>([A,F]\d{3})|[S,M]\d{4})");
            }
            public SpeedLevel(SpeedUnit speedunit, string speed, LevelUnit levelunit, string level)
                :base(speedunit+speed+levelunit+level)
            {
                this._speedunit = speedunit;
                this._speed = speed;
                this._levelunit = levelunit;
                this._level = level;
                
            }

            public SpeedLevel(string representation)
            :base(representation)
            {
                Match m = Regex.Match(representation, @"(?<speed>[K,N]\d{4}|M\d{3})(?<level>([A,F]\d{3})|[S,M]\d{4})");
                string speed = m.Groups["speed"].Value;
                string level = m.Groups["level"].Value;
                //Get units for speed and level
                this._speedunit = speed.Substring(0, 1);
                this._levelunit = level.Substring(0, 1);
                this._speed = speed.Substring(1);
                this._level = level.Substring(1);
            }
            }
        class Airway:RouteElement{
            private string _designator;
            public static bool IsValid(string candidate){
                return Regex.IsMatch(candidate, @"\w{2,7}");
            }
            public Airway(string designator)
            :base(designator)
            {
                _designator =designator;

            }
        }
        class ChangeOfSpeedLevelPoint:RouteElement{
            private SignificantPoint _point;
            private SpeedLevel _changeofspeedlevel;
            public static bool  IsValid(string candidate){
                string [] candidateparts = candidate.Split('/');
                if(candidateparts.Length == 2){
                    string pointpart = candidateparts[0];
                    string speedlevelpart = candidateparts[1];
                    //Check pointpart for each category and check speedlevel part.
                    return SpeedLevel.IsValid(speedlevelpart) & (NavaidPoint.IsValid(pointpart)|CoordinatePoint.IsValid(pointpart)|NamedPoint.IsValid(pointpart));
                }
                return false;
            }
            public ChangeOfSpeedLevelPoint(SignificantPoint point, SpeedLevel changeofspeedlevel)
            :base(string.Format("{0}/{1}", point.representation, changeofspeedlevel.representation))
            {
                this._point = point;
                this._changeofspeedlevel = changeofspeedlevel;

            }

        }
        /// <summary>
        /// Factory class to create route elements
        /// 
        /// Needs two adjacent elements to correctly parse the current string, it is to be used by route constructor
        /// </summary>
        class RouteElementFactory
        {
            //Messy code below, BEWARE!
            public static SignificantPoint GetSignificantPoint(string current)
            {
                if (NavaidPoint.IsValid(current))
                {
                    //Create NavaidPoint
                    return new NavaidPoint(current);
                }
                //Check CoordinatePoint
                else if (CoordinatePoint.IsValid(current))
                {
                    //Create CoordinatePoint                            
                    return new CoordinatePoint(current);
                }
                //Definitely NamedPoint
                else
                {
                    return new NamedPoint(current);
                }
            }
            public static RouteElement GetRouteElement(RouteElement previous, string current, string next)
            {
                //Check Direct
                if (Direct.IsValid(current))
                {
                    //Create a new Direct element
                    return new Direct();
                }
                //First element
                if (previous == null)
                {
                    //Either speedlevel or airway
                    if (SpeedLevel.IsValid(current))
                    {
                        return new SpeedLevel(current);
                    }
                    //Definitely airway
                    else
                    {
                        return new Airway(current);
                    }
                }
                //Middle element or last element
                else
                {
                    //Last element
                    if (next.Equals(string.Empty))
                    {
                        //Either NamedPoint, CoordinatePoint or NavaidPoint or STAR(TO BE IMPLEMENTED)
                        return GetSignificantPoint(current);

                    }
                    //Middle element
                    else
                    {
                        //Either ChangeOfFlightRule, ChangeOfSpeedLevelPoint, NavaidPoint, CoordinatePoint, NamedPoint or Airway
                        //Check ChangeOfFlightRule
                        if (ChangeOfFlightRule.IsValid(current))
                        {
                            return new ChangeOfFlightRule(current);
                        }
                        //Check ChangeOfSpeedLevelPoint
                        else if (ChangeOfSpeedLevelPoint.IsValid(current))
                        {
                            
                            string[] currentparts = current.Split('/');
                            SignificantPoint _significantPoint = GetSignificantPoint(currentparts[0]);
                            SpeedLevel _speedLevel = new SpeedLevel(currentparts[1]);
                            return new ChangeOfSpeedLevelPoint(_significantPoint, _speedLevel);
                        }
                        //Check CoordinatePoint before checking airway since they can be adjacent
                        else if (CoordinatePoint.IsValid(current))
                        {
                            //Create CoordinatePoint                            
                            return new CoordinatePoint(current);
                        }
                        //Either SignificantPoint or Airway
                        //Find the type of previous element
                        else if (previous is Airway | previous is SpeedLevel | previous is Direct)
                        {
                            //Current is a NamedPoint
                            return GetSignificantPoint(current);
                        }
                        else
                        {
                            //Current is an Airway
                            return new Airway(current);
                        }
                    }
                }
            }
        }
        //Attributes
        private List<RouteElement> _routeElements;
        /// <summary>
        /// Constructs a route object from a given string in ICAO Route Plan format
        /// 
        /// Only use in conjunction with valid route strings
        /// </summary>
        /// <param name="routestring"></param>
        public Route(string routestring)
        {
            _routeElements = new List<RouteElement>();
            //Split string into tokens
            string[] routestringparts = routestring.Split(' ');
            //Create first element
            RouteElement firstElement = RouteElementFactory.GetRouteElement(null, routestringparts[0], routestringparts[1]);
            _routeElements.Add(firstElement);
            //Create middle elements //CHECK FOR OFF BY ONE ERROR
            for (int i = 1; i < routestringparts.Length-1; i++)
            {
                RouteElement tempElement = RouteElementFactory.GetRouteElement(_routeElements[_routeElements.Count-1], routestringparts[i], routestringparts[i + 1]);
                _routeElements.Add(tempElement);
            }
            //Create last element
            RouteElement lastElement = RouteElementFactory.GetRouteElement(_routeElements[_routeElements.Count - 1], routestringparts[routestringparts.Length - 1], string.Empty);
            _routeElements.Add(lastElement);
        }
        /// <summary>
        /// Prints out the representation and type of each route element in the current instance on a new line
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder strbuilder = new StringBuilder();
            foreach (RouteElement element in _routeElements)
            {
                strbuilder.Append(string.Format("Representation is {0}, Type is {1}\n", element.representation, element));
            }
            return strbuilder.ToString();            
        }
        }
    }
