using BuildingMapPolygons.Domain;
using BuildingMapPolygons.Domain.Dtos;
using System.Data;

namespace BuildingMapPolygons.Services
{
    internal static class MapDtoMappingExtensions
    {
        const string ELEMENT_TYPE_RELATION = "relation";
        const string ELEMENT_TYPE_WAY = "way";
        const string WAY_ROLE_INNER = "inner";
        const string WAY_ROLE_OUTER = "outer";
        const string TAG_BUILDING = "building";
        const string TAG_VALUE_YES = "yes";

        public static Coordinate MapToCoordinate(this MapElementDto node)
        {
            var coordinate = new Coordinate(node.Id, node.Lat!.Value, node.Lon!.Value);
            return coordinate;
        }

        public static BuildingPolygon MapToBuildingPolygon(this RelationMemberDto relationMember, Dictionary<long, MapElementDto> allElementsDictionary)
        {
            var way = allElementsDictionary[relationMember.Ref];

            var nodes = way.Nodes
                .Select(wayNodeId => allElementsDictionary[wayNodeId])
                .Select(node => node.MapToCoordinate())
                .ToArray();

            var role = relationMember.Role == WAY_ROLE_OUTER ? MemberRole.Outer : MemberRole.Inner;
            var result = new BuildingPolygon(way.Id, role, nodes);
            return result;
        }

        public static BuildingPolygon MapToBuildingPolygon(this MapElementDto way, MemberRole role, Dictionary<long, MapElementDto> allElementsDictionary)
        {
            var nodes = way.Nodes
                .Select(wayNodeId => allElementsDictionary[wayNodeId])
                .Select(node => node.MapToCoordinate())
                .ToArray();

            var result = new BuildingPolygon(way.Id, role, nodes);
            return result;
        }


        public static Building MapToBuilding(this MapElementDto relationOrWay, Dictionary<long, MapElementDto> allElementsDictionary)
        {
            Building building;

            switch (relationOrWay.Type)
            {
                case ELEMENT_TYPE_WAY:
                    building = new Building(relationOrWay.Id, BuildingType.Way, new BuildingPolygon[] { relationOrWay.MapToBuildingPolygon(MemberRole.Outer, allElementsDictionary) });
                    break;
                case ELEMENT_TYPE_RELATION:
                    var buildingPolygons = relationOrWay.Members
                        .Select(member => member.MapToBuildingPolygon(allElementsDictionary))
                        .ToArray();
                    building = new Building(relationOrWay.Id, BuildingType.Relation, buildingPolygons);
                    break;

                default:
                    throw new ArgumentException(nameof(relationOrWay));
            }

            return building;
        }

        public static Building[] MapToBuildings(this MapDto mapDto)
        {
            var elementsDictionary = mapDto.Elements.ToDictionary(x => x.Id);

            var buildingElementDtos = mapDto.Elements
                .Where(x =>
                {
                    var hasBuildingTag = x.Tags.TryGetValue(TAG_BUILDING, out string? yesValue);
                    if (yesValue != null && yesValue == TAG_VALUE_YES)
                    {
                        return true;
                    }
                    return false;
                })
                .Select(buildingElementDto => buildingElementDto.MapToBuilding(elementsDictionary))
                .ToArray();
            return buildingElementDtos;
        }
    }
}
