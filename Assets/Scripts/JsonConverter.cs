using System;

namespace FreeJSON
{
	public abstract class JsonConverter
	{
		public abstract string Serialize(object value);
        
		public abstract object Deserialize(Type type, string value);
        
		public abstract bool CanConvert(Type type);
	}
}
