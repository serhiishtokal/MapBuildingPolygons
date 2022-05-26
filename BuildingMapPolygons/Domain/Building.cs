using BuildingMapPolygons.Domain.Enums;

namespace BuildingMapPolygons.Domain
{
    internal class Building<T> where T : ICoordinate
    {
        public long Id { get; set; }
        public BuildingType Type { get; set; }
        public BuildingPolygon<T>[] BuildingPolygons { get; set; } = new BuildingPolygon<T>[0];

        public Building(long id, BuildingType type, BuildingPolygon<T>[] buildingPolygons)
        {
            Id = id;
            BuildingPolygons = buildingPolygons;
            Type = type;
        }
    }

    internal class BuildingPolygon<T> where T : ICoordinate
    {
        public long Id { get; set; }
        public MemberRole Role { get; set; }
        public T[] Nodes { get; set; } = new T[0];
        public BuildingPolygon(long id, MemberRole role, T[] nodes)
        {
            Id = id;
            Role = role;
            Nodes = nodes;
        }
    }

    internal interface ICoordinate
    {
        long Id { get; set; }
    }

    /// <summary>
    /// Epsg4326
    /// Geographic
    /// </summary>
    internal class GeoCoordinate : ICoordinate
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoCoordinate(long id, double latitude, double longitude)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
        }

        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
    /// <summary>
    /// Epsg3857
    /// Mercator pseudo projection
    /// </summary>
    internal class MercatorCoordinate : ICoordinate
    {
        public long Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public MercatorCoordinate(long id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public MercatorCoordinate(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
