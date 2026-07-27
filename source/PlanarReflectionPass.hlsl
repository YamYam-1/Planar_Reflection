#ifndef PLANAL_REFLECTION_PASS_INCLUDED
#define PLANAL_REFLECTION_PASS_INCLUDED


float4 _BaseColor;
float _Metallic;
float4 _Specular;
float _Smoothness;
//float _Alpha;

struct Surface
{
    float3 albedo;
    float3 viewDir;
    float3 normal;
    float alpha;
    float metallic;
    float smoothness;
    
};

TEXTURE2D(_PlanarReflectionTexture);
SAMPLER(sampler_PlanarReflectionTexture);


struct Attributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 baseUV : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 positionWS : TEXCOORD0;
    float3 normalWS : VAR_NORMAL;
    float4 screenPos : TEXCOORD1;
    float2 baseUV : TEXCOORD2;
};


Varyings PlanarReflectionVertex(Attributes input)
{
    Varyings output;
    output.positionCS = TransformObjectToHClip(input.positionOS);
    output.positionWS = TransformObjectToWorld(input.positionOS);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
    output.screenPos = ComputeScreenPos(output.positionCS);
    output.baseUV = input.baseUV;
    return output;

}



float4 PlanarReflectionFragment(Varyings input) : SV_Target
{
    
    Surface surface;
    surface.albedo = _BaseColor.rgb;
    surface.viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
    surface.normal = normalize(input.normalWS);
    surface.alpha = _BaseColor.a;
    surface.metallic = _Metallic;
    surface.smoothness = _Smoothness;
    
    BRDFData brdf;
    Light mainLight = GetMainLight();
    
    
    InitializeBRDFData(surface.albedo, surface.metallic, float3(0.0, 0.0, 0.0), surface.smoothness, surface.alpha, brdf);
    
    float3 lighting = LightingPhysicallyBased(brdf, mainLight, surface.normal, surface.viewDir);
    
    float2 UV = input.screenPos.xy / input.screenPos.w;
    
    // 좌우 반대 
    UV.x = 1.0 - UV.x;
    
    float roughness = 1.0 - surface.smoothness;

    
    float4 reflectionTex = SAMPLE_TEXTURE2D(_PlanarReflectionTexture, sampler_PlanarReflectionTexture
    , UV);
    
    
    float reflectStrength = surface.smoothness * surface.metallic;
    float3 color = lighting + reflectionTex.rgb * reflectStrength;
    
    
    return float4(color, 1.0);

}

#endif
