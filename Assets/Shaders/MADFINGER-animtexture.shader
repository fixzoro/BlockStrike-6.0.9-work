Shader "MADFINGER/FX/Anim texture" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _NumTexTiles ("Num tex tiles", Vector) = (4,4,0,0)
 _ReplaySpeed ("Replay speed - FPS", Float) = 4
 _Color ("Color", Color) = (1,1,1,1)
}
SubShader { 
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
Vector 6 [_Color]
Vector 7 [_NumTexTiles]
Float 8 [_ReplaySpeed]
"!!ARBvp1.0
# 28 ALU
PARAM c[9] = { { 60, 1, 0 },
		state.matrix.mvp,
		program.local[5..8] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MUL R0.x, vertex.color.w, c[0];
ADD R0.x, R0, c[5].y;
MUL R5.x, R0, c[8];
FLR R4.x, R5;
RCP R3.xz, c[7].x;
ADD R0.x, R4, c[0].y;
MUL R0.y, R0.x, R3.x;
FLR R0.y, R0;
MOV R4.zw, R0.xyxy;
MUL R0.x, R4, R3;
FLR R4.y, R0.x;
RCP R3.yw, c[7].y;
MUL R0, R4, R3;
ABS R0, R0;
ABS R1.xy, c[7];
FRC R0, R0;
MUL R0, R0, R1.xyxy;
SLT R2, R4, c[0].z;
ADD R1, -R0, -R0;
MAD R0, R1, R2, R0;
ADD R0, vertex.texcoord[0].xyxy, R0;
MUL result.texcoord[0], R0, R3.xyxy;
MUL result.color.xyz, vertex.color, c[6];
ADD result.color.w, -R4.x, R5.x;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 28 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Time]
Vector 5 [_Color]
Vector 6 [_NumTexTiles]
Float 7 [_ReplaySpeed]
"vs_2_0
; 41 ALU
def c8, 60.00000000, 1.00000000, 0.00000000, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
mul r0.x, v2.w, c8
add r0.x, r0, c4.y
mul r0.x, r0, c7
frc r4.x, r0
add r0.x, r0, -r4
rcp r3.xz, c6.x
add r0.z, r0.x, c8.y
mul r0.y, r0.z, r3.x
frc r0.w, r0.y
add r0.w, r0.y, -r0
mul r0.y, r0.x, r3.x
frc r1.x, r0.y
add r0.y, r0, -r1.x
rcp r3.yw, c6.y
mul r1, r0, r3
slt r0, r0, c8.z
abs r1, r1
max r0, -r0, r0
slt r0, c8.z, r0
add r2, -r0, c8.y
frc r1, r1
abs r3.zw, c6.xyxy
mul r1, r1, r3.zwzw
mul r2, r1, r2
mad r0, r0, -r1, r2
add r0, v1.xyxy, r0
mul oT0, r0, r3.xyxy
mul oD0.xyz, v2, c5
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
mov oD0.w, r4.x
"
}
SubProgram "d3d11 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 64
Vector 16 [_Color]
Vector 32 [_NumTexTiles]
Float 48 [_ReplaySpeed]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0
eefiecedoopbhihglbgpcpfmphcpkpakigfclgoaabaaaaaanaaeaaaaadaaaaaa
cmaaaaaapeaaaaaagiabaaaaejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogmaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaagfaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaedepemepfcaaklfdeieefcgaadaaaaeaaaabaaniaaaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaa
acaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaa
fpaaaaadpcbabaaaafaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaad
pccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacadaaaaaadiaaaaai
pcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaa
adaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakbcaabaaaaaaaaaaa
dkbabaaaafaaaaaaabeaaaaaaaaahaecbkiacaaaabaaaaaaaaaaaaaadiaaaaai
ccaabaaaaaaaaaaaakaabaaaaaaaaaaaakiacaaaaaaaaaaaadaaaaaaebaaaaaf
bcaabaaaabaaaaaabkaabaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaaakaabaaa
abaaaaaaabeaaaaaaaaaiadpaoaaaaaiccaabaaaaaaaaaaabkaabaaaaaaaaaaa
akiacaaaaaaaaaaaacaaaaaaebaaaaaficaabaaaabaaaaaabkaabaaaaaaaaaaa
aoaaaaaiccaabaaaaaaaaaaaakaabaaaabaaaaaaakiacaaaaaaaaaaaacaaaaaa
ebaaaaafccaabaaaabaaaaaabkaabaaaaaaaaaaaaaaaaaahecaabaaaabaaaaaa
akaabaaaabaaaaaaabeaaaaaaaaaiadpaoaaaaaipcaabaaaacaaaaaaegaobaaa
abaaaaaaegiecaaaaaaaaaaaacaaaaaadcaaaaaliccabaaaacaaaaaaakaabaaa
aaaaaaaaakiacaaaaaaaaaaaadaaaaaaakaabaiaebaaaaaaabaaaaaabnaaaaai
pcaabaaaaaaaaaaaegaobaaaacaaaaaaegaobaiaebaaaaaaacaaaaaabkaaaaag
pcaabaaaabaaaaaaegaobaiaibaaaaaaacaaaaaadhaaaaakpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaegaobaiaebaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegiecaaaaaaaaaaaacaaaaaaegbebaaa
adaaaaaaaoaaaaalpcaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpegiecaaaaaaaaaaaacaaaaaadiaaaaahpccabaaaabaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaadiaaaaaihccabaaaacaaaaaaegbcbaaaafaaaaaa
egiccaaaaaaaaaaaabaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
ConstBuffer "$Globals" 64
Vector 16 [_Color]
Vector 32 [_NumTexTiles]
Float 48 [_ReplaySpeed]
ConstBuffer "UnityPerCamera" 128
Vector 0 [_Time]
ConstBuffer "UnityPerDraw" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  "$Globals" 0
BindCB  "UnityPerCamera" 1
BindCB  "UnityPerDraw" 2
"vs_4_0_level_9_3
eefiecednkgokaopjmpbhajbmepiccgciaagolnhabaaaaaaeiahaaaaaeaaaaaa
daaaaaaakeacaaaaamagaaaaneagaaaaebgpgodjgmacaaaagmacaaaaaaacpopp
caacaaaaemaaaaaaadaaceaaaaaaeiaaaaaaeiaaaaaaceaaabaaeiaaaaaaabaa
adaaabaaaaaaaaaaabaaaaaaabaaaeaaaaaaaaaaacaaaaaaaeaaafaaaaaaaaaa
aaaaaaaaabacpoppfbaaaaafajaaapkaaaaahaecaaaaiadpaaaaaaaaaaaaaaaa
bpaaaaacafaaaaiaaaaaapjabpaaaaacafaaadiaadaaapjabpaaaaacafaaafia
afaaapjaabaaaaacaaaaabiaajaaaakaaeaaaaaeaaaaabiaafaappjaaaaaaaia
aeaaffkaafaaaaadaaaaaciaaaaaaaiaadaaaakabdaaaaacaaaaaeiaaaaaffia
acaaaaadabaaabiaaaaakkibaaaaffiaaeaaaaaeabaaaioaaaaaaaiaadaaaaka
abaaaaibacaaaaadabaaaeiaabaaaaiaajaaffkaacaaaaadaaaaabiaabaaaaia
ajaaffkaagaaaaacacaaafiaacaaaakaafaaaaadaaaaabiaaaaaaaiaacaakkia
bdaaaaacaaaaaciaaaaaaaiaacaaaaadabaaaiiaaaaaffibaaaaaaiaafaaaaad
aaaaabiaabaaaaiaacaakkiabdaaaaacaaaaaciaaaaaaaiaacaaaaadabaaacia
aaaaffibaaaaaaiaagaaaaacacaaakiaacaaffkaafaaaaadaaaaapiaabaaoeia
acaaoeiacdaaaaacabaaapiaaaaaoeiaanaaaaadaaaaapiaaaaaoeiaaaaaoeib
bdaaaaacabaaapiaabaaoeiabcaaaaaeadaaapiaaaaaoeiaabaaoeiaabaaoeib
aeaaaaaeaaaaapiaadaaoeiaacaaeekaadaaeejaafaaaaadaaaaapoaacaaoeia
aaaaoeiaafaaaaadabaaahoaafaaoejaabaaoekaafaaaaadaaaaapiaaaaaffja
agaaoekaaeaaaaaeaaaaapiaafaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapia
ahaaoekaaaaakkjaaaaaoeiaaeaaaaaeaaaaapiaaiaaoekaaaaappjaaaaaoeia
aeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeia
ppppaaaafdeieefcgaadaaaaeaaaabaaniaaaaaafjaaaaaeegiocaaaaaaaaaaa
aeaaaaaafjaaaaaeegiocaaaabaaaaaaabaaaaaafjaaaaaeegiocaaaacaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaadaaaaaafpaaaaad
pcbabaaaafaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagiaaaaacadaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaacaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaacaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaacaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaacaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakbcaabaaaaaaaaaaadkbabaaa
afaaaaaaabeaaaaaaaaahaecbkiacaaaabaaaaaaaaaaaaaadiaaaaaiccaabaaa
aaaaaaaaakaabaaaaaaaaaaaakiacaaaaaaaaaaaadaaaaaaebaaaaafbcaabaaa
abaaaaaabkaabaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaaakaabaaaabaaaaaa
abeaaaaaaaaaiadpaoaaaaaiccaabaaaaaaaaaaabkaabaaaaaaaaaaaakiacaaa
aaaaaaaaacaaaaaaebaaaaaficaabaaaabaaaaaabkaabaaaaaaaaaaaaoaaaaai
ccaabaaaaaaaaaaaakaabaaaabaaaaaaakiacaaaaaaaaaaaacaaaaaaebaaaaaf
ccaabaaaabaaaaaabkaabaaaaaaaaaaaaaaaaaahecaabaaaabaaaaaaakaabaaa
abaaaaaaabeaaaaaaaaaiadpaoaaaaaipcaabaaaacaaaaaaegaobaaaabaaaaaa
egiecaaaaaaaaaaaacaaaaaadcaaaaaliccabaaaacaaaaaaakaabaaaaaaaaaaa
akiacaaaaaaaaaaaadaaaaaaakaabaiaebaaaaaaabaaaaaabnaaaaaipcaabaaa
aaaaaaaaegaobaaaacaaaaaaegaobaiaebaaaaaaacaaaaaabkaaaaagpcaabaaa
abaaaaaaegaobaiaibaaaaaaacaaaaaadhaaaaakpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaaegaobaiaebaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegaobaaaaaaaaaaaegiecaaaaaaaaaaaacaaaaaaegbebaaaadaaaaaa
aoaaaaalpcaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadp
egiecaaaaaaaaaaaacaaaaaadiaaaaahpccabaaaabaaaaaaegaobaaaaaaaaaaa
egaobaaaabaaaaaadiaaaaaihccabaaaacaaaaaaegbcbaaaafaaaaaaegiccaaa
aaaaaaaaabaaaaaadoaaaaabejfdeheomaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaakbaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaakjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahaaaaaalaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaalaaaaaaa
abaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaaljaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafaepfdejfeejepeoaafeebeoehefeofeaaeoepfc
enebemaafeeffiedepepfceeaaedepemepfcaaklepfdeheogmaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaagfaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaedepemepfcaakl"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D 0
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 5 ALU, 2 TEX
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0].zwzw, texture[0], 2D;
ADD R1, R1, -R0;
MAD R0, fragment.color.primary.w, R1, R0;
MUL result.color, R0, fragment.color.primary;
END
# 5 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D 0
"ps_2_0
; 6 ALU, 2 TEX
dcl_2d s0
dcl t0
dcl v0
texld r1, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
texld r0, r0, s0
add r0, r0, -r1
mad r0, v0.w, r0, r1
mul r0, r0, v0
mov_pp oC0, r0
"
}
SubProgram "d3d11 " {
SetTexture 0 [_MainTex] 2D 0
"ps_4_0
eefiecedkoofbgfkhamegdcohnoobfhnhfklblgfabaaaaaaniabaaaaadaaaaaa
cmaaaaaakaaaaaaaneaaaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaagfaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaedepemepfcaakl
epfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcpmaaaaaaeaaaaaaa
dpaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
gcbaaaadpcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaipcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaiaebaaaaaaabaaaaaadcaaaaajpcaabaaaaaaaaaaa
pgbpbaaaacaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaa
aaaaaaaaegaobaaaaaaaaaaaegbobaaaacaaaaaadoaaaaab"
}
SubProgram "d3d11_9x " {
SetTexture 0 [_MainTex] 2D 0
"ps_4_0_level_9_3
eefiecedmmlniekgdleaaajppemaliciabpikjncabaaaaaajeacaaaaaeaaaaaa
daaaaaaaoiaaaaaaomabaaaagaacaaaaebgpgodjlaaaaaaalaaaaaaaaaacpppp
iiaaaaaaciaaaaaaaaaaciaaaaaaciaaaaaaciaaabaaceaaaaaaciaaaaaaaaaa
abacppppbpaaaaacaaaaaaiaaaaaaplabpaaaaacaaaaaaiaabaaaplabpaaaaac
aaaaaajaaaaiapkaabaaaaacaaaaadiaaaaaoolaecaaaaadabaaapiaaaaaoela
aaaioekaecaaaaadaaaaapiaaaaaoeiaaaaioekabcaaaaaeacaaapiaabaappla
aaaaoeiaabaaoeiaafaaaaadaaaacpiaacaaoeiaabaaoelaabaaaaacaaaicpia
aaaaoeiappppaaaafdeieefcpmaaaaaaeaaaaaaadpaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaadpcbabaaaabaaaaaa
gcbaaaadpcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaa
efaaaaajpcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaaaaaaaaipcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaia
ebaaaaaaabaaaaaadcaaaaajpcaabaaaaaaaaaaapgbpbaaaacaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaadiaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaa
egbobaaaacaaaaaadoaaaaabejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaagfaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaedepemepfcaakl
epfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl"
}
}
 }
}
}