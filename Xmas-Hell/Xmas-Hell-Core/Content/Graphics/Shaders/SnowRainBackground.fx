sampler TextureSampler : register(s0);

float3 color0 = float3(0.53, 0.8, 0.88);
float3 color1 = float3(0.24, 0.67, 0.82);

struct PixelShaderInput
{
    float2 TexCoord : TEXCOORD0;
};

float rand(float2 p)
{
    return frac(sin(dot(p.xy, float2(54.90898,18.233))) * 4337.5453);
}

float2 rand2(float2 p)
{
    p = float2(dot(p, float2(12.9898,78.233)), dot(p, float2(26.65125, 83.054543)));
    return frac(sin(p) * 43758.5453);
}

// Thanks to David Hoskins https://www.shadertoy.com/view/4djGRh
float stars(in float2 x, float numCells, float size, float br)
{
    float2 n = x * numCells;
    float2 f = floor(n);

    float d = 1.0e10;
    // for (int i = -1; i <= 1; ++i)
    // {
    //     for (int j = -1; j <= 1; ++j)
    //     {
    //         float2 g = f + float2(float(i), float(j));
    //         g = n - g - rand2(fmod(g, numCells)) + rand(g);
    //         // Control size
    //         g *= 1. / (numCells * size);
    //         d = min(d, dot(g, g));
    //     }
    // }

    return br * (smoothstep(.95, 1., (1. - sqrt(d))));
}

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{

    float2 A = float2(0.5, 0.);
    float2 B = float2(0.5, 1.0);
    float3 color0 = float3(0.53, 0.8, 0.88);
    float3 color1 = float3(0.24, 0.67, 0.82);

    float2 V = B - A;

    float s = dot(input.TexCoord.xy - A, V) / dot(V, V); // floattor projection.
    s = clamp(s, 0.0, 1.0); // Saturate scaler.
    float3 result = lerp(color0, color1, s); // Gradient color interpolation.

    float2 coord = input.TexCoord.xy / 500;
    // coord.y += iGlobalTime / 20.;

    result += stars(coord, 1., 0.1, 2.);
    // result += stars(coord, 8., 0.05, 1.) * float3(.97, .74, .74);
    // result += stars(coord, 16., 0.025, 0.5) * float3(.9, .9, .95);

    return float4(result, 1.);
}

technique SnowRainBackground {
    pass P0 {
        // PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
        // PixelShader = compile ps_3_0 PixelShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();

        // #if SM4
        //     PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
        // #elif SM3
        //     PixelShader = compile ps_3_0 PixelShaderFunction();
        // #else
        //     PixelShader = compile ps_2_0 PixelShaderFunction();
        // #endif
    }
}
