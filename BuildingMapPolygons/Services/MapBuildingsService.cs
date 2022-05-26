using BuildingMapPolygons.Domain;
using BuildingMapPolygons.Domain.Dtos;
using BuildingMapPolygons.Helpers;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Web;
using WeatherServiceApp.Extensions;

namespace BuildingMapPolygons.Services
{
    internal interface IBuildingsPolygonsMapService
    {
        Task<Building<GeoCoordinate>[]> GetMapBuildingsWithGeoCoordinatesAsync(double latitude, double longitude, double radiusM);
        Task<Building<MercatorCoordinate>[]> GetMapBuildingsWithMercatorCoordinatesAsync(double x, double y, double radiusM);
    }

    internal class MapBuildingsService : IBuildingsPolygonsMapService
    {
        private const string BASE_API_ADDRESS = "https://www.openstreetmap.org/api/0.6/";
        private const string WAY_REQUEST = "way/{0}";
        private const string MAP_REQUEST = "map";
        private const string MAP_REQUEST_PARAMETERS_BBOX = "bbox";

        private readonly HttpClient _client;
        public MapBuildingsService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private Uri CreateRequestUri(string relatedUrl, Dictionary<string, string>? queryParams = null)
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            if (queryParams != null)
            {
                foreach (var queryParam in queryParams)
                {
                    queryString[queryParam.Key] = queryParam.Value;
                }
            }

            var uriBuilder = new UriBuilder(BASE_API_ADDRESS);
            uriBuilder.Query = queryString.ToString();
            uriBuilder.AppendToPath(relatedUrl);
            var uriReult = uriBuilder.Uri;
            return uriReult;
        }

        private async Task<HttpResponseMessage> GetRequestAsync(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode) throw new Exception(response.StatusCode.ToString());
            return response;
        }
        public async Task<MapDto> GetMapAsync(double latitude, double longitude, double radiusM)
        {
            CoordinateBoundaries mapBoundaries = new CoordinateBoundaries(latitude, longitude, radiusM, DistanceUnit.Meters);
            var paramsDict = new Dictionary<string, string>();
            paramsDict[MAP_REQUEST_PARAMETERS_BBOX] = $"{mapBoundaries.MinLongitude},{mapBoundaries.MinLatitude},{mapBoundaries.MaxLongitude},{mapBoundaries.MaxLatitude}";
            var requestUri = CreateRequestUri(MAP_REQUEST, paramsDict);
            Console.WriteLine(requestUri.ToString());
            using var response = await GetRequestAsync(requestUri.ToString());

            var mapJson = await response.Content.ReadAsStringAsync();

            //await File.WriteAllTextAsync("E:\\C# testRepo\\Building_pologons_Map\\mapData.json", mapJson); 

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true }
                },
                Formatting = Formatting.Indented
            };

            var result = JsonConvert.DeserializeObject<MapDto>(mapJson, settings);
            return result!;
        }

        public async Task<Building<GeoCoordinate>[]> GetMapBuildingsWithGeoCoordinatesAsync(double latitude, double longitude, double radiusM)
        {
            var map = await GetMapAsync(latitude, longitude, radiusM);
            var buildings = map.MapToBuildings(MapDtoMappingExtensions.MapToGeoCoordinate);
            return buildings;
        }

        public async Task<Building<MercatorCoordinate>[]> GetMapBuildingsWithMercatorCoordinatesAsync(double x, double y, double radiusM)
        {
            (double latitude, double longitude) = SphericalMercator.ToGeoCoord(x, y);
            var map = await GetMapAsync(latitude, longitude, radiusM);
            var buildings = map.MapToBuildings(MapDtoMappingExtensions.MapToMercatorCoordinate);
            return buildings;
        }
    }
}
