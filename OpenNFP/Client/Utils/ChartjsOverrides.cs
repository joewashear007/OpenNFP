using PSC.Blazor.Components.Chartjs.Interfaces;
using PSC.Blazor.Components.Chartjs.Models.Common;
using System.Text.Json.Serialization;

namespace OpenNFP.Client
{

    public class YAxesEx : YAxes
    {
        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Position { get; set; } = "center";
    }

    public class TicksEx : Ticks
    {
        [JsonPropertyName("display")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Display { get; set; } = false;
    }

    public class LineDatasetEx : Dataset
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<decimal?> Data { get; set; }

        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BackgroundColor { get; set; }

        [JsonPropertyName("borderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BorderColor { get; set; }

        [JsonPropertyName("fill")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Fill { get; set; }

        [JsonPropertyName("tension")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal Tension { get; set; }

    }

    public class LineDataEx : Data<LineDatasetEx>
    {

    }

    public class LineChartConfigEx : IChartConfig
    {
        [JsonIgnore]
        public string CanvasId { get; } = Guid.NewGuid().ToString();


        [JsonPropertyName("type")]
        public string Type { get; set; } = "line";


        [JsonPropertyName("data")]
        public LineDataEx Data { get; set; } = new LineDataEx();


        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Options Options { get; set; }
    }
}
