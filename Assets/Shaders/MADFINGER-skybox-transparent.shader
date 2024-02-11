Shader "MADFINGER/Environment/Skybox - transparent - no fog" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("HACK: temporary to fix lightmap bouncing light (will be fixed in RC1)", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent-20" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="Vertex" "QUEUE"="Transparent-20" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { combine texture }
 }
}
}