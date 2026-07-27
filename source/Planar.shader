Shader "Custom/Planar"
{
	Properties
	{
		_BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Metallic("Metallic", Range(0, 1)) = 0
		_Specular("Specular", Color) = (0.0, 0.0, 0.0)
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
	}

	SubShader
	{
		Tags{
			"RenderPipeline" = "UniversalPipeline"
			}

		Pass
		{
			Tags{
				  "LightMode"="UniversalForward"
				}

			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#pragma vertex PlanarReflectionVertex
			#pragma fragment PlanarReflectionFragment
			#include "PlanarReflectionPass.hlsl"

			ENDHLSL
			}
	}

}
