using System.Text.Json.Serialization;

namespace Movies.Contracts.Responses
{
    public class HalResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Link> Links { get; set; } = new();
    }

    public class Link
    {
        public required string Rel { get; set; }
        public required string Href { get; set; }
        public required string Type { get; set; }
    }
}
