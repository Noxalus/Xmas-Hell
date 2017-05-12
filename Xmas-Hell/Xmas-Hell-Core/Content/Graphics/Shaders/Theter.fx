sampler TextureSampler : register(s0);

float2 uResolution;
float2 uMetaBalls[4];

float noise(float2 x) {
    return sin(1.5 * x.x) * sin(1.5 * x.y);
}

struct PixelShaderInput
{
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float4 gray = float4(0.1, 0.1, 0.1, 1.0);
    float4 sum = float4(0.0, 0.0, 0.0, 0.0);
    float radius = 0.5;

    // Add all the metaball data up
    for (int i = 0; i < 4; i++) {
        sum += lerp(sum, gray, radius / distance(input.TexCoord.xy, uMetaBalls[i]));
    }

    // Smooth out contrasts in metaballs
    float t = (sum.r + sum.g + sum.b) / 3.0;
    sum = lerp(gray, sum, t);
    sum = 1.0 - pow(abs(sum), float4(0.1, 0.1, 0.1, 0.1));

    // Add Vignette
    // float4 vignette = float4(0.0, 0.0, 0.0, 0.0);
    // vignette.a = clamp(1.0 - pow(abs((uResolution.y / 2.0) / (distance(input.TexCoord.xy, uResolution.xy / 2.0))), 0.33), 0., 1.);

    // // Add Noise
    // sum = float4(lerp(sum.rgb, vignette.rgb, vignette.a), 1.0);
    // sum *= noise(input.TexCoord.xy);

    return sum;
}

technique Theter {
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
