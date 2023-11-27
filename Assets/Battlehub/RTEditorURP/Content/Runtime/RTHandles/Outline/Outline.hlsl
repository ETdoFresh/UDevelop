#ifndef BATTLEHUB_URP_OUTLINE_INCLUDED
#define BATTLEHUB_URP_OUTLINE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct VertexInput {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput {
	float4 clipPos : SV_POSITION;
	float2 uv : TEXCOORD0;
	UNITY_VERTEX_OUTPUT_STEREO
};

TEXTURE2D_X(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_TexelSize;

struct PrepassVertexInput {
	float4 pos : POSITION;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct PrepassVertexOutput {
	float4 clipPos : SV_POSITION;
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};

PrepassVertexOutput PrepassVertex(PrepassVertexInput input) {
	PrepassVertexOutput output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
	output.clipPos = TransformObjectToHClip(input.pos.xyz);
	return output;
}

float4 PrepassFragment(PrepassVertexOutput input) : SV_TARGET{

	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	return float4(1, 0, 0, 1);
}


VertexOutput BlurPassVertex(VertexInput input) {
	VertexOutput output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
	output.clipPos = TransformObjectToHClip(input.pos.xyz);
	output.uv = input.uv;
	return output;
}


float2 _BlurDirection;
float _OutlineStrength;

static const float4 kCurveWeights[9] = {
	float4(0.0204001988,0.0204001988,0.0204001988,0),
	float4(0.0577929595,0.0577929595,0.0577929595,0),
	float4(0.1215916882,0.1215916882,0.1215916882,0),
	float4(0.1899858519,0.1899858519,0.1899858519,0),
	float4(0.2204586031,0.2204586031,0.2204586031,1),
	float4(0.1899858519,0.1899858519,0.1899858519,0),
	float4(0.1215916882,0.1215916882,0.1215916882,0),
	float4(0.0577929595,0.0577929595,0.0577929595,0),
	float4(0.0204001988,0.0204001988,0.0204001988,0)
};

float4 Sample(float2 uv)
{
	float2 step = _MainTex_TexelSize.xy * _BlurDirection;
	uv = uv - step * 4;
	float4 col = 0;
	for (int tap = 0; tap < 9; ++tap)
	{
		col += SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv) * kCurveWeights[tap];
		uv += step;
	}
	return col * _OutlineStrength;
}

float4 BlurPassFragment(VertexOutput input) : SV_TARGET{

	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);
	return Sample(input.uv);
}


TEXTURE2D_X(_PrepassTex);
SAMPLER(sampler_PrepassTex);

TEXTURE2D_X(_BlurredTex);
SAMPLER(sampler_BlurredTex);

VertexOutput CompositePassVertex(VertexInput input) {
	VertexOutput output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
	output.clipPos = TransformObjectToHClip(input.pos.xyz);
	output.uv = input.uv;
	return output;
}

float4 _OutlineColor;

float4 CompositePassFragment(VertexOutput input) : SV_TARGET
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);
	float4 glow = max(0, SAMPLE_TEXTURE2D_X(_BlurredTex, sampler_BlurredTex, uv) - SAMPLE_TEXTURE2D_X(_PrepassTex, sampler_PrepassTex, uv));
	float4 col = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv);
	
	return lerp(col, _OutlineColor, glow.r);
}

#endif 