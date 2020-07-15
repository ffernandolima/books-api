using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;

namespace BooksApi.Models.MySQL
{
    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> builder) where T : new()
        {
            var converter = new ValueConverter<T, string>(
                v => v.ToJson(),
                v => v.FromJson<T>() ?? new T()
            );

            var comparer = new ValueComparer<T>(
                (l, r) => l.ToJson() == r.ToJson(),
                v => v == null ? 0 : v.ToJson().GetHashCode(),
                v => v.ToSnapshot()
            );

            builder.HasConversion(converter);

            builder.Metadata.SetValueConverter(converter);
            builder.Metadata.SetValueComparer(comparer);

            builder.HasColumnType("json");

            return builder;
        }
    }

    public static class ObjectExtensions
    {
        public static string ToJson<T>(this T source)
        {
            var formatting = Formatting.None;
            var settings = Serialization.DefaultSettings;

            var json = JsonConvert.SerializeObject(source, formatting, settings);

            return json;
        }

        public static T ToSnapshot<T>(this T source)
        {
            var json = source.ToJson();
            var snapshot = json.FromJson<T>();

            return snapshot;
        }
    }

    public static class StringExtensions
    {
        public static T FromJson<T>(this string json)
        {
            var objectResult = (T)FromJson(json, typeof(T));

            return objectResult;
        }

        public static object FromJson(this string json, Type type)
        {
            var settings = Serialization.DefaultSettings;
            var objectResult = JsonConvert.DeserializeObject(json, type, settings);

            return objectResult;
        }
    }

    public static class Serialization
    {
        private static readonly Lazy<JsonSerializer> DefaultSerializerFactory = new Lazy<JsonSerializer>(
             () => JsonSerializer.CreateDefault(DefaultSettings), isThreadSafe: true
         );

        private static readonly Lazy<JsonSerializerSettings> DefaultSettingsFactory = new Lazy<JsonSerializerSettings>(
            () => CreateDefaultSettings(), isThreadSafe: true
        );

        public static JsonSerializer DefaultSerializer => DefaultSerializerFactory.Value;
        public static JsonSerializerSettings DefaultSettings => DefaultSettingsFactory.Value;

        private static JsonSerializerSettings CreateDefaultSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };

            return settings;
        }
    }
}
