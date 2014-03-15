
// Camera settings.
float4x4 View;
float4x4 Projection;


float4 AmbientColor;

float3 DiffuseDirection;
float4 DiffuseColor;
float DiffuseIntensity;


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


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;

	float3 Normal : NORMAL0;

	// PER INSTANCE DATA
	float4x4 World : TEXCOORD5;
	float4 Colour : COLOR0;
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;


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

	output.Colour =  input.Colour;
	output.Normal = input.Normal;
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
    float4 norm = float4(input.Normal, 1.0);
    float4 diffuse = saturate(dot(-DiffuseDirection,norm));
 
    return color*AmbientColor*input.Colour+color*DiffuseIntensity*DiffuseColor*diffuse*input.Colour;
}



technique HardwareInstanceLow
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunctionLow();
    }
}

