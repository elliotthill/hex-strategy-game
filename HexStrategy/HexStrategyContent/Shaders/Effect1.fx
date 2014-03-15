
// Camera settings.
float4x4 View;
float4x4 Projection;


float3 LightPosition;
float4 DiffuseColor = float4(0.71, 0.59, 0.41, 0.7);
float LightRange = 55;


float3 LightPosition2;
float4 DiffuseColor2 = float4(0.12, 0.27, 0.41, 0.4f);
float LightRange2 = 55;

float3 LightPosition3;
float4 DiffuseColor3 = float4(0, 0, 0, 0);
float LightRange3 = 52;

float4 SpecularColor = float4(0.1, 0.1,0.1,0.1);
float Specularity = 1;
float3 EyePosition;

float Contrast = 1.15;
float Brightness = 0.1;


float Intensity = 0.06;
float Transparency = 1;

int FlipTangents = 0;

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

texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
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
	float3 Tangent : TANGENT0;
    float3 Binormal : BINORMAL0;

		// PER INSTANCE DATA
	float4x4 World : TEXCOORD5;
	float4 Colour : COLOR0;
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;

	//Per instance lighting
     float4 Colour : COLOR0;

	float3 View : TEXCOORD1;
	float3x3 WorldToTangentSpace : TEXCOORD2;
	float4 Light : TEXCOORD5;
	float4 Light2 : TEXCOORD6;
	float4 Light3 : TEXCOORD7;
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
    
	//Normal stuff
	float3x3 worldToTangentSpace;
	worldToTangentSpace[0] = mul(normalize(input.Tangent), instanceTransform);
	worldToTangentSpace[1] = mul(normalize(input.Binormal), instanceTransform);
	worldToTangentSpace[2] = mul(normalize(input.Normal), instanceTransform);

		if(FlipTangents == 1)
		{
	worldToTangentSpace[0] = mul(normalize(-input.Tangent), instanceTransform);
		}

	output.WorldToTangentSpace = worldToTangentSpace;

	// calculate distance to light in world space
	float3 L = LightPosition - worldPosition;
	float3 L2 = LightPosition2 - worldPosition;
	float3 L3 = LightPosition3 - worldPosition;

	// Transform light to tangent space
	output.Light.xyz = normalize(mul(worldToTangentSpace, L)); 	// L, light
	output.Light2.xyz = normalize(mul(worldToTangentSpace, L2)); 	// L2, light
	output.Light3.xyz = normalize(mul(worldToTangentSpace, L3)); 

	// Add range to the light, attenuation
	output.Light.w = saturate( 1 - dot(L / LightRange, L / LightRange));
	output.Light2.w = saturate( 1 - dot(L2 / LightRange2, L2 / LightRange2));
	output.Light3.w = saturate( 1 - dot(L3 / LightRange3, L3 / LightRange3)); 

	//output.View = normalize(float4(EyePosition,1.0) - worldPosition);
	output.View = mul(worldToTangentSpace,float4(EyePosition,1.0) - worldPosition);

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

	 //Get the color from the normal
	 float3 normalMap = (2.0 *(tex2D(NormalMapSampler, input.TexCoord))) - 0.5;
	 	//normalMap = normalize(mul(normalMap, input.WorldToTangentSpace));

    float3 ViewDir = normalize(input.View);	
	float3 LightDir = normalize(input.Light.xyz);	// L
    float3 LightDir2 = normalize(input.Light2.xyz);	// L2

		// diffuse
	float D = saturate(dot(normalMap, LightDir)); 
	float D2 = saturate(dot(normalMap, LightDir2)); 




	float4 light1final = ((color * D  * DiffuseColor ) * (input.Light.w));
	float4 light2final = ((color * D2  * DiffuseColor2) * (input.Light2.w));

	float4 finalColor = ((0.1 * color + light1final+ light2final+ (input.Colour*0.3)+ (color* Intensity + 0.1))* Contrast ) - Brightness-0.15;


	finalColor.a =input.Colour;
	return finalColor;
}



technique HardwareInstanceLow
{
    pass Pass1
    {
	       // AlphaBlendEnable = TRUE;
       // DestBlend = INVSRCALPHA;
       // SrcBlend = SRCALPHA;
        VertexShader = compile vs_3_0 HardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunctionLow();
    }
}

