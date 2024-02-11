using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

    [CustomPropertyDrawer(typeof(CryptoInt2))]
    public class CryptoIntDrawer2 : ObscuredPropertyDrawer
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
                    currentCryptoKey = cryptoKey.intValue = CryptoInt2.cryptoKeyEditor;
                }
                hiddenValue.intValue = CryptoInt2.Encrypt(0, currentCryptoKey);
                inited.boolValue = true;
            }
            else
            {
                val = CryptoInt2.Decrypt(hiddenValue.intValue, currentCryptoKey);
            }

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.IntField(position, label, val);
            if (EditorGUI.EndChangeCheck())
                hiddenValue.intValue = CryptoInt2.Encrypt(val, currentCryptoKey);

            fakeValue.intValue = val;
        }
    }