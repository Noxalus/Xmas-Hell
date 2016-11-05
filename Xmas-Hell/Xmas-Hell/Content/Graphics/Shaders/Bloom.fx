//BLOOM.FX

//Original image (not bloomed)
sampler TextureSampler : register(s0); 

//Bloomed image
sampler BloomSampler : register(s1);

//Power: sampling range multiplier (used in gaussian blur filter)
float BlurPower;

//Intensity of the base render and the bloom render
float BaseIntensity, BloomIntensity;

//Saturation scale
float BaseSaturation, BloomSaturation;

//Range of offsets to sample from
static const float2 offsets[12] = {
   -0.326212, -0.405805,
   -0.840144, -0.073580,
   -0.695914,  0.457137,
   -0.203345,  0.620716,
    0.962340, -0.194983,
    0.473434, -0.480026,
    0.519456,  0.767022,
    0.185461, -0.893124,
    0.507431,  0.064425,
    0.896420,  0.412458,
   -0.321940, -0.932615,
   -0.791559, -0.597705,
};

struct PixelShaderInput 
{ 
    float2 TexCoord : TEXCOORD0; 
}; 

float4 AdjustSaturation(float4 color, float saturation) {
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color.rgb, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}

float4 bloomEffect(PixelShaderInput Input) : COLOR0 { 

	//Get base color (from render target)
    float4 original = tex2D(TextureSampler, Input.TexCoord);
    
    //Compute bloom color after gaussian filtering
    float4 sum = tex2D(BloomSampler, Input.TexCoord);
    for(int i = 0; i < 12; i++){
        sum += tex2D(BloomSampler, Input.TexCoord + BlurPower * offsets[i]);
    }
    sum /= 13;
    
    //Adjust intensity
    original = AdjustSaturation(original, BaseSaturation) * BaseIntensity;
    sum = AdjustSaturation(sum, BloomSaturation) * BloomIntensity;
    
    return sum + original;
}


technique Bloom { 
    pass P0{ 
		#if SM4
			PixelShader = compile ps_4_0_level_9_1 bloomEffect();
		#elif SM3
			PixelShader = compile ps_3_0 bloomEffect();
		#else
			PixelShader = compile ps_2_0 bloomEffect();
		#endif
    } 
}
