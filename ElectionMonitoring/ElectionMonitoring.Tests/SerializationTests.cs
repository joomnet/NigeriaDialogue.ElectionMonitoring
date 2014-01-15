using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using ElectionMonitoring.Models;
using NUnit.Framework;
using Newtonsoft.Json;

namespace ElectionMonitoring.Tests
{
    [TestFixture]
    public class SerializationTests
    {
        string Serialize<T>(MediaTypeFormatter formatter, T value)
        {
            // Create a dummy HTTP Content.
            Stream stream = new MemoryStream();
            var content = new StreamContent(stream);
            /// Serialize the object.
            formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
            // Read the serialized string.
            stream.Position = 0;
            return content.ReadAsStringAsync().Result;
        }

        T Deserialize<T>(MediaTypeFormatter formatter, string str) where T : class
        {
            // Write the serialized string to a memory stream.
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            // Deserialize to an object of type T
            return formatter.ReadFromStreamAsync(typeof(T), stream, null, null).Result as T;
        }

        [Test]
        public void TestSerialization()
        {
            var value = new Candidate {Party = new Party()};
            value.Party.Candidates.Add(value);

            var xml = new XmlMediaTypeFormatter();
            string str = Serialize(xml, value);

            var json = new JsonMediaTypeFormatter() {SerializerSettings = new JsonSerializerSettings {}};

            str = Serialize(json, value);

            // Round trip
            var person2 = Deserialize<Candidate>(json, str);
        }
    }
}
