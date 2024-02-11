using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeJSON
{
	public class JsonObject : Json
	{
		public bool ContainsKey(string key)
		{
			return this.values.ContainsKey(key);
		}
        
		public string GetKey(int index)
		{
			return this.values.Keys.ElementAt(index);
		}
        
		public bool Remove(string key)
		{
			return this.values.Remove(key);
		}
        
		public void Add(string key, object value)
		{
			base.AddData(key, value);
		}
        
		public T Get<T>(string key)
		{
			return (T)((object)this.Get(key, typeof(T)));
		}
        
		public T Get<T>(string key, T defaultValue)
		{
			if (this.ContainsKey(key))
			{
				return (T)((object)this.Get(key, typeof(T)));
			}
			return defaultValue;
		}
        
		public object Get(string key, Type type)
		{
			return base.GetData(key, type);
		}
        
		public object Get(string key, Type type, object defaultValue)
		{
			if (this.ContainsKey(key))
			{
				return base.GetData(key, type);
			}
			return defaultValue;
		}
        
		public static bool isJson(string jsonString)
		{
			return jsonString[0].ToString() == "{" && jsonString[jsonString.Length - 1].ToString() == "}";
		}
        
		public static JsonObject Parse(string jsonString)
		{
			JsonObject.Parser parser = new JsonObject.Parser();
			return parser.Parse(jsonString);
		}
        
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('{');
			foreach (KeyValuePair<string, string> keyValuePair in this.values)
			{
				stringBuilder.Append("\"" + keyValuePair.Key + "\"");
				stringBuilder.Append(':');
				stringBuilder.Append(keyValuePair.Value.ToString());
				stringBuilder.Append(',');
			}
			if (this.values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}
        
		private class Parser
		{
			public JsonObject Parse(string json)
			{
				for (int i = 0; i < json.Length; i++)
				{
					char c = json[i];
					if (c == '"')
					{
						i = this.AppendUntilStringEnd(true, i, json);
					}
					else if (!char.IsWhiteSpace(c))
					{
						this.stringBuilder.Append(c);
					}
				}
				List<string> list = this.Split(json);
				this.mainJson = new JsonObject();
				for (int j = 0; j < list.Count; j += 2)
				{
					if (list[j].Length > 2)
					{
						string key = list[j].Substring(1, list[j].Length - 2);
						string value = list[j + 1];
						this.mainJson.values.Add(key, value);
					}
				}
				return this.mainJson;
			}
            
			private int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
			{
				this.stringBuilder.Append(json[startIdx]);
				for (int i = startIdx + 1; i < json.Length; i++)
				{
					if (json[i] == '\\')
					{
						if (appendEscapeCharacter)
						{
							this.stringBuilder.Append(json[i]);
						}
						this.stringBuilder.Append(json[i + 1]);
						i++;
					}
					else
					{
						if (json[i] == '"')
						{
							this.stringBuilder.Append(json[i]);
							return i;
						}
						this.stringBuilder.Append(json[i]);
					}
				}
				return json.Length - 1;
			}
            
			private List<string> Split(string json)
			{
				List<string> list = (this.splitArrayPool.Count <= 0) ? new List<string>() : this.splitArrayPool.Pop();
				list.Clear();
				int num = 0;
				this.stringBuilder.Length = 0;
				int i = 1;
				while (i < json.Length - 1)
				{
					char c = json[i];
					switch (c)
					{
					case '[':
						goto IL_91;
					default:
						switch (c)
						{
						case '{':
							goto IL_91;
						default:
							if (c != '"')
							{
								if (c != ',' && c != ':')
								{
									goto IL_DF;
								}
								if (num != 0)
								{
									goto IL_DF;
								}
								list.Add(this.stringBuilder.ToString());
								this.stringBuilder.Length = 0;
							}
							else
							{
								i = this.AppendUntilStringEnd(true, i, json);
							}
							break;
						case '}':
							goto IL_9A;
						}
						break;
					case ']':
						goto IL_9A;
					}
					IL_F2:
					i++;
					continue;
					IL_DF:
					this.stringBuilder.Append(json[i]);
					goto IL_F2;
					IL_9A:
					num--;
					goto IL_DF;
					IL_91:
					num++;
					goto IL_DF;
				}
				list.Add(this.stringBuilder.ToString());
				return list;
			}
            
			private Stack<List<string>> splitArrayPool = new Stack<List<string>>();
            
			private JsonObject mainJson = new JsonObject();
            
			private StringBuilder stringBuilder = new StringBuilder();
		}
	}
}
