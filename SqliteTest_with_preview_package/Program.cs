using Microsoft.Data.Sqlite;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace NtsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dictionaries = new List<Dictionary<string, object>>();
            using (SqliteConnection connection = new SqliteConnection(@"Data Source = Data/Data.db;"))
            {
                connection.Open();
                InjectSQLiteFunctions(connection);
                using (SqliteCommand commend = connection.CreateCommand())
                {
                    commend.CommandText =
@"SELECT
    osm_id,
    house_no,
    name,
    postcode,
    geometry,
    CS_GeometryString(geometry) As geometryString,
    CS_GetGeometryType(geometry) As geometryType

    FROM place";

                    using (DbDataReader reader = commend.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dictionary = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldType = reader.GetFieldType(i);

                                if (!reader.IsDBNull(i))
                                {
                                    dictionary.Add(fieldName, reader.GetValue(i));
                                }
                            }
                            dictionaries.Add(dictionary);
                        }
                    }
                }
            }

            foreach (var dictionary in dictionaries)
            {
                PrintDictionary(dictionary);
                Console.WriteLine();
            }
        }

        static void PrintDictionary(Dictionary<string, object> dictionary)
        {
            foreach(var keyValue in dictionary)
            {
                var fieldName = keyValue.Key;
                var fieldType = keyValue.Value.GetType();
                var fieldValue = keyValue.Value;

                if (fieldName == "geometry" && fieldType == typeof(byte[]))
                {
                    var wkb = (byte[])fieldValue;
                    Console.WriteLine($"geometry: {CS_GeometryString(wkb)}");
                    try
                    {
                        Console.WriteLine($"geometryTypeFromGeometry: {CS_GetGeometryType(wkb)}");
                    }
                    catch { }
                }
                else
                {
                    Console.WriteLine($"{fieldName}: {fieldValue}");
                }
            }
        }

        static void InjectSQLiteFunctions(SqliteConnection connection)
        {
            connection.CreateFunction("CS_GetGeometryType", (Func<byte[], string>)CS_GetGeometryType);

            connection.CreateFunction("CS_GeometryString", (Func<byte[], string>)CS_GeometryString);
        }

        static string CS_GetGeometryType(byte[] wkb)
        {
            try
            {
                return new WKBReader().Read(wkb).GeometryType;
            }
            catch
            {
                return "Unknown";
            }
        }

        static string CS_GeometryString(byte[] wkb)
        {
            return string.Join(" ", wkb);
        }
    }
}
