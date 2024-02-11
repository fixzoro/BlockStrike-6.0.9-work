using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

    [CustomPropertyDrawer(typeof(CryptoInt3))]
    public class CryptoIntDrawer3 : ObscuredPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            SerializedProperty hiddenValue = prop.FindPropertyRelative("hiddenValue");
            SetBoldIfValueOverridePrefab(prop, hiddenValue);

            SerializedProperty cryptoKey = prop.FindPropertyRelative("cryptoKey");
            SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
            SerializedProperty inited = prop.FindPropertyRelative("inited");

            int val = 0;

            if (!inited.boolValue)
            {
                do
                {
                    cryptoKey.intValue = CryptoManager.Next();
                }
                while (cryptoKey.intValue == 0);
                hiddenValue.intValue = cryptoKey.intValue;
                fakeValue.intValue = -cryptoKey.intValue;
                inited.boolValue = true;
            }
            val = hiddenValue.intValue - cryptoKey.intValue;

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.IntField(position, label, val);
            if (EditorGUI.EndChangeCheck())
            {
                do
                {
                    cryptoKey.intValue = CryptoManager.Next();
                }
                while (cryptoKey.intValue == 0);
                hiddenValue.intValue = val + cryptoKey.intValue;
                fakeValue.intValue = val - cryptoKey.intValue;
            }
        }
    }