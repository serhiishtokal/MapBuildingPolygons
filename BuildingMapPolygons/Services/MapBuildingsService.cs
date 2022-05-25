﻿using BuildingMapPolygons.Domain;
using BuildingMapPolygons.Domain.Dtos;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Web;
using WeatherServiceApp.Extensions;
using Coordinate = BuildingMapPolygons.Domain.Coordinate;

namespace BuildingMapPolygons.Services
{
    internal interface IBuildingsPolygonsMapService
    {
        Task<Building[]> GetMapBuildingsAsync(double latitude, double longitude, double radiusM);
    }

    internal class MapBuildingsService : IBuildingsPolygonsMapService
    {
        private const string BASE_API_ADDRESS = "https://www.openstreetmap.org/api/0.6/";
        private const string WAY_REQUEST = "way/{0}";
        private const string MAP_REQUEST = "map";

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

        private async Task<HttpResponseMessage> GetAsync(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode) throw new Exception(response.StatusCode.ToString());
            return response;
        }

        //public async Task<HttpResponseMessage> GetWayAsync(int wayId = 483754843)
        //{
        //    var requestUri = CreateRequestUri(string.Format(WAY_REQUEST, wayId));
        //    HttpResponseMessage response = await _client.GetAsync(requestUri);

        //    var responseJson = await response.Content.ReadAsStringAsync();

        //    var settings = new JsonSerializerSettings
        //    {
        //        ContractResolver = new DefaultContractResolver
        //        {
        //            NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true }
        //        },
        //        Formatting = Formatting.Indented
        //    };

        //    var result = JsonConvert.DeserializeObject<object>(responseJson, settings);
        //    return response;
        //}

        public async Task<Building[]> GetMapBuildingsAsync(double latitude, double longitude, double radiusM)
        {

            var map = await GetMapAsync(latitude, longitude, radiusM);
            var buildings = map.MapToBuildings();
            return buildings;
        }

        public async Task<MapDto> GetMapAsync(double latitude, double longitude, double radiusM)
        {
            CoordinateBoundaries mapBoundaries = new CoordinateBoundaries(latitude, longitude, radiusM, DistanceUnit.Meters);
            var paramsDict = new Dictionary<string, string>();
            paramsDict["bbox"] = $"{mapBoundaries.MinLongitude},{mapBoundaries.MinLatitude},{mapBoundaries.MaxLongitude},{mapBoundaries.MaxLatitude}";
            var requestUri = CreateRequestUri(MAP_REQUEST, paramsDict);

            using var response = await GetAsync(requestUri.ToString());

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

    }
}