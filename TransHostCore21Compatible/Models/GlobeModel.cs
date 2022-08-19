using System;

namespace TransHostService.Models
{
    public enum DistanceUnit { Km, Mile }
    
    public abstract class GlobeModel
    {
        public abstract double Distance(double lat1, double lon1, double lat2, double lon2);        
    }


    public class GlobeModelSimple : GlobeModel
    {
        readonly DistanceUnit lenUnit = DistanceUnit.Km;
        public GlobeModelSimple()
        {
        }
        public GlobeModelSimple(DistanceUnit unit)
        {
            lenUnit = unit;
        }

        public override double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            return Distance(lat1, lon1, lat2, lon2, lenUnit == DistanceUnit.Mile);
        }
        private static double Distance(double lat1, double lon1, double lat2, double lon2, bool inMiles)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(theta));
                dist = Math.Acos(dist);
                dist = Rad2deg(dist);
                dist = dist * 60 * 1.1515 * (inMiles ? 1 : 1.609344);                 
                return dist;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
        
    }   
}
