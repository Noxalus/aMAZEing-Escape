//-----------------------------------------
//	Normal Mapping and Parallax Mapping
//-----------------------------------------

//------------------
//--- Parameters ---
#define MaxBones 60
float4x4 Bones[MaxBones];

float4x4 World : WORLD;
float4x4 View : VIEW;
float4x4 Projection : PROJECTION;

float3 cameraPosition;
float2 scaleBias;

//light properties
float3 lightPosition;
float4 ambientLightColor;
float4 diffuseLightColor;
float4 specularLightColor;

//material properties
float specularPower = 16;
float specularIntensity = 1;


texture2D Texture;
texture2D NormalMap;
texture2D HeightMap;
texture2D SpecularMap;

sampler2D DiffuseTextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

sampler2D NormalTextureSampler = sampler_state
{
    Texture = <NormalMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

sampler2D HeightTextureSampler = sampler_state
{
    Texture = <HeightMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

sampler2D SpecularTextureSampler = sampler_state
{
    Texture = <SpecularMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

//--------------------
//--- VertexShader ---

struct VertexInput
{
	float3 position			: POSITION0;
	float2 texCoord			: TEXCOORD0;
  float3 normal				: NORMAL0;
  float3 binormal			: BINORMAL0;	
  float3 tangent			: TANGENT0;
  float4 BoneIndices	: BLENDINDICES0;
  float4 BoneWeights	: BLENDWEIGHT0;
};

struct VertexOutput
{
	float4 Position					: POSITION0;
	float2 texCoord					: TEXCOORD0;
  float3 directionToLight	: TEXCOORD1;
  float3 viewDirection		: TEXCOORD2;
  float3x3 tangentToWorld	: TEXCOORD3;
};

VertexOutput VertexShader( VertexInput input )
{
	VertexOutput output = (VertexOutput)0;

  // Blend between the weighted bone matrices.
  float4x4 skinTransform = 0;
  
  skinTransform += Bones[input.BoneIndices.x] * input.BoneWeights.x;
	skinTransform += Bones[input.BoneIndices.y] * input.BoneWeights.y;
	skinTransform += Bones[input.BoneIndices.z] * input.BoneWeights.z;
	skinTransform += Bones[input.BoneIndices.w] * input.BoneWeights.w;
	
  // Skin the vertex position.
  float4 position = mul(float4(input.position,1.0f), skinTransform);
  //float4 position = float4(input.position,1.0f);
    
	float4x4 wvp = mul(mul(World, View), Projection);

	output.Position = mul(position, wvp);
	float4 worldPosition = mul(position, World);

	output.directionToLight = lightPosition - worldPosition;
	output.viewDirection = cameraPosition - worldPosition;

	float3 tangent = mul(input.tangent, skinTransform);
	output.tangentToWorld[0] = mul(tangent,   World);
	//float3 b = cross(input.normal, input.tangent) * input.tangent.w;
	float3 binormal = mul(input.binormal, skinTransform);
	output.tangentToWorld[1] = mul(binormal,  World);
	float3 normal = mul(input.normal, skinTransform);
	output.tangentToWorld[2] = mul(normal,    World);

	output.texCoord = input.texCoord;

	return ( output );
}

//-------------------
//--- PixelShader ---

float4 PixelShader( VertexOutput input, uniform bool bParallax ) : COLOR0
{
	float2 texCoord;
	input.directionToLight = normalize(input.directionToLight);
	input.viewDirection = normalize(input.viewDirection);

	if (bParallax == true)
	{
			float height = tex2D(HeightTextureSampler, input.texCoord).r;
	    
			height = height * scaleBias.x + scaleBias.y;
			texCoord = input.texCoord + (height * input.viewDirection.xy);
	}
	else
	{
			texCoord = input.texCoord;
	}

	float3 normalFromMap = normalize(tex2D(NormalTextureSampler, texCoord).rgb * 2.0f - 1.0f);
	normalFromMap = mul(normalFromMap, input.tangentToWorld);
	normalFromMap = normalize(normalFromMap);

	//Diffuse
	float4 diffuseTexture = tex2D(DiffuseTextureSampler, texCoord);
	float nDotL = saturate( dot(normalFromMap, input.directionToLight));
	float4 diffuse = (diffuseLightColor * nDotL);

	//Specular
	float4 SpecularTexture = tex2D(SpecularTextureSampler, texCoord);
	float3 reflectedLight = reflect(-input.directionToLight, normalFromMap);
	float rDotV = saturate(dot(reflectedLight, input.viewDirection));
	float4 specular = (10 * SpecularTexture) * specularLightColor * pow(rDotV, specularPower);

	return ( (ambientLightColor + diffuse + specular) * diffuseTexture);
}

//------------------
//--- Techniques ---

Technique NormalMapping
{
    Pass Go
    {
        VertexShader = compile vs_2_0 VertexShader();
        PixelShader = compile ps_2_0 PixelShader(false);
    }
}

Technique ParallaxMapping
{
    Pass Go
    {
        VertexShader = compile vs_2_0 VertexShader();
        PixelShader = compile ps_2_0 PixelShader(true);
    }
}