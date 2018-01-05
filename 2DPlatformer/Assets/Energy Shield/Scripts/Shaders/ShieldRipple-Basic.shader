// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShieldRipple-Basic" 
{
	Properties 
	{
		_Color("Colour Tint", Color) = (1,1,1,1)
		_MainTex("Ripple Texture", 2D) = "white" {}
		
		_Intensity("Colour Intensity", Range(0,1)) = 1
		
		_FresnelBias("Fresnel Bias", float) = -0.34	
		_FresnelScale("Fresnel Scale", float) = 1.55
		_FresnelPower("Fresnel Power", float) = 1.5
		
		_ImpactPos("Impact Center", Vector) = (0,0,0,0)
	}

	Category 
	{

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		
		SubShader
		{
			// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
			// on to the screen
			Pass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
				
				Blend One OneMinusSrcAlpha
				ZWrite off
				
				ZTest LEqual
								
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct vertexIn
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					half3 normal : NORMAL;
				};

				struct vertexOut
				{
					float4 vertex : SV_POSITION;
					float2 uvmain : TEXCOORD0;
					half3 normalDir : TEXCOORD1;
					half3 impactDir : TEXCOORD2;
					UNITY_FOG_COORDS(3)
				};

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _ImpactPos;
				float _FresnelBias;
				float _FresnelScale;
				float _FresnelPower;
				fixed _Intensity;
				
				vertexOut vert (vertexIn v)
				{
					vertexOut o;
					
					o.vertex = UnityObjectToClipPos(v.vertex);
					
					o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					half3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
					
					o.normalDir = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
					
					o.impactDir = normalize(_ImpactPos - posWorld);
					
					UNITY_TRANSFER_FOG(o, o.vertex);
					
					return o;
				}

				half4 frag (vertexOut i) : SV_Target
				{					
					fixed impact = abs(dot(i.impactDir, i.normalDir));
					fixed fresnel = saturate(_FresnelBias + _FresnelScale * pow(impact, _FresnelPower));
					
					if(-impact * _MainTex_ST.x + _MainTex_ST.z < 0.1 || -impact * _MainTex_ST.x + _MainTex_ST.z > 1)
					{
						_Intensity = 0;
					}

					half4 rippleTex = tex2D(_MainTex, float2(-impact, 0.5) * _MainTex_ST.xy + _MainTex_ST.zw);
					
					rippleTex = (rippleTex * fresnel) * _Intensity;
					
					rippleTex *= _Color;
					
					UNITY_APPLY_FOG(i.fogCoord, rippleTex);
					return rippleTex;
				}
				
				ENDCG
			}
		}
	}
}
