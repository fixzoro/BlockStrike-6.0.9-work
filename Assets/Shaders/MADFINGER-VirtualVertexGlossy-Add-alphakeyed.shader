Shader "MADFINGER/Environment/Virtual Gloss Per-Vertex Additive AlphaKeyed (Supports Lightmap)" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _SpecOffset ("Specular Offset from Camera", Vector) = (1,10,2,0)
 _SpecRange ("Specular Range", Float) = 20
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _Shininess ("Shininess", Range(0.01,1)) = 0.078125
 _SpecStrength ("Specular Strength", Float) = 2
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 9 [_MainTex_ST]
Vector 10 [unity_LightmapST]
Vector 11 [_SpecOffset]
Float 12 [_SpecRange]
Vector 13 [_SpecColor]
Float 14 [_Shininess]
Float 15 [_SpecStrength]
"!!ARBvp1.0
# 36 ALU
PARAM c[17] = { { 1, -1, 0, 0.5 },
		state.matrix.modelview[0],
		state.matrix.mvp,
		program.local[9..15],
		{ 128 } };
TEMP R0;
TEMP R1;
MOV R1.xy, c[0];
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MAD R0.xyz, R1.xxyw, -c[11], R0;
DP3 R0.w, -R0, -R0;
RSQ R0.w, R0.w;
MAD R0.xyz, R0.w, -R0, c[0].zzxw;
MUL R0.xyz, R0, c[0].w;
DP3 R1.x, R0, R0;
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, R0;
DP3 R0.z, vertex.normal, c[3];
DP3 R0.y, vertex.normal, c[2];
DP3 R0.x, vertex.normal, c[1];
DP3 R0.x, R0, R1;
MIN R0.x, R0, c[0];
MAX R0.y, R0.x, c[0].z;
RCP R0.z, R0.w;
RCP R0.w, c[12].x;
MOV R0.x, c[16];
MUL R0.z, R0, R0.w;
MUL R0.x, R0, c[14];
POW R0.x, R0.y, R0.x;
MIN R0.z, R0, c[0].x;
MAX R0.y, R0.z, c[0].z;
MUL R0.x, R0, c[13];
ADD R0.y, -R0, c[0].x;
MUL R0.x, R0, c[15];
MUL result.texcoord[2], R0.x, R0.y;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[9], c[9].zwzw;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[10], c[10].zwzw;
DP4 result.position.w, vertex.position, c[8];
DP4 result.position.z, vertex.position, c[7];
DP4 result.position.y, vertex.position, c[6];
DP4 result.position.x, vertex.position, c[5];
END
# 36 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_modelview0]
Matrix 4 [glstate_matrix_mvp]
Vector 8 [_MainTex_ST]
Vector 9 [unity_LightmapST]
Vector 10 [_SpecOffset]
Float 11 [_SpecRange]
Vector 12 [_SpecColor]
Float 13 [_Shininess]
Float 14 [_SpecStrength]
"vs_2_0
; 39 ALU
def c15, 1.00000000, -1.00000000, 0.00000000, 0.50000000
def c16, 128.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mov r1.xyz, c10
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mad r0.xyz, c15.xxyw, -r1, r0
dp3 r0.w, -r0, -r0
rsq r0.w, r0.w
mad r0.xyz, r0.w, -r0, c15.zzxw
mul r0.xyz, r0, c15.w
dp3 r1.x, r0, r0
rsq r1.x, r1.x
mul r1.xyz, r1.x, r0
dp3 r0.y, v1, c1
dp3 r0.z, v1, c2
dp3 r0.x, v1, c0
dp3 r0.x, r0, r1
mov r0.y, c13.x
min r0.x, r0, c15
mul r0.y, c16.x, r0
max r0.x, r0, c15.z
pow r1, r0.x, r0.y
rcp r0.y, c11.x
rcp r0.x, r0.w
mul r0.x, r0, r0.y
min r0.y, r0.x, c15.x
mov r0.x, r1
max r0.y, r0, c15.z
mul r0.x, r0, c12
add r0.y, -r0, c15.x
mul r0.x, r0, c14
mul oT2, r0.x, r0.y
mad oT0.xy, v2, c8, c8.zwzw
mad oT1.xy, v3, c9, c9.zwzw
dp4 oPos.w, v0, c7
dp4 oPos.z, v0, c6
dp4 oPos.y, v0, c5
dp4 oPos.x, v0, c4
"
}
SubProgram "d3d11 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 96
Vector 16 [_MainTex_ST]
Vector 32 [unity_LightmapST]
Vector 48 [_SpecOffset] 3
Float 60 [_SpecRange]
Vector 64 [_SpecColor] 3
Float 76 [_Shininess]
Float 80 [_SpecStrength]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [glstate_matrix_modelview0]
BindCB  "$Globals" 0
BindCB  "UnityPerDraw" 1
"vs_4_0
eefiecedilgheofogjfcoifpfcojebckpkigkcmkabaaaaaahiagaaaaadaaaaaa
cmaaaaaapeaaaaaahmabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaa
aiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaaaaaaaaaa
adaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefc
peaeaaaaeaaaabaadnabaaaafjaaaaaeegiocaaaaaaaaaaaagaaaaaafjaaaaae
egiocaaaabaaaaaaaiaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaadhcbabaaa
acaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaaddcbabaaaaeaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldccabaaaabaaaaaaegbabaaa
adaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaadcaaaaal
mccabaaaabaaaaaaagbebaaaaeaaaaaaagiecaaaaaaaaaaaacaaaaaakgiocaaa
aaaaaaaaacaaaaaadiaaaaaihcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiccaaa
abaaaaaaafaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaaabaaaaaaaeaaaaaa
agbabaaaaaaaaaaaegacbaaaaaaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaa
abaaaaaaagaaaaaakgbkbaaaaaaaaaaaegacbaaaaaaaaaaadcaaaaakhcaabaaa
aaaaaaaaegiccaaaabaaaaaaahaaaaaapgbpbaaaaaaaaaaaegacbaaaaaaaaaaa
dcaaaaaohcaabaaaaaaaaaaaegiccaiaebaaaaaaaaaaaaaaadaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaialpaaaaaaaaegacbaaaaaaaaaaabaaaaaajicaabaaa
aaaaaaaaegacbaiaebaaaaaaaaaaaaaaegacbaiaebaaaaaaaaaaaaaaeeaaaaaf
icaabaaaaaaaaaaadkaabaaaaaaaaaaadcaaaaanhcaabaaaabaaaaaaegacbaia
ebaaaaaaaaaaaaaapgapbaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadp
aaaaaaaabaaaaaahbcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaaaaaaaaa
elaaaaafbcaabaaaaaaaaaaaakaabaaaaaaaaaaaaocaaaaibcaabaaaaaaaaaaa
akaabaaaaaaaaaaadkiacaaaaaaaaaaaadaaaaaaaaaaaaaibcaabaaaaaaaaaaa
akaabaiaebaaaaaaaaaaaaaaabeaaaaaaaaaiadpdiaaaaakocaabaaaaaaaaaaa
agajbaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaadpaaaaaadpaaaaaadpbaaaaaah
bcaabaaaabaaaaaajgahbaaaaaaaaaaajgahbaaaaaaaaaaaeeaaaaafbcaabaaa
abaaaaaaakaabaaaabaaaaaadiaaaaahocaabaaaaaaaaaaafgaobaaaaaaaaaaa
agaabaaaabaaaaaadiaaaaaihcaabaaaabaaaaaafgbfbaaaacaaaaaaegiccaaa
abaaaaaaafaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaaabaaaaaaaeaaaaaa
agbabaaaacaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaa
abaaaaaaagaaaaaakgbkbaaaacaaaaaaegacbaaaabaaaaaabacaaaahccaabaaa
aaaaaaaaegacbaaaabaaaaaajgahbaaaaaaaaaaacpaaaaafccaabaaaaaaaaaaa
bkaabaaaaaaaaaaadiaaaaaiecaabaaaaaaaaaaadkiacaaaaaaaaaaaaeaaaaaa
abeaaaaaaaaaaaeddiaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaackaabaaa
aaaaaaaabjaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaadiaaaaaipcaabaaa
abaaaaaafgafbaaaaaaaaaaaegikcaaaaaaaaaaaaeaaaaaadiaaaaaipcaabaaa
abaaaaaaegaobaaaabaaaaaaagiacaaaaaaaaaaaafaaaaaadiaaaaahpccabaaa
acaaaaaaagaabaaaaaaaaaaaegaobaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 96
Vector 16 [_MainTex_ST]
Vector 32 [unity_LightmapST]
Vector 48 [_SpecOffset] 3
Float 60 [_SpecRange]
Vector 64 [_SpecColor] 3
Float 76 [_Shininess]
Float 80 [_SpecStrength]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [glstate_matrix_modelview0]
BindCB  "$Globals" 0
BindCB  "UnityPerDraw" 1
"vs_4_0_level_9_3
eefieceddhipidfgiakbjibnllkeeaedpepaochpabaaaaaajmajaaaaaeaaaaaa
daaaaaaafaadaaaaemaiaaaabeajaaaaebgpgodjbiadaaaabiadaaaaaaacpopp
niacaaaaeaaaaaaaacaaceaaaaaadmaaaaaadmaaaaaaceaaabaadmaaaaaaabaa
afaaabaaaaaaaaaaabaaaaaaaiaaagaaaaaaaaaaaaaaaaaaabacpoppfbaaaaaf
aoaaapkaaaaaiadpaaaaialpaaaaaaaaaaaaaadpfbaaaaafapaaapkaaaaaaaed
aaaaaaaaaaaaaaaaaaaaaaaabpaaaaacafaaaaiaaaaaapjabpaaaaacafaaacia
acaaapjabpaaaaacafaaadiaadaaapjabpaaaaacafaaaeiaaeaaapjaaeaaaaae
aaaaadoaadaaoejaabaaoekaabaaookaafaaaaadaaaaahiaaaaaffjaalaaoeka
aeaaaaaeaaaaahiaakaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaahiaamaaoeka
aaaakkjaaaaaoeiaaeaaaaaeaaaaahiaanaaoekaaaaappjaaaaaoeiaabaaaaac
abaaadiaaoaaoekaaeaaaaaeaaaaahiaadaaoekaabaanaibaaaaoeiaceaaaaac
abaaahiaaaaaoeibaiaaaaadaaaaabiaaaaaoeiaaaaaoeiaahaaaaacaaaaabia
aaaaaaiaagaaaaacaaaaabiaaaaaaaiaacaaaaadaaaaaoiaabaajaiaaoaacika
afaaaaadaaaaaoiaaaaaoeiaaoaappkaceaaaaacabaaahiaaaaapjiaafaaaaad
aaaaaoiaacaaffjaalaajakaaeaaaaaeaaaaaoiaakaajakaacaaaajaaaaaoeia
aeaaaaaeaaaaaoiaamaajakaacaakkjaaaaaoeiaaiaaaaadaaaaaciaaaaapjia
abaaoeiaalaaaaadaaaaaciaaaaaffiaaoaakkkaakaaaaadaaaaaciaaaaaffia
aoaaaakaabaaaaacaaaaaiiaaeaappkaafaaaaadaaaaaeiaaaaappiaapaaaaka
caaaaaadabaaabiaaaaaffiaaaaakkiaafaaaaadabaaapiaabaaaaiaaeaakeka
afaaaaadabaaapiaabaaoeiaafaaaakaagaaaaacaaaaaciaadaappkaafaaaaad
aaaaabiaaaaaffiaaaaaaaiaalaaaaadaaaaabiaaaaaaaiaaoaakkkaakaaaaad
aaaaabiaaaaaaaiaaoaaaakaacaaaaadaaaaabiaaaaaaaibaoaaaakaafaaaaad
abaaapoaaaaaaaiaabaaoeiaaeaaaaaeaaaaamoaaeaabejaacaabekaacaaleka
afaaaaadaaaaapiaaaaaffjaahaaoekaaeaaaaaeaaaaapiaagaaoekaaaaaaaja
aaaaoeiaaeaaaaaeaaaaapiaaiaaoekaaaaakkjaaaaaoeiaaeaaaaaeaaaaapia
ajaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeia
abaaaaacaaaaammaaaaaoeiappppaaaafdeieefcpeaeaaaaeaaaabaadnabaaaa
fjaaaaaeegiocaaaaaaaaaaaagaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaadhcbabaaaacaaaaaafpaaaaaddcbabaaa
adaaaaaafpaaaaaddcbabaaaaeaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaagfaaaaadpccabaaa
acaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaaldccabaaaabaaaaaaegbabaaaadaaaaaaegiacaaaaaaaaaaa
abaaaaaaogikcaaaaaaaaaaaabaaaaaadcaaaaalmccabaaaabaaaaaaagbebaaa
aeaaaaaaagiecaaaaaaaaaaaacaaaaaakgiocaaaaaaaaaaaacaaaaaadiaaaaai
hcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiccaaaabaaaaaaafaaaaaadcaaaaak
hcaabaaaaaaaaaaaegiccaaaabaaaaaaaeaaaaaaagbabaaaaaaaaaaaegacbaaa
aaaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaaabaaaaaaagaaaaaakgbkbaaa
aaaaaaaaegacbaaaaaaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaaabaaaaaa
ahaaaaaapgbpbaaaaaaaaaaaegacbaaaaaaaaaaadcaaaaaohcaabaaaaaaaaaaa
egiccaiaebaaaaaaaaaaaaaaadaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaialp
aaaaaaaaegacbaaaaaaaaaaabaaaaaajicaabaaaaaaaaaaaegacbaiaebaaaaaa
aaaaaaaaegacbaiaebaaaaaaaaaaaaaaeeaaaaaficaabaaaaaaaaaaadkaabaaa
aaaaaaaadcaaaaanhcaabaaaabaaaaaaegacbaiaebaaaaaaaaaaaaaapgapbaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadpaaaaaaaabaaaaaahbcaabaaa
aaaaaaaaegacbaaaaaaaaaaaegacbaaaaaaaaaaaelaaaaafbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaaocaaaaibcaabaaaaaaaaaaaakaabaaaaaaaaaaadkiacaaa
aaaaaaaaadaaaaaaaaaaaaaibcaabaaaaaaaaaaaakaabaiaebaaaaaaaaaaaaaa
abeaaaaaaaaaiadpdiaaaaakocaabaaaaaaaaaaaagajbaaaabaaaaaaaceaaaaa
aaaaaaaaaaaaaadpaaaaaadpaaaaaadpbaaaaaahbcaabaaaabaaaaaajgahbaaa
aaaaaaaajgahbaaaaaaaaaaaeeaaaaafbcaabaaaabaaaaaaakaabaaaabaaaaaa
diaaaaahocaabaaaaaaaaaaafgaobaaaaaaaaaaaagaabaaaabaaaaaadiaaaaai
hcaabaaaabaaaaaafgbfbaaaacaaaaaaegiccaaaabaaaaaaafaaaaaadcaaaaak
hcaabaaaabaaaaaaegiccaaaabaaaaaaaeaaaaaaagbabaaaacaaaaaaegacbaaa
abaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaaabaaaaaaagaaaaaakgbkbaaa
acaaaaaaegacbaaaabaaaaaabacaaaahccaabaaaaaaaaaaaegacbaaaabaaaaaa
jgahbaaaaaaaaaaacpaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaadiaaaaai
ecaabaaaaaaaaaaadkiacaaaaaaaaaaaaeaaaaaaabeaaaaaaaaaaaeddiaaaaah
ccaabaaaaaaaaaaabkaabaaaaaaaaaaackaabaaaaaaaaaaabjaaaaafccaabaaa
aaaaaaaabkaabaaaaaaaaaaadiaaaaaipcaabaaaabaaaaaafgafbaaaaaaaaaaa
egikcaaaaaaaaaaaaeaaaaaadiaaaaaipcaabaaaabaaaaaaegaobaaaabaaaaaa
agiacaaaaaaaaaaaafaaaaaadiaaaaahpccabaaaacaaaaaaagaabaaaaaaaaaaa
egaobaaaabaaaaaadoaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaa
aiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaaheaaaaaaabaaaaaaaaaaaaaa
adaaaaaaabaaaaaaamadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 7 ALU, 2 TEX
PARAM c[1] = { { 8 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[1], 2D;
MAD R0.xyz, fragment.texcoord[2], R0.w, R0;
MUL R1.xyz, R1.w, R1;
MUL R0.xyz, R1, R0;
MUL result.color.xyz, R0, c[0].x;
MOV result.color.w, R0;
END
# 7 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"ps_2_0
; 6 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c0, 8.00000000, 0, 0, 0
dcl t0.xy
dcl t1.xy
dcl t2.xyz
texld r1, t0, s0
texld r0, t1, s1
mul_pp r0.xyz, r0.w, r0
mad_pp r1.xyz, t2, r1.w, r1
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
eefiecedbddjiggbcpnngkflhlhnblliohelidglabaaaaaaeaacaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaapahaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcfaabaaaaeaaaaaaafeaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
fibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaad
mcbabaaaabaaaaaagcbaaaadhcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaa
abaaaaaaaagabaaaabaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaaaaaaaebdiaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaa
aaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadcaaaaajhcaabaaaabaaaaaaegbcbaaaacaaaaaapgapbaaa
abaaaaaaegacbaaaabaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaa
diaaaaahhccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaadoaaaaab
"
}
SubProgram "d3d11_9x " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"ps_4_0_level_9_3
eefiecedhpmglclgaoggjkijlnoogjadfoifadliabaaaaaaeeadaaaaaeaaaaaa
daaaaaaadaabaaaaiiacaaaabaadaaaaebgpgodjpiaaaaaapiaaaaaaaaacpppp
mmaaaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppfbaaaaafaaaaapkaaaaaaaebaaaaaaaaaaaaaaaaaaaaaaaa
bpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaaiaabaacplabpaaaaacaaaaaaja
aaaiapkabpaaaaacaaaaaajaabaiapkaabaaaaacaaaaadiaaaaaollaecaaaaad
abaacpiaaaaaoelaaaaioekaecaaaaadaaaacpiaaaaaoeiaabaioekaafaaaaad
aaaaciiaaaaappiaaaaaaakaafaaaaadaaaachiaaaaaoeiaaaaappiaaeaaaaae
acaachiaabaaoelaabaappiaabaaoeiaafaaaaadabaachiaaaaaoeiaacaaoeia
abaaaaacaaaicpiaabaaoeiappppaaaafdeieefcfaabaaaaeaaaaaaafeaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagcbaaaadmcbabaaaabaaaaaagcbaaaadhcbabaaaacaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaa
abaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaadiaaaaahicaabaaaaaaaaaaa
dkaabaaaaaaaaaaaabeaaaaaaaaaaaebdiaaaaahhcaabaaaaaaaaaaaegacbaaa
aaaaaaaapgapbaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaajhcaabaaaabaaaaaaegbcbaaa
acaaaaaapgapbaaaabaaaaaaegacbaaaabaaaaaadgaaaaaficcabaaaaaaaaaaa
dkaabaaaabaaaaaadiaaaaahhccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaa
abaaaaaadoaaaaabejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaa
abaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
abaaaaaaadadaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamamaaaa
heaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaapahaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaa
caaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgf
heaaklkl"
}
}
 }
}
}