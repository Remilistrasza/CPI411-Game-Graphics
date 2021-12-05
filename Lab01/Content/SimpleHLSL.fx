texture MyTexture;
sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};

struct VertexPositionTexture {
	float4 Position: POSITION;
	float2 TextureCoordinate : TEXCOORD;
};

struct VertexPositionColor {
	float4 Position: POSITION;
	float4 Color: COLOR;
};

VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	return input;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	//float4 color = input.Color;
	//color += 0.3f;
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
