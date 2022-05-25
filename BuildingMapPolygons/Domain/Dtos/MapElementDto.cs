

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildingMapPolygons.Domain.Dtos

{
    public class MapElementDto
    {
        //[JsonConverter(typeof(StringEnumConverter))]
        //(relation | way | node)
        //(building | buildingPolygon | buildingPolygonNode)
        public string Type { get; set; } = string.Empty; 
        public long Id {get;set;}
        public double? Lat { get; set; } //polygon node
        public double? Lon { get; set; } //polygon node

        public DateTime Timestamp { get; set; }
        public int Version {get;set;}
        public long Changeset {get;set;}
        public string? User {get;set;}
        public long Uid {get;set;}

        public long[] Nodes { get; set; } = new long[] { };//for way (polygon part of building)
        //for relation(building)
        //ways
        public RelationMemberDto[] Members { get; set; } = new RelationMemberDto[] { };
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    }

    public class RelationMemberDto
    {
        //[JsonConverter(typeof(StringEnumConverter))]
        public string Type {get;set;} = string.Empty;
        public long Ref {get;set;} // ref to way (polygon)
        public string Role { get; set; } = string.Empty;
    }
}