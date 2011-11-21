Shader "Custom/Mirk" {
	Properties {
		NoiseScale("Noise Scale", float) = 1.0
		NoiseSpeed("Noise Speed", float) = 0.1
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		LOD 200
		Zwrite off
		Cull off

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert alpha vertex:vert

#include "noise3d.cginc"

uniform float NoiseScale, NoiseSpeed;

struct Input {
	float3 worldPos;
};

void vert(inout appdata_full v) {

	// stick vertices to integer distances in view space...
	// v.vertex.xyz;

	//float3 viewpos = mul(UNITY_MATRIX_MV, v.vertex).xyz;
	//viewpos.z = int(viewpos.z);
	//v.vertex.xyz = mul( (float3x3)UNITY_MATRIX_IT_MV, viewpos);

	//v.vertex.xyz = floor(v.vertex.xyz);
}

void surf (Input IN, inout SurfaceOutput o) {
	o.Albedo = float3(1,1,1);
	o.Alpha = snoise(IN.worldPos * NoiseScale + _Time*NoiseSpeed);
}
ENDCG

	} 
	FallBack "Diffuse"
}
