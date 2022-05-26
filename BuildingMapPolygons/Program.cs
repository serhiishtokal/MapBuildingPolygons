// See https://aka.ms/new-console-template for more information
using BuildingMapPolygons.Helpers;
using BuildingMapPolygons.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var latitude = 48.136534841007986;
var longitude = 11.697632556192227;
var x = 1302174.4996421365;
var y = 6129599.500002872;
var x2 = SphericalMercator.LonToX(longitude);
var y2 = SphericalMercator.LatToY(latitude);

var radiusM = 1000;

IBuildingsPolygonsMapService mapService = new MapBuildingsService();
var buildingsGeo = await mapService.GetMapBuildingsWithGeoCoordinatesAsync(latitude, longitude, radiusM);
var buildingsMercator = await mapService.GetMapBuildingsWithMercatorCoordinatesAsync(x, y, radiusM);

JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
{
    WriteIndented = true,
    Converters ={
        new JsonStringEnumConverter()
    }
};

var buildingsGeoJson =  JsonSerializer.Serialize(buildingsGeo, jsonSerializerOptions);
var buildingsMercatorJson = JsonSerializer.Serialize(buildingsMercator, jsonSerializerOptions);

File.WriteAllText(@"D:\pawelNiemcyGeo.json", buildingsGeoJson);
File.WriteAllText(@"D:\pawelNiemcyMercator.json", buildingsMercatorJson);
Console.WriteLine("Enter to exit:");
Console.ReadLine();