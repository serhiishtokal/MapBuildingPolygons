// See https://aka.ms/new-console-template for more information
using BuildingMapPolygons.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var latitude = 51.24631;
var longitude = 22.55780;
var radiusM = 150;

IBuildingsPolygonsMapService mapService = new MapBuildingsService();


var buildings = await mapService.GetMapBuildingsAsync(latitude, longitude, radiusM);

JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
{
    WriteIndented = true,
    Converters ={
        new JsonStringEnumConverter()
    }
};

var buildingsJson =  JsonSerializer.Serialize(buildings, jsonSerializerOptions);

Console.WriteLine(buildingsJson);
Console.WriteLine("Enter to exit:");
Console.ReadLine();