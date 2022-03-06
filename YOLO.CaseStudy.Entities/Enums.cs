using System.Text.Json.Serialization;

namespace YOLO.CaseStudy.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WordProcessType
    {
        ReverseWords = 1,
        ReverseCharacters = 2
    }
}
