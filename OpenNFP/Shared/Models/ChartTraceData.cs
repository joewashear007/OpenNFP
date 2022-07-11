namespace OpenNFP.Shared.Models
{
    public class ChartTraceData
    {
        public List<object?> Values { get; } = new List<object?>();
        public List<string> Annotations { get; } = new List<string>();

        public void Add(object? value, string annotation)
        {
            Values.Add(value);
            Annotations.Add(annotation);
        }

        public void Add(object? value)
        {
            Values.Add(value);
            Annotations.Add(value?.ToString() ?? string.Empty);
        }
    }
}
