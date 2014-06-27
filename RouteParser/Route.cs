using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
namespace RouteParser
{
    public class Route
    {
        //Unit enumerations and symbols
        public class Symbol : Attribute {
            public string symbol { get; protected set; }
            public Symbol(string value) {
                this.symbol = value;
            }
        }
        public static string GetEnumSymbol(Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            Symbol[] attribs = fieldInfo.GetCustomAttributes(
                typeof(Symbol), false) as Symbol[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].symbol : null;
        }
        public enum SpeedUnit{
            [Symbol("K")]
            Kilometer=0,
            [Symbol("N")]
            Knot=1,
            [Symbol("M")]
            Mach=2
        }
        public enum LevelUnit{
            [Symbol("F")]
            FlightLevel=0,
            [Symbol("S")]
            StandardMetricLevel=1,
            [Symbol("A")]
            AltitudeInFeet=2,
            [Symbol("M")]
            AltitudeInMeter=3
        }
        //Route elements begin
        abstract class RouteElement
        {
            private string _representation;
            public string representation
            {
                get
                {
                    return this._representation;
                }
            }
            public RouteElement(string representation)
            {

                this._representation = representation;
            }
        }     
        class NamedPoint : RouteElement
        {
            public static bool isValid(string candidate)
            {
                return Regex.IsMatch(candidate, @"\D{2,5}");
            }
            private string _name;

            public NamedPoint(string name)
                : base(name)
                
            {
                this._name = name;
            }
        }
        class CoordinatePoint : RouteElement
        {
            private string _verticalcoordinate;
            private string _horizontalcoordinate;

            public static bool isValid(string candidate)
            {
                return Regex.IsMatch(candidate, @"(?<vertical>(\d{2}|\d{4})[N,S])(?<horizontal>((\d{3}|\d{5})[E,W]))");
            }

            public CoordinatePoint(string verticalcoordinate, string horizontalcoordinate)
                :base(verticalcoordinate+horizontalcoordinate)
            {
                this._verticalcoordinate = verticalcoordinate;
                this._horizontalcoordinate = horizontalcoordinate;
            }
        }
        class NavaidPoint:RouteElement
        {
            private string _navaidname;
            private string _bearing;
            private string _distance;

            public static bool isValid(string candidate){
                return Regex.IsMatch(candidate, @"(?<navaid>\w{2,3})(?<bearing>\d{3})(?<distance>\d{3})");
            }
            public NavaidPoint(string navaidname, string bearing, string distance)
                :base(navaidname+bearing+distance)
            {
                this._navaidname = navaidname;
                this._bearing = bearing;
                this._distance = distance;
                
            }
            
        }
        class Direct:RouteElement{
            public static bool isValid(string candidate){
                return Regex.IsMatch(candidate, @"DCT");
            }
            public Direct()
            :base("DCT"){}

        }
        class ChangeOfFlightRule:RouteElement{
            private string _flightrule;
            public static bool isValid(string candidate){
                return Regex.IsMatch(candidate, @"IFR|VFR");
            }
            public ChangeOfFlightRule(string flightrule)
             
            :base(flightrule)
            {
                this._flightrule = flightrule;
            }
        }
        class SpeedLevel:RouteElement{
            private SpeedUnit _speedunit;
            private string _speed;
            private LevelUnit _levelunit;
            private string _level;

