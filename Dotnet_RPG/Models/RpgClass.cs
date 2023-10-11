using System.Text.Json.Serialization;

namespace Dotnet_RPG.Models;

// Property that enables us to see the proper names of the configured
// enums instead of their numeric values, e. g. Knight instead of 1.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RpgClass
{
    // Add Character Classes
    Knight = 1,
    Mage = 2,
    Cleric = 3,
}