using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeJSON
{
	public class JsonArray : Json
	{
		public bool HasValue(int index)
		{
			return this.values.Count - 1 >= index && this.values.Values.ElementAt(index) != string.Empty && this.values.Values.ElementAt(index) != "null";
		}
        
		public bool Contains(object value)
		{
			string value2 = base.ConvertToString(value);
			if (string.IsNullOrEmpty(value2))
			{
				Debug.LogWarning("Json does not support this type");
				return false;
			}
			return this.values.ContainsValue(value2);
		}
        
		public void RemoveAt(int index)
		{
			this.values.Remove(this.values.Keys.ElementAt(index));
		}
        
		public bool Remove(object value)
		{
			string text = base.ConvertToString(value);
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Json does not support this type");
				return false;
			}
			bool result = false;
			foreach (KeyValuePair<string, string> keyValuePair in this.values)
			{
				if (keyValuePair.Value == text)
				{
					this.values.Remove(keyValuePair.Key);
					result = true;
				}
			}
			return result;
		}
        
		public void Add(object value)
		{
			this.id++;
			base.AddData(this.id.ToString(), value);
		}
        
		public T Get<T>(int index)
		{
			return (T)((object)this.Get(index, typeof(T)));
		}
        
		public object Get(int index, Type type)
		{
			return base.GetData(this.values.Keys.ElementAt(index), type);
		}
        
		public static JsonArray Parse(string jsonString)
		{
			return JsonArray.Parser.Parse(jsonString);
		}
        
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			foreach (KeyValuePair<string, string> keyValuePair in this.values)
			{
				stringBuilder.Append(keyValuePair.Value);
				stringBuilder.Append(',');
			}
			if (this.values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
        
		private int id;
        
		private class Parser
		{
			public static JsonArray Parse(string json)
			{
				for (int i = 0; i < json.Length; i++)
				{
					char c = json[i];
					if (c == '"')
					{
						i = JsonArray.Parser.AppendUntilStringEnd(true, i, json);
					}
					else if (!char.IsWhiteSpace(c))
					{
						JsonArray.Parser.stringBuilder.Append(c);
					}
				}
				List<string> list = JsonArray.Parser.Split(json);
				JsonArray.Parser.jsonArray = new JsonArray();
				for (int j = 0; j < list.Count; j++)
				{
					if (!string.IsNullOrEmpty(list[j]))
					{
						JsonArray.Parser.id++;
						JsonArray.Parser.jsonArray.values.Add(JsonArray.Parser.id.ToString(), list[j]);
					}
				}
				return JsonArray.Parser.jsonArray;
			}
            
			private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
			{
				JsonArray.Parser.stringBuilder.Append(json[startIdx]);
				for (int i = startIdx + 1; i < json.Length; i++)
				{
					if (json[i] == '\\')
					{
						if (appendEscapeCharacter)
						{
							JsonArray.Parser.stringBuilder.Append(json[i]);
						}
						JsonArray.Parser.stringBuilder.Append(json[i + 1]);
						i++;
					}
					else
					{
						if (json[i] == '"')
						{
							JsonArray.Parser.stringBuilder.Append(json[i]);
							return i;
						}
						JsonArray.Parser.stringBuilder.Append(json[i]);
					}
				}
				return json.Length - 1;
			}
            
			private static List<string> Split(string json)
			{
				List<string> list = (JsonArray.Parser.splitArrayPool.Count <= 0) ? new List<string>() : JsonArray.Parser.splitArrayPool.Pop();
				list.Clear();
				int num = 0;
				JsonArray.Parser.stringBuilder.Length = 0;
				int i = 1;
				while (i < json.Length - 1)
				{
					char c = json[i];
					switch (c)
					{
					case '[':
						goto IL_8E;
					default:
						switch (c)
						{
						case '{':
							goto IL_8E;
						default:
							if (c != '"')
							{
								if (c != ',' && c != ':')
								{
									goto IL_D9;
								}
								if (num != 0)
								{
									goto IL_D9;
								}
								list.Add(JsonArray.Parser.stringBuilder.ToString());
								JsonArray.Parser.stringBuilder.Length = 0;
							}
							else
							{
								i = JsonArray.Parser.AppendUntilStringEnd(true, i, json);
							}
							break;
						case '}':
							goto IL_97;
						}
						break;
					case ']':
						goto IL_97;
					}
					IL_EB:
					i++;
					continue;
					IL_D9:
					JsonArray.Parser.stringBuilder.Append(json[i]);
					goto IL_EB;
					IL_97:
					num--;
					goto IL_D9;
					IL_8E:
					num++;
					goto IL_D9;
				}
				list.Add(JsonArray.Parser.stringBuilder.ToString());
				return list;
			}
            
			private static Stack<List<string>> splitArrayPool = new Stack<List<string>>();
            
			private static JsonArray jsonArray = new JsonArray();
            
			private static StringBuilder stringBuilder = new StringBuilder();
            
			private static int id = 0;
		}
	}
}
