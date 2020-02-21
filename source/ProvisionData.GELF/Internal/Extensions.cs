namespace ProvisionData.GELF.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.Json;

    internal static class GelfMessageExtensions
    {
        public static Byte[] GetBytes(this Message message) => Encoding.UTF8.GetBytes(message.GetJson());

        public static String GetJson(this Message message)
        {
            using var stream = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(stream);

            jsonWriter.WriteStartObject();
            jsonWriter.MaybeWrite("version", message.Version);
            jsonWriter.MaybeWrite("host", message.Host);
            jsonWriter.MaybeWrite("short_message", message.ShortMessage);
            jsonWriter.MaybeWrite("full_message", message.FullMessage);
            jsonWriter.WriteNumber("level", (Int32)message.Level);
            jsonWriter.MaybeWrite("facility", message.Facility);
            jsonWriter.MaybeWrite("line", message.Line);
            jsonWriter.MaybeWrite("file", message.File);
            jsonWriter.MaybeWrite("_unixtimestamp", message.Timestamp);

            foreach (var field in message.AdditionalFields)
            {
                WriteAdditionalField(jsonWriter, field);
            }

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteAdditionalField(Utf8JsonWriter jsonWriter, KeyValuePair<String, Object> field)
        {
            var key = $"_{field.Key}";

            switch (field.Value)
            {
                case null:
                    break;
                case SByte value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Byte value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Int16 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case UInt16 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Int32 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case UInt32 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Int64 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case UInt64 value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Single value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Double value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                case Decimal value:
                    jsonWriter.WriteNumber(key, value);
                    break;
                default:
                    jsonWriter.WriteString(key, field.Value.ToString());
                    break;
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, String? value)
        {
            if (value != null)
            {
                writer.WriteString(property, value);
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, Int32? value)
        {
            if (value.HasValue)
            {
                writer.WriteNumber(property, value.Value);
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, Double? value)
        {
            if (value.HasValue)
            {
                writer.WriteNumber(property, value.Value);
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, DateTime? value)
        {
            if (value.HasValue)
            {
                writer.WriteString(property, value.Value.ToString("o"));
            }
        }

        private static void MaybeWrite(this Utf8JsonWriter writer, String property, TimeSpan? value)
        {
            if (value.HasValue)
            {
                writer.WriteNumber(property, value.Value.TotalSeconds);
            }
        }
    }
}
