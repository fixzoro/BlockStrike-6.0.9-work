Shader "MADFINGER/Environment/Scroll 2 Layers Aditive - additive blend" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _AMultiplier ("Layer Multiplier", Float) = 0.5
 _TintColor ("Tint color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Vector 5 [_Time]
Vector 6 [_MainTex_ST]
Vector 7 [_DetailTex_ST]
Float 8 [_ScrollX]
Float 9 [_ScrollY]
Float 10 [_Scroll2X]
Float 11 [_Scroll2Y]
Float 12 [_AMultiplier]
Vector 13 [_TintColor]
"!!ARBvp1.0
# 20 ALU
PARAM c[14] = { program.local[0],
		state.matrix.mvp,
		program.local[5..13] };
TEMP R0;
TEMP R1;
MUL R1, vertex.color, c[13];
MUL R1, R1, c[13].w;
MAD R0.zw, vertex.texcoord[0].xyxy, c[6].xyxy, c[6];
MOV R0.y, c[9].x;
MOV R0.x, c[8];
MUL R0.xy, R0, c[5];
FRC R0.xy, R0;
ADD result.texcoord[0].xy, R0.zwzw, R0;
MOV R0.y, c[11].x;
MOV R0.x, c[10];
MUL R0.xy, R0, c[5];
FRC R0.xy, R0;
MAD R0.zw, vertex.texcoord[0].xyxy, c[7].xyxy, c[7];
ADD result.texcoord[1].xy, R0.zwzw, R0;
MUL result.texcoord[2].xyz, R1, c[12].x;
MOV result.texcoord[2].w, R1;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 20 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Time]
Vector 5 [_MainTex_ST]
Vector 6 [_DetailTex_ST]
Float 7 [_ScrollX]
Float 8 [_ScrollY]
Float 9 [_Scroll2X]
Float 10 [_Scroll2Y]
Float 11 [_AMultiplier]
Vector 12 [_TintColor]
"vs_2_0
; 24 ALU
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
mul r1, v2, c12
mul r1, r1, c12.w
mad r0.zw, v1.xyxy, c5.xyxy, c5
mov r0.y, c8.x
mov r0.x, c7
mul r0.xy, r0, c4
frc r0.xy, r0
add oT0.xy, r0.zwzw, r0
mov r0.y, c10.x
mov r0.x, c9
mul r0.xy, r0, c4
frc r0.xy, r0
mad r0.zw, v1.xyxy, c6.xyxy, c6
add oT1.xy, r0.zwzw, r0
mul oT2.xyz, r1, c11.x
mov oT2.w, r1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "d3d11 " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 96
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_AMultiplier]
Vector 80 [_TintColor]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedijpbemibpjfncefpileiceeecobhpblfabaaaaaaeiaeaaaaadaaaaaa
cmaaaaaapeaaaaaahmabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaa
aiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaaaaaaaaaa
adaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefc
meacaaaaeaaaabaalbaaaaaafjaaaaaeegiocaaaaaaaaaaaagaaaaaafjaaaaae
egiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaaaeaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaadpcbabaaaafaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
mccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaai
pcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaa
adaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaa
egbabaaaadaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaa
diaaaaajpcaabaaaabaaaaaaegiocaaaaaaaaaaaadaaaaaaegiecaaaabaaaaaa
aaaaaaaabkaaaaafpcaabaaaabaaaaaaegaobaaaabaaaaaaaaaaaaahdccabaaa
abaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaadcaaaaaldcaabaaaaaaaaaaa
egbabaaaadaaaaaaegiacaaaaaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaa
aaaaaaahmccabaaaabaaaaaakgaobaaaabaaaaaaagaebaaaaaaaaaaadgaaaaaf
hcaabaaaaaaaaaaaegbcbaaaafaaaaaadgaaaaagicaabaaaaaaaaaaadkiacaaa
aaaaaaaaafaaaaaadiaaaaaipcaabaaaaaaaaaaaegaobaaaaaaaaaaaegiocaaa
aaaaaaaaafaaaaaadgaaaaagbcaabaaaabaaaaaadkiacaaaaaaaaaaaafaaaaaa
dgaaaaaficaabaaaabaaaaaadkbabaaaafaaaaaadiaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaagambaaaabaaaaaadiaaaaaihccabaaaacaaaaaaegacbaaa
aaaaaaaaagiacaaaaaaaaaaaaeaaaaaadgaaaaaficcabaaaacaaaaaadkaabaaa
aaaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 96
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_AMultiplier]
Vector 80 [_TintColor]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedpiabjianhbeinomohbkehhcmpijaiahiabaaaaaabeagaaaaaeaaaaaa
daaaaaaapiabaaaameaeaaaaimafaaaaebgpgodjmaabaaaamaabaaaaaaacpopp
heabaaaaemaaaaaaadaaceaaaaaaeiaaaaaaeiaaaaaaceaaabaaeiaaaaaaabaa
afaaabaaaaaaaaaaabaaaaaaabaaagaaaaaaaaaaacaaaaaaaeaaahaaaaaaaaaa
aaaaaaaaabacpoppbpaaaaacafaaaaiaaaaaapjabpaaaaacafaaadiaadaaapja
bpaaaaacafaaafiaafaaapjaaeaaaaaeaaaaadiaadaaoejaabaaoekaabaaooka
abaaaaacabaaapiaadaaoekaafaaaaadabaaapiaabaaoeiaagaaeekabdaaaaac
abaaapiaabaaoeiaacaaaaadaaaaadoaaaaaoeiaabaaoeiaaeaaaaaeaaaaadia
adaaobjaacaaobkaacaaolkaacaaaaadaaaaamoaabaaleiaaaaaeeiaabaaaaac
aaaaahiaafaaoejaabaaaaacaaaaaiiaafaappkaafaaaaadaaaaapiaaaaaoeia
afaaoekaabaaaaacabaaabiaafaappkaabaaaaacabaaaiiaafaappjaafaaaaad
aaaaapiaaaaaoeiaabaamaiaafaaaaadabaaahoaaaaaoeiaaeaaaakaabaaaaac
abaaaioaaaaappiaafaaaaadaaaaapiaaaaaffjaaiaaoekaaeaaaaaeaaaaapia
ahaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaajaaoekaaaaakkjaaaaaoeia
aeaaaaaeaaaaapiaakaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappia
aaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaafdeieefcmeacaaaa
eaaaabaalbaaaaaafjaaaaaeegiocaaaaaaaaaaaagaaaaaafjaaaaaeegiocaaa
abaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaadpcbabaaaafaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaaegbabaaa
adaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaadiaaaaaj
pcaabaaaabaaaaaaegiocaaaaaaaaaaaadaaaaaaegiecaaaabaaaaaaaaaaaaaa
bkaaaaafpcaabaaaabaaaaaaegaobaaaabaaaaaaaaaaaaahdccabaaaabaaaaaa
egaabaaaaaaaaaaaegaabaaaabaaaaaadcaaaaaldcaabaaaaaaaaaaaegbabaaa
adaaaaaaegiacaaaaaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaaaaaaaaah
mccabaaaabaaaaaakgaobaaaabaaaaaaagaebaaaaaaaaaaadgaaaaafhcaabaaa
aaaaaaaaegbcbaaaafaaaaaadgaaaaagicaabaaaaaaaaaaadkiacaaaaaaaaaaa
afaaaaaadiaaaaaipcaabaaaaaaaaaaaegaobaaaaaaaaaaaegiocaaaaaaaaaaa
afaaaaaadgaaaaagbcaabaaaabaaaaaadkiacaaaaaaaaaaaafaaaaaadgaaaaaf
icaabaaaabaaaaaadkbabaaaafaaaaaadiaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaagambaaaabaaaaaadiaaaaaihccabaaaacaaaaaaegacbaaaaaaaaaaa
agiacaaaaaaaaaaaaeaaaaaadgaaaaaficcabaaaacaaaaaadkaabaaaaaaaaaaa
doaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
apaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaaaaaalaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaaabaaaaaaaaaaaaaa
adaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaaadaaaaaaafaaaaaa
apapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfcenebemaafeeffied
epepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklkl"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Vector 5 [_Time]
Vector 6 [_MainTex_ST]
Vector 7 [_DetailTex_ST]
Float 8 [_ScrollX]
Float 9 [_ScrollY]
Float 10 [_Scroll2X]
Float 11 [_Scroll2Y]
Float 12 [_AMultiplier]
Vector 13 [_TintColor]
"!!ARBvp1.0
# 20 ALU
PARAM c[14] = { program.local[0],
		state.matrix.mvp,
		program.local[5..13] };
