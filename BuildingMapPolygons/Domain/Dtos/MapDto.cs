namespace BuildingMapPolygons.Domain.Dtos
{
    internal class MapDto
    {
        public MapBoundsDto? Bounds { get; set; }
        public MapElementDto[] Elements { get; set; } = new MapElementDto[] {};
    }

    internal class MapBoundsDto
    {
        public double Minlat { get; set; }
        public double Minlon { get; set; }
        public double Maxlat { get; set; }
        public double Maxlon { get; set; }
    }
}
