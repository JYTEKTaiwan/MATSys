#if NET6_0_OR_GREATER||NETSTANDARD2_0
using System.Text.Json;
using System.Text.Json.Serialization;
#endif
namespace MATSys.Commands
{
#if NET6_0_OR_GREATER || NETSTANDARD2_0
    internal sealed class CommandBaseJsonConverter : JsonConverterFactory
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
                t!, options)!;

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
                var name = reader.GetString()!;
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
        private class CommandConverter<T1> : JsonConverter<Command<T1?>>
        {
            private readonly JsonConverter<T1?> _v1Converter;
            private readonly Type _v1Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1?>)options
                                    .GetConverter(typeof(T1?));
                _v1Type = typeof(T1?);

            }

            public override Command<T1?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString()!;
                reader.Read();//start of array
                reader.Read();
                T1? v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1?> value,
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
        private class CommandConverter<T1, T2> : JsonConverter<Command<T1?, T2?>>
        {
            private readonly JsonConverter<T1?> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2?> _v2Converter;
            private readonly Type _v2Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1?>)options
                                    .GetConverter(typeof(T1?));
                _v1Type = typeof(T1?);
                _v2Converter = (JsonConverter<T2?>)options
                        .GetConverter(typeof(T2?));
                _v2Type = typeof(T2?);

            }

            public override Command<T1?, T2?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString()!;
                reader.Read();//start of array
                reader.Read();
                T1? v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2? v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1?, T2?> value,
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
        private class CommandConverter<T1, T2, T3> : JsonConverter<Command<T1?, T2?, T3?>>
        {
            private readonly JsonConverter<T1?> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2?> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3?> _v3Converter;
            private readonly Type _v3Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1?>)options
                                    .GetConverter(typeof(T1?));
                _v1Type = typeof(T1?);
                _v2Converter = (JsonConverter<T2?>)options
                        .GetConverter(typeof(T2?));
                _v2Type = typeof(T2?);
                _v3Converter = (JsonConverter<T3?>)options
                                    .GetConverter(typeof(T3?));
                _v3Type = typeof(T1?);

            }

            public override Command<T1?, T2?, T3?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString()!;
                reader.Read();//start of array
                reader.Read();
                T1? v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2? v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3? v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1?, T2?, T3?> value,
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
        private class CommandConverter<T1, T2, T3, T4> : JsonConverter<Command<T1?, T2?, T3?, T4?>>
        {
            private readonly JsonConverter<T1?> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2?> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3?> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4?> _v4Converter;
            private readonly Type _v4Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1?>)options
                                    .GetConverter(typeof(T1?));
                _v1Type = typeof(T1?);
                _v2Converter = (JsonConverter<T2?>)options
                        .GetConverter(typeof(T2?));
                _v2Type = typeof(T2?);
                _v3Converter = (JsonConverter<T3?>)options
                                    .GetConverter(typeof(T3?));
                _v3Type = typeof(T1?);
                _v4Converter = (JsonConverter<T4?>)options
                                    .GetConverter(typeof(T4?));
                _v4Type = typeof(T4?);

            }

            public override Command<T1?, T2?, T3?, T4?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString()!;
                reader.Read();//start of array
                reader.Read();
                T1? v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2? v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3? v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4? v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1?, T2?, T3?, T4?> value,
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
        private class CommandConverter<T1, T2, T3, T4, T5> : JsonConverter<Command<T1?, T2?, T3?, T4?, T5?>>
        {
            private readonly JsonConverter<T1?> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2?> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3?> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4?> _v4Converter;
            private readonly Type _v4Type;
            private readonly JsonConverter<T5?> _v5Converter;
            private readonly Type _v5Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1?>)options
                                    .GetConverter(typeof(T1?));
                _v1Type = typeof(T1?);
                _v2Converter = (JsonConverter<T2?>)options
                        .GetConverter(typeof(T2?));
                _v2Type = typeof(T2?);
                _v3Converter = (JsonConverter<T3?>)options
                                    .GetConverter(typeof(T3?));
                _v3Type = typeof(T1?);
                _v4Converter = (JsonConverter<T4?>)options
                                    .GetConverter(typeof(T4?));
                _v4Type = typeof(T4?);
                _v5Converter = (JsonConverter<T5?>)options
                                    .GetConverter(typeof(T5?));
                _v5Type = typeof(T5?);

            }

            public override Command<T1?, T2?, T3?, T4?, T5?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString()!;
                reader.Read();//start of array
                reader.Read();
                T1? v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2? v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3? v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4? v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();
                T5? v5 = _v5Converter.Read(ref reader, _v5Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1?, T2?, T3?, T4?, T5?> value,
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
        private class CommandConverter<T1, T2, T3, T4, T5, T6> : JsonConverter<Command<T1?, T2?, T3?, T4?, T5?, T6?>>
        {
            private readonly JsonConverter<T1?> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2?> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3?> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4?> _v4Converter;
            private readonly Type _v4Type;
            private readonly JsonConverter<T5?> _v5Converter;
            private readonly Type _v5Type;
            private readonly JsonConverter<T6?> _v6Converter;
            private readonly Type _v6Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1?>)options
                                    .GetConverter(typeof(T1?));
                _v1Type = typeof(T1?);
                _v2Converter = (JsonConverter<T2?>)options
                        .GetConverter(typeof(T2?));
                _v2Type = typeof(T2?);
                _v3Converter = (JsonConverter<T3?>)options
                                    .GetConverter(typeof(T3?));
                _v3Type = typeof(T1?);
                _v4Converter = (JsonConverter<T4?>)options
                                    .GetConverter(typeof(T4?));
                _v4Type = typeof(T4?);
                _v5Converter = (JsonConverter<T5?>)options
                                    .GetConverter(typeof(T5?));
                _v5Type = typeof(T5?);
                _v6Converter = (JsonConverter<T6?>)options
                                    .GetConverter(typeof(T6?));
                _v6Type = typeof(T6?);

            }

            public override Command<T1?, T2?, T3?, T4?, T5?, T6?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString()!;
                reader.Read();//start of array
                reader.Read();
                T1? v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2? v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3? v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4? v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();
                T5? v5 = _v5Converter.Read(ref reader, _v5Type, options);
                reader.Read();
                T6? v6 = _v6Converter.Read(ref reader, _v6Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5, v6);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1?, T2?, T3?, T4?, T5?, T6?> value,
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
        private class CommandConverter<T1, T2, T3, T4, T5, T6, T7> : JsonConverter<Command<T1?, T2?, T3?, T4?, T5?, T6?, T7?>>
        {
            private readonly JsonConverter<T1?> _v1Converter;
            private readonly Type _v1Type;
            private readonly JsonConverter<T2?> _v2Converter;
            private readonly Type _v2Type;
            private readonly JsonConverter<T3?> _v3Converter;
            private readonly Type _v3Type;
            private readonly JsonConverter<T4?> _v4Converter;
            private readonly Type _v4Type;
            private readonly JsonConverter<T5?> _v5Converter;
            private readonly Type _v5Type;
            private readonly JsonConverter<T6?> _v6Converter;
            private readonly Type _v6Type;
            private readonly JsonConverter<T7?> _v7Converter;
            private readonly Type _v7Type;

            public CommandConverter(JsonSerializerOptions options)
            {
                _v1Converter = (JsonConverter<T1?>)options
                                    .GetConverter(typeof(T1?));
                _v1Type = typeof(T1?);
                _v2Converter = (JsonConverter<T2?>)options
                        .GetConverter(typeof(T2?));
                _v2Type = typeof(T2?);
                _v3Converter = (JsonConverter<T3?>)options
                                    .GetConverter(typeof(T3?));
                _v3Type = typeof(T1?);
                _v4Converter = (JsonConverter<T4?>)options
                                    .GetConverter(typeof(T4?));
                _v4Type = typeof(T4?);
                _v5Converter = (JsonConverter<T5?>)options
                                    .GetConverter(typeof(T5?));
                _v5Type = typeof(T5?);
                _v6Converter = (JsonConverter<T6?>)options
                                    .GetConverter(typeof(T6?));
                _v6Type = typeof(T6?);
                _v7Converter = (JsonConverter<T7?>)options
                                    .GetConverter(typeof(T7?));
                _v7Type = typeof(T7?);

            }

            public override Command<T1?, T2?, T3?, T4?, T5?, T6?, T7?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();//property name
                var name = reader.GetString()!;
                reader.Read();//start of array
                reader.Read();
                T1? v1 = _v1Converter.Read(ref reader, _v1Type, options);
                reader.Read();
                T2? v2 = _v2Converter.Read(ref reader, _v2Type, options);
                reader.Read();
                T3? v3 = _v3Converter.Read(ref reader, _v3Type, options);
                reader.Read();
                T4? v4 = _v4Converter.Read(ref reader, _v4Type, options);
                reader.Read();
                T5? v5 = _v5Converter.Read(ref reader, _v5Type, options);
                reader.Read();
                T6? v6 = _v6Converter.Read(ref reader, _v6Type, options);
                reader.Read();
                T7? v7 = _v7Converter.Read(ref reader, _v7Type, options);
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5, v6, v7);

            }

            public override void Write(
                Utf8JsonWriter writer,
                Command<T1?, T2?, T3?, T4?, T5?, T6?, T7?> value,
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

#else
    internal sealed class CommandBaseJsonConverter : Newtonsoft.Json.Converters.CustomCreationConverter<CommandBase>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(CommandBase).IsAssignableFrom(typeToConvert);

        }
        public override bool CanWrite => true;
        public override bool CanRead => true;
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var conveter = CreateConverter(value.GetType());
            conveter.WriteJson(writer, value, serializer);
        }
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var conveter = CreateConverter(objectType);
            return conveter.ReadJson(reader, objectType, existingValue, serializer);
        }

        private static Newtonsoft.Json.JsonConverter CreateConverter(Type type)
        {

            var paramTypes = type.GetGenericArguments();
            Type t = null;

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

            Newtonsoft.Json.JsonConverter converter = (Newtonsoft.Json.JsonConverter)Activator.CreateInstance(t);

            return converter;
        }

        public override CommandBase Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        private class CommandConverter : Newtonsoft.Json.JsonConverter<Command>
        {

            public override Command ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name);
            }


            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
        private class CommandConverter<T1> : Newtonsoft.Json.JsonConverter<Command<T1>>
        {
            public override Command<T1> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command<T1> existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1);
            }


            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command<T1> value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                serializer.Serialize(writer, value.Item1);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        private class CommandConverter<T1, T2> : Newtonsoft.Json.JsonConverter<Command<T1, T2>>
        {
            public override Command<T1, T2> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command<T1, T2> existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T2 v2 = Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2);
            }


            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command<T1, T2> value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteValue(value.Item1);
                writer.WriteValue(value.Item2);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        private class CommandConverter<T1, T2, T3> : Newtonsoft.Json.JsonConverter<Command<T1, T2, T3>>
        {

            public override Command<T1, T2, T3> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command<T1, T2, T3> existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T2 v2 = Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T3 v3 = Newtonsoft.Json.JsonConvert.DeserializeObject<T3>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));

                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command<T1, T2, T3> value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteValue(value.Item1);
                writer.WriteValue(value.Item2);
                writer.WriteValue(value.Item3);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
        private class CommandConverter<T1, T2, T3, T4> : Newtonsoft.Json.JsonConverter<Command<T1, T2, T3, T4>>
        {
            public override Command<T1, T2, T3, T4> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command<T1, T2, T3, T4> existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T2 v2 = Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T3 v3 = Newtonsoft.Json.JsonConvert.DeserializeObject<T3>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T4 v4 = Newtonsoft.Json.JsonConvert.DeserializeObject<T4>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));

                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command<T1, T2, T3, T4> value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteValue(value.Item1);
                writer.WriteValue(value.Item2);
                writer.WriteValue(value.Item3);
                writer.WriteValue(value.Item4);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }

        }
        private class CommandConverter<T1, T2, T3, T4, T5> : Newtonsoft.Json.JsonConverter<Command<T1, T2, T3, T4, T5>>
        {
            public override Command<T1, T2, T3, T4, T5> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command<T1, T2, T3, T4, T5> existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T2 v2 = Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T3 v3 = Newtonsoft.Json.JsonConvert.DeserializeObject<T3>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T4 v4 = Newtonsoft.Json.JsonConvert.DeserializeObject<T4>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T5 v5 = Newtonsoft.Json.JsonConvert.DeserializeObject<T5>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));

                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command<T1, T2, T3, T4, T5> value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteValue(value.Item1);
                writer.WriteValue(value.Item2);
                writer.WriteValue(value.Item3);
                writer.WriteValue(value.Item4);
                writer.WriteValue(value.Item5);

                writer.WriteEndArray();
                writer.WriteEndObject();
            }


        }
        private class CommandConverter<T1, T2, T3, T4, T5, T6> : Newtonsoft.Json.JsonConverter<Command<T1, T2, T3, T4, T5, T6>>
        {
            public override Command<T1, T2, T3, T4, T5, T6> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command<T1, T2, T3, T4, T5, T6> existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T2 v2 = Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T3 v3 = Newtonsoft.Json.JsonConvert.DeserializeObject<T3>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T4 v4 = Newtonsoft.Json.JsonConvert.DeserializeObject<T4>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T5 v5 = Newtonsoft.Json.JsonConvert.DeserializeObject<T5>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T6 v6 = Newtonsoft.Json.JsonConvert.DeserializeObject<T6>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));

                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5, v6);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command<T1, T2, T3, T4, T5, T6> value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteValue(value.Item1);
                writer.WriteValue(value.Item2);
                writer.WriteValue(value.Item3);
                writer.WriteValue(value.Item4);
                writer.WriteValue(value.Item5);
                writer.WriteValue(value.Item6);

                writer.WriteEndArray();
                writer.WriteEndObject();
            }


        }
        private class CommandConverter<T1, T2, T3, T4, T5, T6, T7> : Newtonsoft.Json.JsonConverter<Command<T1, T2, T3, T4, T5, T6, T7>>
        {
            public override Command<T1, T2, T3, T4, T5, T6, T7> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Command<T1, T2, T3, T4, T5, T6, T7> existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                reader.Read();//property name
                var name = reader.Value.ToString();
                reader.Read();//start of array
                reader.Read();
                T1 v1 = Newtonsoft.Json.JsonConvert.DeserializeObject<T1>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T2 v2 = Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T3 v3 = Newtonsoft.Json.JsonConvert.DeserializeObject<T3>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T4 v4 = Newtonsoft.Json.JsonConvert.DeserializeObject<T4>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T5 v5 = Newtonsoft.Json.JsonConvert.DeserializeObject<T5>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T6 v6 = Newtonsoft.Json.JsonConvert.DeserializeObject<T6>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));
                reader.Read();
                T7 v7 = Newtonsoft.Json.JsonConvert.DeserializeObject<T7>(Newtonsoft.Json.JsonConvert.SerializeObject(reader.Value));

                reader.Read();//end of array
                reader.Read();//end of object
                return CommandBase.Create(name, v1, v2, v3, v4, v5, v6, v7);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Command<T1, T2, T3, T4, T5, T6, T7> value, Newtonsoft.Json.JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(value.MethodName);
                writer.WriteStartArray();
                writer.WriteValue(value.Item1);
                writer.WriteValue(value.Item2);
                writer.WriteValue(value.Item3);
                writer.WriteValue(value.Item4);
                writer.WriteValue(value.Item5);
                writer.WriteValue(value.Item6);
                writer.WriteValue(value.Item7);

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

        }


    }

#endif



}
