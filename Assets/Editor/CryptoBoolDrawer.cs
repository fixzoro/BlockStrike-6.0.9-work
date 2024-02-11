using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

	[CustomPropertyDrawer(typeof(CryptoBool))]
	public class CryptoBoolDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			SerializedProperty cryptoKey = prop.FindPropertyRelative("cryptoKey");
			SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
			SerializedProperty fakeValueChanged = prop.FindPropertyRelative("fakeValueChanged");
			SerializedProperty inited = prop.FindPropertyRelative("inited");

			int currentCryptoKey = cryptoKey.intValue;
			bool val = false;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = CryptoBool.cryptoKeyEditor;
				}
				inited.boolValue = true;
				hiddenValue.intValue = CryptoBool.Encrypt(val, (byte)currentCryptoKey);
			}
			else
			{
				val = CryptoBool.Decrypt(hiddenValue.intValue, (byte)currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.Toggle(position, label, val);
			if (EditorGUI.EndChangeCheck())
				hiddenValue.intValue = CryptoBool.Encrypt(val, (byte)currentCryptoKey);

			fakeValue.boolValue = val;
			fakeValueChanged.boolValue = true;
		}
	}