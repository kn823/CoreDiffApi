using Newtonsoft.Json;

namespace CoreDiffApi.Models
{
    /// <summary>
    /// Input Base64 Encode data from http endpoint POST
    /// </summary>
    public class DiffEncode64Data
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
