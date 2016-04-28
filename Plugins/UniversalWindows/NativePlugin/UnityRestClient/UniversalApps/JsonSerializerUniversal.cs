﻿#if NETFX_CORE
using System.IO;
using System.Runtime.Serialization.Json;

namespace RestSharp.Serializers
{
	/// <summary>
	/// Default JSON serializer for request bodies
	/// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
	/// </summary>
	public class JsonSerializer : ISerializer
	{
		/// <summary>
		/// Default serializer
		/// </summary>
		public JsonSerializer()
		{
			ContentType = "application/json";
            DateFormat = RestSharp.DateFormat.Iso8601;
		}

		/// <summary>
		/// Serialize the object as JSON
		/// </summary>
		/// <param name="obj">Object to serialize</param>
		/// <returns>JSON as String</returns>
		public string Serialize(object obj)
		{
            DataContractJsonSerializer sera = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            sera.WriteObject(ms, obj);

            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();

		}

		/// <summary>
		/// Unused for JSON Serialization
		/// </summary>
		public string DateFormat { get; set; }
		/// <summary>
		/// Unused for JSON Serialization
		/// </summary>
		public string RootElement { get; set; }
		/// <summary>
		/// Unused for JSON Serialization
		/// </summary>
		public string Namespace { get; set; }
		/// <summary>
		/// Content type for serialized content
		/// </summary>
		public string ContentType { get; set; }
	}
}
#endif