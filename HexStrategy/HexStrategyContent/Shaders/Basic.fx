// XNA 4.0 Shader Programming #2 - Diffuse light
 
// Matrix
float4x4 World;
float4x4 View;
float4x4 Projection;
 
// Light related
float4 AmbientColor;
float AmbientIntensity;
 
float3 DiffuseDirection;
float4 DiffuseColor;
float DiffuseIntensity;


texture2D ColorMap;
sampler2D ColorMapSampler = sampler_state
{
    Texture = <ColorMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};


// The input for the VertexShader
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};
 
// The output from the vertex shader, used for later processing
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal : TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
};
 
// The VertexShader.
VertexShaderOutput VertexShaderFunction(VertexShaderInput input,float3 Normal : NORMAL)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    float3 normal = normalize(mul(Normal, World));
    output.Normal = normal;
	output.TexCoord = input.TexCoord;
	
    return output;
}
 
// The Pixel Shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);
    float4 norm = float4(input.Normal, 1.0);
    float4 diffuse = saturate(dot(-DiffuseDirection,norm));
 
    return color*AmbientColor*AmbientIntensity+color*DiffuseIntensity*DiffuseColor*diffuse;
	
}
 
// Our Techinique
technique Technique1
{
    pass Pass1
    {

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}