Shader "MADFINGER/Environment/Blinking emissive" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _IntensityScaleBias ("Intensity scale X / bias Y", Vector) = (1,0.1,0,0)
 _SwitchOnOffDuration ("Switch ON (X) / OFF (Y) duration", Vector) = (1,3,0,0)
 _BlinkingRate ("Blinking rate", Float) = 10
 _RndGridSize ("Randomization grid size", Float) = 5
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
  Blend One One
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Vector 5 [_Time]
Vector 6 [_IntensityScaleBias]
Vector 7 [_SwitchOnOffDuration]
Float 8 [_BlinkingRate]
Vector 9 [_MainTex_ST]
"!!ARBvp1.0
# 75 ALU
PARAM c[16] = { { 0.79577452, 0, 40, 0.25 },
		state.matrix.mvp,
		program.local[5..9],
		{ 24.980801, -24.980801, -60.145809, 60.145809 },
		{ 0, 0.5, 1, -1 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 },
		{ 3, 7.993, 0.15915491, 17 },
		{ 10, 0.80000001, 0.40000001 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
DP4 R0.x, vertex.color, vertex.color;
MUL R0.w, R0.x, c[0].z;
MOV R0.y, c[8].x;
MUL R0.x, R0.y, c[5].y;
MAD R0.y, R0.x, c[14].x, R0.w;
MAD R0.x, R0, c[0], -c[0].w;
ADD R0.y, R0, c[14];
MUL R0.y, R0, c[14].z;
ADD R0.y, R0, -c[0].w;
FRC R2.w, R0.y;
FRC R1.w, R0.x;
ADD R1.xyz, -R2.w, c[11];
MUL R0.xyz, R1, R1;
MAD R2.xyz, R0, c[10].xyxw, c[10].zwzw;
MAD R3.xyz, R2, R0, c[12].xyxw;
MAD R3.xyz, R3, R0, c[12].zwzw;
MAD R3.xyz, R3, R0, c[13].xyxw;
ADD R1.xyz, -R1.w, c[11];
MUL R1.xyz, R1, R1;
MAD R2.xyz, R1, c[10].xyxw, c[10].zwzw;
MAD R2.xyz, R2, R1, c[12].xyxw;
MAD R2.xyz, R2, R1, c[12].zwzw;
MAD R3.xyz, R3, R0, c[11].wzww;
SLT R4.x, R2.w, c[0].w;
SGE R4.yz, R2.w, c[13].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[11].wzww;
DP3 R2.w, R3, -R0;
MAD R0.xyz, R2, R1, c[13].xyxw;
MAD R1.xyz, R0, R1, c[11].wzww;
SLT R0.x, R1.w, c[0].w;
SGE R0.yz, R1.w, c[13].xzww;
FRC R1.w, R0;
MOV R2.xz, R0;
DP3 R2.y, R0, c[11].wzww;
DP3 R0.x, R1, -R2;
MUL R2.w, R2, c[15].x;
MAD R0.x, R0, c[14].w, R2.w;
MUL R0.x, R0, c[14].z;
FRC R1.x, R0;
ADD R0.xyz, -R1.x, c[11];
MUL R0.xyz, R0, R0;
SGE R1.yz, R1.x, c[13].xzww;
MAD R2.xyz, R0, c[10].xyxw, c[10].zwzw;
MAD R2.xyz, R2, R0, c[12].xyxw;
MAD R2.xyz, R2, R0, c[12].zwzw;
MAD R2.xyz, R2, R0, c[13].xyxw;
SLT R1.x, R1, c[0].w;
MAD R0.xyz, R2, R0, c[11].wzww;
MAD R1.w, R1, c[15].z, c[15].y;
MUL R2.xy, R1.w, c[7];
ADD R2.y, R2.x, R2;
DP3 R1.y, R1, c[11].wzww;
DP3 R0.x, R0, -R1;
ABS R0.x, R0;
ADD R1.w, R0, c[5].y;
RCP R2.z, R2.y;
MUL R1.w, R1, R2.z;
ABS R0.y, R1.w;
FRC R0.y, R0;
ABS R0.z, R2.y;
MUL R0.z, R0.y, R0;
SLT R0.y, R0.w, -c[5];
ADD R0.w, -R0.z, -R0.z;
MAD R0.y, R0.w, R0, R0.z;
SLT R0.y, R0, R2.x;
SLT R0.x, c[11].y, R0;
MUL R0.x, R0, R0.y;
MUL R0, vertex.color, R0.x;
MAD result.texcoord[1], R0, c[6].x, c[6].y;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[9], c[9].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 75 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Time]
Vector 5 [_IntensityScaleBias]
Vector 6 [_SwitchOnOffDuration]
Float 7 [_BlinkingRate]
Vector 8 [_MainTex_ST]
"vs_2_0
; 79 ALU
def c9, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c10, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c11, 40.00000000, 0.79577452, 0.50000000, 3.00000000
def c12, 6.28318501, -3.14159298, 7.99300003, 10.00000000
def c13, 0.15915491, 0.50000000, 17.00000000, 0.00000000
def c14, 0.40000001, 0.80000001, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
dp4 r0.x, v2, v2
mul r2.x, r0, c11
mov r0.y, c4
mul r0.x, c7, r0.y
mad r0.y, r0.x, c11.w, r2.x
add r0.y, r0, c12.z
mad r0.y, r0, c13.x, c13
frc r0.y, r0
mad r0.y, r0, c12.x, c12
mad r0.x, r0, c11.y, c11.z
sincos r1.xy, r0.y, c10.xyzw, c9.xyzw
frc r0.x, r0
mad r1.x, r0, c12, c12.y
sincos r0.xy, r1.x, c10.xyzw, c9.xyzw
mul r0.x, r1.y, c12.w
mad r0.x, r0.y, c13.z, r0
frc r0.y, r2.x
mad r0.y, r0, c14.x, c14
mul r1.xy, r0.y, c6
mad r0.x, r0, c13, c13.y
frc r0.x, r0
mad r1.z, r0.x, c12.x, c12.y
sincos r0.xy, r1.z, c10.xyzw, c9.xyzw
add r1.y, r1.x, r1
rcp r0.z, r1.y
add r0.y, r2.x, c4
mul r0.z, r0.y, r0
slt r0.y, r2.x, -c4
max r0.w, -r0.y, r0.y
abs r0.z, r0
frc r0.y, r0.z
abs r0.z, r1.y
slt r0.w, c13, r0
mul r0.y, r0, r0.z
add r1.y, -r0.w, c9.z
mul r0.z, r0.y, r1.y
mad r0.y, r0.w, -r0, r0.z
abs r0.x, r0
slt r0.y, r0, r1.x
slt r0.x, c9.w, r0
mul r0.x, r0, r0.y
mul r0, v2, r0.x
mad oT1, r0, c5.x, c5.y
mad oT0.xy, v1, c8, c8.zwzw
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
ConstBuffer "$Globals" 64
Vector 16 [_IntensityScaleBias] 2
Vector 24 [_SwitchOnOffDuration] 2
Float 32 [_BlinkingRate]
Vector 48 [_MainTex_ST]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedkbcfiidojndiidghlmiocjaehepbcgibabaaaaaapaafaaaaadaaaaaa
cmaaaaaapeaaaaaageabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaafmaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklfdeieefcieaeaaaaeaaaabaacbabaaaafjaaaaaeegiocaaaaaaaaaaa
aeaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaad
pcbabaaaafaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldccabaaaabaaaaaaegbabaaa
adaaaaaaegiacaaaaaaaaaaaadaaaaaaogikcaaaaaaaaaaaadaaaaaabbaaaaah
bcaabaaaaaaaaaaaegbobaaaafaaaaaaegbobaaaafaaaaaadiaaaaahccaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaacaecbkaaaaafccaabaaaaaaaaaaa
bkaabaaaaaaaaaaadcaaaaajccaabaaaaaaaaaaabkaabaaaaaaaaaaaabeaaaaa
mnmmmmdoabeaaaaamnmmemdpdiaaaaaigcaabaaaaaaaaaaafgafbaaaaaaaaaaa
kgilcaaaaaaaaaaaabaaaaaaaaaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
bkaabaaaaaaaaaaadcaaaaakicaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaa
aaaacaecbkiacaaaabaaaaaaaaaaaaaaaoaaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaackaabaaaaaaaaaaabnaaaaaibcaabaaaabaaaaaadkaabaaaaaaaaaaa
dkaabaiaebaaaaaaaaaaaaaabkaaaaagicaabaaaaaaaaaaadkaabaiaibaaaaaa
aaaaaaaadhaaaaakicaabaaaaaaaaaaaakaabaaaabaaaaaadkaabaaaaaaaaaaa
dkaabaiaebaaaaaaaaaaaaaadiaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
dkaabaaaaaaaaaaadbaaaaahccaabaaaaaaaaaaackaabaaaaaaaaaaabkaabaaa
aaaaaaaadiaaaaajecaabaaaaaaaaaaaakiacaaaaaaaaaaaacaaaaaabkiacaaa
abaaaaaaaaaaaaaadiaaaaakmcaabaaaaaaaaaaakgakbaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaakaeaaaaaeaeadcaaaaajbcaabaaaaaaaaaaaakaabaaa
aaaaaaaaabeaaaaaaaaacaecdkaabaaaaaaaaaaaaaaaaaahbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaabeaaaaakimgppeaenaaaaagfcaabaaaaaaaaaaaaanaaaaa
agacbaaaaaaaaaaadiaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaa
aaaacaebdcaaaaajbcaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaiieb
akaabaaaaaaaaaaaenaaaaagaanaaaaabcaabaaaaaaaaaaaakaabaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaaabeaaaaaaaaaaadpakaabaiaibaaaaaaaaaaaaaa
abaaaaakdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaaaaaaaaaaaaadiaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaa
aaaaaaaadiaaaaahpcaabaaaaaaaaaaaagaabaaaaaaaaaaaegbobaaaafaaaaaa
dcaaaaalpccabaaaacaaaaaaegaobaaaaaaaaaaaagiacaaaaaaaaaaaabaaaaaa
fgifcaaaaaaaaaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 64
Vector 16 [_IntensityScaleBias] 2
Vector 24 [_SwitchOnOffDuration] 2
Float 32 [_BlinkingRate]
Vector 48 [_MainTex_ST]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedmdbcldealcbckefoabhdaknaaggnkhfjabaaaaaaaiakaaaaaeaaaaaa
daaaaaaaeeaeaaaanaaiaaaajiajaaaaebgpgodjamaeaaaaamaeaaaaaaacpopp
maadaaaaemaaaaaaadaaceaaaaaaeiaaaaaaeiaaaaaaceaaabaaeiaaaaaaabaa
adaaabaaaaaaaaaaabaaaaaaabaaaeaaaaaaaaaaacaaaaaaaeaaafaaaaaaaaaa
aaaaaaaaabacpoppfbaaaaafajaaapkaaaaacaecoelheldpaaaaaadpaaaaeaea
fbaaaaafakaaapkanlapmjeanlapejmakimgppeaaaaacaebfbaaaaafalaaapka
idpjccdoaaaaaadpaaaaiiebaaaaaaaafbaaaaafamaaapkamnmmmmdomnmmemdp
aaaaaaaaaaaaaaaafbaaaaafanaaapkaabannalfgballglhklkkckdlijiiiidj
fbaaaaafaoaaapkaklkkkklmaaaaaaloaaaaiadpaaaaaadpbpaaaaacafaaaaia
aaaaapjabpaaaaacafaaadiaadaaapjabpaaaaacafaaafiaafaaapjaaeaaaaae
aaaaadoaadaaoejaadaaoekaadaaookaajaaaaadaaaaabiaafaaoejaafaaoeja
afaaaaadaaaaaciaaaaaaaiaajaaaakaabaaaaacabaaabiaajaaaakaaeaaaaae
aaaaabiaaaaaaaiaabaaaaiaaeaaffkabdaaaaacaaaaaeiaaaaaffiaaeaaaaae
aaaaaeiaaaaakkiaamaaaakaamaaffkaafaaaaadaaaaamiaaaaakkiaabaaoeka
acaaaaadaaaaaiiaaaaappiaaaaakkiaagaaaaacabaaabiaaaaappiaafaaaaad
aaaaabiaaaaaaaiaabaaaaiacdaaaaacabaaabiaaaaaaaiaanaaaaadaaaaabia
aaaaaaiaaaaaaaibbdaaaaacabaaabiaabaaaaiabcaaaaaeacaaabiaaaaaaaia
abaaaaiaabaaaaibafaaaaadaaaaabiaaaaappiaacaaaaiaamaaaaadaaaaabia
aaaaaaiaaaaakkiaabaaaaacabaaaciaaeaaffkaafaaaaadaaaaaeiaabaaffia
acaaaakaaeaaaaaeaaaaaciaaaaakkiaajaappkaaaaaffiaaeaaaaaeaaaaaeia
aaaakkiaajaaffkaajaakkkabdaaaaacaaaaaeiaaaaakkiaaeaaaaaeaaaaaeia
aaaakkiaakaaaakaakaaffkacfaaaaaeabaaaciaaaaakkiaanaaoekaaoaaoeka
acaaaaadaaaaaciaaaaaffiaakaakkkaaeaaaaaeaaaaaciaaaaaffiaalaaaaka
alaaffkabdaaaaacaaaaaciaaaaaffiaaeaaaaaeaaaaaciaaaaaffiaakaaaaka
akaaffkacfaaaaaeacaaaciaaaaaffiaanaaoekaaoaaoekaafaaaaadaaaaacia
acaaffiaakaappkaaeaaaaaeaaaaaciaabaaffiaalaakkkaaaaaffiaaeaaaaae
aaaaaciaaaaaffiaalaaaakaalaaffkabdaaaaacaaaaaciaaaaaffiaaeaaaaae
aaaaaciaaaaaffiaakaaaakaakaaffkacfaaaaaeabaaabiaaaaaffiaanaaoeka
aoaaoekacdaaaaacaaaaaciaabaaaaiaamaaaaadaaaaaciaajaakkkaaaaaffia
afaaaaadaaaaabiaaaaaaaiaaaaaffiaafaaaaadaaaaapiaaaaaaaiaafaaoeja
aeaaaaaeabaaapoaaaaaoeiaabaaaakaabaaffkaafaaaaadaaaaapiaaaaaffja
agaaoekaaeaaaaaeaaaaapiaafaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapia
ahaaoekaaaaakkjaaaaaoeiaaeaaaaaeaaaaapiaaiaaoekaaaaappjaaaaaoeia
aeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeia
ppppaaaafdeieefcieaeaaaaeaaaabaacbabaaaafjaaaaaeegiocaaaaaaaaaaa
aeaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaad
pcbabaaaafaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaaldccabaaaabaaaaaaegbabaaa
adaaaaaaegiacaaaaaaaaaaaadaaaaaaogikcaaaaaaaaaaaadaaaaaabbaaaaah
bcaabaaaaaaaaaaaegbobaaaafaaaaaaegbobaaaafaaaaaadiaaaaahccaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaacaecbkaaaaafccaabaaaaaaaaaaa
bkaabaaaaaaaaaaadcaaaaajccaabaaaaaaaaaaabkaabaaaaaaaaaaaabeaaaaa
mnmmmmdoabeaaaaamnmmemdpdiaaaaaigcaabaaaaaaaaaaafgafbaaaaaaaaaaa
kgilcaaaaaaaaaaaabaaaaaaaaaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
bkaabaaaaaaaaaaadcaaaaakicaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaa
aaaacaecbkiacaaaabaaaaaaaaaaaaaaaoaaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaackaabaaaaaaaaaaabnaaaaaibcaabaaaabaaaaaadkaabaaaaaaaaaaa
dkaabaiaebaaaaaaaaaaaaaabkaaaaagicaabaaaaaaaaaaadkaabaiaibaaaaaa
aaaaaaaadhaaaaakicaabaaaaaaaaaaaakaabaaaabaaaaaadkaabaaaaaaaaaaa
dkaabaiaebaaaaaaaaaaaaaadiaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
dkaabaaaaaaaaaaadbaaaaahccaabaaaaaaaaaaackaabaaaaaaaaaaabkaabaaa
aaaaaaaadiaaaaajecaabaaaaaaaaaaaakiacaaaaaaaaaaaacaaaaaabkiacaaa
abaaaaaaaaaaaaaadiaaaaakmcaabaaaaaaaaaaakgakbaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaakaeaaaaaeaeadcaaaaajbcaabaaaaaaaaaaaakaabaaa
aaaaaaaaabeaaaaaaaaacaecdkaabaaaaaaaaaaaaaaaaaahbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaabeaaaaakimgppeaenaaaaagfcaabaaaaaaaaaaaaanaaaaa
agacbaaaaaaaaaaadiaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaa
aaaacaebdcaaaaajbcaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaiieb
akaabaaaaaaaaaaaenaaaaagaanaaaaabcaabaaaaaaaaaaaakaabaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaaabeaaaaaaaaaaadpakaabaiaibaaaaaaaaaaaaaa
abaaaaakdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaaaaaaaaaaaaadiaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaa
aaaaaaaadiaaaaahpcaabaaaaaaaaaaaagaabaaaaaaaaaaaegbobaaaafaaaaaa
dcaaaaalpccabaaaacaaaaaaegaobaaaaaaaaaaaagiacaaaaaaaaaaaabaaaaaa
fgifcaaaaaaaaaaaabaaaaaadoaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaa
jiaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaa
aaaaaaaaadaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
acaaaaaaahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaa
laaaaaaaabaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaa
aaaaaaaaadaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofe
aaeoepfcenebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogiaaaaaa
adaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
fmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaafmaaaaaaabaaaaaa
aaaaaaaaadaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklkl"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D 0
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 2 ALU, 1 TEX
TEMP R0;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MUL result.color, R0, fragment.texcoord[1];
END
# 2 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D 0
"ps_2_0
; 2 ALU, 1 TEX
dcl_2d s0
dcl t0.xy
dcl t1
texld r0, t0, s0
mul r0, r0, t1
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
SetTexture 0 [_MainTex] 2D 0
"ps_4_0
eefiecedjhmegfpchcmhidddkpfpjaohcifccijlabaaaaaagmabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcjeaaaaaaeaaaaaaacfaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacabaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaa
egbobaaaacaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
SetTexture 0 [_MainTex] 2D 0
"ps_4_0_level_9_3
eefiecedlnbnkffjlmkgfidbbjhbkhpkkjfggkdlabaaaaaapiabaaaaaeaaaaaa
daaaaaaaliaaaaaafeabaaaameabaaaaebgpgodjiaaaaaaaiaaaaaaaaaacpppp
fiaaaaaaciaaaaaaaaaaciaaaaaaciaaaaaaciaaabaaceaaaaaaciaaaaaaaaaa
abacppppbpaaaaacaaaaaaiaaaaaadlabpaaaaacaaaaaaiaabaaaplabpaaaaac
aaaaaajaaaaiapkaecaaaaadaaaaapiaaaaaoelaaaaioekaafaaaaadaaaacpia
aaaaoeiaabaaoelaabaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcjeaaaaaa
eaaaaaaacfaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaahpccabaaaaaaaaaaa
egaobaaaaaaaaaaaegbobaaaacaaaaaadoaaaaabejfdeheogiaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
}
 }
}
}