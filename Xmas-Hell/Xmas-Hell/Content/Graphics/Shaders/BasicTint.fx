sampler TextureSampler : register(s0);

float3 tintColor;

struct PixelShaderInput
{
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TexCoord);
    float3 tint = tintColor * color.a;

    return float4(tint, 1.0f) * color;
}

technique BasicTint {
    pass P0 {
        #if SM4
            PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
        #elif SM3
            PixelShader = compile ps_3_0 PixelShaderFunction();
        #else
            PixelShader = compile ps_2_0 PixelShaderFunction();
        #endif
    }
}
