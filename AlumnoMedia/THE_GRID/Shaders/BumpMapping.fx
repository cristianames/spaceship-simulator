// ------------------------------------------------------------------------------------
// Shader de BumpMapping e iluminacion dinamica
// Solo soporta TgcMesh con RenderType del tipo DIFFUSE_MAP
// Techniques: de BumpMappingTechnique, Spotlight, Join (PostProcesado Unificador)
// -------------------------------------------------------------------------------------


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

float screen_dx; // tamaño de la pantalla en pixels
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

//Parametros de Spot
float3 spotLightDir; //Direccion del cono de luz
float spotLightAngleCos; //Angulo de apertura del cono de luz (en radianes)
float spotLightExponent; //Exponente de atenuacion dentro del cono de luz

/**************************************************************************************/
/* Estructuras! */
/**************************************************************************************/

//Input del Vertex Shader del Bump
struct VS_INPUT 
{
	float4 Position : POSITION0;
	float3 Normal :   NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
};

//Output del Vertex Shader del Bump
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

//Input del Pixel Shader del Bump
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

//Input del Vertex Shader para Spotlight
struct VS_INPUT_Spotlight
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
};

//Output del Vertex Shader para Spotlight
struct VS_OUTPUT_Spotlight
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float3 WorldPosition : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 LightVec	: TEXCOORD2;
	float3 HalfAngleVec	: TEXCOORD3;
};

//Input del Pixel Shader para Spotlight
struct PS_INPUT_Spotlight
{
	float4 Color : COLOR0;
	float3 WorldPosition : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 LightVec	: TEXCOORD2;
	float3 HalfAngleVec	: TEXCOORD3;
};


/**************************************************************************************/
/* BumpMappingTechnique */
/**************************************************************************************/
float k_la = 0.3;							// luz ambiente global
float k_ld = 0.9;							// luz difusa
float k_ls = 0.8;							// luz specular
float fSpecularPower = 16.84;				// exponente de la luz specular

//Vertex Shader
VS_OUTPUT vs_bump(VS_INPUT input)
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

//Pixel Shader
float4 ps_bump(PS_INPUT input) : COLOR0
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

/*
* Technique de BumpMapping
*/
technique BumpMappingTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_bump();
		PixelShader = compile ps_2_0 ps_bump();
	}

}

/**************************************************************************************/
/* Spotlight */
/**************************************************************************************/

//Vertex Shader
VS_OUTPUT_Spotlight vs_Spotlight(VS_INPUT_Spotlight input)
{
	VS_OUTPUT_Spotlight output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.LightVec = lightPosition.xyz - output.WorldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;

		//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
		output.HalfAngleVec = viewVector + output.LightVec;

	return output;
}

//Pixel Shader
float4 ps_Spotlight(PS_INPUT_Spotlight input) : COLOR0
{
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
		float3 Ln = normalize(input.LightVec);
		float3 Hn = normalize(input.HalfAngleVec);

		//Calcular atenuacion por distancia
		float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;

	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, Ln);
	spotAtten = (spotAtten > spotLightAngleCos)
		? pow(spotAtten, spotLightExponent)
		: 0.0;

	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
	float intensity = lightIntensity * spotAtten / distAtten;

	//Componente Ambient
	float3 ambientLight = intensity * lightColor * materialAmbientColor;

		//Componente Diffuse: N dot L
		float3 n_dot_l = dot(Nn, Ln);
		float3 diffuseLight = intensity * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l); //Controlamos que no de negativo
		//Componente Specular: (N dot H)^exp
		float3 n_dot_h = dot(Nn, Hn);
		n_dot_h = pow(n_dot_h, fSpecularPower);
	le += n_dot_h*k_ls;
	float3 specularLight = n_dot_l <= 0.0
		? float3(0.0, 0.0, 0.0)
		: (intensity * lightColor * materialSpecularColor * pow(max(0.0, le), materialSpecularExp));
	return float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * input.Color + specularLight, materialDiffuseColor.a);
}

/*
* Technique Spotlight
*/
technique Spotlight
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_Spotlight();
		PixelShader = compile ps_2_0 ps_Spotlight();
	}
}

/**************************************************************************************/
/* Join */
/**************************************************************************************/

float Ksol = float(2);
float Kalfa = float(5);
float Kder = float(0.8);
float Kizq = float(0.8);
float Kfront = float(0.8);

//Vertex Shader
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
		//Color = sol + saturate((sol*Ksol) + (izq*Kizq) + (der*Kder) + (front*Kfront));
		float4 sol_rel = sol*Ksol;
		float4 izq_rel = float4(izq.rgb, izq.a*Kalfa) * Kizq;
		float4 der_rel = float4(der.rgb, der.a*Kalfa) * Kder;
		float4 front_rel = float4(front.rgb, front.a*Kalfa) * Kfront;

	Color = sol + saturate(sol_rel + izq_rel + der_rel + front_rel);
}

/*
* Technique de Join (Post Procesado)
*/

technique Join
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_join();
		PixelShader = compile ps_2_0 ps_join();
	}
}



