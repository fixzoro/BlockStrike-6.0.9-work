using System;
using System.Reflection;

namespace FreeJSON
{
	public class JsonConvert
	{
		public static string Serialize(object obj)
		{
			Type type = obj.GetType();
			JsonObject jsonObject = new JsonObject();
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				string key = string.Empty;
				key = fields[i].Name;
				jsonObject.Add(key, fields[i].GetValue(obj));
			}
			return jsonObject.ToString();
		}
        
		public static object Deserialize(Type type, string jsonString)
		{
			object obj = Activator.CreateInstance(type);
			JsonObject jsonObject = JsonObject.Parse(jsonString);
			if (jsonObject == null)
			{
				return null;
			}
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				string key = string.Empty;
				key = fields[i].Name;
				if (jsonObject.ContainsKey(key))
				{
					fields[i].SetValue(obj, jsonObject.Get(key, fields[i].FieldType));
				}
			}
			return obj;
		}
	}
}