TEMP R0;
TEMP R1;
MUL R1, vertex.color, c[13];
MUL R1, R1, c[13].w;
MAD R0.zw, vertex.texcoord[0].xyxy, c[6].xyxy, c[6];
MOV R0.y, c[9].x;
MOV R0.x, c[8];
MUL R0.xy, R0, c[5];
FRC R0.xy, R0;
ADD result.texcoord[0].xy, R0.zwzw, R0;
MOV R0.y, c[11].x;
MOV R0.x, c[10];
MUL R0.xy, R0, c[5];
FRC R0.xy, R0;
MAD R0.zw, vertex.texcoord[0].xyxy, c[7].xyxy, c[7];
ADD result.texcoord[1].xy, R0.zwzw, R0;
MUL result.texcoord[2].xyz, R1, c[12].x;
MOV result.texcoord[2].w, R1;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 20 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Time]
Vector 5 [_MainTex_ST]
Vector 6 [_DetailTex_ST]
Float 7 [_ScrollX]
Float 8 [_ScrollY]
Float 9 [_Scroll2X]
Float 10 [_Scroll2Y]
Float 11 [_AMultiplier]
Vector 12 [_TintColor]
"vs_2_0
; 24 ALU
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
mul r1, v2, c12
mul r1, r1, c12.w
mad r0.zw, v1.xyxy, c5.xyxy, c5
mov r0.y, c8.x
mov r0.x, c7
mul r0.xy, r0, c4
frc r0.xy, r0
add oT0.xy, r0.zwzw, r0
mov r0.y, c10.x
mov r0.x, c9
mul r0.xy, r0, c4
frc r0.xy, r0
mad r0.zw, v1.xyxy, c6.xyxy, c6
add oT1.xy, r0.zwzw, r0
mul oT2.xyz, r1, c11.x
mov oT2.w, r1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "d3d11 " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 96
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_AMultiplier]
Vector 80 [_TintColor]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedijpbemibpjfncefpileiceeecobhpblfabaaaaaaeiaeaaaaadaaaaaa
cmaaaaaapeaaaaaahmabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaa
aiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaaaaaaaaaa
adaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefc
meacaaaaeaaaabaalbaaaaaafjaaaaaeegiocaaaaaaaaaaaagaaaaaafjaaaaae
egiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaaaeaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaadpcbabaaaafaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
mccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaai
pcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaa
adaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaa
egbabaaaadaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaa
diaaaaajpcaabaaaabaaaaaaegiocaaaaaaaaaaaadaaaaaaegiecaaaabaaaaaa
aaaaaaaabkaaaaafpcaabaaaabaaaaaaegaobaaaabaaaaaaaaaaaaahdccabaaa
abaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaadcaaaaaldcaabaaaaaaaaaaa
egbabaaaadaaaaaaegiacaaaaaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaa
aaaaaaahmccabaaaabaaaaaakgaobaaaabaaaaaaagaebaaaaaaaaaaadgaaaaaf
hcaabaaaaaaaaaaaegbcbaaaafaaaaaadgaaaaagicaabaaaaaaaaaaadkiacaaa
aaaaaaaaafaaaaaadiaaaaaipcaabaaaaaaaaaaaegaobaaaaaaaaaaaegiocaaa
aaaaaaaaafaaaaaadgaaaaagbcaabaaaabaaaaaadkiacaaaaaaaaaaaafaaaaaa
dgaaaaaficaabaaaabaaaaaadkbabaaaafaaaaaadiaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaagambaaaabaaaaaadiaaaaaihccabaaaacaaaaaaegacbaaa
aaaaaaaaagiacaaaaaaaaaaaaeaaaaaadgaaaaaficcabaaaacaaaaaadkaabaaa
aaaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 96
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_AMultiplier]
Vector 80 [_TintColor]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedpiabjianhbeinomohbkehhcmpijaiahiabaaaaaabeagaaaaaeaaaaaa
daaaaaaapiabaaaameaeaaaaimafaaaaebgpgodjmaabaaaamaabaaaaaaacpopp
heabaaaaemaaaaaaadaaceaaaaaaeiaaaaaaeiaaaaaaceaaabaaeiaaaaaaabaa
afaaabaaaaaaaaaaabaaaaaaabaaagaaaaaaaaaaacaaaaaaaeaaahaaaaaaaaaa
aaaaaaaaabacpoppbpaaaaacafaaaaiaaaaaapjabpaaaaacafaaadiaadaaapja
bpaaaaacafaaafiaafaaapjaaeaaaaaeaaaaadiaadaaoejaabaaoekaabaaooka
abaaaaacabaaapiaadaaoekaafaaaaadabaaapiaabaaoeiaagaaeekabdaaaaac
abaaapiaabaaoeiaacaaaaadaaaaadoaaaaaoeiaabaaoeiaaeaaaaaeaaaaadia
adaaobjaacaaobkaacaaolkaacaaaaadaaaaamoaabaaleiaaaaaeeiaabaaaaac
aaaaahiaafaaoejaabaaaaacaaaaaiiaafaappkaafaaaaadaaaaapiaaaaaoeia
afaaoekaabaaaaacabaaabiaafaappkaabaaaaacabaaaiiaafaappjaafaaaaad
aaaaapiaaaaaoeiaabaamaiaafaaaaadabaaahoaaaaaoeiaaeaaaakaabaaaaac
abaaaioaaaaappiaafaaaaadaaaaapiaaaaaffjaaiaaoekaaeaaaaaeaaaaapia
ahaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaajaaoekaaaaakkjaaaaaoeia
aeaaaaaeaaaaapiaakaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappia
aaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaafdeieefcmeacaaaa
eaaaabaalbaaaaaafjaaaaaeegiocaaaaaaaaaaaagaaaaaafjaaaaaeegiocaaa
abaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaadpcbabaaaafaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaaegbabaaa
adaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaadiaaaaaj
pcaabaaaabaaaaaaegiocaaaaaaaaaaaadaaaaaaegiecaaaabaaaaaaaaaaaaaa
bkaaaaafpcaabaaaabaaaaaaegaobaaaabaaaaaaaaaaaaahdccabaaaabaaaaaa
egaabaaaaaaaaaaaegaabaaaabaaaaaadcaaaaaldcaabaaaaaaaaaaaegbabaaa
adaaaaaaegiacaaaaaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaaaaaaaaah
mccabaaaabaaaaaakgaobaaaabaaaaaaagaebaaaaaaaaaaadgaaaaafhcaabaaa
aaaaaaaaegbcbaaaafaaaaaadgaaaaagicaabaaaaaaaaaaadkiacaaaaaaaaaaa
afaaaaaadiaaaaaipcaabaaaaaaaaaaaegaobaaaaaaaaaaaegiocaaaaaaaaaaa
afaaaaaadgaaaaagbcaabaaaabaaaaaadkiacaaaaaaaaaaaafaaaaaadgaaaaaf
icaabaaaabaaaaaadkbabaaaafaaaaaadiaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaagambaaaabaaaaaadiaaaaaihccabaaaacaaaaaaegacbaaaaaaaaaaa
agiacaaaaaaaaaaaaeaaaaaadgaaaaaficcabaaaacaaaaaadkaabaaaaaaaaaaa
doaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
apaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaaaaaalaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaaabaaaaaaaaaaaaaa
adaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaaadaaaaaaafaaaaaa
apapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfcenebemaafeeffied
epepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklkl"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 4 ALU, 2 TEX
TEMP R0;
TEMP R1;
TEX R1, fragment.texcoord[1], texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
ADD R0, R0, R1;
MUL result.color, R0, fragment.texcoord[2];
END
# 4 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_2_0
; 3 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
dcl t0.xy
dcl t1.xy
dcl t2
texld r0, t1, s1
texld r1, t0, s0
add_pp r0, r1, r0
mul_pp r0, r0, t2
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0
eefiecedpkcicbddbkocildigcbbmljklhelndfpabaaaaaaomabaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaapapaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcpmaaaaaaeaaaaaaadpaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
fibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaad
mcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaabaaaaaa
eghobaaaabaaaaaaaagabaaaabaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaa
egbobaaaacaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0_level_9_3
eefiecedpjakiecgjbhljcdfkjkpdkogfandkfkkabaaaaaaleacaaaaaeaaaaaa
daaaaaaapeaaaaaapiabaaaaiaacaaaaebgpgodjlmaaaaaalmaaaaaaaaacpppp
jaaaaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppbpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaaiaabaacpla
bpaaaaacaaaaaajaaaaiapkabpaaaaacaaaaaajaabaiapkaabaaaaacaaaaadia
aaaaollaecaaaaadabaacpiaaaaaoelaaaaioekaecaaaaadaaaacpiaaaaaoeia
abaioekaacaaaaadaaaacpiaaaaaoeiaabaaoeiaafaaaaadaaaacpiaaaaaoeia
abaaoelaabaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcpmaaaaaaeaaaaaaa
dpaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadmcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaaaaaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaa
aaaaaaaaegaobaaaaaaaaaaaegbobaaaacaaaaaadoaaaaabejfdeheoiaaaaaaa
aeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
heaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaa
aaaaaaaaadaaaaaaabaaaaaaamamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaa
acaaaaaaapapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
epfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 4 ALU, 2 TEX
TEMP R0;
TEMP R1;
TEX R1, fragment.texcoord[1], texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
ADD R0, R0, R1;
MUL result.color, R0, fragment.texcoord[2];
END
# 4 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_2_0
; 3 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
dcl t0.xy
dcl t1.xy
dcl t2
texld r0, t1, s1
texld r1, t0, s0
add_pp r0, r1, r0
mul_pp r0, r0, t2
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0
eefiecedpkcicbddbkocildigcbbmljklhelndfpabaaaaaaomabaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaapapaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcpmaaaaaaeaaaaaaadpaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
fibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaad
mcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaabaaaaaa
eghobaaaabaaaaaaaagabaaaabaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaa
egbobaaaacaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0_level_9_3
eefiecedpjakiecgjbhljcdfkjkpdkogfandkfkkabaaaaaaleacaaaaaeaaaaaa
daaaaaaapeaaaaaapiabaaaaiaacaaaaebgpgodjlmaaaaaalmaaaaaaaaacpppp
jaaaaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppbpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaaiaabaacpla
bpaaaaacaaaaaajaaaaiapkabpaaaaacaaaaaajaabaiapkaabaaaaacaaaaadia
aaaaollaecaaaaadabaacpiaaaaaoelaaaaioekaecaaaaadaaaacpiaaaaaoeia
abaioekaacaaaaadaaaacpiaaaaaoeiaabaaoeiaafaaaaadaaaacpiaaaaaoeia
abaaoelaabaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcpmaaaaaaeaaaaaaa
dpaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadmcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaaaaaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaa
aaaaaaaaegaobaaaaaaaaaaaegbobaaaacaaaaaadoaaaaabejfdeheoiaaaaaaa
aeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
heaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaa
aaaaaaaaadaaaaaaabaaaaaaamamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaa
acaaaaaaapapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
epfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
}
 }
}
}