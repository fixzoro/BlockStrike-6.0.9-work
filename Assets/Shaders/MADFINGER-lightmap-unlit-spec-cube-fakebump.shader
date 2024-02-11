Shader "MADFINGER/Environment/Cubemap specular + Lightmap + fake bump" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _SpecCubeTex ("SpecCube", CUBE) = "black" {}
 _SpecularStrength ("Specular strength weights", Vector) = (0,0,0,2)
 _ScrollingSpeed ("Scrolling speed", Vector) = (0,0,0,0)
 _Params ("Bumpiness - x", Vector) = (2,0,0,0)
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
Program "vp" {
SubProgram "opengl " {
Keywords { "UNITY_SHADER_DETAIL_LOW" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 5 [_Time]
Vector 6 [unity_LightmapST]
Vector 7 [_ScrollingSpeed]
Vector 8 [_Params]
"!!ARBvp1.0
# 11 ALU
PARAM c[9] = { { 2, 1 },
		state.matrix.mvp,
		program.local[5..8] };
TEMP R0;
MOV R0.x, c[5].y;
MUL R0.zw, R0.x, c[7].xyxy;
MOV R0.xy, c[0];
FRC R0.zw, R0;
MUL result.texcoord[0].zw, R0.xyxy, c[8].x;
ADD result.texcoord[0].xy, vertex.texcoord[0], R0.zwzw;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[6], c[6].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 11 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "UNITY_SHADER_DETAIL_LOW" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Time]
Vector 5 [unity_LightmapST]
Vector 6 [_ScrollingSpeed]
Vector 7 [_Params]
"vs_2_0
; 13 ALU
def c8, 2.00000000, 1.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_texcoord1 v2
mov r0.xy, c6
mul r0.zw, c4.y, r0.xyxy
mov r0.x, c7
frc r0.zw, r0
mul oT0.zw, c8.xyxy, r0.x
add oT0.xy, v1, r0.zwzw
mad oT1.xy, v2, c5, c5.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SHADER_DETAIL_LOW" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [unity_LightmapST]
Vector 32 [_ScrollingSpeed]
Vector 64 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecednmfahiaaocjalnfegeapipjioogmglllabaaaaaaeaadaaaaadaaaaaa
cmaaaaaapeaaaaaageabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaafmaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklfdeieefcneabaaaaeaaaabaahfaaaaaafjaaaaaeegiocaaaaaaaaaaa
afaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaad
dcbabaaaaeaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaa
abaaaaaagfaaaaaddccabaaaacaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaa
aaaaaaaaacaaaaaafgifcaaaabaaaaaaaaaaaaaabkaaaaafdcaabaaaaaaaaaaa
egaabaaaaaaaaaaaaaaaaaahdccabaaaabaaaaaaegaabaaaaaaaaaaaegbabaaa
adaaaaaadiaaaaalmccabaaaabaaaaaaagiacaaaaaaaaaaaaeaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaeaaaaaiadpdcaaaaaldccabaaaacaaaaaaegbabaaa
aeaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaadoaaaaab
"
}
SubProgram "d3d11_9x " {
Keywords { "UNITY_SHADER_DETAIL_LOW" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [unity_LightmapST]
Vector 32 [_ScrollingSpeed]
Vector 64 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedgccnhkgjibbmphkoomdddndegdmnmcgcabaaaaaalmaeaaaaaeaaaaaa
daaaaaaakiabaaaaieadaaaaemaeaaaaebgpgodjhaabaaaahaabaaaaaaacpopp
biabaaaafiaaaaaaaeaaceaaaaaafeaaaaaafeaaaaaaceaaabaafeaaaaaaabaa
acaaabaaaaaaaaaaaaaaaeaaabaaadaaaaaaaaaaabaaaaaaabaaaeaaaaaaaaaa
acaaaaaaaeaaafaaaaaaaaaaaaaaaaaaabacpoppfbaaaaafajaaapkaaaaaaaea
aaaaiadpaaaaaaaaaaaaaaaabpaaaaacafaaaaiaaaaaapjabpaaaaacafaaadia
adaaapjabpaaaaacafaaaeiaaeaaapjaabaaaaacaaaaadiaacaaoekaafaaaaad
aaaaadiaaaaaoeiaaeaaffkabdaaaaacaaaaadiaaaaaoeiaacaaaaadaaaaadoa
aaaaoeiaadaaoejaabaaaaacaaaaabiaadaaaakaafaaaaadaaaaamoaaaaaaaia
ajaaeekaaeaaaaaeabaaadoaaeaaoejaabaaoekaabaaookaafaaaaadaaaaapia
aaaaffjaagaaoekaaeaaaaaeaaaaapiaafaaoekaaaaaaajaaaaaoeiaaeaaaaae
aaaaapiaahaaoekaaaaakkjaaaaaoeiaaeaaaaaeaaaaapiaaiaaoekaaaaappja
aaaaoeiaaeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaamma
aaaaoeiappppaaaafdeieefcneabaaaaeaaaabaahfaaaaaafjaaaaaeegiocaaa
aaaaaaaaafaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaa
acaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaa
fpaaaaaddcbabaaaaeaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaad
pccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagiaaaaacabaaaaaadiaaaaai
pcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaa
adaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaajdcaabaaaaaaaaaaa
egiacaaaaaaaaaaaacaaaaaafgifcaaaabaaaaaaaaaaaaaabkaaaaafdcaabaaa
aaaaaaaaegaabaaaaaaaaaaaaaaaaaahdccabaaaabaaaaaaegaabaaaaaaaaaaa
egbabaaaadaaaaaadiaaaaalmccabaaaabaaaaaaagiacaaaaaaaaaaaaeaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaeaaaaaiadpdcaaaaaldccabaaaacaaaaaa
egbabaaaaeaaaaaaegiacaaaaaaaaaaaabaaaaaaogikcaaaaaaaaaaaabaaaaaa
doaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
apaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaaaaaalaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaaabaaaaaaaaaaaaaa
adaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaaaaaaaaaaadaaaaaaafaaaaaa
apaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfcenebemaafeeffied
epepfceeaaedepemepfcaaklepfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl"
}
SubProgram "opengl " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [_WorldSpaceCameraPos]
Vector 11 [unity_LightmapST]
Vector 12 [_ScrollingSpeed]
Vector 13 [_Params]
"!!ARBvp1.0
# 24 ALU
PARAM c[14] = { { 2, 1 },
		state.matrix.mvp,
		program.local[5..13] };
TEMP R0;
TEMP R1;
DP3 R1.z, vertex.normal, c[7];
DP3 R1.x, vertex.normal, c[5];
DP3 R1.y, vertex.normal, c[6];
DP3 R0.x, R1, R1;
RSQ R0.w, R0.x;
MUL R1.xyz, R0.w, R1;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD R0.xyz, -R0, c[10];
DP3 R0.w, R1, -R0;
MUL R1.xyz, R1, R0.w;
MAD result.texcoord[2].xyz, -R1, c[0].x, -R0;
MOV R0.w, c[9].y;
MUL R0.zw, R0.w, c[12].xyxy;
MOV R0.xy, c[0];
FRC R0.zw, R0;
MUL result.texcoord[0].zw, R0.xyxy, c[13].x;
ADD result.texcoord[0].xy, vertex.texcoord[0], R0.zwzw;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[11], c[11].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 24 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [_WorldSpaceCameraPos]
Vector 10 [unity_LightmapST]
Vector 11 [_ScrollingSpeed]
Vector 12 [_Params]
"vs_2_0
; 26 ALU
def c13, 2.00000000, 1.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
dp3 r1.z, v1, c6
dp3 r1.x, v1, c4
dp3 r1.y, v1, c5
dp3 r0.x, r1, r1
rsq r0.w, r0.x
mul r1.xyz, r0.w, r1
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add r0.xyz, -r0, c9
dp3 r0.w, r1, -r0
mul r1.xyz, r1, r0.w
mad oT2.xyz, -r1, c13.x, -r0
mov r2.xy, c11
mul r0.zw, c8.y, r2.xyxy
mov r0.x, c12
frc r0.zw, r0
mul oT0.zw, c13.xyxy, r0.x
add oT0.xy, v2, r0.zwzw
mad oT1.xy, v3, c10, c10.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [unity_LightmapST]
Vector 32 [_ScrollingSpeed]
Vector 64 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
Vector 64 [_WorldSpaceCameraPos] 3
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 192 [_Object2World]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedhaphmablhjhecikgjodnnncknhhagbllabaaaaaafaafaaaaadaaaaaa
cmaaaaaapeaaaaaahmabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaa
aiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaaheaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaadamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaa
ahaiaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefc
mmadaaaaeaaaabaapdaaaaaafjaaaaaeegiocaaaaaaaaaaaafaaaaaafjaaaaae
egiocaaaabaaaaaaafaaaaaafjaaaaaeegiocaaaacaaaaaabaaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaadhcbabaaaacaaaaaafpaaaaaddcbabaaaadaaaaaa
fpaaaaaddcbabaaaaeaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaad
pccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaadhccabaaaadaaaaaa
giaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
acaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
acaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
diaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaacaaaaaafgifcaaaabaaaaaa
aaaaaaaabkaaaaafdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaaaaaaahdccabaaa
abaaaaaaegaabaaaaaaaaaaaegbabaaaadaaaaaadiaaaaalmccabaaaabaaaaaa
agiacaaaaaaaaaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaeaaaaaiadp
dcaaaaaldccabaaaacaaaaaaegbabaaaaeaaaaaaegiacaaaaaaaaaaaabaaaaaa
ogikcaaaaaaaaaaaabaaaaaadiaaaaaihcaabaaaaaaaaaaafgbfbaaaacaaaaaa
egiccaaaacaaaaaaanaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaaacaaaaaa
amaaaaaaagbabaaaacaaaaaaegacbaaaaaaaaaaadcaaaaakhcaabaaaaaaaaaaa
egiccaaaacaaaaaaaoaaaaaakgbkbaaaacaaaaaaegacbaaaaaaaaaaabaaaaaah
icaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaaaaaaaaaeeaaaaaficaabaaa
aaaaaaaadkaabaaaaaaaaaaadiaaaaahhcaabaaaaaaaaaaapgapbaaaaaaaaaaa
egacbaaaaaaaaaaadiaaaaaihcaabaaaabaaaaaafgbfbaaaaaaaaaaaegiccaaa
acaaaaaaanaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaaacaaaaaaamaaaaaa
agbabaaaaaaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaa
acaaaaaaaoaaaaaakgbkbaaaaaaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaa
abaaaaaaegiccaaaacaaaaaaapaaaaaapgbpbaaaaaaaaaaaegacbaaaabaaaaaa
aaaaaaajhcaabaaaabaaaaaaegacbaiaebaaaaaaabaaaaaaegiccaaaabaaaaaa
aeaaaaaabaaaaaaiicaabaaaaaaaaaaaegacbaiaebaaaaaaabaaaaaaegacbaaa
aaaaaaaaaaaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaaaaaaaaa
dcaaaaalhccabaaaadaaaaaaegacbaaaaaaaaaaapgapbaiaebaaaaaaaaaaaaaa
egacbaiaebaaaaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [unity_LightmapST]
Vector 32 [_ScrollingSpeed]
Vector 64 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
Vector 64 [_WorldSpaceCameraPos] 3
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 192 [_Object2World]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedbobpjfmpffdplfdfkjpihhgilocgemgdabaaaaaameahaaaaaeaaaaaa
daaaaaaakaacaaaaheagaaaadmahaaaaebgpgodjgiacaaaagiacaaaaaaacpopp
piabaaaahaaaaaaaagaaceaaaaaagmaaaaaagmaaaaaaceaaabaagmaaaaaaabaa
acaaabaaaaaaaaaaaaaaaeaaabaaadaaaaaaaaaaabaaaaaaabaaaeaaaaaaaaaa
abaaaeaaabaaafaaaaaaaaaaacaaaaaaaeaaagaaaaaaaaaaacaaamaaaeaaakaa
aaaaaaaaaaaaaaaaabacpoppfbaaaaafaoaaapkaaaaaaaeaaaaaiadpaaaaaaaa
aaaaaaaabpaaaaacafaaaaiaaaaaapjabpaaaaacafaaaciaacaaapjabpaaaaac
afaaadiaadaaapjabpaaaaacafaaaeiaaeaaapjaabaaaaacaaaaadiaacaaoeka
afaaaaadaaaaadiaaaaaoeiaaeaaffkabdaaaaacaaaaadiaaaaaoeiaacaaaaad
aaaaadoaaaaaoeiaadaaoejaabaaaaacaaaaabiaadaaaakaafaaaaadaaaaamoa
aaaaaaiaaoaaeekaafaaaaadaaaaahiaaaaaffjaalaaoekaaeaaaaaeaaaaahia
akaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaahiaamaaoekaaaaakkjaaaaaoeia
aeaaaaaeaaaaahiaanaaoekaaaaappjaaaaaoeiaacaaaaadaaaaahiaaaaaoeib
afaaoekaafaaaaadabaaahiaacaaffjaalaaoekaaeaaaaaeabaaahiaakaaoeka
acaaaajaabaaoeiaaeaaaaaeabaaahiaamaaoekaacaakkjaabaaoeiaceaaaaac
acaaahiaabaaoeiaaiaaaaadaaaaaiiaaaaaoeibacaaoeiaacaaaaadaaaaaiia
aaaappiaaaaappiaaeaaaaaeacaaahoaacaaoeiaaaaappibaaaaoeibaeaaaaae
abaaadoaaeaaoejaabaaoekaabaaookaafaaaaadaaaaapiaaaaaffjaahaaoeka
aeaaaaaeaaaaapiaagaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaaiaaoeka
aaaakkjaaaaaoeiaaeaaaaaeaaaaapiaajaaoekaaaaappjaaaaaoeiaaeaaaaae
aaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaa
fdeieefcmmadaaaaeaaaabaapdaaaaaafjaaaaaeegiocaaaaaaaaaaaafaaaaaa
fjaaaaaeegiocaaaabaaaaaaafaaaaaafjaaaaaeegiocaaaacaaaaaabaaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaadhcbabaaaacaaaaaafpaaaaaddcbabaaa
adaaaaaafpaaaaaddcbabaaaaeaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaadpccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaadhccabaaa
adaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaacaaaaaafgifcaaa
abaaaaaaaaaaaaaabkaaaaafdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaaaaaaah
dccabaaaabaaaaaaegaabaaaaaaaaaaaegbabaaaadaaaaaadiaaaaalmccabaaa
abaaaaaaagiacaaaaaaaaaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaea
aaaaiadpdcaaaaaldccabaaaacaaaaaaegbabaaaaeaaaaaaegiacaaaaaaaaaaa
abaaaaaaogikcaaaaaaaaaaaabaaaaaadiaaaaaihcaabaaaaaaaaaaafgbfbaaa
acaaaaaaegiccaaaacaaaaaaanaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaa
acaaaaaaamaaaaaaagbabaaaacaaaaaaegacbaaaaaaaaaaadcaaaaakhcaabaaa
aaaaaaaaegiccaaaacaaaaaaaoaaaaaakgbkbaaaacaaaaaaegacbaaaaaaaaaaa
baaaaaahicaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaaaaaaaaaeeaaaaaf
icaabaaaaaaaaaaadkaabaaaaaaaaaaadiaaaaahhcaabaaaaaaaaaaapgapbaaa
aaaaaaaaegacbaaaaaaaaaaadiaaaaaihcaabaaaabaaaaaafgbfbaaaaaaaaaaa
egiccaaaacaaaaaaanaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaaacaaaaaa
amaaaaaaagbabaaaaaaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaaabaaaaaa
egiccaaaacaaaaaaaoaaaaaakgbkbaaaaaaaaaaaegacbaaaabaaaaaadcaaaaak
hcaabaaaabaaaaaaegiccaaaacaaaaaaapaaaaaapgbpbaaaaaaaaaaaegacbaaa
abaaaaaaaaaaaaajhcaabaaaabaaaaaaegacbaiaebaaaaaaabaaaaaaegiccaaa
abaaaaaaaeaaaaaabaaaaaaiicaabaaaaaaaaaaaegacbaiaebaaaaaaabaaaaaa
egacbaaaaaaaaaaaaaaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaa
aaaaaaaadcaaaaalhccabaaaadaaaaaaegacbaaaaaaaaaaapgapbaiaebaaaaaa
aaaaaaaaegacbaiaebaaaaaaabaaaaaadoaaaaabejfdeheomaaaaaaaagaaaaaa
aiaaaaaajiaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaacaaaaaaahahaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaa
apadaaaalaaaaaaaabaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeo
ehefeofeaaeoepfcenebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheo
iaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaa
apaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaaheaaaaaa
abaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaheaaaaaaacaaaaaaaaaaaaaa
adaaaaaaadaaaaaaahaiaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklkl"
}
SubProgram "opengl " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [_WorldSpaceCameraPos]
Vector 11 [unity_LightmapST]
Vector 12 [_ScrollingSpeed]
Vector 13 [_Params]
"!!ARBvp1.0
# 24 ALU
PARAM c[14] = { { 2, 1 },
		state.matrix.mvp,
		program.local[5..13] };
TEMP R0;
TEMP R1;
DP3 R1.z, vertex.normal, c[7];
DP3 R1.x, vertex.normal, c[5];
DP3 R1.y, vertex.normal, c[6];
DP3 R0.x, R1, R1;
RSQ R0.w, R0.x;
MUL R1.xyz, R0.w, R1;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD R0.xyz, -R0, c[10];
DP3 R0.w, R1, -R0;
MUL R1.xyz, R1, R0.w;
MAD result.texcoord[2].xyz, -R1, c[0].x, -R0;
MOV R0.w, c[9].y;
MUL R0.zw, R0.w, c[12].xyxy;
MOV R0.xy, c[0];
FRC R0.zw, R0;
MUL result.texcoord[0].zw, R0.xyxy, c[13].x;
ADD result.texcoord[0].xy, vertex.texcoord[0], R0.zwzw;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[11], c[11].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 24 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [_WorldSpaceCameraPos]
Vector 10 [unity_LightmapST]
Vector 11 [_ScrollingSpeed]
Vector 12 [_Params]
"vs_2_0
; 26 ALU
def c13, 2.00000000, 1.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
dp3 r1.z, v1, c6
dp3 r1.x, v1, c4
dp3 r1.y, v1, c5
dp3 r0.x, r1, r1
rsq r0.w, r0.x
mul r1.xyz, r0.w, r1
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add r0.xyz, -r0, c9
dp3 r0.w, r1, -r0
mul r1.xyz, r1, r0.w
mad oT2.xyz, -r1, c13.x, -r0
mov r2.xy, c11
mul r0.zw, c8.y, r2.xyxy
mov r0.x, c12
frc r0.zw, r0
mul oT0.zw, c13.xyxy, r0.x
add oT0.xy, v2, r0.zwzw
mad oT1.xy, v3, c10, c10.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [unity_LightmapST]
Vector 32 [_ScrollingSpeed]
Vector 64 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
Vector 64 [_WorldSpaceCameraPos] 3
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 192 [_Object2World]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedhaphmablhjhecikgjodnnncknhhagbllabaaaaaafaafaaaaadaaaaaa
cmaaaaaapeaaaaaahmabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheoiaaaaaaaaeaaaaaa
aiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaaheaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaadamaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaa
ahaiaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefc
mmadaaaaeaaaabaapdaaaaaafjaaaaaeegiocaaaaaaaaaaaafaaaaaafjaaaaae
egiocaaaabaaaaaaafaaaaaafjaaaaaeegiocaaaacaaaaaabaaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaadhcbabaaaacaaaaaafpaaaaaddcbabaaaadaaaaaa
fpaaaaaddcbabaaaaeaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaad
pccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaadhccabaaaadaaaaaa
giaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
acaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
acaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
diaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaacaaaaaafgifcaaaabaaaaaa
aaaaaaaabkaaaaafdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaaaaaaahdccabaaa
abaaaaaaegaabaaaaaaaaaaaegbabaaaadaaaaaadiaaaaalmccabaaaabaaaaaa
agiacaaaaaaaaaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaeaaaaaiadp
dcaaaaaldccabaaaacaaaaaaegbabaaaaeaaaaaaegiacaaaaaaaaaaaabaaaaaa
ogikcaaaaaaaaaaaabaaaaaadiaaaaaihcaabaaaaaaaaaaafgbfbaaaacaaaaaa
egiccaaaacaaaaaaanaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaaacaaaaaa
amaaaaaaagbabaaaacaaaaaaegacbaaaaaaaaaaadcaaaaakhcaabaaaaaaaaaaa
egiccaaaacaaaaaaaoaaaaaakgbkbaaaacaaaaaaegacbaaaaaaaaaaabaaaaaah
icaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaaaaaaaaaeeaaaaaficaabaaa
aaaaaaaadkaabaaaaaaaaaaadiaaaaahhcaabaaaaaaaaaaapgapbaaaaaaaaaaa
egacbaaaaaaaaaaadiaaaaaihcaabaaaabaaaaaafgbfbaaaaaaaaaaaegiccaaa
acaaaaaaanaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaaacaaaaaaamaaaaaa
agbabaaaaaaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaa
acaaaaaaaoaaaaaakgbkbaaaaaaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaa
abaaaaaaegiccaaaacaaaaaaapaaaaaapgbpbaaaaaaaaaaaegacbaaaabaaaaaa
aaaaaaajhcaabaaaabaaaaaaegacbaiaebaaaaaaabaaaaaaegiccaaaabaaaaaa
aeaaaaaabaaaaaaiicaabaaaaaaaaaaaegacbaiaebaaaaaaabaaaaaaegacbaaa
aaaaaaaaaaaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaaaaaaaaa
dcaaaaalhccabaaaadaaaaaaegacbaaaaaaaaaaapgapbaiaebaaaaaaaaaaaaaa
egacbaiaebaaaaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
ConstBuffer "$Globals" 80
Vector 16 [unity_LightmapST]
Vector 32 [_ScrollingSpeed]
Vector 64 [_Params]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
Vector 64 [_WorldSpaceCameraPos] 3
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 192 [_Object2World]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecedbobpjfmpffdplfdfkjpihhgilocgemgdabaaaaaameahaaaaaeaaaaaa
daaaaaaakaacaaaaheagaaaadmahaaaaebgpgodjgiacaaaagiacaaaaaaacpopp
piabaaaahaaaaaaaagaaceaaaaaagmaaaaaagmaaaaaaceaaabaagmaaaaaaabaa
acaaabaaaaaaaaaaaaaaaeaaabaaadaaaaaaaaaaabaaaaaaabaaaeaaaaaaaaaa
abaaaeaaabaaafaaaaaaaaaaacaaaaaaaeaaagaaaaaaaaaaacaaamaaaeaaakaa
aaaaaaaaaaaaaaaaabacpoppfbaaaaafaoaaapkaaaaaaaeaaaaaiadpaaaaaaaa
aaaaaaaabpaaaaacafaaaaiaaaaaapjabpaaaaacafaaaciaacaaapjabpaaaaac
afaaadiaadaaapjabpaaaaacafaaaeiaaeaaapjaabaaaaacaaaaadiaacaaoeka
afaaaaadaaaaadiaaaaaoeiaaeaaffkabdaaaaacaaaaadiaaaaaoeiaacaaaaad
aaaaadoaaaaaoeiaadaaoejaabaaaaacaaaaabiaadaaaakaafaaaaadaaaaamoa
aaaaaaiaaoaaeekaafaaaaadaaaaahiaaaaaffjaalaaoekaaeaaaaaeaaaaahia
akaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaahiaamaaoekaaaaakkjaaaaaoeia
aeaaaaaeaaaaahiaanaaoekaaaaappjaaaaaoeiaacaaaaadaaaaahiaaaaaoeib
afaaoekaafaaaaadabaaahiaacaaffjaalaaoekaaeaaaaaeabaaahiaakaaoeka
acaaaajaabaaoeiaaeaaaaaeabaaahiaamaaoekaacaakkjaabaaoeiaceaaaaac
acaaahiaabaaoeiaaiaaaaadaaaaaiiaaaaaoeibacaaoeiaacaaaaadaaaaaiia
aaaappiaaaaappiaaeaaaaaeacaaahoaacaaoeiaaaaappibaaaaoeibaeaaaaae
abaaadoaaeaaoejaabaaoekaabaaookaafaaaaadaaaaapiaaaaaffjaahaaoeka
aeaaaaaeaaaaapiaagaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaaiaaoeka
aaaakkjaaaaaoeiaaeaaaaaeaaaaapiaajaaoekaaaaappjaaaaaoeiaaeaaaaae
aaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaa
fdeieefcmmadaaaaeaaaabaapdaaaaaafjaaaaaeegiocaaaaaaaaaaaafaaaaaa
fjaaaaaeegiocaaaabaaaaaaafaaaaaafjaaaaaeegiocaaaacaaaaaabaaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaadhcbabaaaacaaaaaafpaaaaaddcbabaaa
adaaaaaafpaaaaaddcbabaaaaeaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaadpccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaadhccabaaa
adaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaacaaaaaafgifcaaa
abaaaaaaaaaaaaaabkaaaaafdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaaaaaaah
dccabaaaabaaaaaaegaabaaaaaaaaaaaegbabaaaadaaaaaadiaaaaalmccabaaa
abaaaaaaagiacaaaaaaaaaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaea
aaaaiadpdcaaaaaldccabaaaacaaaaaaegbabaaaaeaaaaaaegiacaaaaaaaaaaa
abaaaaaaogikcaaaaaaaaaaaabaaaaaadiaaaaaihcaabaaaaaaaaaaafgbfbaaa
acaaaaaaegiccaaaacaaaaaaanaaaaaadcaaaaakhcaabaaaaaaaaaaaegiccaaa
acaaaaaaamaaaaaaagbabaaaacaaaaaaegacbaaaaaaaaaaadcaaaaakhcaabaaa
aaaaaaaaegiccaaaacaaaaaaaoaaaaaakgbkbaaaacaaaaaaegacbaaaaaaaaaaa
baaaaaahicaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaaaaaaaaaeeaaaaaf
icaabaaaaaaaaaaadkaabaaaaaaaaaaadiaaaaahhcaabaaaaaaaaaaapgapbaaa
aaaaaaaaegacbaaaaaaaaaaadiaaaaaihcaabaaaabaaaaaafgbfbaaaaaaaaaaa
egiccaaaacaaaaaaanaaaaaadcaaaaakhcaabaaaabaaaaaaegiccaaaacaaaaaa
amaaaaaaagbabaaaaaaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaaabaaaaaa
egiccaaaacaaaaaaaoaaaaaakgbkbaaaaaaaaaaaegacbaaaabaaaaaadcaaaaak
hcaabaaaabaaaaaaegiccaaaacaaaaaaapaaaaaapgbpbaaaaaaaaaaaegacbaaa
abaaaaaaaaaaaaajhcaabaaaabaaaaaaegacbaiaebaaaaaaabaaaaaaegiccaaa
abaaaaaaaeaaaaaabaaaaaaiicaabaaaaaaaaaaaegacbaiaebaaaaaaabaaaaaa
egacbaaaaaaaaaaaaaaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaa
aaaaaaaadcaaaaalhccabaaaadaaaaaaegacbaaaaaaaaaaapgapbaiaebaaaaaa
aaaaaaaaegacbaiaebaaaaaaabaaaaaadoaaaaabejfdeheomaaaaaaaagaaaaaa
aiaaaaaajiaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaacaaaaaaahahaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaa
apadaaaalaaaaaaaabaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapadaaaaljaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaafaaaaaaapaaaaaafaepfdejfeejepeoaafeebeo
ehefeofeaaeoepfcenebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheo
iaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaa
apaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaaheaaaaaa
abaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaheaaaaaaacaaaaaaaaaaaaaa
adaaaaaaadaaaaaaahaiaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklkl"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "UNITY_SHADER_DETAIL_LOW" }
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
Keywords { "UNITY_SHADER_DETAIL_LOW" }
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
Keywords { "UNITY_SHADER_DETAIL_LOW" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"ps_4_0
eefiecedfbboikkbfmamddkehelidlgedhchgpglabaaaaaapiabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefccaabaaaaeaaaaaaaeiaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagcbaaaaddcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
acaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaacaaaaaaeghobaaaabaaaaaa
aagabaaaabaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaa
aaaaaaebdiaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaahhccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaa
dgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "UNITY_SHADER_DETAIL_LOW" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [unity_Lightmap] 2D 1
"ps_4_0_level_9_3
eefiecedgedhabjdcalkoobeelpbkmbgjiceejnaabaaaaaanmacaaaaaeaaaaaa
daaaaaaabaabaaaadiacaaaakiacaaaaebgpgodjniaaaaaaniaaaaaaaaacpppp
kmaaaaaacmaaaaaaaaaacmaaaaaacmaaaaaacmaaacaaceaaaaaacmaaaaaaaaaa
abababaaabacppppfbaaaaafaaaaapkaaaaaaaebaaaaaaaaaaaaaaaaaaaaaaaa
bpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaaiaabaaadlabpaaaaacaaaaaaja
aaaiapkabpaaaaacaaaaaajaabaiapkaecaaaaadaaaacpiaaaaaoelaaaaioeka
ecaaaaadabaacpiaabaaoelaabaioekaafaaaaadabaaciiaabaappiaaaaaaaka
afaaaaadabaachiaabaaoeiaabaappiaafaaaaadaaaachiaaaaaoeiaabaaoeia
abaaaaacaaaicpiaaaaaoeiappppaaaafdeieefccaabaaaaeaaaaaaaeiaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagcbaaaaddcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
acaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaacaaaaaaeghobaaaabaaaaaa
aagabaaaabaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaa
aaaaaaebdiaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaahhccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaa
dgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaadoaaaaabejfdeheogiaaaaaa
adaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
fmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapadaaaafmaaaaaaabaaaaaa
aaaaaaaaadaaaaaaacaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaa
aaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
SubProgram "opengl " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
Vector 0 [_SpecularStrength]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 12 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MAD R1.xyz, R0, fragment.texcoord[0].z, -fragment.texcoord[0].w;
ADD R2.xyz, fragment.texcoord[2], R1;
DP4 R2.w, R0, c[0];
MOV result.color.w, R0;
TEX R1, fragment.texcoord[1], texture[2], 2D;
TEX R2.xyz, R2, texture[1], CUBE;
MUL R2.xyz, R2, R2.w;
ADD R0.xyz, R0, R2;
MUL R1.xyz, R1.w, R1;
MUL R0.xyz, R1, R0;
MUL result.color.xyz, R0, c[1].x;
END
# 12 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
Vector 0 [_SpecularStrength]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
"ps_2_0
; 10 ALU, 3 TEX
dcl_2d s0
dcl_cube s1
dcl_2d s2
def c1, 8.00000000, 0, 0, 0
dcl t0
dcl t1.xy
dcl t2.xyz
texld r2, t0, s0
mad r0.xyz, r2, t0.z, -t0.w
add r1.xyz, t2, r0
dp4 r3.x, r2, c0
texld r0, t1, s2
texld r1, r1, s1
mul_pp r0.xyz, r0.w, r0
mul r1.xyz, r1, r3.x
add_pp r1.xyz, r2, r1
mul_pp r0.xyz, r0, r1
mov_pp r0.w, r2
mul_pp r0.xyz, r0, c1.x
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
ConstBuffer "$Globals" 80
Vector 48 [_SpecularStrength]
BindCB  "$Globals" 0
"ps_4_0
eefiecedgndfgpgdifjibdegmdefehhhdoldklkaabaaaaaapeacaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaahahaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcaeacaaaaeaaaaaaaibaaaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafidaaaae
aahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaagcbaaaad
pcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaadhcbabaaaadaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaacaaaaaaeghobaaaacaaaaaaaagabaaaacaaaaaadiaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaebdiaaaaahhcaabaaaaaaaaaaa
egacbaaaaaaaaaaapgapbaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaakhcaabaaaacaaaaaa
egacbaaaabaaaaaakgbkbaaaabaaaaaapgbpbaiaebaaaaaaabaaaaaaaaaaaaah
hcaabaaaacaaaaaaegacbaaaacaaaaaaegbcbaaaadaaaaaaefaaaaajpcaabaaa
acaaaaaaegacbaaaacaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaabbaaaaai
icaabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaaegaobaaaabaaaaaadcaaaaaj
hcaabaaaabaaaaaaegacbaaaacaaaaaapgapbaaaaaaaaaaaegacbaaaabaaaaaa
dgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaadiaaaaahhccabaaaaaaaaaaa
egacbaaaaaaaaaaaegacbaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "UNITY_SHADER_DETAIL_MEDIUM" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
ConstBuffer "$Globals" 80
Vector 48 [_SpecularStrength]
BindCB  "$Globals" 0
"ps_4_0_level_9_3
eefiecedifaenfpjjlpngcpdhondnjjdlfoebpgpabaaaaaafiaeaaaaaeaaaaaa
daaaaaaajaabaaaajmadaaaaceaeaaaaebgpgodjfiabaaaafiabaaaaaaacpppp
bmabaaaadmaaaaaaabaadaaaaaaadmaaaaaadmaaadaaceaaaaaadmaaaaaaaaaa
abababaaacacacaaaaaaadaaabaaaaaaaaaaaaaaabacppppfbaaaaafabaaapka
aaaaaaebaaaaaaaaaaaaaaaaaaaaaaaabpaaaaacaaaaaaiaaaaaaplabpaaaaac
aaaaaaiaabaaadlabpaaaaacaaaaaaiaacaaahlabpaaaaacaaaaaajaaaaiapka
bpaaaaacaaaaaajiabaiapkabpaaaaacaaaaaajaacaiapkaecaaaaadaaaacpia
aaaaoelaaaaioekaaeaaaaaeabaaahiaaaaaoeiaaaaakklaaaaapplbacaaaaad
abaaahiaabaaoeiaacaaoelaecaaaaadacaacpiaabaaoelaacaioekaecaaaaad
abaaapiaabaaoeiaabaioekaafaaaaadabaaciiaacaappiaabaaaakaafaaaaad
acaachiaacaaoeiaabaappiaajaaaaadabaaaiiaaaaaoekaaaaaoeiaaeaaaaae
abaachiaabaaoeiaabaappiaaaaaoeiaafaaaaadaaaachiaacaaoeiaabaaoeia
abaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcaeacaaaaeaaaaaaaibaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaeaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaafidaaaaeaahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaa
ffffaaaagcbaaaadpcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaad
hcbabaaaadaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaefaaaaaj
pcaabaaaaaaaaaaaegbabaaaacaaaaaaeghobaaaacaaaaaaaagabaaaacaaaaaa
diaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaebdiaaaaah
hcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaak
hcaabaaaacaaaaaaegacbaaaabaaaaaakgbkbaaaabaaaaaapgbpbaiaebaaaaaa
abaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaaegbcbaaaadaaaaaa
efaaaaajpcaabaaaacaaaaaaegacbaaaacaaaaaaeghobaaaabaaaaaaaagabaaa
abaaaaaabbaaaaaiicaabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaaegaobaaa
abaaaaaadcaaaaajhcaabaaaabaaaaaaegacbaaaacaaaaaapgapbaaaaaaaaaaa
egacbaaaabaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaadiaaaaah
hccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaadoaaaaabejfdeheo
iaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaa
apaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapapaaaaheaaaaaa
abaaaaaaaaaaaaaaadaaaaaaacaaaaaaadadaaaaheaaaaaaacaaaaaaaaaaaaaa
adaaaaaaadaaaaaaahahaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
SubProgram "opengl " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
Vector 0 [_SpecularStrength]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 12 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MAD R1.xyz, R0, fragment.texcoord[0].z, -fragment.texcoord[0].w;
ADD R2.xyz, fragment.texcoord[2], R1;
DP4 R2.w, R0, c[0];
MOV result.color.w, R0;
TEX R1, fragment.texcoord[1], texture[2], 2D;
TEX R2.xyz, R2, texture[1], CUBE;
MUL R2.xyz, R2, R2.w;
ADD R0.xyz, R0, R2;
MUL R1.xyz, R1.w, R1;
MUL R0.xyz, R1, R0;
MUL result.color.xyz, R0, c[1].x;
END
# 12 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
Vector 0 [_SpecularStrength]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
"ps_2_0
; 10 ALU, 3 TEX
dcl_2d s0
dcl_cube s1
dcl_2d s2
def c1, 8.00000000, 0, 0, 0
dcl t0
dcl t1.xy
dcl t2.xyz
texld r2, t0, s0
mad r0.xyz, r2, t0.z, -t0.w
add r1.xyz, t2, r0
dp4 r3.x, r2, c0
texld r0, t1, s2
texld r1, r1, s1
mul_pp r0.xyz, r0.w, r0
mul r1.xyz, r1, r3.x
add_pp r1.xyz, r2, r1
mul_pp r0.xyz, r0, r1
mov_pp r0.w, r2
mul_pp r0.xyz, r0, c1.x
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
ConstBuffer "$Globals" 80
Vector 48 [_SpecularStrength]
BindCB  "$Globals" 0
"ps_4_0
eefiecedgndfgpgdifjibdegmdefehhhdoldklkaabaaaaaapeacaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaahahaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcaeacaaaaeaaaaaaaibaaaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafidaaaae
aahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaagcbaaaad
pcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaadhcbabaaaadaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaacaaaaaaeghobaaaacaaaaaaaagabaaaacaaaaaadiaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaebdiaaaaahhcaabaaaaaaaaaaa
egacbaaaaaaaaaaapgapbaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaakhcaabaaaacaaaaaa
egacbaaaabaaaaaakgbkbaaaabaaaaaapgbpbaiaebaaaaaaabaaaaaaaaaaaaah
hcaabaaaacaaaaaaegacbaaaacaaaaaaegbcbaaaadaaaaaaefaaaaajpcaabaaa
acaaaaaaegacbaaaacaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaabbaaaaai
icaabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaaegaobaaaabaaaaaadcaaaaaj
hcaabaaaabaaaaaaegacbaaaacaaaaaapgapbaaaaaaaaaaaegacbaaaabaaaaaa
dgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaadiaaaaahhccabaaaaaaaaaaa
egacbaaaaaaaaaaaegacbaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Keywords { "UNITY_SHADER_DETAIL_HIGH" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SpecCubeTex] CUBE 1
SetTexture 2 [unity_Lightmap] 2D 2
ConstBuffer "$Globals" 80
Vector 48 [_SpecularStrength]
BindCB  "$Globals" 0
"ps_4_0_level_9_3
eefiecedifaenfpjjlpngcpdhondnjjdlfoebpgpabaaaaaafiaeaaaaaeaaaaaa
daaaaaaajaabaaaajmadaaaaceaeaaaaebgpgodjfiabaaaafiabaaaaaaacpppp
bmabaaaadmaaaaaaabaadaaaaaaadmaaaaaadmaaadaaceaaaaaadmaaaaaaaaaa
abababaaacacacaaaaaaadaaabaaaaaaaaaaaaaaabacppppfbaaaaafabaaapka
aaaaaaebaaaaaaaaaaaaaaaaaaaaaaaabpaaaaacaaaaaaiaaaaaaplabpaaaaac
aaaaaaiaabaaadlabpaaaaacaaaaaaiaacaaahlabpaaaaacaaaaaajaaaaiapka
bpaaaaacaaaaaajiabaiapkabpaaaaacaaaaaajaacaiapkaecaaaaadaaaacpia
aaaaoelaaaaioekaaeaaaaaeabaaahiaaaaaoeiaaaaakklaaaaapplbacaaaaad
abaaahiaabaaoeiaacaaoelaecaaaaadacaacpiaabaaoelaacaioekaecaaaaad
abaaapiaabaaoeiaabaioekaafaaaaadabaaciiaacaappiaabaaaakaafaaaaad
acaachiaacaaoeiaabaappiaajaaaaadabaaaiiaaaaaoekaaaaaoeiaaeaaaaae
abaachiaabaaoeiaabaappiaaaaaoeiaafaaaaadaaaachiaacaaoeiaabaaoeia
abaaaaacaaaicpiaaaaaoeiappppaaaafdeieefcaeacaaaaeaaaaaaaibaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaeaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaafidaaaaeaahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaa
ffffaaaagcbaaaadpcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaad
hcbabaaaadaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaefaaaaaj
pcaabaaaaaaaaaaaegbabaaaacaaaaaaeghobaaaacaaaaaaaagabaaaacaaaaaa
diaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaebdiaaaaah
hcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaak
hcaabaaaacaaaaaaegacbaaaabaaaaaakgbkbaaaabaaaaaapgbpbaiaebaaaaaa
abaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaaacaaaaaaegbcbaaaadaaaaaa
efaaaaajpcaabaaaacaaaaaaegacbaaaacaaaaaaeghobaaaabaaaaaaaagabaaa
abaaaaaabbaaaaaiicaabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaaegaobaaa
abaaaaaadcaaaaajhcaabaaaabaaaaaaegacbaaaacaaaaaapgapbaaaaaaaaaaa
egacbaaaabaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaadiaaaaah
hccabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaabaaaaaadoaaaaabejfdeheo
iaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaa
apaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapapaaaaheaaaaaa
abaaaaaaaaaaaaaaadaaaaaaacaaaaaaadadaaaaheaaaaaaacaaaaaaaaaaaaaa
adaaaaaaadaaaaaaahahaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
}
 }
}
}