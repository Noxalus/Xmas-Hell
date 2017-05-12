sampler TextureSampler : register(s0);

#define ANIMATE
#define OCTAVES 5

float uTime;

struct PixelShaderInput
{
    float2 TexCoord : TEXCOORD0;
};

float3 mod289(float3 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float2 mod289(float2 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float3 permute(float3 x)
{
  return mod289(((x*34.0)+1.0)*x);
}

// Simplex noise
// https://github.com/ashima/webgl-noise
// Copyright (C) 2011 Ashima Arts. All rights reserved.
float snoise(float2 v)
  {
  const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                      0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                     -0.577350269189626,  // -1.0 + 2.0 * C.x
                      0.024390243902439); // 1.0 / 41.0
// First corner
  float2 i  = floor(v + dot(v, C.yy) );
  float2 x0 = v -   i + dot(i, C.xx);

// Other corners
  float2 i1;
  //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
  //i1.y = 1.0 - i1.x;
  i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
  // x0 = x0 - 0.0 + 0.0 * C.xx ;
  // x1 = x0 - i1 + 1.0 * C.xx ;
  // x2 = x0 - 1.0 + 2.0 * C.xx ;
  float4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;

// Permutations
  i = mod289(i); // Avoid truncation effects in permutation
  float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 ))
    + i.x + float3(0.0, i1.x, 1.0 ));

  float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m ;
  m = m*m ;

// Gradients: 41 points uniformly over a line, mapped onto a diamond.
// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

  float3 x = 2.0 * frac(p * C.www) - 1.0;
  float3 h = abs(x) - 0.5;
  float3 ox = floor(x + 0.5);
  float3 a0 = x - ox;

// Normalise gradients implicitly by scaling m
// Approximation of: m *= inversesqrt( a0*a0 + h*h );
  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

// Compute final noise value at P
  float3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

float2 rand2(float2 p)
{
    p = float2(dot(p, float2(12.9898,78.233)), dot(p, float2(26.65125, 83.054543)));
    return frac(sin(p) * 43758.5453);
}

float rand(float2 p)
{
    return frac(sin(dot(p.xy ,float2(54.90898,18.233))) * 4337.5453);
}

float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Thanks to David Hoskins https://www.shadertoy.com/view/4djGRh
float stars(in float2 x, float numCells, float size, float br)
{
    float2 n = x * numCells;
    float2 f = floor(n);

  float d = 1.0e10;
    for (int i = -1; i <= 1; ++i)
    {
        for (int j = -1; j <= 1; ++j)
        {
            float2 g = f + float2(float(i), float(j));
      g = n - g - rand2(fmod(g, numCells)) + rand(g);
            // Control size
            g *= 1. / (numCells * size);
      d = min(d, dot(g, g));
        }
    }

    return br * (smoothstep(.95, 1., (1. - sqrt(d))));
}

// Simple fractal noise
// persistence - A multiplier that determines how quickly the amplitudes diminish for
// each successive octave.
// lacunarity - A multiplier that determines how quickly the frequency increases for
// each successive octave.
float fractalNoise(in float2 coord, in float persistence, in float lacunarity)
{
    float n = 0.;
    float frequency = 1.;
    float amplitude = 1.;
    for (int o = 0; o < OCTAVES; ++o)
    {
        n += amplitude * snoise(coord * frequency);
        amplitude *= persistence;
        frequency *= lacunarity;
    }
    return n;
}

float3 fractalNebula(in float2 coord, float3 color, float transparency)
{
    float n = fractalNoise(coord, .5, 2.);
    return n * color * transparency;
}

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float resolution = 1920;

    float2 coord = (input.TexCoord.xy / resolution);
    float2 starCoord = coord;
    starCoord.y += uTime/50.;

    coord.y += uTime/5.;

    float3 result = float3(0., 0.06, 0.22);

#ifdef ANIMATE
    float3 nebulaColor1 = hsv2rgb(float3(.5+.5*sin(uTime*.1), 0.5, .25));
    float3 nebulaColor2 = hsv2rgb(float3(.5+.5*sin(uTime*.21), 1., .25));
#else
    float3 nebulaColor1 = hsv2rgb(float3(.5, 0.5, .25));
    float3 nebulaColor2 = hsv2rgb(float3(.7, 1., .25));
#endif
    result += fractalNebula(coord + float2(.1, .1), nebulaColor1, 1.);
    result += fractalNebula(coord + float2(0., .2), nebulaColor2, .5);

    result += stars(starCoord, 4., 0.1, 2.) * float3(.74, .74, .74);
    result += stars(starCoord, 8., 0.05, 1.) * float3(.97, .74, .74);
    result += stars(starCoord, 16., 0.025, 0.5) * float3(.9, .9, .95);

    return float4(result, 1.);
}

technique StarBackground {
    pass P0 {
            PixelShader = compile ps_3_0 PixelShaderFunction();

    }
}