            public static bool isValid(string candidate){
                return Regex.IsMatch(candidate, @"(?<speed>[K,N]\d{4}|M\d{3})(?<clevel>([A,F]\d{3})|[S,M]\d{4})");
            }
            public SpeedLevel(SpeedUnit speedunit, string speed, LevelUnit levelunit, string level)
                :base(GetEnumSymbol(speedunit)+speed+GetEnumSymbol(levelunit)+level)
            {
                this._speedunit = speedunit;
                this._speed = speed;
                this._levelunit = levelunit;
                this._level = level;
                
            }

            }
        class Airway:RouteElement{
            private string _designator;
            public static bool isValid(string candidate){
                return Regex.IsMatch(candidate, @"\w{2,7}");
            }
            public Airway(string designator)
            :base(designator)
            {
                this._designator =designator;

            }
        }
        class ChangeOfSpeedLevelPoint:RouteElement{
            private string _point;
            private string _changeofspeedlevel;
            public static bool  isValid(string candidate){
                string [] candidateparts = candidate.Split('/');
                if(candidateparts.Length == 2){
                    string pointpart = candidateparts[0];
                    string speedlevelpart = candidateparts[1];
                    //Check pointpart for each category and check speedlevel part.
                    return SpeedLevel.isValid(speedlevelpart) & (NavaidPoint.isValid(pointpart)|CoordinatePoint.isValid(pointpart)|NamedPoint.isValid(pointpart));
                }
                return false;
            }
            public ChangeOfSpeedLevelPoint(string point, string changeofspeedlevel)
            :base(point+"/"+changeofspeedlevel)
            {
                this._point = point;
                this._changeofspeedlevel = changeofspeedlevel;

            }

        }
        //Construction classes
        class RouteElementFactory
        {
            public static RouteElement GetRouteElement(RouteElement previous, string current, string next)
            {
                RouteElement element = null;

                //Check Direct
                if (Direct.isValid(current))
                {
                    //Create a new Direct element
                    element = new Direct();
                    return element;
                }
                //First element
                if (previous == null)
                {
                    //Either speedlevel or airway
                    if (SpeedLevel.isValid(current))
                    {
                        //Create a new SpeedLevel element
                        //Split into speed and level parts
                        Match m = Regex.Match(current, @"(?<speed>[K,N]\d{4}|M\d{3})(?<level>([A,F]\d{3})|[S,M]\d{4})");
                        string speed = m.Groups["speed"].Value;
                        string level = m.Groups["level"].Value;
                        SpeedUnit spunit;
                        LevelUnit lvlunit;
                        //Get units for speed and level
                        switch ((char)speed[0])
                        {
                            case 'K':
                                spunit = SpeedUnit.Kilometer;
                                break;
                            case 'N':
                                spunit = SpeedUnit.Knot;
                                break;
                            //case 'M'
                            default:
                                spunit = SpeedUnit.Mach;
                                break;
                        }
                        switch ((char)level[0])
                        {
                            case 'F':
                                lvlunit = LevelUnit.FlightLevel;
                                break;
                            case 'S':
                                lvlunit = LevelUnit.StandardMetricLevel;
                                break;
                            case 'A':
                                lvlunit = LevelUnit.AltitudeInFeet;
                                break;
                            //case 'M'
                            default:
                                lvlunit = LevelUnit.AltitudeInMeter;
                                break;
                        }
                        speed = speed.Substring(1);
                        level = level.Substring(1);
                        element = new SpeedLevel(spunit, speed, lvlunit, level);
                    }
                    //Definitely airway
                    else
                    {
                        element = new Airway(current);
                    }
                }
                //Middle element or last element
                else
                {
                    //Last element
                    if (next.Equals(string.Empty))
                    {
                        //Either NamedPoint, CoordinatePoint or NavaidPoint or STAR(TO BE IMPLEMENTED)
                        //Check NavaidPoint
                        if (NavaidPoint.isValid(current))
                        {
                            //Create NavaidPoint
                            Match m = Regex.Match(current, @"(?<navaid>\w{2,3})(?<bearing>\d{3})(?<distance>\d{3})");
                            string navaid = m.Groups["navaid"].Value;
                            string bearing = m.Groups["bearing"].Value;
                            string distance = m.Groups["distance"].Value;

                            element = new NavaidPoint(navaid, bearing, distance);
                        }
                        //Check CoordinatePoint
                        else if (CoordinatePoint.isValid(current))
                        {
                            //Create CoordinatePoint
                            Match m = Regex.Match(current, @"(?<vertical>(\d{2}|\d{4})[N,S])(?<horizontal>((\d{3}|\d{5})[E,W]))");
                            string vertical = m.Groups["vertical"].Value;
                            string horizontal = m.Groups["horizontal"].Value;

                            element = new CoordinatePoint(vertical, horizontal);
                        }
                        //Definitely NamedPoint
                        else{
                            element = new NamedPoint(current);
                        }

                    }
                    //Middle element
                    else
                    {
                        //Either ChangeOfFlightRule, ChangeOfSpeedLevelPoint, NavaidPoint, CoordinatePoint, NamedPoint or Airway
                        //Check ChangeOfFlightRule
                        if (ChangeOfFlightRule.isValid(current))
                        {
                            element = new ChangeOfFlightRule(current);
                        }
                        //Check ChangeOfSpeedLevelPoint
                        else if (ChangeOfSpeedLevelPoint.isValid(current))
                        {
                            string[] currentparts = current.Split('/');
                            element = new ChangeOfSpeedLevelPoint(currentparts[0], currentparts[1]);
                            return element;
                        }
                        //Check NavaidPoint
                        else if (NavaidPoint.isValid(current))
                        {
                            //Create NavaidPoint
                            Match m = Regex.Match(current, @"(?<navaid>\w{2,3})(?<bearing>\d{3})(?<distance>\d{3})");
                            string navaid = m.Groups["navaid"].Value;
                            string bearing = m.Groups["bearing"].Value;
                            string distance = m.Groups["distance"].Value;

                            element = new NavaidPoint(navaid, bearing, distance);
                        }
                        //Check CoordinatePoint
                        else if (CoordinatePoint.isValid(current))
                        {
                            //Create CoordinatePoint
                            Match m = Regex.Match(current, @"(?<vertical>(\d{2}|\d{4})[N,S])(?<horizontal>((\d{3}|\d{5})[E,W]))");
                            string vertical = m.Groups["vertical"].Value;
                            string horizontal = m.Groups["horizontal"].Value;

                            element = new CoordinatePoint(vertical, horizontal);
                        }
                        //Either NamedPoint or Airway
                        //Find the type of previous element
                        else if (previous is Airway | previous is SpeedLevel | previous is Direct)
                        {
                            //Current is a NamedPoint
                            element = new NamedPoint(current);
                        }
                        else
                        {
                            //Current is an Airway
                            element = new Airway(current);
                        }
                    }
                }
                return element;
            }
        }
        //Attributes
        private List<RouteElement> routeElements;
        //Constructor and helper methods
        public Route(string routestring)
        {
            this.routeElements = new List<RouteElement>();
            //Split string into elements
            string[] routestringparts = routestring.Split(' ');
            //Create first element
            RouteElement firstElement = RouteElementFactory.GetRouteElement(null, routestringparts[0], routestringparts[1]);
            this.routeElements.Add(firstElement);
            //Create middle elements //CHECK FOR OFF BY ONE ERROR
            for (int i = 1; i < routestringparts.Length-1; i++)
            {
                RouteElement tempElement = RouteElementFactory.GetRouteElement(this.routeElements[this.routeElements.Count-1], routestringparts[i], routestringparts[i + 1]);
                this.routeElements.Add(tempElement);
            }
            //Create last element
            RouteElement lastElement = RouteElementFactory.GetRouteElement(this.routeElements[this.routeElements.Count - 1], routestringparts[routestringparts.Length - 1], string.Empty);
            this.routeElements.Add(lastElement);
        }
        public override string ToString()
        {
            StringBuilder strbuilder = new StringBuilder();
            foreach (RouteElement element in this.routeElements)
            {
                strbuilder.Append(string.Format("Representation is {0}, Type is {1}\n", element.representation, element.ToString()));
            }
            return strbuilder.ToString();            
        }
        }
    }
