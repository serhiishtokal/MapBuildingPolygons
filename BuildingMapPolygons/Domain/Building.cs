using BuildingMapPolygons.Domain.Enums;

namespace BuildingMapPolygons.Domain
{
    internal class Building
    {
        public long Id { get; set; }
        public BuildingType Type { get; set; }
        public BuildingPolygon[] BuildingPolygons { get; set; } = new BuildingPolygon[0];

        public Building(long id, BuildingType type, BuildingPolygon[] buildingPolygons)
        {
            Id = id;
            BuildingPolygons = buildingPolygons;
            Type = type;
        }
    }

    internal class BuildingPolygon
    {
        public long Id { get; set; }
        public MemberRole Role { get; set; }
        public Coordinate[] Nodes { get; set; } = new Coordinate[0];
        public BuildingPolygon(long id, MemberRole role, Coordinate[] nodes)
        {
            Id = id;
            Role = role;
            Nodes = nodes;
        }
    }

    internal class Coordinate
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Coordinate(long id, double latitude, double longitude)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
