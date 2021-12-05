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
	//Color Drawing 
	//float4 color = input.Color;
	//if (color.r %0.1 < 0.05f) 
	//	return float4(1, 1, 1, 1);
	//else 
	//	return color;

	//Image drawing 
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
