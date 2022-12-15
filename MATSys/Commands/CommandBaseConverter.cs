using System.Text.Json;
using System.Text.Json.Serialization;

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
                t, options)!;

            return converter;
        }
        private class CommandConverter : JsonConverter<Command>
        {

            public CommandConverter(JsonSerializerOptions options)
            {

            }

            public override Command Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }
        private class CommandConverter<T1> : JsonConverter<Command<T1>>
        {
            private readonly JsonConverter<T1> _v1Converter;
            private readonly Type _v1Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1>)options
                                    .GetConverter(typeof(T1));
                _v1Type = typeof(T1);

            }

            public override Command<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                _v1Converter.Write(writer, value.Item1, options);
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }
        private class CommandConverter<T1, T2> : JsonConverter<Command<T1, T2>>
        {
            private readonly JsonConverter<T1> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2> _v2Converter;
            private readonly Type _v2Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1>)options
                                    .GetConverter(typeof(T1));
                _v1Type = typeof(T1);
                _v2Converter = (JsonConverter<T2>)options
                        .GetConverter(typeof(T2));
                _v2Type = typeof(T2);

            }

            public override Command<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2 v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                _v1Converter.Write(writer, value.Item1, options);
                _v2Converter.Write(writer, value.Item2, options);
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }
        private class CommandConverter<T1, T2, T3> : JsonConverter<Command<T1, T2, T3>>
        {
            private readonly JsonConverter<T1> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3> _v3Converter;
            private readonly Type _v3Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1>)options
                                    .GetConverter(typeof(T1));
                _v1Type = typeof(T1);
                _v2Converter = (JsonConverter<T2>)options
                        .GetConverter(typeof(T2));
                _v2Type = typeof(T2);
                _v3Converter = (JsonConverter<T3>)options
                                    .GetConverter(typeof(T3));
                _v3Type = typeof(T1);

            }

            public override Command<T1, T2, T3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2 v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3 v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                _v1Converter.Write(writer, value.Item1, options);
                _v2Converter.Write(writer, value.Item2, options);
                _v3Converter.Write(writer, value.Item3, options);
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }
        private class CommandConverter<T1, T2, T3, T4> : JsonConverter<Command<T1, T2, T3, T4>>
        {
            private readonly JsonConverter<T1> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4> _v4Converter;
            private readonly Type _v4Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1>)options
                                    .GetConverter(typeof(T1));
                _v1Type = typeof(T1);
                _v2Converter = (JsonConverter<T2>)options
                        .GetConverter(typeof(T2));
                _v2Type = typeof(T2);
                _v3Converter = (JsonConverter<T3>)options
                                    .GetConverter(typeof(T3));
                _v3Type = typeof(T1);
                _v4Converter = (JsonConverter<T4>)options
                                    .GetConverter(typeof(T4));
                _v4Type = typeof(T4);

            }

            public override Command<T1, T2, T3, T4> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2 v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3 v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4 v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                _v1Converter.Write(writer, value.Item1, options);
                _v2Converter.Write(writer, value.Item2, options);
                _v3Converter.Write(writer, value.Item3, options);
                _v4Converter.Write(writer, value.Item4, options);
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }
        private class CommandConverter<T1, T2, T3, T4, T5> : JsonConverter<Command<T1, T2, T3, T4, T5>>
        {
            private readonly JsonConverter<T1> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4> _v4Converter;
            private readonly Type _v4Type;
            private readonly JsonConverter<T5> _v5Converter;
            private readonly Type _v5Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1>)options
                                    .GetConverter(typeof(T1));
                _v1Type = typeof(T1);
                _v2Converter = (JsonConverter<T2>)options
                        .GetConverter(typeof(T2));
                _v2Type = typeof(T2);
                _v3Converter = (JsonConverter<T3>)options
                                    .GetConverter(typeof(T3));
                _v3Type = typeof(T1);
                _v4Converter = (JsonConverter<T4>)options
                                    .GetConverter(typeof(T4));
                _v4Type = typeof(T4);
                _v5Converter = (JsonConverter<T5>)options
                                    .GetConverter(typeof(T5));
                _v5Type = typeof(T5);

            }

            public override Command<T1, T2, T3, T4, T5> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2 v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3 v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4 v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();
                T5 v5 = _v5Converter.Read(ref reader, _v5Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4, T5> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                _v1Converter.Write(writer, value.Item1, options);
                _v2Converter.Write(writer, value.Item2, options);
                _v3Converter.Write(writer, value.Item3, options);
                _v4Converter.Write(writer, value.Item4, options);
                _v5Converter.Write(writer, value.Item5, options);
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }
        private class CommandConverter<T1, T2, T3, T4, T5, T6> : JsonConverter<Command<T1, T2, T3, T4, T5, T6>>
        {
            private readonly JsonConverter<T1> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4> _v4Converter;
            private readonly Type _v4Type;
            private readonly JsonConverter<T5> _v5Converter;
            private readonly Type _v5Type;
            private readonly JsonConverter<T6> _v6Converter;
            private readonly Type _v6Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1>)options
                                    .GetConverter(typeof(T1));
                _v1Type = typeof(T1);
                _v2Converter = (JsonConverter<T2>)options
                        .GetConverter(typeof(T2));
                _v2Type = typeof(T2);
                _v3Converter = (JsonConverter<T3>)options
                                    .GetConverter(typeof(T3));
                _v3Type = typeof(T1);
                _v4Converter = (JsonConverter<T4>)options
                                    .GetConverter(typeof(T4));
                _v4Type = typeof(T4);
                _v5Converter = (JsonConverter<T5>)options
                                    .GetConverter(typeof(T5));
                _v5Type = typeof(T5);
                _v6Converter = (JsonConverter<T6>)options
                                    .GetConverter(typeof(T6));
                _v6Type = typeof(T6);

            }

            public override Command<T1, T2, T3, T4, T5, T6> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2 v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3 v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4 v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();
                T5 v5 = _v5Converter.Read(ref reader, _v5Type, options);
                reader.Read();
                T6 v6 = _v6Converter.Read(ref reader, _v6Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5, v6);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4, T5, T6> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                _v1Converter.Write(writer, value.Item1, options);
                _v2Converter.Write(writer, value.Item2, options);
                _v3Converter.Write(writer, value.Item3, options);
                _v4Converter.Write(writer, value.Item4, options);
                _v5Converter.Write(writer, value.Item5, options);
                _v6Converter.Write(writer, value.Item6, options);
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }
        private class CommandConverter<T1, T2, T3, T4, T5, T6, T7> : JsonConverter<Command<T1, T2, T3, T4, T5, T6, T7>>
        {
            private readonly JsonConverter<T1> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4> _v4Converter;
            private readonly Type _v4Type;
            private readonly JsonConverter<T5> _v5Converter;
            private readonly Type _v5Type;
            private readonly JsonConverter<T6> _v6Converter;
            private readonly Type _v6Type;
            private readonly JsonConverter<T7> _v7Converter;
            private readonly Type _v7Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1>)options
                                    .GetConverter(typeof(T1));
                _v1Type = typeof(T1);
                _v2Converter = (JsonConverter<T2>)options
                        .GetConverter(typeof(T2));
                _v2Type = typeof(T2);
                _v3Converter = (JsonConverter<T3>)options
                                    .GetConverter(typeof(T3));
                _v3Type = typeof(T1);
                _v4Converter = (JsonConverter<T4>)options
                                    .GetConverter(typeof(T4));
                _v4Type = typeof(T4);
                _v5Converter = (JsonConverter<T5>)options
                                    .GetConverter(typeof(T5));
                _v5Type = typeof(T5);
                _v6Converter = (JsonConverter<T6>)options
                                    .GetConverter(typeof(T6));
                _v6Type = typeof(T6);
                _v7Converter = (JsonConverter<T7>)options
                                    .GetConverter(typeof(T7));
                _v7Type = typeof(T7);

            }

            public override Command<T1, T2, T3, T4, T5, T6, T7> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2 v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3 v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4 v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();
                T5 v5 = _v5Converter.Read(ref reader, _v5Type, options);
                reader.Read();
                T6 v6 = _v6Converter.Read(ref reader, _v6Type, options);
                reader.Read();
                T7 v7 = _v7Converter.Read(ref reader, _v7Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5, v6, v7);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1, T2, T3, T4, T5, T6, T7> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                _v1Converter.Write(writer, value.Item1, options);
                _v2Converter.Write(writer, value.Item2, options);
                _v3Converter.Write(writer, value.Item3, options);
                _v4Converter.Write(writer, value.Item4, options);
                _v5Converter.Write(writer, value.Item5, options);
                _v6Converter.Write(writer, value.Item6, options);
                _v7Converter.Write(writer, value.Item7, options);
                writer.WriteEndArray();
                writer.WriteEndObject();

            }


        }


    }
}
