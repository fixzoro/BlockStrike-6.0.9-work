using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

	[CustomPropertyDrawer(typeof(CryptoInt))]
	public class CryptoIntDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			SerializedProperty cryptoKey = prop.FindPropertyRelative("cryptoKey");
			SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
			SerializedProperty inited = prop.FindPropertyRelative("inited");

			int currentCryptoKey = cryptoKey.intValue;
			int val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = CryptoInt.cryptoKeyEditor;
				}
				hiddenValue.intValue = CryptoInt.Encrypt(0, currentCryptoKey);
				inited.boolValue = true;
			}
			else
			{
				val = CryptoInt.Decrypt(hiddenValue.intValue, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.IntField(position, label, val);
			if (EditorGUI.EndChangeCheck())
				hiddenValue.intValue = CryptoInt.Encrypt(val, currentCryptoKey);

			fakeValue.intValue = val;
		}
	}