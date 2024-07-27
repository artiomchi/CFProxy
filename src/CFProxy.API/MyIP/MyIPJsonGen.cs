using System.Text.Json.Serialization;

namespace CFProxy.API.MyIP;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(IPAddressResponse))]
[JsonSerializable(typeof(MyIPConfig))]
internal partial class MyIPJsonGen : JsonSerializerContext
{
}
