namespace Lumina.Systems.DynamicHdrp
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal enum DynamicHdrpPropertyType
    {
        Float,
        Bool,
    }

    internal sealed class DynamicHdrpPropertyDescriptor
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public DynamicHdrpPropertyType Type { get; set; }

        public float Min { get; set; }

        public float Max { get; set; }

        public float Step { get; set; }
    }

    internal sealed class DynamicHdrpComponentDescriptor
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string Category { get; set; }

        public bool DefaultEnabled { get; set; }

        public List<DynamicHdrpPropertyDescriptor> Properties { get; set; } = new List<DynamicHdrpPropertyDescriptor>();
    }

    internal static class DynamicHdrpJson
    {
        public static string BuildMetadataJson(IEnumerable<DynamicHdrpComponentDescriptor> components)
        {
            var sb = new StringBuilder();
            sb.Append("[");

            bool isFirst = true;
            foreach (var component in components)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }

                isFirst = false;
                sb.Append("{");
                sb.Append("\"id\":\"").Append(Escape(component.Id)).Append("\",");
                sb.Append("\"displayName\":\"").Append(Escape(component.DisplayName)).Append("\",");
                sb.Append("\"category\":\"").Append(Escape(component.Category)).Append("\",");
                sb.Append("\"defaultEnabled\":").Append(component.DefaultEnabled ? "true" : "false").Append(",");
                sb.Append("\"properties\":[");

                for (int i = 0; i < component.Properties.Count; i++)
                {
                    var property = component.Properties[i];
                    if (i > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append("{");
                    sb.Append("\"id\":\"").Append(Escape(property.Id)).Append("\",");
                    sb.Append("\"displayName\":\"").Append(Escape(property.DisplayName)).Append("\",");
                    sb.Append("\"type\":\"").Append(property.Type == DynamicHdrpPropertyType.Float ? "float" : "bool").Append("\",");
                    sb.Append("\"min\":").Append(property.Min.ToString(CultureInfo.InvariantCulture)).Append(",");
                    sb.Append("\"max\":").Append(property.Max.ToString(CultureInfo.InvariantCulture)).Append(",");
                    sb.Append("\"step\":").Append(property.Step.ToString(CultureInfo.InvariantCulture));
                    sb.Append("}");
                }

                sb.Append("]");
                sb.Append("}");
            }

            sb.Append("]");
            return sb.ToString();
        }

        public static string BuildStateJson(IEnumerable<DynamicHdrpComponentRuntimeState> state)
        {
            var sb = new StringBuilder();
            sb.Append("[");

            bool isFirst = true;
            foreach (var component in state)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }

                isFirst = false;
                sb.Append("{");
                sb.Append("\"componentId\":\"").Append(Escape(component.ComponentId)).Append("\",");
                sb.Append("\"enabled\":").Append(component.Enabled ? "true" : "false").Append(",");
                sb.Append("\"values\":{");

                int propertyCounter = 0;
                foreach (var kvp in component.Values)
                {
                    if (propertyCounter > 0)
                    {
                        sb.Append(",");
                    }

                    propertyCounter++;
                    sb.Append("\"").Append(Escape(kvp.Key)).Append("\":");

                    if (kvp.Value is bool boolValue)
                    {
                        sb.Append(boolValue ? "true" : "false");
                    }
                    else if (kvp.Value is float floatValue)
                    {
                        sb.Append(floatValue.ToString(CultureInfo.InvariantCulture));
                    }
                    else if (kvp.Value is int intValue)
                    {
                        sb.Append(intValue.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        sb.Append("\"").Append(Escape(kvp.Value?.ToString() ?? string.Empty)).Append("\"");
                    }
                }

                sb.Append("}");
                sb.Append("}");
            }

            sb.Append("]");
            return sb.ToString();
        }

        private static string Escape(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }

    internal sealed class DynamicHdrpComponentRuntimeState
    {
        public string ComponentId { get; set; }

        public bool Enabled { get; set; }

        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
    }
}
