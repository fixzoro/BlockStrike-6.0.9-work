Shader "MADFINGER/Environment/Lightmap + Wind" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _Wind ("Wind params", Vector) = (1,1,1,1)
 _WindEdgeFlutter ("Wind edge fultter factor", Float) = 0.5
 _WindEdgeFlutterFreqScale ("Wind edge fultter freq scale", Float) = 0.5
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [_Wind]
Vector 15 [_MainTex_ST]
Vector 16 [unity_LightmapST]
Float 17 [_WindEdgeFlutter]
Float 18 [_WindEdgeFlutterFreqScale]
"!!ARBvp1.0
# 38 ALU
PARAM c[21] = { { -0.5, 1, 2, 3 },
		state.matrix.mvp,
		program.local[5..18],
		{ 1.975, 0.79299998, 0.375, 0.193 },
		{ 0.30000001, 0.1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MOV R0.x, c[0].y;
DP3 R0.x, R0.x, c[8];
ADD R0.w, R0.x, c[17].x;
MOV R0.y, R0.x;
MOV R0.z, c[18].x;
DP3 R0.x, vertex.position, R0.w;
MAD R0.xy, R0.z, c[13].y, R0;
MUL R0, R0.xxyy, c[19];
FRC R0, R0;
MAD R0, R0, c[0].z, c[0].x;
FRC R0, R0;
MAD R0, R0, c[0].z, -c[0].y;
ABS R0, R0;
MAD R1, -R0, c[0].z, c[0].w;
MUL R0, R0, R0;
MUL R1, R0, R1;
MOV R0.xyz, c[14];
ADD R3.xy, R1.xzzw, R1.ywzw;
DP3 R1.z, R0, c[11];
DP3 R1.y, R0, c[10];
DP3 R1.x, R0, c[9];
MUL R0.xy, vertex.normal.xzzw, c[17].x;
MUL R0.xz, R0.xyyw, c[20].y;
MUL R2.xyz, R1, R3.y;
MUL R0.w, vertex.color, c[14];
MUL R2.xyz, vertex.color.w, R2;
MUL R0.y, vertex.color.w, c[20].x;
MAD R0.xyz, R3.xyxw, R0, R2;
MAD R0.xyz, R0, R0.w, vertex.position;
MOV R0.w, vertex.position;
MAD R0.xyz, vertex.color.w, R1, R0;
DP4 result.position.w, R0, c[4];
DP4 result.position.z, R0, c[3];
DP4 result.position.y, R0, c[2];
DP4 result.position.x, R0, c[1];
MOV result.texcoord[2].xyz, vertex.color;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[15], c[15].zwzw;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[16], c[16].zwzw;
END
# 38 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [_Wind]
Vector 14 [_MainTex_ST]
Vector 15 [unity_LightmapST]
Float 16 [_WindEdgeFlutter]
Float 17 [_WindEdgeFlutterFreqScale]
"vs_2_0
; 43 ALU
def c18, 1.00000000, 2.00000000, -0.50000000, -1.00000000
def c19, 1.97500002, 0.79299998, 0.37500000, 0.19300000
def c20, 2.00000000, 3.00000000, 0.30000001, 0.10000000
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
dcl_color0 v4
mov r0.xyz, c7
dp3 r0.y, c18.x, r0
add r0.x, r0.y, c16
mov r0.z, c12.y
dp3 r0.x, v0, r0.x
mad r0.xy, c17.x, r0.z, r0
mul r0, r0.xxyy, c19
frc r0, r0
mad r0, r0, c18.y, c18.z
frc r0, r0
mad r0, r0, c18.y, c18.w
abs r0, r0
mad r1, -r0, c20.x, c20.y
mul r0, r0, r0
mul r1, r0, r1
add r3.xy, r1.xzzw, r1.ywzw
mov r0.xyz, c10
dp3 r1.z, c13, r0
mov r0.xyz, c8
dp3 r1.x, c13, r0
mov r2.xyz, c9
dp3 r1.y, c13, r2
mul r0.xy, v1.xzzw, c16.x
mul r0.xz, r0.xyyw, c20.w
mul r2.xyz, r1, r3.y
mul r0.w, v4, c13
mul r2.xyz, v4.w, r2
mul r0.y, v4.w, c20.z
mad r0.xyz, r3.xyxw, r0, r2
mad r0.xyz, r0, r0.w, v0
mov r0.w, v0
mad r0.xyz, v4.w, r1, r0
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
mov oT2.xyz, v4
mad oT0.xy, v2, c14, c14.zwzw
mad oT1.xy, v3, c15, c15.zwzw
"
}
SubProgram "d3d11 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [_Wind]
Vector 32 [_MainTex_ST]
Vector 48 [unity_LightmapST]
Float 64 [_WindEdgeFlutter]
Float 68 [_WindEdgeFlutterFreqScale]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 192 [_Object2World]
Matrix 256 [_World2Object]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedbmmomjogafldcbdbhlpnhenmnccegbbpabaaaaaabaahaaaaadaaaaaa
cmaaaaaapeaaaaaahmabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahafaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaa
aiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaaaaaaaaaa
adaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaiaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefc
imafaaaaeaaaabaagdabaaaafjaaaaaeegiocaaaaaaaaaaaafaaaaaafjaaaaae
egiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaabdaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaadfcbabaaaacaaaaaafpaaaaaddcbabaaaadaaaaaa
fpaaaaaddcbabaaaaeaaaaaafpaaaaadpcbabaaaafaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
gfaaaaadhccabaaaacaaaaaagiaaaaacaeaaaaaadgaaaaagbcaabaaaaaaaaaaa
dkiacaaaacaaaaaaamaaaaaadgaaaaagccaabaaaaaaaaaaadkiacaaaacaaaaaa
anaaaaaadgaaaaagecaabaaaaaaaaaaadkiacaaaacaaaaaaaoaaaaaabaaaaaak
ccaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaaaaaaaaaaaaiecaabaaaaaaaaaaabkaabaaaaaaaaaaaakiacaaaaaaaaaaa
aeaaaaaabaaaaaahbcaabaaaaaaaaaaaegbcbaaaaaaaaaaakgakbaaaaaaaaaaa
dcaaaaalpcaabaaaaaaaaaaafgifcaaaabaaaaaaaaaaaaaafgifcaaaaaaaaaaa
aeaaaaaaagafbaaaaaaaaaaadiaaaaakpcaabaaaaaaaaaaaegaobaaaaaaaaaaa
aceaaaaamnmmpmdpamaceldpaaaamadomlkbefdobkaaaaafpcaabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaappcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaa
aaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaaalpaaaaaalpaaaaaalp
aaaaaalpbkaaaaafpcaabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaappcaabaaa
aaaaaaaaegaobaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaea
aceaaaaaaaaaialpaaaaialpaaaaialpaaaaialpdiaaaaajpcaabaaaabaaaaaa
egaobaiaibaaaaaaaaaaaaaaegaobaiaibaaaaaaaaaaaaaadcaaaabapcaabaaa
aaaaaaaaegaobaiambaaaaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaea
aaaaaaeaaceaaaaaaaaaeaeaaaaaeaeaaaaaeaeaaaaaeaeadiaaaaahpcaabaaa
aaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaaaaaaaaahhcaabaaaaaaaaaaa
ngafbaaaaaaaaaaaigaabaaaaaaaaaaadiaaaaajhcaabaaaabaaaaaafgifcaaa
aaaaaaaaabaaaaaaegiccaaaacaaaaaabbaaaaaadcaaaaalhcaabaaaabaaaaaa
egiccaaaacaaaaaabaaaaaaaagiacaaaaaaaaaaaabaaaaaaegacbaaaabaaaaaa
dcaaaaalhcaabaaaabaaaaaaegiccaaaacaaaaaabcaaaaaakgikcaaaaaaaaaaa
abaaaaaaegacbaaaabaaaaaadiaaaaahhcaabaaaacaaaaaafgafbaaaaaaaaaaa
egacbaaaabaaaaaadiaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaapgbpbaaa
afaaaaaadiaaaaaiicaabaaaaaaaaaaaakiacaaaaaaaaaaaaeaaaaaaabeaaaaa
mnmmmmdndiaaaaahfcaabaaaadaaaaaapgapbaaaaaaaaaaaagbcbaaaacaaaaaa
diaaaaahccaabaaaadaaaaaadkbabaaaafaaaaaaabeaaaaajkjjjjdodcaaaaaj
hcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaadaaaaaaegacbaaaacaaaaaa
diaaaaaiicaabaaaaaaaaaaadkbabaaaafaaaaaadkiacaaaaaaaaaaaabaaaaaa
dcaaaaajhcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaaegbcbaaa
aaaaaaaadcaaaaajhcaabaaaaaaaaaaapgbpbaaaafaaaaaaegacbaaaabaaaaaa
egacbaaaaaaaaaaadiaaaaaipcaabaaaabaaaaaafgafbaaaaaaaaaaaegiocaaa
acaaaaaaabaaaaaadcaaaaakpcaabaaaabaaaaaaegiocaaaacaaaaaaaaaaaaaa
agaabaaaaaaaaaaaegaobaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
acaaaaaaacaaaaaakgakbaaaaaaaaaaaegaobaaaabaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaaldccabaaaabaaaaaaegbabaaaadaaaaaaegiacaaaaaaaaaaaacaaaaaa
ogikcaaaaaaaaaaaacaaaaaadcaaaaalmccabaaaabaaaaaaagbebaaaaeaaaaaa
agiecaaaaaaaaaaaadaaaaaakgiocaaaaaaaaaaaadaaaaaadgaaaaafhccabaaa
acaaaaaaegbcbaaaafaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [_Wind]
Vector 32 [_MainTex_ST]
Vector 48 [unity_LightmapST]
Float 64 [_WindEdgeFlutter]
Float 68 [_WindEdgeFlutterFreqScale]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 192 [_Object2World]
Matrix 256 [_World2Object]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedhcellmbdocnobijggnllomgbbmonmddnabaaaaaakaakaaaaaeaaaaaa
daaaaaaalmadaaaafaajaaaabiakaaaaebgpgodjieadaaaaieadaaaaaaacpopp
caadaaaageaaaaaaafaaceaaaaaagaaaaaaagaaaaaaaceaaabaagaaaaaaaabaa
aeaaabaaaaaaaaaaabaaaaaaabaaafaaaaaaaaaaacaaaaaaaeaaagaaaaaaaaaa
acaaamaaadaaakaaaaaaaaaaacaabaaaadaaanaaaaaaaaaaaaaaaaaaabacpopp
fbaaaaafbaaaapkaaaaaiadpaaaaaaeaaaaaaalpaaaaialpfbaaaaafbbaaapka
mnmmpmdpamaceldpaaaamadomlkbefdofbaaaaafbcaaapkaaaaaaaeaaaaaeaea
mnmmmmdnjkjjjjdobpaaaaacafaaaaiaaaaaapjabpaaaaacafaaaciaacaaapja
bpaaaaacafaaadiaadaaapjabpaaaaacafaaaeiaaeaaapjabpaaaaacafaaafia
afaaapjaaeaaaaaeaaaaadoaadaaoejaacaaoekaacaaookaaeaaaaaeaaaaamoa
aeaabejaadaabekaadaalekaabaaaaacaaaaabiaakaappkaabaaaaacaaaaacia
alaappkaabaaaaacaaaaaeiaamaappkaaiaaaaadaaaaaciaaaaaoeiabaaaaaka
acaaaaadaaaaaeiaaaaaffiaaeaaaakaaiaaaaadaaaaabiaaaaaoejaaaaakkia
abaaaaacabaaadiaaeaaoekaaeaaaaaeaaaaapiaafaaffkaabaaffiaaaaafaia
afaaaaadaaaaapiaaaaaoeiabbaaoekabdaaaaacaaaaapiaaaaaoeiaaeaaaaae
aaaaapiaaaaaoeiabaaaffkabaaakkkabdaaaaacaaaaapiaaaaaoeiaaeaaaaae
aaaaapiaaaaaoeiabaaaffkabaaappkacdaaaaacaaaaapiaaaaaoeiaafaaaaad
acaaapiaaaaaoeiaaaaaoeiaaeaaaaaeaaaaapiaaaaaoeiabcaaaakbbcaaffka
afaaaaadaaaaapiaaaaaoeiaacaaoeiaacaaaaadaaaaahiaaaaanniaaaaamiia
abaaaaacacaaahiaabaaoekaafaaaaadabaaaoiaacaaffiaaoaajakaaeaaaaae
abaaaoiaanaajakaacaaaaiaabaaoeiaaeaaaaaeabaaaoiaapaajakaacaakkia
abaaoeiaafaaaaadacaaahiaaaaaffiaabaapjiaafaaaaadacaaahiaacaaoeia
afaappjaafaaaaadaaaaaiiaabaaaaiabcaakkkaafaaaaadadaaafiaaaaappia
acaaoejaafaaaaadadaaaciaafaappjabcaappkaaeaaaaaeaaaaahiaaaaaoeia
adaaoeiaacaaoeiaafaaaaadaaaaaiiaafaappjaabaappkaaeaaaaaeaaaaahia
aaaaoeiaaaaappiaaaaaoejaaeaaaaaeaaaaahiaafaappjaabaapjiaaaaaoeia
afaaaaadabaaapiaaaaaffiaahaaoekaaeaaaaaeabaaapiaagaaoekaaaaaaaia
abaaoeiaaeaaaaaeaaaaapiaaiaaoekaaaaakkiaabaaoeiaaeaaaaaeaaaaapia
ajaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeia
abaaaaacaaaaammaaaaaoeiaabaaaaacabaaahoaafaaoejappppaaaafdeieefc
imafaaaaeaaaabaagdabaaaafjaaaaaeegiocaaaaaaaaaaaafaaaaaafjaaaaae
egiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaabdaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaadfcbabaaaacaaaaaafpaaaaaddcbabaaaadaaaaaa
fpaaaaaddcbabaaaaeaaaaaafpaaaaadpcbabaaaafaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
gfaaaaadhccabaaaacaaaaaagiaaaaacaeaaaaaadgaaaaagbcaabaaaaaaaaaaa
dkiacaaaacaaaaaaamaaaaaadgaaaaagccaabaaaaaaaaaaadkiacaaaacaaaaaa
anaaaaaadgaaaaagecaabaaaaaaaaaaadkiacaaaacaaaaaaaoaaaaaabaaaaaak
ccaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaaaaaaaaaaaaiecaabaaaaaaaaaaabkaabaaaaaaaaaaaakiacaaaaaaaaaaa
aeaaaaaabaaaaaahbcaabaaaaaaaaaaaegbcbaaaaaaaaaaakgakbaaaaaaaaaaa
dcaaaaalpcaabaaaaaaaaaaafgifcaaaabaaaaaaaaaaaaaafgifcaaaaaaaaaaa
aeaaaaaaagafbaaaaaaaaaaadiaaaaakpcaabaaaaaaaaaaaegaobaaaaaaaaaaa
aceaaaaamnmmpmdpamaceldpaaaamadomlkbefdobkaaaaafpcaabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaappcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaa
aaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaaalpaaaaaalpaaaaaalp
aaaaaalpbkaaaaafpcaabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaappcaabaaa
aaaaaaaaegaobaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaea
aceaaaaaaaaaialpaaaaialpaaaaialpaaaaialpdiaaaaajpcaabaaaabaaaaaa
egaobaiaibaaaaaaaaaaaaaaegaobaiaibaaaaaaaaaaaaaadcaaaabapcaabaaa
aaaaaaaaegaobaiambaaaaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaea
aaaaaaeaaceaaaaaaaaaeaeaaaaaeaeaaaaaeaeaaaaaeaeadiaaaaahpcaabaaa
aaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaaaaaaaaahhcaabaaaaaaaaaaa
ngafbaaaaaaaaaaaigaabaaaaaaaaaaadiaaaaajhcaabaaaabaaaaaafgifcaaa
aaaaaaaaabaaaaaaegiccaaaacaaaaaabbaaaaaadcaaaaalhcaabaaaabaaaaaa
egiccaaaacaaaaaabaaaaaaaagiacaaaaaaaaaaaabaaaaaaegacbaaaabaaaaaa
dcaaaaalhcaabaaaabaaaaaaegiccaaaacaaaaaabcaaaaaakgikcaaaaaaaaaaa
abaaaaaaegacbaaaabaaaaaadiaaaaahhcaabaaaacaaaaaafgafbaaaaaaaaaaa
egacbaaaabaaaaaadiaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaapgbpbaaa
afaaaaaadiaaaaaiicaabaaaaaaaaaaaakiacaaaaaaaaaaaaeaaaaaaabeaaaaa
mnmmmmdndiaaaaahfcaabaaaadaaaaaapgapbaaaaaaaaaaaagbcbaaaacaaaaaa
diaaaaahccaabaaaadaaaaaadkbabaaaafaaaaaaabeaaaaajkjjjjdodcaaaaaj
hcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaadaaaaaaegacbaaaacaaaaaa
diaaaaaiicaabaaaaaaaaaaadkbabaaaafaaaaaadkiacaaaaaaaaaaaabaaaaaa
dcaaaaajhcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaaegbcbaaa
aaaaaaaadcaaaaajhcaabaaaaaaaaaaapgbpbaaaafaaaaaaegacbaaaabaaaaaa
egacbaaaaaaaaaaadiaaaaaipcaabaaaabaaaaaafgafbaaaaaaaaaaaegiocaaa
acaaaaaaabaaaaaadcaaaaakpcaabaaaabaaaaaaegiocaaaacaaaaaaaaaaaaaa
agaabaaaaaaaaaaaegaobaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
acaaaaaaacaaaaaakgakbaaaaaaaaaaaegaobaaaabaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaaldccabaaaabaaaaaaegbabaaaadaaaaaaegiacaaaaaaaaaaaacaaaaaa
ogikcaaaaaaaaaaaacaaaaaadcaaaaalmccabaaaabaaaaaaagbebaaaaeaaaaaa
agiecaaaaaaaaaaaadaaaaaakgiocaaaaaaaaaaaadaaaaaadgaaaaafhccabaaa
acaaaaaaegbcbaaaafaaaaaadoaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaa
jiaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaa
aaaaaaaaadaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
acaaaaaaahafaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaa
laaaaaaaabaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaa
aaaaaaaaadaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofe
aaeoepfcenebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaa
aeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
heaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaa
aaaaaaaaadaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaa
acaaaaaaahaiaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 6 ALU, 2 TEX
PARAM c[1] = { { 8 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[1], 2D;
MUL R1.xyz, R1.w, R1;
MUL R0.xyz, R1, R0;
MUL result.color.xyz, R0, c[0].x;
MOV result.color.w, R0;
END
# 6 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c0, 8.00000000, 0, 0, 0
dcl t0.xy
dcl t1.xy
texld r0, t1, s1
texld r1, t0, s0
mul_pp r0.xyz, r0.w, r0
mul_pp r0.xyz, r0, r1
mov_pp r0.w, r1
mul_pp r0.xyz, r0, c0.x
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"ps_4_0
eefiecedcahbilgcedmmndcancfeabnoimhcophpabaaaaaabaacaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaaaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefccaabaaaaeaaaaaaaeiaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
fibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaad
mcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaaj
pcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaa
diaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaebdiaaaaah
hcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaah
hccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaadgaaaaaficcabaaa
aaaaaaaadkaabaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"ps_4_0_level_9_3
eefiecedangepenebfniabgdeknfgdgpmmmckpojabaaaaaapeacaaaaaeaaaaaa
daaaaaaabaabaaaadiacaaaamaacaaaaebgpgodjniaaaaaaniaaaaaaaaacpppp
kmaaaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppfbaaaaafaaaaapkaaaaaaaebaaaaaaaaaaaaaaaaaaaaaaaa
bpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaajaaaaiapkabpaaaaacaaaaaaja
abaiapkaabaaaaacaaaaadiaaaaaollaecaaaaadabaacpiaaaaaoelaaaaioeka
ecaaaaadaaaacpiaaaaaoeiaabaioekaafaaaaadaaaaciiaaaaappiaaaaaaaka
afaaaaadaaaachiaaaaaoeiaaaaappiaafaaaaadabaachiaaaaaoeiaabaaoeia
abaaaaacaaaicpiaabaaoeiappppaaaafdeieefccaabaaaaeaaaaaaaeiaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagcbaaaadmcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
acaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaaabaaaaaa
aagabaaaabaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaa
aaaaaaebdiaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaahhccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaa
dgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaadoaaaaabejfdeheoiaaaaaaa
aeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
heaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaa
aaaaaaaaadaaaaaaabaaaaaaamamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaa
acaaaaaaahaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
epfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
}
 }
}
}