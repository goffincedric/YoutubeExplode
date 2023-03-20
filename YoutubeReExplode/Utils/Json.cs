using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace YoutubeReExplode.Utils;

internal static class Json
{
    public static string Create(Action<JsonWriter> write)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        var objectWriter = new JsonWriter(writer);
        write(objectWriter);

        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static string Extract(string source)
    {
        var buffer = new StringBuilder();

        var depth = 0;
        var isInsideString = false;

        // We trust that the source contains valid json, we just need to extract it.
        // To do it, we will be matching curly braces until we even out.
        for (var i = 0; i < source.Length; i++)
        {
            var ch = source[i];
            var chPrv = i > 0 ? source[i - 1] : default;

            buffer.Append(ch);

            // Detect if inside a string
            if (ch == '"' && chPrv != '\\')
                isInsideString = !isInsideString;
            // Opening brace
            else if (ch == '{' && !isInsideString)
                depth++;
            // Closing brace
            else if (ch == '}' && !isInsideString)
                depth--;

            // Break when evened out
            if (depth == 0)
                break;
        }

        return buffer.ToString();
    }

    public static JsonElement Parse(string source)
    {
        using var document = JsonDocument.Parse(source);
        return document.RootElement.Clone();
    }

    public static JsonElement? TryParse(string source)
    {
        try
        {
            return Parse(source);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}