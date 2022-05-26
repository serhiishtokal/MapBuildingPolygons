namespace BuildingMapPolygons.Helpers
{
    using System;

    public class SphericalMercator
    {
        private static readonly double DEG2RAD = Math.PI / 180.0;
        private static readonly double RAD2Deg = 180.0 / Math.PI;
        public static double RADIUS = 6378137.0; /* in meters on the equator */

        /* These functions take their length parameter in meters and return an angle in degrees */

        public static double YToLat(double aY)
        {
            return RadToDeg(Math.Atan(Math.Exp(aY / RADIUS)) * 2 - Math.PI / 2);
        }
        public static double XToLon(double aX)
        {
            return RadToDeg(aX / RADIUS);
        }

        public static (double X, double Y) ToPixel(double lat, double lon)
        {
            return ( LonToX(lon), LatToY(lat) );
        }

        public static (double Lat, double Lon) ToGeoCoord(double x, double y)
        {
            return (YToLat(y), XToLon(x));
        }

        public static double LatToY(double aLat)
        {
            return Math.Log(Math.Tan(Math.PI / 4 + DegToRad(aLat) / 2)) * RADIUS;
        }
        public static double LonToX(double aLong)
        {
            return DegToRad(aLong) * RADIUS;
        }

        private static double RadToDeg(double rad)
        {
            return rad * RAD2Deg;
        }

        private static double DegToRad(double deg)
        {
            return deg * DEG2RAD;
        }
    }
}
