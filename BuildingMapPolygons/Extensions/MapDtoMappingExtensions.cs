using BuildingMapPolygons.Domain;
using BuildingMapPolygons.Domain.Dtos;
using BuildingMapPolygons.Domain.Enums;
using BuildingMapPolygons.Helpers;
using System.Data;

namespace WeatherServiceApp.Extensions
{
    internal static class MapDtoMappingExtensions
    {
        private const string ELEMENT_TYPE_RELATION = "relation";
        private const string ELEMENT_TYPE_WAY = "way";
        private const string WAY_ROLE_INNER = "inner";
        private const string WAY_ROLE_OUTER = "outer";
        private const string TAG_BUILDING = "building";
        private const string TAG_VALUE_YES = "yes";

        public static GeoCoordinate MapToGeoCoordinate(this MapElementDto node)
        {
            var coordinate = new GeoCoordinate(node.Id, node.Lat!.Value, node.Lon!.Value);
            return coordinate;
        }

        public static MercatorCoordinate MapToMercatorCoordinate(this MapElementDto node)
        {
            var (x, y) = SphericalMercator.ToPixel(node.Lat!.Value, node.Lon!.Value);
            var coordinate = new MercatorCoordinate(node.Id, x, y);
            return coordinate;
        }

        private static BuildingPolygon<T> MapToBuildingPolygon<T>(this RelationMemberDto relationMember, Dictionary<long, MapElementDto> allElementsDictionary, Func<MapElementDto, T> coordinateSelector) where T : ICoordinate
        {
            var way = allElementsDictionary[relationMember.Ref];

            var nodes = way.Nodes
                .Select(wayNodeId => allElementsDictionary[wayNodeId])
                .Select(node => coordinateSelector(node))
                .ToArray();

            var role = relationMember.Role == WAY_ROLE_OUTER ? MemberRole.Outer : MemberRole.Inner;
            var result = new BuildingPolygon<T>(way.Id, role, nodes);
            return result;
        }

        private static BuildingPolygon<T> MapToBuildingPolygon<T>(this MapElementDto way, MemberRole role, Dictionary<long, MapElementDto> allElementsDictionary, Func<MapElementDto, T> coordinateSelector) where T : ICoordinate
        {
            var nodes = way.Nodes
                .Select(wayNodeId => allElementsDictionary[wayNodeId])
                .Select(node => coordinateSelector(node))
                .ToArray();

            var result = new BuildingPolygon<T>(way.Id, role, nodes);
            return result;
        }


        private static Building<T> MapToBuilding<T>(this MapElementDto relationOrWay, Dictionary<long, MapElementDto> allElementsDictionary, Func<MapElementDto, T> coordinateSelector) where T : ICoordinate
        {
            Building<T> building;

            switch (relationOrWay.Type)
            {
                case ELEMENT_TYPE_WAY:
                    building = new Building<T>(relationOrWay.Id, BuildingType.Way, new BuildingPolygon<T>[] { relationOrWay.MapToBuildingPolygon<T>(MemberRole.Outer, allElementsDictionary, coordinateSelector) });
                    break;
                case ELEMENT_TYPE_RELATION:
                    var buildingPolygons = relationOrWay.Members
                        .Select(member => member.MapToBuildingPolygon(allElementsDictionary, coordinateSelector))
                        .ToArray();
                    building = new Building<T>(relationOrWay.Id, BuildingType.Relation, buildingPolygons);
                    break;

                default:
                    throw new ArgumentException(nameof(relationOrWay));
            }

            return building;
        }

        public static Building<T>[] MapToBuildings<T>(this MapDto mapDto, Func<MapElementDto,T> coordinateSelector) where T : ICoordinate
        {
            var elementsDictionary = mapDto.Elements.ToDictionary(x => x.Id);
            var buildingElementDtos = mapDto.Elements
                .Where(x =>
                {
                    var hasBuildingTag = x.Tags.TryGetValue(TAG_BUILDING, out string? yesValue);
                    return (yesValue != null && yesValue == TAG_VALUE_YES);
                })
                .Select(buildingElementDto => buildingElementDto.MapToBuilding(elementsDictionary, coordinateSelector))
                .ToArray();
            return buildingElementDtos;
        }
    }
}
