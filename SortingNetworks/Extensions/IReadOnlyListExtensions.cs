using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SortingNetworks.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static void SaveToFile<T>(this IReadOnlyList<T> list, string filePath)
        {
            Directory.CreateDirectory("SavedNetworks");

            var jsonOptions = new JsonSerializerOptions()
            {
                IncludeFields = true
            };

            //var path = $"SavedNetworks/nets_{size}_{i}_{DateTime.Now:yyyyMMddHHmmssfff}.json";
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(list, jsonOptions));
            }

            Trace.WriteLine($"Saved network in {filePath}");
        }
    }
}
