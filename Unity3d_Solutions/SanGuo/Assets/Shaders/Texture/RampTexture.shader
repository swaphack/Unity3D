Shader "Custom/Texture/RampTexture" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_RampTex("Ramp Tex", 2D) = "white" {}
		_Specular("Specular", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8,256)) = 20
	}
	SubShader {
		Pass {
			Tags { "LightModel" = "ForwardBase" }

			CGPROGRAM
			#include "Lighting.cginc"

			#pragma vertex vert
			#pragma fragment frag 

			fixed4 _Color;
			sampler2D _RampTex;
			float4 _RampTex_ST;
			fixed4 _Specular;
			float _Gloss;


			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0; 
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};


			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal).xyz;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _RampTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed halfLambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
				fixed3 diffuse = tex2D(_RampTex, fixed2(halfLambert, halfLambert)).rgb * _Color.rgb * _LightColor0.rgb;

				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 halfDir = normalize(viewDir  + worldLightDir);				

				fixed3 specular = _LightColor0.rgb * _SpecColor.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

				return  fixed4(ambient + diffuse + specular, 1.0);
			}
			
			ENDCG
		}
	}

	Fallback "Specular"
}