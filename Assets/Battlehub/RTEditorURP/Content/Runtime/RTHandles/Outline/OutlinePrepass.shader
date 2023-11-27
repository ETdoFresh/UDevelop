Shader "Battlehub/URP/OutlinePrepass"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			HLSLPROGRAM

			#include "Outline.hlsl"
			
			#pragma multi_compile_instancing
			#pragma vertex PrepassVertex
			#pragma fragment PrepassFragment


			ENDHLSL
		}
	}
}
