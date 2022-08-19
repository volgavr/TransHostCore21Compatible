//using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TransHostService.Models;

namespace TransHostService.Helpers
{

    public struct GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static double DistanceBetween(GeoPoint p1, GeoPoint p2, DistanceUnit unit = DistanceUnit.Km)
        {
            var model = new GlobeModelSimple(unit);
            return model.Distance(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude);
        }
    }

    public static class JsonHelper
    {
        public static async Task<GeoPoint> GetAirportCoordinatesAsync(HttpClient client, string iataCode)
        {
            if(string.IsNullOrEmpty(client?.BaseAddress?.ToString()))
                throw new InvalidOperationException("BaseAddress parameter is not to be empty.");

            bool isIataValid = true;
            if (iataCode?.Length == 3)
                foreach (char ch in iataCode)
                {
                    if (ch.CompareTo('A') >= 0 && ch.CompareTo('Z') <= 0) continue;
                    else isIataValid = false;
                    
                }
            else isIataValid = false;
            if(!isIataValid)
                throw new ArgumentException(nameof(iataCode) + " is expected to make of 3 uppercase letters");

            var jsonString = await client.GetStringAsync(client.BaseAddress + iataCode);
            return GetLocation(jsonString);
        }

        private static GeoPoint GetLocation(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);// JsonNode.Parse(jsonString).AsObject();
            // read data from DOM
            var loc = jsonObject["location"];
            var coord = new GeoPoint();
            if (loc != null)
            {
                coord.Latitude = (double)loc["lat"];
                coord.Longitude = (double)loc["lon"];
            }            
            return coord;           
        }
    }
}
