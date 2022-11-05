using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MATSys.Commands
{
    internal sealed class CommandBaseConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(CommandBase).IsAssignableFrom(typeToConvert);

        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {

            var paramTypes = type.GetGenericArguments();
            Type? t = null;

            switch (paramTypes.Length)
            {
                case 0:
                    t = typeof(CommandConverter);
                    break;
                case 1:
                    t = typeof(CommandConverter<>).MakeGenericType(paramTypes);
                    break;
                case 2:
                    t = typeof(CommandConverter<,>).MakeGenericType(paramTypes);
                    break;
                case 3:
                    t = typeof(CommandConverter<,,>).MakeGenericType(paramTypes);
                    break;
                case 4:
                    t = typeof(CommandConverter<,,,>).MakeGenericType(paramTypes);
                    break;
                case 5:
                    t = typeof(CommandConverter<,,,,>).MakeGenericType(paramTypes);
                    break;
                case 6:
                    t = typeof(CommandConverter<,,,,,>).MakeGenericType(paramTypes);
                    break;
                case 7:
                    t = typeof(CommandConverter<,,,,,,>).MakeGenericType(paramTypes);
                    break;
                default:
                    break;
            }

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                t
                )!;

            return converter;
        }
        private class CommandConverter : JsonConverter<Command>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;
                return CommandBase.Create(name);
            }

            public override void Write(
                Utf8JsonWriter writer,
                Command value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                writer.WriteStringValue(sb.ToString());

            }

        }
        private class CommandConverter<T1> : JsonConverter<Command<T1>>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;
                return CommandBase.Create(name,
                    JsonSerializer.Deserialize<T1>(matches[1].Value, options)
                    )!;
            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1> value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item1, options));
                writer.WriteStringValue(sb.ToString());
            }

        }
        private class CommandConverter<T1, T2> : JsonConverter<Command<T1, T2>>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;
                return CommandBase.Create(name,
                    JsonSerializer.Deserialize<T1>(matches[1].Value, options),
                    JsonSerializer.Deserialize<T2>(matches[2].Value, options)
                    )!;
            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2> value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item1, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item2, options));
                writer.WriteStringValue(sb.ToString());
            }

        }
        private class CommandConverter<T1, T2, T3> : JsonConverter<Command<T1, T2, T3>>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command<T1, T2, T3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;
                return CommandBase.Create(name,
                    JsonSerializer.Deserialize<T1>(matches[1].Value, options),
                    JsonSerializer.Deserialize<T2>(matches[2].Value, options),
                    JsonSerializer.Deserialize<T3>(matches[3].Value, options)
                    )!;
            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3> value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item1, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item2, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item3, options));
                writer.WriteStringValue(sb.ToString());
            }

        }
        private class CommandConverter<T1, T2, T3, T4> : JsonConverter<Command<T1, T2, T3, T4>>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command<T1, T2, T3, T4> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;
                return CommandBase.Create(name,
                    JsonSerializer.Deserialize<T1>(matches[1].Value, options),
                    JsonSerializer.Deserialize<T2>(matches[2].Value, options),
                    JsonSerializer.Deserialize<T3>(matches[3].Value, options),
                    JsonSerializer.Deserialize<T4>(matches[4].Value, options)
                    )!;
            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4> value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item1, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item2, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item3, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item4, options));
                writer.WriteStringValue(sb.ToString());
            }

        }
        private class CommandConverter<T1, T2, T3, T4, T5> : JsonConverter<Command<T1, T2, T3, T4, T5>>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command<T1, T2, T3, T4, T5> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;
                return CommandBase.Create(name,
                    JsonSerializer.Deserialize<T1>(matches[1].Value, options),
                    JsonSerializer.Deserialize<T2>(matches[2].Value, options),
                    JsonSerializer.Deserialize<T3>(matches[3].Value, options),
                    JsonSerializer.Deserialize<T4>(matches[4].Value, options),
                    JsonSerializer.Deserialize<T5>(matches[5].Value, options)
                    )!;
            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4, T5> value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item1, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item2, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item3, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item4, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item5, options));
                writer.WriteStringValue(sb.ToString());
            }

        }
        private class CommandConverter<T1, T2, T3, T4, T5, T6> : JsonConverter<Command<T1, T2, T3, T4, T5, T6>>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command<T1, T2, T3, T4, T5, T6> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;
                return CommandBase.Create(name,
                    JsonSerializer.Deserialize<T1>(matches[1].Value, options),
                    JsonSerializer.Deserialize<T2>(matches[2].Value, options),
                    JsonSerializer.Deserialize<T3>(matches[3].Value, options),
                    JsonSerializer.Deserialize<T4>(matches[4].Value, options),
                    JsonSerializer.Deserialize<T5>(matches[5].Value, options),
                    JsonSerializer.Deserialize<T6>(matches[6].Value, options)

                    )!;
            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4, T5, T6> value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item1, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item2, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item3, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item4, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item5, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item6, options));
                writer.WriteStringValue(sb.ToString());
            }

        }
        private class CommandConverter<T1, T2, T3, T4, T5, T6, T7> : JsonConverter<Command<T1, T2, T3, T4, T5, T6, T7>>
        {
            private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
            public override Command<T1, T2, T3, T4, T5, T6, T7> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Stopwatch sw = new Stopwatch();

                var value = reader.GetString();
                var matches = regex.Matches(value);
                var name = matches[0].Value;

                var cmd = CommandBase.Create(name,
                    JsonSerializer.Deserialize<T1>(matches[1].Value, options),
                    JsonSerializer.Deserialize<T2>(matches[2].Value, options),
                    JsonSerializer.Deserialize<T3>(matches[3].Value, options),
                    JsonSerializer.Deserialize<T4>(matches[4].Value, options),
                    JsonSerializer.Deserialize<T5>(matches[5].Value, options),
                    JsonSerializer.Deserialize<T6>(matches[6].Value, options),
                    JsonSerializer.Deserialize<T7>(matches[7].Value, options)
                                    )!;

                return cmd;


            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4, T5, T6, T7> value,
                JsonSerializerOptions options)
            {
                var sb = new StringBuilder();
                sb.Append(value.MethodName);
                sb.Append("=");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item1, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item2, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item3, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item4, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item5, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item6, options));
                sb.Append(",");
                sb.Append(JsonSerializer.Serialize(value.Parameter.Item7, options));
                writer.WriteStringValue(sb.ToString());
            }

        }


    }
}
