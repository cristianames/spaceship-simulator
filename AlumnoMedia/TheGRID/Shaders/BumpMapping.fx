/*
* Shader utilizado para efecto de BumpMapping sobre un TgcMesh.
* Solo soporta TgcMesh con RenderType del tipo DIFFUSE_MAP
* Tiene una Technique de BumpMappingTechnique y una de PostProcesado
*/


/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura utilizada para BumpMapping
texture texNormalMap;
sampler2D normalMap = sampler_state
{
	Texture = (texNormalMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float screen_dx;					// tamaño de la pantalla en pixels
float screen_dy;

//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular

//Parametros de la Luz
float3 lightColor; //Color RGB de la luz
float4 lightPosition; //Posicion de la luz
float4 eyePosition; //Posicion de la camara
float lightIntensity; //Intensidad de la luz
float lightAttenuation; //Factor de atenuacion de la luz


//Intensidad de efecto Bump
float bumpiness;
const float3 BUMP_SMOOTH = { 0.5f, 0.5f, 1.0f };

//Factor de reflexion
float reflection;

//Texturas de Post Procesado de luces
texture luzSolarTarget;
sampler SolTarget = sampler_state
{
	Texture = <luzSolarTarget>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};
texture luzIzqTarget;
sampler IzqTarget = sampler_state
{
	Texture = <luzIzqTarget>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};
texture luzDerTarget;
sampler DerTarget = sampler_state
{
	Texture = <luzDerTarget>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};
texture luzFrontalTarget;
sampler FrontalTarget = sampler_state
{
	Texture = <luzFrontalTarget>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


/**************************************************************************************/
/* BumpMappingTechnique */
/**************************************************************************************/


//Input del Vertex Shader
struct VS_INPUT 
{
	float4 Position : POSITION0;
	float3 Normal :   NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
};

//Output del Vertex Shader
struct VS_OUTPUT 
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
    float3 WorldTangent	: TEXCOORD3;
    float3 WorldBinormal : TEXCOORD4;
	float3 LightVec	: TEXCOORD5;
	float3 HalfAngleVec	: TEXCOORD6;
};

//Vertex Shader
VS_OUTPUT vs_general(VS_INPUT input)
{
	VS_OUTPUT output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Las Coordenadas de textura quedan igual
	output.Texcoord = input.Texcoord;
	
	//Posicion pasada a World-Space
	output.WorldPosition = mul(input.Position, matWorld).xyz;
	
	//Pasar normal, tangent y binormal a World-Space
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    output.WorldTangent = mul(input.Tangent, matInverseTransposeWorld).xyz;
    output.WorldBinormal = mul(input.Binormal, matInverseTransposeWorld).xyz;
	
	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.LightVec = lightPosition.xyz - output.WorldPosition;
	
	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;
	
	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.HalfAngleVec = viewVector + output.LightVec;
	
	
	return output;
}


//Input del Pixel Shader
struct PS_INPUT 
{
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
    float3 WorldTangent	: TEXCOORD3;
    float3 WorldBinormal : TEXCOORD4;
	float3 LightVec	: TEXCOORD5;
	float3 HalfAngleVec	: TEXCOORD6;
};
	

float k_la = 0.3;							// luz ambiente global
float k_ld = 0.9;							// luz difusa
float k_ls = 0.8;							// luz specular
float fSpecularPower = 16.84;				// exponente de la luz specular

//Pixel Shader
float4 ps_general(PS_INPUT input) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular

	//Normalizar vectores
	float3 N = normalize(input.WorldNormal);
	float3 Pos = normalize(input.WorldPosition);

	float3 Tn = normalize(input.WorldTangent);
	float3 Bn = normalize(input.WorldBinormal);

	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
	float intensity = lightIntensity / distAtten;

	//Obtener texel de la textura
	float4 fvBaseColor = tex2D(diffuseMap, input.Texcoord);
	
	//Obtener normal de normalMap y ajustar rango de [0, 1] a [-1, 1]
	float3 bumpNormal = tex2D(normalMap, input.Texcoord).rgb;
	bumpNormal = (bumpNormal * 2.0f) - 1.0f;
	
	//Suavizar con bumpiness
	bumpNormal = lerp(BUMP_SMOOTH, bumpNormal, bumpiness);
	
	//Pasar de Tangent-Space a World-Space
	bumpNormal = N + bumpNormal.x * Tn + bumpNormal.y * Bn;
	bumpNormal = normalize(bumpNormal);

	//Obtenemos los valores de las constantes de luz
	float3 LD = normalize(lightPosition - float3(Pos.x, Pos.y, Pos.z));
		ld += saturate(dot(bumpNormal, LD))*k_ld;
	float3 D = normalize(float3(Pos.x, Pos.y, Pos.z) - lightPosition);
		float ks = saturate(dot(reflect(LD, bumpNormal), D));
	ks = pow(ks, fSpecularPower);
	le += ks*k_ls;

	//Calculamos los componentes de las luces
	float3 ambientLight = intensity * lightColor * materialAmbientColor * k_la;
	float3 diffuseLight = intensity * lightColor * materialDiffuseColor.rgb * max(0.0, ld); //Controlamos que no de negativo
	float3 specularLight = (intensity * lightColor * materialSpecularColor * pow(max(0.0, le), materialSpecularExp));

	float4 finalColor = float4((materialEmissiveColor + ambientLight + diffuseLight) * fvBaseColor + specularLight, materialDiffuseColor.a);

	return finalColor;
}

///////////////////////////////Efecto Join//////////////////////////////////////////////////////////

//Deben Sumar 100% Para que tenga sentido
float Ksol = float(0.7);
float Kder = float(0.1);
float Kizq = float(0.1);
float Kfront = float(0.1);

void vs_join(float4 vPos : POSITION, float2 vTex : TEXCOORD0, out float4 oPos : POSITION, out float2 oScreenPos : TEXCOORD0)
{
	oPos = vPos;
	oScreenPos = vTex;
	oPos.w = 1;
}

//Pixel Shader
void ps_join(in float2 Texcoord: TEXCOORD0, out float4 Color : COLOR)
{	//Obtener los textels
	float4 sol = tex2D(SolTarget, Texcoord);
	float4 izq = tex2D(IzqTarget, Texcoord);
	float4 der = tex2D(DerTarget, Texcoord);
	float4 front = tex2D(FrontalTarget, Texcoord);
	//mergeamos los textels
	Color = (sol*Ksol)+(izq*Kizq)+(der*Kder)+(front*Kfront);
}

//////////////////////////////////////////////////////////////////////////////////////////////////

/*
* Technique de BumpMapping
*/
technique BumpMappingTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_general();
	  PixelShader = compile ps_2_0 ps_general();
   }

}
/*
* Technique de PosProcesado
*/

technique JoinBumpsTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_join();
		PixelShader = compile ps_2_0 ps_join();
	}
}
