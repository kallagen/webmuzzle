using System;
using System.Text.Json.Serialization;

namespace TSensor.Proxy.Commands
{
    public class ControllerCommand
    {
        [JsonPropertyName("command")]
        public string Command { get; set; }
        
        [JsonPropertyName("ccGuid")]
        public string Guid { get; set; }
    }
}