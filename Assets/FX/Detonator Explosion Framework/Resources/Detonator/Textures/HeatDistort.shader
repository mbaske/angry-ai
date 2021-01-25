// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "HeatDistort" {
Properties {
	_BumpAmt  ("Distortion", range (0,128)) = 10
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
}

CGINCLUDE
#pragma fragmentoption ARB_precision_hint_fastest
#pragma fragmentoption ARB_fog_exp2
#include "UnityCG.cginc"

sampler2D _GrabTexture : register(s0);
float4 _GrabTexture_TexelSize;
sampler2D _BumpMap : register(s1);
sampler2D _MainTex : register(s2);

struct v2f {
	float4 vertex : POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	float2 uvmain : TEXCOORD2;
};

uniform float _BumpAmt;


half4 frag( v2f i ) : COLOR
{
	// calculate perturbed coordinates
	half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x & y without reconstructing the Z
	float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	
	//half4 col = tex2Dproj( _GrabTexture, i.uvgrab.xyw );
	//half4 col = tex2Dproj( _GrabTexture, i.uvgrab);
	//half4 tint = tex2D( _MainTex, i.uvmain );
	//return col;// * tint;
	//return tint;

	float2 uv = i.uvgrab.xy / i.uvgrab.w;
 
    half4 col = tex2D( _GrabTexture, uv );
    //half4 col = tex2Dproj( _GrabTexture, i.uvgrab.xyw );
 
    half4 tint = tex2D( _MainTex, i.uvmain );
    return col * tint;
}
ENDCG

Category {

	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent+100" "RenderType"="Opaque" }
	//Tags { "Queue"="Transparent" }


	SubShader {

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		GrabPass {							
			Name "BASE"
			Tags { "LightMode" = "Always" }
 		}
 		
 		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
 		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
};

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	o.uvbump = MultiplyUV( UNITY_MATRIX_TEXTURE1, v.texcoord );
	o.uvmain = MultiplyUV( UNITY_MATRIX_TEXTURE2, v.texcoord );
	return o;
}
ENDCG
		}
	}
}

}
