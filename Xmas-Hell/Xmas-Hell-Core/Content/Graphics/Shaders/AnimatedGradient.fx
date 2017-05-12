sampler TextureSampler : register(s0);

float3 gradientPoint0Color;
float3 gradientPoint1Color;
float2 gradientPoint0Position;
float2 gradientPoint1Position;

struct PixelShaderInput
{
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float2 diffPosition = gradientPoint1Position - gradientPoint0Position;

    // Vector projection
    float s = dot(input.TexCoord.xy - gradientPoint0Position, diffPosition) / dot(diffPosition, diffPosition);
    // Saturate scaler
    s = clamp(s, 0.0, 1.0);
    // Gradient color interpolation
    float3 color = lerp(gradientPoint0Color, gradientPoint1Color, s);
    // sRGB gamma encode
    // color = pow(color, float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));

    return float4(color, 1.0);
}

technique AnimatedGradient {
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
