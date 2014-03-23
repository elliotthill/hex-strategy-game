
// Camera settings.
float4x4 View;
float4x4 Projection;


float4 AmbientColor;
float3 EyePosition;

float3 DiffuseDirection;
float4 DiffuseColor;
float DiffuseIntensity;
float4 SpecularColor;
float contrast = 1.1f;
float brightness = -0.01f;
float opacity = -1.0f;

texture2D ColorMap;
sampler2D Sampler = sampler_state
{
    Texture = <ColorMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};

// Define our normal map
texture NormalMap;
// Create a sampler for the NormalMap texture using leanear filtering and clamping
sampler NormalMapSampler = sampler_state
{
   Texture = <NormalMap>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;   
   AddressU  = Clamp;
   AddressV  = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;

	float3 Normal : NORMAL0;
	float3 Binormal : BINORMAL0;
	float3 Tangent : TANGENT0;

	// PER INSTANCE DATA
	float4x4 World : TEXCOORD5;
	float4 Colour : COLOR0;
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
	
	float3 View : TEXCOORD1;
	float3x3 WorldToTangentSpace : TEXCOORD2;

	//Per instance lighting
    float4 Colour : COLOR0;
};


// Vertex shader helper function shared between the two techniques.
VertexShaderOutput VertexShaderCommon(VertexShaderInput input)
{
    VertexShaderOutput output;
	float4x4 instanceTransform = transpose( input.World);

    // Apply the world and camera matrices to compute the output position.
    float4 worldPosition = mul(input.Position, instanceTransform);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;

	output.WorldToTangentSpace[0] = mul(normalize(input.Tangent), input.World);
	output.WorldToTangentSpace[1] = mul(normalize(input.Binormal), input.World);
	output.WorldToTangentSpace[2] = mul(normalize(input.Normal), input.World);
	
	output.View = normalize(float4(EyePosition,1.0) - worldPosition);

	output.Colour =  input.Colour;
	
    return output;
}


// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
VertexShaderOutput HardwareInstancingVertexShader(VertexShaderInput input
                                                 )
{
    return VertexShaderCommon(input);
}


float4 PixelShaderFunctionLow(VertexShaderOutput input) : COLOR0
{

	 //Get the color from the sampler
	float4 color = tex2D(Sampler, input.TexCoord);
    float3 normalMap = 2.0 *(tex2D(NormalMapSampler, input.TexCoord)) - 1.0;
		normalMap = normalize(mul(normalMap, input.WorldToTangentSpace));
	float4 normal = float4(normalMap,1.0);

    float4 diffuse = saturate(dot(-DiffuseDirection,normal));
 	float4 reflect = normalize(2*diffuse*normal-float4(DiffuseDirection,1.0));
	float4 specular = pow(saturate(dot(reflect,input.View)),32);


    float4 clr = color*AmbientColor*input.Colour
	+color*DiffuseIntensity*DiffuseColor*diffuse*input.Colour
	+color * SpecularColor * specular;

	clr =  (clr + brightness) * contrast;


	if (opacity != -1.0f) {
		clr.a = opacity;
	} else {
		clr.a = color.a;
	}
	return clr;
}



technique HardwareInstanceLow
{
    pass Pass1
    {
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        VertexShader = compile vs_3_0 HardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunctionLow();
    }
}

