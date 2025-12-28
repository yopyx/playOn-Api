using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> property, JsonSerializerOptions options) {
            var jsonConverter = new ValueConverter<T, string>(
                    v => JsonSerializer.Serialize(v, options),
                    v => JsonSerializer.Deserialize<T>(v, options)!
                );
            var jsonComparer = new ValueComparer<T>(
                    (a,b) => JsonSerializer.Serialize(a, options) == JsonSerializer.Serialize(b, options),
                    v => JsonSerializer.Serialize(v, options).GetHashCode(),
                    v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, options), options)!
                );
            property.HasConversion(jsonConverter);
            property.Metadata.SetValueComparer(jsonComparer);
            return property;
        }
    }   
}
