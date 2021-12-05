float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;

texture DecalMap;
texture EnvironmentMap;
float Reflectivity; //percentage of reflected color vs original color

sampler tsampler1 = sampler_state {
	texture = <DecalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

samplerCUBE SkyBoxSampler = sampler_state {
	texture = <EnvironmentMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput {
	float4 Position: POSITION0;
	float2 TexCoord: TEXCOORD0;
	float4 Normal: NORMAL0;
};

struct VertexShaderOutput {
	float4 Position: POSITION0;
	float2 TexCoord: TEXCOORD0;
	float3 R: TEXCOORD1;
};

VertexShaderOutput ReflectionVSFunction(VertexShaderInput input) {
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	float3 I = normalize(worldPosition.xyz - CameraPosition);
	output.R = reflect(I, N);
	//output.R = refract(I, N, 0.90);
	return output;
}

float4 ReflectionPSFunction(VertexShaderOutput input): COLOR0{
	float4 reflectedColor = texCUBE(SkyBoxSampler, input.R);
	float4 decalColor = tex2D(tsampler1, input.TexCoord);
	return lerp(decalColor, reflectedColor, Reflectivity);
}

technique Reflection {
	pass Pass1 {
		VertexShader = compile vs_4_0 ReflectionVSFunction();
		PixelShader = compile ps_4_0 ReflectionPSFunction();
	}
}
