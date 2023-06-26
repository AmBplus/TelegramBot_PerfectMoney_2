using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace testProject.Settings;

public class VerifyAccountSettings
{
    [JsonPropertyName("address")]
    public string Address { get; set; }
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; }
    [JsonPropertyName("trackId")]
    public string TrackId { get; set; }
}
