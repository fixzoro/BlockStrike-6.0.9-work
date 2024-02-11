using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

	[CustomPropertyDrawer(typeof(CryptoFloat))]
	public class CryptoFloatDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
            SerializedProperty hiddenValue1 = prop.FindPropertyRelative("hiddenValue.b1");
            SerializedProperty hiddenValue2 = prop.FindPropertyRelative("hiddenValue.b2");
            SerializedProperty hiddenValue3 = prop.FindPropertyRelative("hiddenValue.b3");
            SerializedProperty hiddenValue4 = prop.FindPropertyRelative("hiddenValue.b4");

            SerializedProperty cryptoKey = prop.FindPropertyRelative("cryptoKey");
            SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
            SerializedProperty inited = prop.FindPropertyRelative("inited");

            int currentCryptoKey = cryptoKey.intValue;

            float val = 0;

            CryptoFloat.FloatIntBytesUnion floatIntBytesUnion = default(CryptoFloat.FloatIntBytesUnion);

            byte b1 = (byte)hiddenValue1.intValue;

            byte b2 = (byte)hiddenValue2.intValue;

            byte b3 = (byte)hiddenValue3.intValue;

            byte b4 = (byte)hiddenValue4.intValue;

            CryptoFloat.Byte4 bytes = new CryptoFloat.Byte4();

            bytes.b1 = b1;
            bytes.b2 = b2;
            bytes.b3 = b3;
            bytes.b4 = b4;

            floatIntBytesUnion.b4 = bytes;
            floatIntBytesUnion.i ^= cryptoKey.intValue;
            val = floatIntBytesUnion.f;

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.FloatField(position, label, val);
            if (EditorGUI.EndChangeCheck())
            {
                CryptoFloat.FloatIntBytesUnion floatIntBytesUnion2 = default(CryptoFloat.FloatIntBytesUnion);
                floatIntBytesUnion2.f = val;
                floatIntBytesUnion2.i ^= currentCryptoKey;
                CryptoFloat.Byte4 bytes2 = floatIntBytesUnion2.b4;
                
                hiddenValue1.intValue = bytes2.b1;
                hiddenValue2.intValue = bytes2.b2;
                hiddenValue3.intValue = bytes2.b3;
                hiddenValue4.intValue = bytes2.b4;
            }

            fakeValue.floatValue = val;
        }

		[StructLayout(LayoutKind.Explicit)]
		private struct IntBytesUnion
		{
			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;
		}
	}