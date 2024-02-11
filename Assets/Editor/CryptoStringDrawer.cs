using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

	[CustomPropertyDrawer(typeof(CryptoString))]
	public class CryptoStringDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
            SerializedProperty hiddenValue = prop.FindPropertyRelative("hiddenValue");
            SetBoldIfValueOverridePrefab(prop, hiddenValue);

            SerializedProperty cryptoKey = prop.FindPropertyRelative("cryptoKey");
            SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
            SerializedProperty inited = prop.FindPropertyRelative("inited");

            string val = "";

            if (inited.boolValue)
            {
                if (string.IsNullOrEmpty(cryptoKey.GetArrayElementAtIndex(0).intValue.ToString()))
                {
                    for (int i = 0; i < CryptoString.cryptoKeyEditor.Length; i++)
                    {
                        cryptoKey.GetArrayElementAtIndex(i).intValue = CryptoString.cryptoKeyEditor[i];
                    }

                }
                byte[] array = new byte[hiddenValue.arraySize];
                for (int i = 0; i < array.Length; i++)
                {
                    int index2 = i % cryptoKey.arraySize;
                    array[i] = (byte)(hiddenValue.GetArrayElementAtIndex(i).intValue ^ cryptoKey.GetArrayElementAtIndex(index2).intValue);
                }
                string @string = CryptoString.GetString(array);
                val = @string;
            }

            int dataIndex = prop.propertyPath.IndexOf("Array.data[");

            if (dataIndex >= 0)
            {
                dataIndex += 11;

                string index = "Element " + prop.propertyPath.Substring(dataIndex, prop.propertyPath.IndexOf("]", dataIndex) - dataIndex);
                label.text = index;
            }

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.TextField(position, label, val);
            if (EditorGUI.EndChangeCheck())
            {
                //this.cryptoKey = new byte[CryptoManager.random.Next(10, 30)];///////////
                cryptoKey.ClearArray();

                byte[] bytes1 = new byte[CryptoManager.random.Next(10, 30)];
                for (int i = 0; i < bytes1.Length; i++)
                {
                    cryptoKey.InsertArrayElementAtIndex(i);
                    cryptoKey.GetArrayElementAtIndex(i).intValue = bytes1[i];
                }
                ///////////////////////////////////////////////////////////////////////////

                //CryptoManager.random.NextBytes(this.cryptoKey);//////////////////////////
                byte[] bytes2 = new byte[cryptoKey.arraySize];
                for (int i = 0; i < cryptoKey.arraySize; i++)
                {
                    bytes2[i] = (byte)cryptoKey.GetArrayElementAtIndex(i).intValue;
                }

                CryptoManager.random.NextBytes(bytes2);
                ///////////////////////////////////////////////////////////////////////////

                //this.hiddenValue = CryptoString.GetBytes(value);/////////////////////////
                hiddenValue.ClearArray();

                byte[] bytes3 = CryptoString.GetBytes(val);
                for (int i = 0; i < bytes3.Length; i++)
                {
                    hiddenValue.InsertArrayElementAtIndex(i);
                    hiddenValue.GetArrayElementAtIndex(i).intValue = bytes3[i];
                }
                ///////////////////////////////////////////////////////////////////////////

                for (int i = 0; i < hiddenValue.arraySize; i++)
                {
                    hiddenValue.GetArrayElementAtIndex(i).intValue = (byte)((byte)hiddenValue.GetArrayElementAtIndex(i).intValue ^ (byte)cryptoKey.GetArrayElementAtIndex(i % cryptoKey.arraySize).intValue);
                }
                fakeValue.stringValue = ((!CryptoManager.fakeValue) ? string.Empty : val);
            }
            fakeValue.stringValue = val;

            EditorGUI.showMixedValue = false;
        }
 
		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		static string GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
	}