Shader "MADFINGER/Environment/Skybox with lightning" {
Properties {
 _MainTex ("Layer 0", 2D) = "white" {}
 _DetailTex ("Layer 1 (with bolts)", 2D) = "white" {}
 _ScrollX ("Scroll speed X", Float) = 1
 _ScrollY ("Scroll speed Y", Float) = 0
 _Params ("x - flash bliking rate, y - spawn rate, z - num bolts in tex", Vector) = (10,0.5,10,0)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Geometry+10" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry+10" "RenderType"="Opaque" }
  ZWrite Off
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_Time]
Float 6 [_ScrollX]
Float 7 [_ScrollY]
Vector 8 [_Params]
"!!ARBvp1.0
# 44 ALU
PARAM c[11] = { { 0.25, 0.5, 1, 2 },
		state.matrix.mvp,
		program.local[5..8],
		{ 3, 12.451168, 175034.19, 255 },
		{ 0.30000001, 0.69999999 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MOV R0.yzw, c[0].xxyz;
MOV R0.x, c[8];
MUL R0, R0, c[5].y;
FLR R2, R0;
ADD R1, R2, c[0].z;
MUL R1, R1, c[9].y;
ADD R1, R1, c[0].y;
MUL R2, R2, c[9].y;
ADD R2, R2, c[0].y;
FRC R3, R2;
FRC R1, R1;
MAD R1, R1, c[0].w, -c[0].z;
ABS R2, R1;
MAD R1, -R1, R2, R1;
MAD R3, R3, c[0].w, -c[0].z;
ABS R2, R3;
MAD R2, -R3, R2, R3;
MUL R1, R1, c[9].z;
FRC R3, R1;
MUL R1, R2, c[9].z;
FRC R2, R1;
FRC R0, R0;
MUL R1, -R0, c[0].w;
ADD R1, R1, c[9].x;
MUL R0, R0, R0;
MUL R0, R0, R1;
ADD R3, R3, -R2;
MAD R0, R0, R3, R2;
MAD R0, R0, c[0].w, -c[0].z;
MUL R1.x, R0.z, c[8].z;
SLT R0.y, R0, c[8];
ADD R1.x, R1, -c[9].w;
MAD result.texcoord[1].y, R1.x, R0, c[9].w;
MOV R1.y, c[7].x;
MOV R1.x, c[6];
MUL R1.xy, R1, c[5];
MAD result.texcoord[1].x, R0, c[10], c[10].y;
FRC R0.xy, R1;
MOV result.texcoord[1].zw, R0;
ADD result.texcoord[0].xy, vertex.texcoord[0], R0;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 44 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Time]
Float 5 [_ScrollX]
Float 6 [_ScrollY]
Vector 7 [_Params]
"vs_2_0
; 56 ALU
def c8, 0.25000000, 0.50000000, 1.00000000, 12.45116806
def c9, 2.00000000, 3.00000000, -1.00000000, 175034.18750000
def c10, 0.00000000, 255.00000000, 0.30000001, 0.69999999
dcl_position0 v0
dcl_texcoord0 v1
mov r0.yzw, c8.xxyz
mov r0.x, c7
mul r1, r0, c4.y
frc r0, r1
add r2, -r0, r1
add r1, r2, c8.z
mad r2, r2, c8.w, c8.y
frc r2, r2
mad r3, r2, c9.x, c9.z
mad r1, r1, c8.w, c8.y
frc r1, r1
mad r1, r1, c9.x, c9.z
abs r2, r1
mad r1, -r1, r2, r1
abs r4, r3
mad r3, -r3, r4, r3
mul r2, r3, c9.w
mul r1, r1, c9.w
frc r2, r2
frc r1, r1
add r3, r1, -r2
mad r1, -r0, c9.x, c9.y
mul r0, r0, r0
mul r0, r0, r1
mad r0, r0, r3, r2
mad r0, r0, c9.x, c9.z
slt r0.y, r0, c7
max r0.y, -r0, r0
slt r0.y, c10.x, r0
add r1.x, -r0.y, c8.z
mul r1.y, r1.x, c10
mul r1.x, r0.z, c7.z
mad oT1.y, r0, r1.x, r1
mov r1.y, c6.x
mov r1.x, c5
mul r1.xy, r1, c4
mad oT1.x, r0, c10.z, c10.w
frc r0.xy, r1
mov oT1.zw, r0
add oT0.xy, v1, r0
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "d3d11 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 48
Float 16 [_ScrollX]
Float 20 [_ScrollY]
Vector 32 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedjlpbdoehmdcpjkmklgobbbngkfgehppfabaaaaaaaiahaaaaadaaaaaa
cmaaaaaapeaaaaaageabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaafmaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklfdeieefcjmafaaaaeaaaabaaghabaaaafjaaaaaeegiocaaaaaaaaaaa
adaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadpccabaaa
acaaaaaagiaaaaacaeaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaabaaaaaaegiacaaa
abaaaaaaaaaaaaaabkaaaaafdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaaaaaaah
dccabaaaabaaaaaaegaabaaaaaaaaaaaegbabaaaadaaaaaadiaaaaajbcaabaaa
aaaaaaaaakiacaaaaaaaaaaaacaaaaaabkiacaaaabaaaaaaaaaaaaaadiaaaaal
ocaabaaaaaaaaaaafgifcaaaabaaaaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaiado
aaaaaadpaaaaiadpbkaaaaafpcaabaaaabaaaaaaegaobaaaaaaaaaaaaaaaaaai
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaiaebaaaaaaabaaaaaaaaaaaaak
pcaabaaaacaaaaaaegaobaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpdcaaaaappcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaapmdheheb
pmdhehebpmdhehebpmdhehebaceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaadp
bkaaaaafpcaabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaappcaabaaaaaaaaaaa
egaobaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaa
aaaaialpaaaaialpaaaaialpaaaaialpdcaaaaalpcaabaaaaaaaaaaaegaobaia
ebaaaaaaaaaaaaaaegaobaiaibaaaaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaak
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaaimoockeiimoockeiimoockei
imoockeibkaaaaafpcaabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaappcaabaaa
acaaaaaaegaobaaaacaaaaaaaceaaaaapmdhehebpmdhehebpmdhehebpmdheheb
aceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaadpbkaaaaafpcaabaaaacaaaaaa
egaobaaaacaaaaaadcaaaaappcaabaaaacaaaaaaegaobaaaacaaaaaaaceaaaaa
aaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaialpaaaaialpaaaaialp
aaaaialpdcaaaaalpcaabaaaacaaaaaaegaobaiaebaaaaaaacaaaaaaegaobaia
ibaaaaaaacaaaaaaegaobaaaacaaaaaadiaaaaakpcaabaaaacaaaaaaegaobaaa
acaaaaaaaceaaaaaimoockeiimoockeiimoockeiimoockeibkaaaaafpcaabaaa
acaaaaaaegaobaaaacaaaaaaaaaaaaaipcaabaaaacaaaaaaegaobaiaebaaaaaa
aaaaaaaaegaobaaaacaaaaaadiaaaaahpcaabaaaadaaaaaaegaobaaaabaaaaaa
egaobaaaabaaaaaadcaaaabapcaabaaaabaaaaaaegaobaiaebaaaaaaabaaaaaa
aceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaeaeaaaaaeaea
aaaaeaeaaaaaeaeadiaaaaahpcaabaaaabaaaaaaegaobaaaabaaaaaaegaobaaa
adaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaaabaaaaaaegaobaaaacaaaaaa
egaobaaaaaaaaaaadcaaaaappcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaa
aaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaialpaaaaialpaaaaialp
aaaaialpdbaaaaaiccaabaaaaaaaaaaabkaabaaaaaaaaaaabkiacaaaaaaaaaaa
acaaaaaadiaaaaaibcaabaaaabaaaaaackaabaaaaaaaaaaackiacaaaaaaaaaaa
acaaaaaadcaaaaapnccabaaaacaaaaaaagaobaaaaaaaaaaaaceaaaaajkjjjjdo
aaaaaaaaaaaaiadpaaaaiadpaceaaaaadddddddpaaaaaaaaaaaaaaaaaaaaaaaa
dhaaaaajcccabaaaacaaaaaabkaabaaaaaaaaaaaakaabaaaabaaaaaaabeaaaaa
aaaahpeddoaaaaab"
}
SubProgram "d3d11_9x " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 48
Float 16 [_ScrollX]
Float 20 [_ScrollY]
Vector 32 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedeicelifdoielimajkeleikiepdjjpddoabaaaaaageakaaaaaeaaaaaa
daaaaaaaiiadaaaacmajaaaapeajaaaaebgpgodjfaadaaaafaadaaaaaaacpopp
aeadaaaaemaaaaaaadaaceaaaaaaeiaaaaaaeiaaaaaaceaaabaaeiaaaaaaabaa
acaaabaaaaaaaaaaabaaaaaaabaaadaaaaaaaaaaacaaaaaaaeaaaeaaaaaaaaaa
aaaaaaaaabacpoppfbaaaaafaiaaapkaaaaaiadoaaaaaadpaaaaiadppmdheheb
fbaaaaafajaaapkaaaaaaaeaaaaaialpimoockeiaaaaeaeafbaaaaafakaaapka
jkjjjjdoaaaaiadpdddddddpaaaaaaaafbaaaaafalaaapkaaaaahpmdaaaahped
aaaaaaaaaaaaaaaabpaaaaacafaaaaiaaaaaapjabpaaaaacafaaadiaadaaapja
abaaaaacaaaaadiaadaaoekaafaaaaadaaaaafiaaaaaneiaabaanekabdaaaaac
aaaaafiaaaaaoeiaacaaaaadaaaaadoaaaaaoiiaadaaoejaafaaaaadabaaabia
aaaaffiaacaaaakaafaaaaadabaaaoiaaaaaffiaaiaajakabdaaaaacaaaaapia
abaaoeiaacaaaaadabaaapiaaaaaoeibabaaoeiaacaaaaadacaaapiaabaaoeia
aiaakkkaaeaaaaaeabaaapiaabaaoeiaaiaappkaaiaaffkabdaaaaacabaaapia
abaaoeiaaeaaaaaeabaaapiaabaaoeiaajaaaakaajaaffkaaeaaaaaeacaaapia
acaaoeiaaiaappkaaiaaffkabdaaaaacacaaapiaacaaoeiaaeaaaaaeacaaapia
acaaoeiaajaaaakaajaaffkacdaaaaacadaaapiaacaaoeiaaeaaaaaeacaaapia
acaaoeiaadaaoeibacaaoeiaafaaaaadacaaapiaacaaoeiaajaakkkabdaaaaac
acaaapiaacaaoeiacdaaaaacadaaapiaabaaoeiaaeaaaaaeabaaapiaabaaoeia
adaaoeibabaaoeiaafaaaaadabaaapiaabaaoeiaajaakkkabdaaaaacabaaapia
abaaoeiaafaaaaadadaaapiaaaaaoeiaaaaaoeiaaeaaaaaeaaaaapiaaaaaoeia
ajaaaakbajaappkaafaaaaadaaaaapiaaaaaoeiaadaaoeiabcaaaaaeadaaapia
aaaaoeiaacaaoeiaabaaoeiaaeaaaaaeaaaaapiaadaaoeiaajaaaakaajaaffka
amaaaaadaaaaaciaaaaaffiaacaaffkaabaaaaacabaaaeiaacaakkkaaeaaaaae
abaaabiaaaaakkiaabaakkiaalaaaakaaeaaaaaeabaaanoaaaaaoeiaakaafeka
akaapgkaaeaaaaaeabaaacoaaaaaffiaabaaaaiaalaaffkaafaaaaadaaaaapia
aaaaffjaafaaoekaaeaaaaaeaaaaapiaaeaaoekaaaaaaajaaaaaoeiaaeaaaaae
aaaaapiaagaaoekaaaaakkjaaaaaoeiaaeaaaaaeaaaaapiaahaaoekaaaaappja
aaaaoeiaaeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaamma
aaaaoeiappppaaaafdeieefcjmafaaaaeaaaabaaghabaaaafjaaaaaeegiocaaa
aaaaaaaaadaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaa
acaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
pccabaaaacaaaaaagiaaaaacaeaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaa
aaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
acaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaa
egaobaaaaaaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaabaaaaaa
egiacaaaabaaaaaaaaaaaaaabkaaaaafdcaabaaaaaaaaaaaegaabaaaaaaaaaaa
aaaaaaahdccabaaaabaaaaaaegaabaaaaaaaaaaaegbabaaaadaaaaaadiaaaaaj
bcaabaaaaaaaaaaaakiacaaaaaaaaaaaacaaaaaabkiacaaaabaaaaaaaaaaaaaa
diaaaaalocaabaaaaaaaaaaafgifcaaaabaaaaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaiadoaaaaaadpaaaaiadpbkaaaaafpcaabaaaabaaaaaaegaobaaaaaaaaaaa
aaaaaaaipcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaiaebaaaaaaabaaaaaa
aaaaaaakpcaabaaaacaaaaaaegaobaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpdcaaaaappcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaa
pmdhehebpmdhehebpmdhehebpmdhehebaceaaaaaaaaaaadpaaaaaadpaaaaaadp
aaaaaadpbkaaaaafpcaabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaappcaabaaa
aaaaaaaaegaobaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaea
aceaaaaaaaaaialpaaaaialpaaaaialpaaaaialpdcaaaaalpcaabaaaaaaaaaaa
egaobaiaebaaaaaaaaaaaaaaegaobaiaibaaaaaaaaaaaaaaegaobaaaaaaaaaaa
diaaaaakpcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaaimoockeiimoockei
imoockeiimoockeibkaaaaafpcaabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaap
pcaabaaaacaaaaaaegaobaaaacaaaaaaaceaaaaapmdhehebpmdhehebpmdheheb
pmdhehebaceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaadpbkaaaaafpcaabaaa
acaaaaaaegaobaaaacaaaaaadcaaaaappcaabaaaacaaaaaaegaobaaaacaaaaaa
aceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaialpaaaaialp
aaaaialpaaaaialpdcaaaaalpcaabaaaacaaaaaaegaobaiaebaaaaaaacaaaaaa
egaobaiaibaaaaaaacaaaaaaegaobaaaacaaaaaadiaaaaakpcaabaaaacaaaaaa
egaobaaaacaaaaaaaceaaaaaimoockeiimoockeiimoockeiimoockeibkaaaaaf
pcaabaaaacaaaaaaegaobaaaacaaaaaaaaaaaaaipcaabaaaacaaaaaaegaobaia
ebaaaaaaaaaaaaaaegaobaaaacaaaaaadiaaaaahpcaabaaaadaaaaaaegaobaaa
abaaaaaaegaobaaaabaaaaaadcaaaabapcaabaaaabaaaaaaegaobaiaebaaaaaa
abaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaeaea
aaaaeaeaaaaaeaeaaaaaeaeadiaaaaahpcaabaaaabaaaaaaegaobaaaabaaaaaa
egaobaaaadaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaaabaaaaaaegaobaaa
acaaaaaaegaobaaaaaaaaaaadcaaaaappcaabaaaaaaaaaaaegaobaaaaaaaaaaa
aceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaialpaaaaialp
aaaaialpaaaaialpdbaaaaaiccaabaaaaaaaaaaabkaabaaaaaaaaaaabkiacaaa
aaaaaaaaacaaaaaadiaaaaaibcaabaaaabaaaaaackaabaaaaaaaaaaackiacaaa
aaaaaaaaacaaaaaadcaaaaapnccabaaaacaaaaaaagaobaaaaaaaaaaaaceaaaaa
jkjjjjdoaaaaaaaaaaaaiadpaaaaiadpaceaaaaadddddddpaaaaaaaaaaaaaaaa
aaaaaaaadhaaaaajcccabaaaacaaaaaabkaabaaaaaaaaaaaakaabaaaabaaaaaa
abeaaaaaaaaahpeddoaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaafmaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklkl"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 11 ALU, 2 TEX
PARAM c[1] = { { 25.5, 0, 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R1, fragment.texcoord[0], texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MUL R2.x, R1.w, c[0];
MOV R2.y, fragment.texcoord[1];
FLR R2.xy, R2;
ADD R2.x, R2, -R2.y;
ABS R2.x, R2;
CMP R2.x, -R2, c[0].y, c[0].z;
ADD R1, R1, -R0;
MUL R2.x, R2, fragment.texcoord[1];
MAD result.color, R2.x, R1, R0;
END
# 11 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_2_0
; 11 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c0, 25.50000000, 1.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xy
texld r1, t0, s1
texld r2, t0, s0
mul r0.x, r1.w, c0
mov_pp r0.y, t1
frc_pp r3.xy, r0
add_pp r0.xy, r0, -r3
add_pp r0.x, r0, -r0.y
abs_pp r0.x, r0
cmp_pp r0.x, -r0, c0.y, c0.z
add_pp r1, r1, -r2
mul r0.x, r0, t1
mad_pp r0, r0.x, r1, r2
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0
eefiecedkjffcdkacjkmpkijlfnfkjglgfigkamiabaaaaaakaacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcmiabaaaaeaaaaaaahcaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagcbaaaaddcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
adaaaaaabkaaaaafccaabaaaaaaaaaaabkbabaaaacaaaaaadgaaaaafccaabaaa
abaaaaaabkbabaaaacaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaabaaaaaa
eghobaaaabaaaaaaaagabaaaabaaaaaadiaaaaahbcaabaaaabaaaaaadkaabaaa
acaaaaaaabeaaaaaaaaammebbkaaaaafbcaabaaaaaaaaaaaakaabaaaabaaaaaa
aaaaaaaidcaabaaaaaaaaaaaegaabaiaebaaaaaaaaaaaaaaegaabaaaabaaaaaa
biaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaaabaaaaah
bcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaakbabaaaacaaaaaaefaaaaajpcaabaaaabaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaipcaabaaa
acaaaaaaegaobaiaebaaaaaaabaaaaaaegaobaaaacaaaaaadcaaaaajpccabaaa
aaaaaaaaagaabaaaaaaaaaaaegaobaaaacaaaaaaegaobaaaabaaaaaadoaaaaab
"
}
SubProgram "d3d11_9x " {
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_DetailTex] 2D 1
"ps_4_0_level_9_3
eefieceddoejiipblglckmhgkibkhclhejmklpgcabaaaaaaoaadaaaaaeaaaaaa
daaaaaaagmabaaaadmadaaaakmadaaaaebgpgodjdeabaaaadeabaaaaaaacpppp
aiabaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppfbaaaaafaaaaapkaaaaammebaaaaaaaaaaaaaaaaaaaaaaaa
bpaaaaacaaaaaaiaaaaaadlabpaaaaacaaaaaaiaabaacplabpaaaaacaaaaaaja
aaaiapkabpaaaaacaaaaaajaabaiapkabdaaaaacaaaacciaabaafflaabaaaaac
abaacciaabaafflaecaaaaadacaacpiaaaaaoelaaaaioekaecaaaaadadaaapia
aaaaoelaabaioekaafaaaaadabaacbiaadaappiaaaaaaakabdaaaaacaaaacbia
abaaaaiaacaaaaadaaaacdiaaaaaoeibabaaoeiaacaaaaadaaaaabiaaaaaffib
aaaaaaiaafaaaaadaaaaabiaaaaaaaiaaaaaaaiafiaaaaaeaaaaabiaaaaaaaib
abaaaalaaaaaffkabcaaaaaeabaacpiaaaaaaaiaadaaoeiaacaaoeiaabaaaaac
aaaicpiaabaaoeiappppaaaafdeieefcmiabaaaaeaaaaaaahcaaaaaafkaaaaad
aagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gcbaaaaddcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaa
bkaaaaafccaabaaaaaaaaaaabkbabaaaacaaaaaadgaaaaafccaabaaaabaaaaaa
bkbabaaaacaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaabaaaaaaeghobaaa
abaaaaaaaagabaaaabaaaaaadiaaaaahbcaabaaaabaaaaaadkaabaaaacaaaaaa
abeaaaaaaaaammebbkaaaaafbcaabaaaaaaaaaaaakaabaaaabaaaaaaaaaaaaai
dcaabaaaaaaaaaaaegaabaiaebaaaaaaaaaaaaaaegaabaaaabaaaaaabiaaaaah
bcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaaabaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaakbabaaaacaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaipcaabaaaacaaaaaa
egaobaiaebaaaaaaabaaaaaaegaobaaaacaaaaaadcaaaaajpccabaaaaaaaaaaa
agaabaaaaaaaaaaaegaobaaaacaaaaaaegaobaaaabaaaaaadoaaaaabejfdeheo
giaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaa
apaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaafmaaaaaa
abaaaaaaaaaaaaaaadaaaaaaacaaaaaaapadaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl
"
}
}
 }
}
}