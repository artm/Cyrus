Shader "Custom/Mirk" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
		AlphaThreshold("AlphaThreshold", Range(0,1)) = 0.5
		Fade("Fade", Range(0,1)) = 1
		NoiseScale("Noise Scale", float) = 1.0
		NoiseSpeed("Noise Speed", float) = 1.0
		Banding("Banding", float) = 1
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		LOD 200
		Zwrite off
		Lighting Off
		AlphaTest Greater [AlphaThreshold]

		// Don't use alpha for blending
		Blend One One
		//Blend SrcColor OneMinusSrcColor
		//Blend One OneMinusSrcColor

		// Nice effect, but needs different fading
		//Blend SrcColor DstColor


		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert

#include "noise3d.cginc"

uniform float NoiseScale, NoiseSpeed, Banding, Fade;

uniform float4 _Color;

struct Input {
	float3 worldPos;
	float3 worldNormal;
};

void surf (Input IN, inout SurfaceOutput o) {
	o.Alpha = frac( Banding * _Color.a * ( .5 + .5 * snoise( (IN.worldPos + normalize(IN.worldNormal) * _Time * NoiseSpeed) * NoiseScale)));
	o.Emission = _Color.rgb * Fade * o.Alpha;
}

ENDCG

	} 
	FallBack "Diffuse"
}
