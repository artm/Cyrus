Shader "Custom/Mirk" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
		AlphaThreshold("AlphaThreshold", Range(0,1)) = 0.5
		Fade("Fade", Range(0,1)) = 1
		NoiseScale1("Noise Scale 1", float) = 1.0
		NoiseScale2("Noise Scale 2", float) = 2.0
		Banding("Banding", float) = 1
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		LOD 200
		Zwrite off
		Lighting Off
		AlphaTest Greater [AlphaThreshold]
		//Blend One OneMinusSrcAlpha
		//Blend SrcAlpha Zero
		Blend One One
		//Blend One SrcAlpha

		CGPROGRAM
		#pragma target 3.0
		//#pragma surface surf Lambert alphatest:AlphaThreshold
		//#pragma surface surf Foo alphatest:AlphaThreshold
		#pragma surface surf Lambert

#include "noise3d.cginc"

uniform float NoiseScale1, NoiseScale2, Banding, Fade;

uniform float4 _Color;

struct Input {
	float3 worldPos;
};

void surf (Input IN, inout SurfaceOutput o) {
	o.Alpha = frac( Banding * _Color.a * ( .5 + .25 * snoise(IN.worldPos * NoiseScale1)
											+ .25 * snoise(IN.worldPos * NoiseScale2) ));
	o.Emission = _Color.rgb * Fade * o.Alpha;
}

ENDCG

	} 
	FallBack "Diffuse"
}
