Shader "Mirk/Overlay" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 0)
		AlphaThreshold("Threshold", Range(0,1)) = 0
		NoiseScale("Noise Scale", float) = 1
		NoiseBands("Noise Bands", float) = 1

		Fade("Fade", Range(0,1)) = 1
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
		//Blend SrcColor One

		// Nice effects, but need different fading
		//Blend SrcColor DstColor
		//Blend DstColor SrcColor


		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert

#include "noise3d.cginc"

uniform float NoiseScale, NoiseBands, Fade;
uniform float4 _Color;
struct Input {
	float3 worldPos;
};
void surf (Input IN, inout SurfaceOutput o) {
	o.Alpha = frac( NoiseBands * (0.5 + 0.5 * snoise( IN.worldPos * NoiseScale)));
	o.Emission = _Color.rgb * Fade * o.Alpha;
}

ENDCG

	} 
	FallBack "Diffuse"
}
