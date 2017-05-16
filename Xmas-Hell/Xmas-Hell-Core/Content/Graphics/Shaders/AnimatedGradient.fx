sampler TextureSampler : register(s0);

float4 uGradientPoint0Color;
float4 uGradientPoint1Color;
float uSpeed;
float uAmplitude;
float2 uResolution;
float uTime;

struct PixelShaderInput
{
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float amplitude = 1. / uAmplitude;

    float ratio = uResolution.x / uResolution.y;
    float2 coord = input.TexCoord.xy;
    // Make sure to have a round circle
    coord.y /= ratio;
    float d = distance(float2(0.5, 0.5 / ratio), coord) * (sin(uTime * uSpeed) + amplitude);
    return lerp(uGradientPoint0Color, uGradientPoint1Color, d);
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
