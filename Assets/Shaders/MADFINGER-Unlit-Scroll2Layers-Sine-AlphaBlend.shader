Shader "MADFINGER/Environment/Scroll 2 Layers Sine AlphaBlended" {
Properties {
 _MainTex ("Base layer (RGB)", 2D) = "white" {}
 _DetailTex ("2nd layer (RGB)", 2D) = "white" {}
 _ScrollX ("Base layer Scroll speed X", Float) = 1
 _ScrollY ("Base layer Scroll speed Y", Float) = 0
 _Scroll2X ("2nd layer Scroll speed X", Float) = 1
 _Scroll2Y ("2nd layer Scroll speed Y", Float) = 0
 _SineAmplX ("Base layer sine amplitude X", Float) = 0.5
 _SineAmplY ("Base layer sine amplitude Y", Float) = 0.5
 _SineFreqX ("Base layer sine freq X", Float) = 10
 _SineFreqY ("Base layer sine freq Y", Float) = 10
 _SineAmplX2 ("2nd layer sine amplitude X", Float) = 0.5
 _SineAmplY2 ("2nd layer sine amplitude Y", Float) = 0.5
 _SineFreqX2 ("2nd layer sine freq X", Float) = 10
 _SineFreqY2 ("2nd layer sine freq Y", Float) = 10
 _Color ("Color", Color) = (1,1,1,1)
 _MMultiplier ("Layer Multiplier", Float) = 2
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha OneMinusSrcAlpha
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
Float 12 [_MMultiplier]
Float 13 [_SineAmplX]
Float 14 [_SineAmplY]
Float 15 [_SineFreqX]
Float 16 [_SineFreqY]
Float 17 [_SineAmplX2]
Float 18 [_SineAmplY2]
Float 19 [_SineFreqX2]
Float 20 [_SineFreqY2]
Vector 21 [_Color]
"!!ARBvp1.0
# 90 ALU
PARAM c[26] = { { 24.980801, -24.980801, 0.15915491, 0.25 },
		state.matrix.mvp,
		program.local[5..21],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R0.x, c[20];
MUL R0.x, R0, c[5];
MAD R0.x, R0, c[0].z, -c[0].w;
FRC R1.w, R0.x;
ADD R1.xyz, -R1.w, c[22];
MOV R0.x, c[19];
MUL R0.w, R0.x, c[5].x;
MUL R2.xyz, R1, R1;
MUL R0.xyz, R2, c[0].xyxw;
MAD R0.w, R0, c[0].z, -c[0];
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[22];
ADD R0.xyz, R0, c[23].xyxw;
MAD R0.xyz, R0, R2, c[23].zwzw;
MAD R0.xyz, R0, R2, c[24].xyxw;
MAD R3.xyz, R0, R2, c[24].zwzw;
MUL R1.xyz, R1, R1;
MUL R0.xyz, R1, c[0].xyxw;
MAD R3.xyz, R3, R2, c[22].wzww;
ADD R2.xyz, R0, c[23].xyxw;
SLT R4.x, R1.w, c[0].w;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[22].wzww;
DP3 R1.w, R3, -R0;
MAD R0.xyz, R2, R1, c[23].zwzw;
MAD R0.xyz, R0, R1, c[24].xyxw;
MAD R0.xyz, R0, R1, c[24].zwzw;
MAD R1.xyz, R0, R1, c[22].wzww;
SGE R0.yz, R0.w, c[25].xxyw;
SLT R0.x, R0.w, c[0].w;
DP3 R0.y, R0, c[22].wzww;
MOV R0.w, c[16].x;
MUL R0.w, R0, c[5].x;
DP3 R0.x, R1, -R0;
MAD R2.zw, vertex.texcoord[0].xyxy, c[7].xyxy, c[7];
MAD R0.w, R0, c[0].z, -c[0];
MOV R2.y, c[11].x;
MOV R2.x, c[10];
MUL R2.xy, R2, c[5];
FRC R2.xy, R2;
ADD R2.xy, R2.zwzw, R2;
MAD result.texcoord[0].w, R1, c[18].x, R2.y;
FRC R1.w, R0;
MAD result.texcoord[0].z, R0.x, c[17].x, R2.x;
ADD R1.xyz, -R1.w, c[22];
MOV R0.x, c[15];
MUL R0.w, R0.x, c[5].x;
MUL R2.xyz, R1, R1;
MUL R0.xyz, R2, c[0].xyxw;
MAD R0.w, R0, c[0].z, -c[0];
FRC R0.w, R0;
ADD R0.xyz, R0, c[23].xyxw;
MAD R0.xyz, R0, R2, c[23].zwzw;
MAD R0.xyz, R0, R2, c[24].xyxw;
MAD R3.xyz, R0, R2, c[24].zwzw;
ADD R1.xyz, -R0.w, c[22];
MUL R1.xyz, R1, R1;
MAD R3.xyz, R3, R2, c[22].wzww;
MUL R0.xyz, R1, c[0].xyxw;
ADD R2.xyz, R0, c[23].xyxw;
SLT R4.x, R1.w, c[0].w;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[22].wzww;
DP3 R1.w, R3, -R0;
MAD R0.xyz, R2, R1, c[23].zwzw;
MAD R0.xyz, R0, R1, c[24].xyxw;
MAD R0.xyz, R0, R1, c[24].zwzw;
MAD R0.xyz, R0, R1, c[22].wzww;
MAD R2.zw, vertex.texcoord[0].xyxy, c[6].xyxy, c[6];
MOV R2.y, c[9].x;
MOV R2.x, c[8];
MUL R2.xy, R2, c[5];
FRC R2.xy, R2;
ADD R4.xy, R2.zwzw, R2;
MAD result.texcoord[0].y, R1.w, c[14].x, R4;
MOV R1, c[21];
MUL R1, R1, c[12].x;
SLT R2.x, R0.w, c[0].w;
SGE R2.yz, R0.w, c[25].xxyw;
MOV R3.xz, R2;
DP3 R3.y, R2, c[22].wzww;
DP3 R0.x, R0, -R3;
MAD result.texcoord[0].x, R0, c[13], R4;
MUL result.texcoord[1], R1, vertex.color;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 90 instructions, 5 R-regs
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
Float 11 [_MMultiplier]
Float 12 [_SineAmplX]
Float 13 [_SineAmplY]
Float 14 [_SineFreqX]
Float 15 [_SineFreqY]
Float 16 [_SineAmplX2]
Float 17 [_SineAmplY2]
Float 18 [_SineFreqX2]
Float 19 [_SineFreqY2]
Vector 20 [_Color]
"vs_2_0
; 87 ALU
def c21, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c22, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c23, 0.15915491, 0.50000000, 6.28318501, -3.14159298
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
mov r0.x, c4
mul r0.x, c19, r0
mad r0.x, r0, c23, c23.y
frc r0.x, r0
mad r1.x, r0, c23.z, c23.w
sincos r0.xy, r1.x, c22.xyzw, c21.xyzw
mov r0.w, c10.x
mov r0.z, c9.x
mul r0.zw, r0, c4.xyxy
frc r1.xy, r0.zwzw
mad r0.zw, v1.xyxy, c6.xyxy, c6
add r2.xy, r0.zwzw, r1
mov r0.x, c4
mad oT0.w, r0.y, c17.x, r2.y
mul r0.y, c18.x, r0.x
mov r0.x, c4
mul r0.z, c15.x, r0.x
mad r0.y, r0, c23.x, c23
frc r0.x, r0.y
mad r0.y, r0.z, c23.x, c23
mad r0.x, r0, c23.z, c23.w
sincos r1.xy, r0.x, c22.xyzw, c21.xyzw
frc r0.y, r0
mad r1.x, r0.y, c23.z, c23.w
sincos r0.xy, r1.x, c22.xyzw, c21.xyzw
mad oT0.z, r1.y, c16.x, r2.x
mov r0.x, c4
mul r0.x, c14, r0
mad r0.x, r0, c23, c23.y
frc r0.x, r0
mad r0.zw, v1.xyxy, c5.xyxy, c5
mov r1.y, c8.x
mov r1.x, c7
mul r1.xy, r1, c4
frc r1.xy, r1
add r1.xy, r0.zwzw, r1
mad r1.z, r0.x, c23, c23.w
mad oT0.y, r0, c13.x, r1
sincos r0.xy, r1.z, c22.xyzw, c21.xyzw
mov r0.x, c11
mul r2, c20, r0.x
mad oT0.x, r0.y, c12, r1
mul oT1, r2, v2
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
ConstBuffer "$Globals" 128
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_MMultiplier]
Float 68 [_SineAmplX]
Float 72 [_SineAmplY]
Float 76 [_SineFreqX]
Float 80 [_SineFreqY]
Float 84 [_SineAmplX2]
Float 88 [_SineAmplY2]
Float 92 [_SineFreqX2]
Float 96 [_SineFreqY2]
Vector 112 [_Color]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedickkmmbalegcppfigondkfbclcbemalfabaaaaaapaaeaaaaadaaaaaa
cmaaaaaapeaaaaaageabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaafmaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklfdeieefcieadaaaaeaaaabaaobaaaaaafjaaaaaeegiocaaaaaaaaaaa
aiaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaad
pcbabaaaafaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaajbcaabaaaaaaaaaaadkiacaaa
aaaaaaaaaeaaaaaaakiacaaaabaaaaaaaaaaaaaaenaaaaagbcaabaaaaaaaaaaa
aanaaaaaakaabaaaaaaaaaaadcaaaaalgcaabaaaaaaaaaaaagbbbaaaadaaaaaa
agibcaaaaaaaaaaaabaaaaaakgilcaaaaaaaaaaaabaaaaaadiaaaaajpcaabaaa
abaaaaaaegiocaaaaaaaaaaaadaaaaaaegiecaaaabaaaaaaaaaaaaaabkaaaaaf
pcaabaaaabaaaaaaegaobaaaabaaaaaaaaaaaaahgcaabaaaaaaaaaaafgagbaaa
aaaaaaaaagabbaaaabaaaaaadcaaaaakbccabaaaabaaaaaaakaabaaaaaaaaaaa
bkiacaaaaaaaaaaaaeaaaaaabkaabaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaa
egbabaaaadaaaaaaegiacaaaaaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaa
aaaaaaahdcaabaaaaaaaaaaaogakbaaaabaaaaaaegaabaaaaaaaaaaadiaaaaaj
icaabaaaaaaaaaaaakiacaaaaaaaaaaaagaaaaaaakiacaaaabaaaaaaaaaaaaaa
enaaaaagicaabaaaaaaaaaaaaanaaaaadkaabaaaaaaaaaaadcaaaaakiccabaaa
abaaaaaadkaabaaaaaaaaaaackiacaaaaaaaaaaaafaaaaaabkaabaaaaaaaaaaa
diaaaaajkcaabaaaaaaaaaaaagimcaaaaaaaaaaaafaaaaaaagiacaaaabaaaaaa
aaaaaaaaenaaaaagkcaabaaaaaaaaaaaaanaaaaafganbaaaaaaaaaaadcaaaaak
cccabaaaabaaaaaabkaabaaaaaaaaaaackiacaaaaaaaaaaaaeaaaaaackaabaaa
aaaaaaaadcaaaaakeccabaaaabaaaaaadkaabaaaaaaaaaaabkiacaaaaaaaaaaa
afaaaaaaakaabaaaaaaaaaaadiaaaaajpcaabaaaaaaaaaaaagiacaaaaaaaaaaa
aeaaaaaaegiocaaaaaaaaaaaahaaaaaadiaaaaahpccabaaaacaaaaaaegaobaaa
aaaaaaaaegbobaaaafaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 128
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_MMultiplier]
Float 68 [_SineAmplX]
Float 72 [_SineAmplY]
Float 76 [_SineFreqX]
Float 80 [_SineFreqY]
Float 84 [_SineAmplX2]
Float 88 [_SineAmplY2]
Float 92 [_SineFreqX2]
Float 96 [_SineFreqY2]
Vector 112 [_Color]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedfknmppnfngaldgkeknbfdegkedpoffknabaaaaaadmajaaaaaeaaaaaa
daaaaaaahiaeaaaaaeaiaaaammaiaaaaebgpgodjeaaeaaaaeaaeaaaaaaacpopp
peadaaaaemaaaaaaadaaceaaaaaaeiaaaaaaeiaaaaaaceaaabaaeiaaaaaaabaa
ahaaabaaaaaaaaaaabaaaaaaabaaaiaaaaaaaaaaacaaaaaaaeaaajaaaaaaaaaa
aaaaaaaaabacpoppfbaaaaafanaaapkagdibihlekblfmpdhlkajlglkkekkckdn
fbaaaaafaoaaapkaaaaaaalpaaaaiadpaaaaaaaaaaaaaaaafbaaaaafapaaapka
idpjccdoaaaaiadonlapmjeanlapejmabpaaaaacafaaaaiaaaaaapjabpaaaaac
afaaadiaadaaapjabpaaaaacafaaafiaafaaapjaabaaaaacaaaaadiaaiaaoeka
afaaaaadaaaaaeiaaaaaaaiaaeaappkaaeaaaaaeaaaaaeiaaaaakkiaapaaaaka
apaaffkabdaaaaacaaaaaeiaaaaakkiaaeaaaaaeaaaaaeiaaaaakkiaapaakkka
apaappkaafaaaaadaaaaaeiaaaaakkiaaaaakkiaaeaaaaaeaaaaaiiaaaaakkia
anaaaakaanaaffkaaeaaaaaeaaaaaiiaaaaakkiaaaaappiaanaakkkaaeaaaaae
aaaaaiiaaaaakkiaaaaappiaanaappkaaeaaaaaeaaaaaiiaaaaakkiaaaaappia
aoaaaakaaeaaaaaeaaaaaeiaaaaakkiaaaaappiaaoaaffkaaeaaaaaeabaaadia
adaaoejaabaaoekaabaaookaafaaaaadacaaapiaaaaaeeiaadaaoekabdaaaaac
acaaapiaacaaoeiaacaaaaadaaaaakiaabaagaiaacaagaiaaeaaaaaeaaaaaboa
aaaakkiaaeaaffkaaaaaffiaafaaaaadaaaaagiaaaaaaaiaafaapakaaeaaaaae
aaaaagiaaaaaoeiaapaaaakaapaaffkabdaaaaacaaaaagiaaaaaoeiaaeaaaaae
aaaaagiaaaaaoeiaapaakkkaapaappkaafaaaaadaaaaagiaaaaaoeiaaaaaoeia
aeaaaaaeabaaadiaaaaaojiaanaaaakaanaaffkaaeaaaaaeabaaadiaaaaaojia
abaaoeiaanaakkkaaeaaaaaeabaaadiaaaaaojiaabaaoeiaanaappkaaeaaaaae
abaaadiaaaaaojiaabaaoeiaaoaaaakaaeaaaaaeaaaaagiaaaaaoeiaabaanaia
aoaaffkaaeaaaaaeaaaaacoaaaaaffiaaeaakkkaaaaappiaaeaaaaaeaaaaakia
adaagajaacaagakaacaaoikaacaaaaadaaaaakiaacaaoiiaaaaaoeiaaeaaaaae
aaaaaeoaaaaakkiaafaaffkaaaaaffiaafaaaaadaaaaabiaaaaaaaiaagaaaaka
aeaaaaaeaaaaabiaaaaaaaiaapaaaakaapaaffkabdaaaaacaaaaabiaaaaaaaia
aeaaaaaeaaaaabiaaaaaaaiaapaakkkaapaappkaafaaaaadaaaaabiaaaaaaaia
aaaaaaiaaeaaaaaeaaaaaciaaaaaaaiaanaaaakaanaaffkaaeaaaaaeaaaaacia
aaaaaaiaaaaaffiaanaakkkaaeaaaaaeaaaaaciaaaaaaaiaaaaaffiaanaappka
aeaaaaaeaaaaaciaaaaaaaiaaaaaffiaaoaaaakaaeaaaaaeaaaaabiaaaaaaaia
aaaaffiaaoaaffkaaeaaaaaeaaaaaioaaaaaaaiaafaakkkaaaaappiaabaaaaac
aaaaabiaaeaaaakaafaaaaadaaaaapiaaaaaaaiaahaaoekaafaaaaadabaaapoa
aaaaoeiaafaaoejaafaaaaadaaaaapiaaaaaffjaakaaoekaaeaaaaaeaaaaapia
ajaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaalaaoekaaaaakkjaaaaaoeia
aeaaaaaeaaaaapiaamaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappia
aaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaafdeieefcieadaaaa
eaaaabaaobaaaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaa
abaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaadpcbabaaaafaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaaabaaaaaagfaaaaadpccabaaa
acaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadiaaaaajbcaabaaaaaaaaaaadkiacaaaaaaaaaaaaeaaaaaaakiacaaa
abaaaaaaaaaaaaaaenaaaaagbcaabaaaaaaaaaaaaanaaaaaakaabaaaaaaaaaaa
dcaaaaalgcaabaaaaaaaaaaaagbbbaaaadaaaaaaagibcaaaaaaaaaaaabaaaaaa
kgilcaaaaaaaaaaaabaaaaaadiaaaaajpcaabaaaabaaaaaaegiocaaaaaaaaaaa
adaaaaaaegiecaaaabaaaaaaaaaaaaaabkaaaaafpcaabaaaabaaaaaaegaobaaa
abaaaaaaaaaaaaahgcaabaaaaaaaaaaafgagbaaaaaaaaaaaagabbaaaabaaaaaa
dcaaaaakbccabaaaabaaaaaaakaabaaaaaaaaaaabkiacaaaaaaaaaaaaeaaaaaa
bkaabaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaaegbabaaaadaaaaaaegiacaaa
aaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaaaaaaaaahdcaabaaaaaaaaaaa
ogakbaaaabaaaaaaegaabaaaaaaaaaaadiaaaaajicaabaaaaaaaaaaaakiacaaa
aaaaaaaaagaaaaaaakiacaaaabaaaaaaaaaaaaaaenaaaaagicaabaaaaaaaaaaa
aanaaaaadkaabaaaaaaaaaaadcaaaaakiccabaaaabaaaaaadkaabaaaaaaaaaaa
ckiacaaaaaaaaaaaafaaaaaabkaabaaaaaaaaaaadiaaaaajkcaabaaaaaaaaaaa
agimcaaaaaaaaaaaafaaaaaaagiacaaaabaaaaaaaaaaaaaaenaaaaagkcaabaaa
aaaaaaaaaanaaaaafganbaaaaaaaaaaadcaaaaakcccabaaaabaaaaaabkaabaaa
aaaaaaaackiacaaaaaaaaaaaaeaaaaaackaabaaaaaaaaaaadcaaaaakeccabaaa
abaaaaaadkaabaaaaaaaaaaabkiacaaaaaaaaaaaafaaaaaaakaabaaaaaaaaaaa
diaaaaajpcaabaaaaaaaaaaaagiacaaaaaaaaaaaaeaaaaaaegiocaaaaaaaaaaa
ahaaaaaadiaaaaahpccabaaaacaaaaaaegaobaaaaaaaaaaaegbobaaaafaaaaaa
doaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
apaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaaaaaalaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaaabaaaaaaaaaaaaaa
adaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaaadaaaaaaafaaaaaa
apapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfcenebemaafeeffied
epepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl"
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
Float 12 [_MMultiplier]
Float 13 [_SineAmplX]
Float 14 [_SineAmplY]
Float 15 [_SineFreqX]
Float 16 [_SineFreqY]
Float 17 [_SineAmplX2]
Float 18 [_SineAmplY2]
Float 19 [_SineFreqX2]
Float 20 [_SineFreqY2]
Vector 21 [_Color]
"!!ARBvp1.0
# 90 ALU
PARAM c[26] = { { 24.980801, -24.980801, 0.15915491, 0.25 },
		state.matrix.mvp,
		program.local[5..21],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R0.x, c[20];
MUL R0.x, R0, c[5];
MAD R0.x, R0, c[0].z, -c[0].w;
FRC R1.w, R0.x;
ADD R1.xyz, -R1.w, c[22];
MOV R0.x, c[19];
MUL R0.w, R0.x, c[5].x;
MUL R2.xyz, R1, R1;
MUL R0.xyz, R2, c[0].xyxw;
MAD R0.w, R0, c[0].z, -c[0];
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[22];
ADD R0.xyz, R0, c[23].xyxw;
MAD R0.xyz, R0, R2, c[23].zwzw;
MAD R0.xyz, R0, R2, c[24].xyxw;
MAD R3.xyz, R0, R2, c[24].zwzw;
MUL R1.xyz, R1, R1;
MUL R0.xyz, R1, c[0].xyxw;
MAD R3.xyz, R3, R2, c[22].wzww;
ADD R2.xyz, R0, c[23].xyxw;
SLT R4.x, R1.w, c[0].w;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[22].wzww;
DP3 R1.w, R3, -R0;
MAD R0.xyz, R2, R1, c[23].zwzw;
MAD R0.xyz, R0, R1, c[24].xyxw;
MAD R0.xyz, R0, R1, c[24].zwzw;
MAD R1.xyz, R0, R1, c[22].wzww;
SGE R0.yz, R0.w, c[25].xxyw;
SLT R0.x, R0.w, c[0].w;
DP3 R0.y, R0, c[22].wzww;
MOV R0.w, c[16].x;
MUL R0.w, R0, c[5].x;
DP3 R0.x, R1, -R0;
MAD R2.zw, vertex.texcoord[0].xyxy, c[7].xyxy, c[7];
MAD R0.w, R0, c[0].z, -c[0];
MOV R2.y, c[11].x;
MOV R2.x, c[10];
MUL R2.xy, R2, c[5];
FRC R2.xy, R2;
ADD R2.xy, R2.zwzw, R2;
MAD result.texcoord[0].w, R1, c[18].x, R2.y;
FRC R1.w, R0;
MAD result.texcoord[0].z, R0.x, c[17].x, R2.x;
ADD R1.xyz, -R1.w, c[22];
MOV R0.x, c[15];
MUL R0.w, R0.x, c[5].x;
MUL R2.xyz, R1, R1;
MUL R0.xyz, R2, c[0].xyxw;
MAD R0.w, R0, c[0].z, -c[0];
FRC R0.w, R0;
ADD R0.xyz, R0, c[23].xyxw;
MAD R0.xyz, R0, R2, c[23].zwzw;
MAD R0.xyz, R0, R2, c[24].xyxw;
MAD R3.xyz, R0, R2, c[24].zwzw;
ADD R1.xyz, -R0.w, c[22];
MUL R1.xyz, R1, R1;
MAD R3.xyz, R3, R2, c[22].wzww;
MUL R0.xyz, R1, c[0].xyxw;
ADD R2.xyz, R0, c[23].xyxw;
SLT R4.x, R1.w, c[0].w;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[22].wzww;
DP3 R1.w, R3, -R0;
MAD R0.xyz, R2, R1, c[23].zwzw;
MAD R0.xyz, R0, R1, c[24].xyxw;
MAD R0.xyz, R0, R1, c[24].zwzw;
MAD R0.xyz, R0, R1, c[22].wzww;
MAD R2.zw, vertex.texcoord[0].xyxy, c[6].xyxy, c[6];
MOV R2.y, c[9].x;
MOV R2.x, c[8];
MUL R2.xy, R2, c[5];
FRC R2.xy, R2;
ADD R4.xy, R2.zwzw, R2;
MAD result.texcoord[0].y, R1.w, c[14].x, R4;
MOV R1, c[21];
MUL R1, R1, c[12].x;
SLT R2.x, R0.w, c[0].w;
SGE R2.yz, R0.w, c[25].xxyw;
MOV R3.xz, R2;
DP3 R3.y, R2, c[22].wzww;
DP3 R0.x, R0, -R3;
MAD result.texcoord[0].x, R0, c[13], R4;
MUL result.texcoord[1], R1, vertex.color;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 90 instructions, 5 R-regs
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
Float 11 [_MMultiplier]
Float 12 [_SineAmplX]
Float 13 [_SineAmplY]
Float 14 [_SineFreqX]
Float 15 [_SineFreqY]
Float 16 [_SineAmplX2]
Float 17 [_SineAmplY2]
Float 18 [_SineFreqX2]
Float 19 [_SineFreqY2]
Vector 20 [_Color]
"vs_2_0
; 87 ALU
def c21, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c22, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c23, 0.15915491, 0.50000000, 6.28318501, -3.14159298
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
mov r0.x, c4
mul r0.x, c19, r0
mad r0.x, r0, c23, c23.y
frc r0.x, r0
mad r1.x, r0, c23.z, c23.w
sincos r0.xy, r1.x, c22.xyzw, c21.xyzw
mov r0.w, c10.x
mov r0.z, c9.x
mul r0.zw, r0, c4.xyxy
frc r1.xy, r0.zwzw
mad r0.zw, v1.xyxy, c6.xyxy, c6
add r2.xy, r0.zwzw, r1
mov r0.x, c4
mad oT0.w, r0.y, c17.x, r2.y
mul r0.y, c18.x, r0.x
mov r0.x, c4
mul r0.z, c15.x, r0.x
mad r0.y, r0, c23.x, c23
frc r0.x, r0.y
mad r0.y, r0.z, c23.x, c23
mad r0.x, r0, c23.z, c23.w
sincos r1.xy, r0.x, c22.xyzw, c21.xyzw
frc r0.y, r0
mad r1.x, r0.y, c23.z, c23.w
sincos r0.xy, r1.x, c22.xyzw, c21.xyzw
mad oT0.z, r1.y, c16.x, r2.x
mov r0.x, c4
mul r0.x, c14, r0
mad r0.x, r0, c23, c23.y
frc r0.x, r0
mad r0.zw, v1.xyxy, c5.xyxy, c5
mov r1.y, c8.x
mov r1.x, c7
mul r1.xy, r1, c4
frc r1.xy, r1
add r1.xy, r0.zwzw, r1
mad r1.z, r0.x, c23, c23.w
mad oT0.y, r0, c13.x, r1
sincos r0.xy, r1.z, c22.xyzw, c21.xyzw
mov r0.x, c11
mul r2, c20, r0.x
mad oT0.x, r0.y, c12, r1
mul oT1, r2, v2
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
ConstBuffer "$Globals" 128
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_MMultiplier]
Float 68 [_SineAmplX]
Float 72 [_SineAmplY]
Float 76 [_SineFreqX]
Float 80 [_SineFreqY]
Float 84 [_SineAmplX2]
Float 88 [_SineAmplY2]
Float 92 [_SineFreqX2]
Float 96 [_SineFreqY2]
Vector 112 [_Color]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedickkmmbalegcppfigondkfbclcbemalfabaaaaaapaaeaaaaadaaaaaa
cmaaaaaapeaaaaaageabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaafmaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklfdeieefcieadaaaaeaaaabaaobaaaaaafjaaaaaeegiocaaaaaaaaaaa
aiaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaad
pcbabaaaafaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaajbcaabaaaaaaaaaaadkiacaaa
aaaaaaaaaeaaaaaaakiacaaaabaaaaaaaaaaaaaaenaaaaagbcaabaaaaaaaaaaa
aanaaaaaakaabaaaaaaaaaaadcaaaaalgcaabaaaaaaaaaaaagbbbaaaadaaaaaa
agibcaaaaaaaaaaaabaaaaaakgilcaaaaaaaaaaaabaaaaaadiaaaaajpcaabaaa
abaaaaaaegiocaaaaaaaaaaaadaaaaaaegiecaaaabaaaaaaaaaaaaaabkaaaaaf
pcaabaaaabaaaaaaegaobaaaabaaaaaaaaaaaaahgcaabaaaaaaaaaaafgagbaaa
aaaaaaaaagabbaaaabaaaaaadcaaaaakbccabaaaabaaaaaaakaabaaaaaaaaaaa
bkiacaaaaaaaaaaaaeaaaaaabkaabaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaa
egbabaaaadaaaaaaegiacaaaaaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaa
aaaaaaahdcaabaaaaaaaaaaaogakbaaaabaaaaaaegaabaaaaaaaaaaadiaaaaaj
icaabaaaaaaaaaaaakiacaaaaaaaaaaaagaaaaaaakiacaaaabaaaaaaaaaaaaaa
enaaaaagicaabaaaaaaaaaaaaanaaaaadkaabaaaaaaaaaaadcaaaaakiccabaaa
abaaaaaadkaabaaaaaaaaaaackiacaaaaaaaaaaaafaaaaaabkaabaaaaaaaaaaa
diaaaaajkcaabaaaaaaaaaaaagimcaaaaaaaaaaaafaaaaaaagiacaaaabaaaaaa
aaaaaaaaenaaaaagkcaabaaaaaaaaaaaaanaaaaafganbaaaaaaaaaaadcaaaaak
cccabaaaabaaaaaabkaabaaaaaaaaaaackiacaaaaaaaaaaaaeaaaaaackaabaaa
aaaaaaaadcaaaaakeccabaaaabaaaaaadkaabaaaaaaaaaaabkiacaaaaaaaaaaa
afaaaaaaakaabaaaaaaaaaaadiaaaaajpcaabaaaaaaaaaaaagiacaaaaaaaaaaa
aeaaaaaaegiocaaaaaaaaaaaahaaaaaadiaaaaahpccabaaaacaaaaaaegaobaaa
aaaaaaaaegbobaaaafaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 128
Vector 16 [_MainTex_ST]
Vector 32 [_DetailTex_ST]
Float 48 [_ScrollX]
Float 52 [_ScrollY]
Float 56 [_Scroll2X]
Float 60 [_Scroll2Y]
Float 64 [_MMultiplier]
Float 68 [_SineAmplX]
Float 72 [_SineAmplY]
Float 76 [_SineFreqX]
Float 80 [_SineFreqY]
Float 84 [_SineAmplX2]
Float 88 [_SineAmplY2]
Float 92 [_SineFreqX2]
Float 96 [_SineFreqY2]
Vector 112 [_Color]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedfknmppnfngaldgkeknbfdegkedpoffknabaaaaaadmajaaaaaeaaaaaa
daaaaaaahiaeaaaaaeaiaaaammaiaaaaebgpgodjeaaeaaaaeaaeaaaaaaacpopp
peadaaaaemaaaaaaadaaceaaaaaaeiaaaaaaeiaaaaaaceaaabaaeiaaaaaaabaa
ahaaabaaaaaaaaaaabaaaaaaabaaaiaaaaaaaaaaacaaaaaaaeaaajaaaaaaaaaa
aaaaaaaaabacpoppfbaaaaafanaaapkagdibihlekblfmpdhlkajlglkkekkckdn
fbaaaaafaoaaapkaaaaaaalpaaaaiadpaaaaaaaaaaaaaaaafbaaaaafapaaapka
idpjccdoaaaaiadonlapmjeanlapejmabpaaaaacafaaaaiaaaaaapjabpaaaaac
afaaadiaadaaapjabpaaaaacafaaafiaafaaapjaabaaaaacaaaaadiaaiaaoeka
afaaaaadaaaaaeiaaaaaaaiaaeaappkaaeaaaaaeaaaaaeiaaaaakkiaapaaaaka
apaaffkabdaaaaacaaaaaeiaaaaakkiaaeaaaaaeaaaaaeiaaaaakkiaapaakkka
apaappkaafaaaaadaaaaaeiaaaaakkiaaaaakkiaaeaaaaaeaaaaaiiaaaaakkia
anaaaakaanaaffkaaeaaaaaeaaaaaiiaaaaakkiaaaaappiaanaakkkaaeaaaaae
aaaaaiiaaaaakkiaaaaappiaanaappkaaeaaaaaeaaaaaiiaaaaakkiaaaaappia
aoaaaakaaeaaaaaeaaaaaeiaaaaakkiaaaaappiaaoaaffkaaeaaaaaeabaaadia
adaaoejaabaaoekaabaaookaafaaaaadacaaapiaaaaaeeiaadaaoekabdaaaaac
acaaapiaacaaoeiaacaaaaadaaaaakiaabaagaiaacaagaiaaeaaaaaeaaaaaboa
aaaakkiaaeaaffkaaaaaffiaafaaaaadaaaaagiaaaaaaaiaafaapakaaeaaaaae
aaaaagiaaaaaoeiaapaaaakaapaaffkabdaaaaacaaaaagiaaaaaoeiaaeaaaaae
aaaaagiaaaaaoeiaapaakkkaapaappkaafaaaaadaaaaagiaaaaaoeiaaaaaoeia
aeaaaaaeabaaadiaaaaaojiaanaaaakaanaaffkaaeaaaaaeabaaadiaaaaaojia
abaaoeiaanaakkkaaeaaaaaeabaaadiaaaaaojiaabaaoeiaanaappkaaeaaaaae
abaaadiaaaaaojiaabaaoeiaaoaaaakaaeaaaaaeaaaaagiaaaaaoeiaabaanaia
aoaaffkaaeaaaaaeaaaaacoaaaaaffiaaeaakkkaaaaappiaaeaaaaaeaaaaakia
adaagajaacaagakaacaaoikaacaaaaadaaaaakiaacaaoiiaaaaaoeiaaeaaaaae
aaaaaeoaaaaakkiaafaaffkaaaaaffiaafaaaaadaaaaabiaaaaaaaiaagaaaaka
aeaaaaaeaaaaabiaaaaaaaiaapaaaakaapaaffkabdaaaaacaaaaabiaaaaaaaia
aeaaaaaeaaaaabiaaaaaaaiaapaakkkaapaappkaafaaaaadaaaaabiaaaaaaaia
aaaaaaiaaeaaaaaeaaaaaciaaaaaaaiaanaaaakaanaaffkaaeaaaaaeaaaaacia
aaaaaaiaaaaaffiaanaakkkaaeaaaaaeaaaaaciaaaaaaaiaaaaaffiaanaappka
aeaaaaaeaaaaaciaaaaaaaiaaaaaffiaaoaaaakaaeaaaaaeaaaaabiaaaaaaaia
aaaaffiaaoaaffkaaeaaaaaeaaaaaioaaaaaaaiaafaakkkaaaaappiaabaaaaac
aaaaabiaaeaaaakaafaaaaadaaaaapiaaaaaaaiaahaaoekaafaaaaadabaaapoa
aaaaoeiaafaaoejaafaaaaadaaaaapiaaaaaffjaakaaoekaaeaaaaaeaaaaapia
ajaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaalaaoekaaaaakkjaaaaaoeia
aeaaaaaeaaaaapiaamaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappia
aaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaafdeieefcieadaaaa
eaaaabaaobaaaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaa
abaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaadpcbabaaaafaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaaabaaaaaagfaaaaadpccabaaa
acaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadiaaaaajbcaabaaaaaaaaaaadkiacaaaaaaaaaaaaeaaaaaaakiacaaa
abaaaaaaaaaaaaaaenaaaaagbcaabaaaaaaaaaaaaanaaaaaakaabaaaaaaaaaaa
dcaaaaalgcaabaaaaaaaaaaaagbbbaaaadaaaaaaagibcaaaaaaaaaaaabaaaaaa
kgilcaaaaaaaaaaaabaaaaaadiaaaaajpcaabaaaabaaaaaaegiocaaaaaaaaaaa
adaaaaaaegiecaaaabaaaaaaaaaaaaaabkaaaaafpcaabaaaabaaaaaaegaobaaa
abaaaaaaaaaaaaahgcaabaaaaaaaaaaafgagbaaaaaaaaaaaagabbaaaabaaaaaa
dcaaaaakbccabaaaabaaaaaaakaabaaaaaaaaaaabkiacaaaaaaaaaaaaeaaaaaa
bkaabaaaaaaaaaaadcaaaaaldcaabaaaaaaaaaaaegbabaaaadaaaaaaegiacaaa
aaaaaaaaacaaaaaaogikcaaaaaaaaaaaacaaaaaaaaaaaaahdcaabaaaaaaaaaaa
ogakbaaaabaaaaaaegaabaaaaaaaaaaadiaaaaajicaabaaaaaaaaaaaakiacaaa
aaaaaaaaagaaaaaaakiacaaaabaaaaaaaaaaaaaaenaaaaagicaabaaaaaaaaaaa
aanaaaaadkaabaaaaaaaaaaadcaaaaakiccabaaaabaaaaaadkaabaaaaaaaaaaa
ckiacaaaaaaaaaaaafaaaaaabkaabaaaaaaaaaaadiaaaaajkcaabaaaaaaaaaaa
agimcaaaaaaaaaaaafaaaaaaagiacaaaabaaaaaaaaaaaaaaenaaaaagkcaabaaa
aaaaaaaaaanaaaaafganbaaaaaaaaaaadcaaaaakcccabaaaabaaaaaabkaabaaa
aaaaaaaackiacaaaaaaaaaaaaeaaaaaackaabaaaaaaaaaaadcaaaaakeccabaaa
abaaaaaadkaabaaaaaaaaaaabkiacaaaaaaaaaaaafaaaaaaakaabaaaaaaaaaaa
diaaaaajpcaabaaaaaaaaaaaagiacaaaaaaaaaaaaeaaaaaaegiocaaaaaaaaaaa
ahaaaaaadiaaaaahpccabaaaacaaaaaaegaobaaaaaaaaaaaegbobaaaafaaaaaa
doaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
apaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaaaaaalaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaaabaaaaaaaaaaaaaa
adaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaaadaaaaaaafaaaaaa
apapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfcenebemaafeeffied
epepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"!!ARBfp1.0
# 4 ALU, 2 TEX
TEMP R0;
TEMP R1;
TEX R1, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MUL R0, R0, R1;
MUL result.color, R0, fragment.texcoord[1];
END
# 4 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
dcl t0
dcl t1
texld r1, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
texld r0, r0, s1
mul_pp r0, r1, r0
mul_pp r0, r0, t1
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0
eefiecedimkcpkanodjjnnmplnomdlpdacpimgbaabaaaaaamiabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcpaaaaaaaeaaaaaaadmaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaadpcbabaaa
abaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
acaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaabaaaaaaeghobaaa
abaaaaaaaagabaaaabaaaaaadiaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaa
egaobaaaabaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaaegbobaaa
acaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_OFF" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0_level_9_3
eefiecedflimgfofdpoiajccjmljleofekncbbdmabaaaaaajaacaaaaaeaaaaaa
daaaaaaapeaaaaaaomabaaaafmacaaaaebgpgodjlmaaaaaalmaaaaaaaaacpppp
jaaaaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppbpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaaiaabaacpla
bpaaaaacaaaaaajaaaaiapkabpaaaaacaaaaaajaabaiapkaabaaaaacaaaaadia
aaaaoolaecaaaaadabaacpiaaaaaoelaaaaioekaecaaaaadaaaacpiaaaaaoeia
abaioekaafaaaaadaaaacpiaaaaaoeiaabaaoeiaafaaaaadaaaacpiaaaaaoeia
abaaoelaabaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcpaaaaaaaeaaaaaaa
dmaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaad
pcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaabaaaaaa
eghobaaaabaaaaaaaagabaaaabaaaaaadiaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaa
egbobaaaacaaaaaadoaaaaabejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklkl"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"!!ARBfp1.0
# 4 ALU, 2 TEX
TEMP R0;
TEMP R1;
TEX R1, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MUL R0, R0, R1;
MUL result.color, R0, fragment.texcoord[1];
END
# 4 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
dcl t0
dcl t1
texld r1, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
texld r0, r0, s1
mul_pp r0, r1, r0
mul_pp r0, r0, t1
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0
eefiecedimkcpkanodjjnnmplnomdlpdacpimgbaabaaaaaamiabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcpaaaaaaaeaaaaaaadmaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaadpcbabaaa
abaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
acaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaabaaaaaaeghobaaa
abaaaaaaaagabaaaabaaaaaadiaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaa
egaobaaaabaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaaegbobaaa
acaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0_level_9_3
eefiecedflimgfofdpoiajccjmljleofekncbbdmabaaaaaajaacaaaaaeaaaaaa
daaaaaaapeaaaaaaomabaaaafmacaaaaebgpgodjlmaaaaaalmaaaaaaaaacpppp
jaaaaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppbpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaaiaabaacpla
bpaaaaacaaaaaajaaaaiapkabpaaaaacaaaaaajaabaiapkaabaaaaacaaaaadia
aaaaoolaecaaaaadabaacpiaaaaaoelaaaaioekaecaaaaadaaaacpiaaaaaoeia
abaioekaafaaaaadaaaacpiaaaaaoeiaabaaoeiaafaaaaadaaaacpiaaaaaoeia
abaaoelaabaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcpaaaaaaaeaaaaaaa
dmaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaad
pcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaabaaaaaa
eghobaaaabaaaaaaaagabaaaabaaaaaadiaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaa
egbobaaaacaaaaaadoaaaaabejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklkl"
}
}
 }
}
}