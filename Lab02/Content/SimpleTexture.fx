//Matrix
float4x4 World;
float4x4 View;
float4x4 Projection;

float3 offset;

texture MyTexture;

sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};

struct VertexPositionTexture {
	float4 Position: POSITION;
	float2 TextureCoordinate : TEXCOORD;
};

VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	VertexPositionTexture output;
	float4 worldPos = mul(input.Position, World); 
	float4 viewPos = mul(worldPos, View);
	float4 projPos = mul(viewPos, Projection);
	output.Position = projPos;
	output.TextureCoordinate = input.TextureCoordinate; // UV data through
	return output;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	float2 textureCoordinate = input.TextureCoordinate;
	return tex2D(mySampler, textureCoordinate);
}

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}
