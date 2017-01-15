// Pixel shader extracts the brighter areas of an image.
// This is the first step in applying a bloom postprocess.

sampler TextureSampler : register(s0);

float BloomThreshold;

struct VSOutput
{
    float4 position   : SV_Position;
    float4 color      : COLOR0;
    float2 texCoord   : TEXCOORD0;
};

float4 PixelShaderFunction(VSOutput input) : SV_Target0
{
    // Look up the original image color.
    float4 c = tex2D(TextureSampler, input.texCoord);

    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass Pass1
    {
        #if SM4
            PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
        #elif SM3
            PixelShader = compile ps_3_0 PixelShaderFunction();
        #else
            PixelShader = compile ps_2_0 PixelShaderFunction();
        #endif
    }
}