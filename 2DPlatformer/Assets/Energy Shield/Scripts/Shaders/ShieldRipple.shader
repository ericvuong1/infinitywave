// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShieldRipple" 
{
	Properties 
	{
		_Color("Colour Tint", Color) = (1,1,1,1)
		_Ripple("Ripple Effect (Normal Map)", 2D) = "bump" {}
		_MainTex("Ripple Texture", 2D) = "white" {}
		
		_Intensity("Colour Intensity", Range(0,1)) = 1

		_BumpAmt("Distortion", range (0,6)) = 4.8
		
		_FresnelBias("Fresnel Bias", float) = -0.34	
		_FresnelScale("Fresnel Scale", float) = 1.55
		_FresnelPower("Fresnel Power", float) = 1.5
		
		_ImpactPos("Impact Center", Vector) = (0,0,0,0)
	}

	Category 
	{

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent-1" "RenderType"="Transparent" }
		
		SubShader
		{
			// This pass grabs the screen behind the object into a texture.
			// We can access the result in the next pass as _GrabTexture
			GrabPass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
			}
			
			// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
			// on to the screen
			Pass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
				
				Blend One OneMinusSrcAlpha
				
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
					float4 uvgrab : TEXCOORD0;
					float2 uvbump : TEXCOORD1;
					float2 uvmain : TEXCOORD2;
					half3 normalDir : TEXCOORD3;
					half3 impactDir : TEXCOORD4;
					UNITY_FOG_COORDS(5)
				};

				float _BumpAmt;
				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _Ripple;
				float4 _Ripple_ST;
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				float4 _ImpactPos;

				float _FresnelBias;
				float _FresnelScale;
				float _FresnelPower;
				
				fixed _Intensity;
				
				vertexOut vert (vertexIn v)
				{
					vertexOut o;
					
					o.vertex = UnityObjectToClipPos(v.vertex);
					
					#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
					#else
					float scale = 1.0;
					#endif
					
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.zw;
					
					o.uvbump = TRANSFORM_TEX(v.texcoord, _Ripple);
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
					
					half2 ripple = UnpackNormal(tex2D(_Ripple, float2(-impact, 0.2) * _MainTex_ST.xy + _MainTex_ST.zw)).rg;
					
					if(-impact * _MainTex_ST.x + _MainTex_ST.z < 0.1 || -impact * _MainTex_ST.x + _MainTex_ST.z > 1)
					{
						_Intensity = 0;
						_BumpAmt = 0;
					}
					
					float2 offset = (ripple * pow(impact, _FresnelPower));
								
					offset *= _BumpAmt;
					offset *= fresnel;
					
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

					half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
					half4 rippleTex = tex2D(_MainTex, float2(-impact, 0.5) * _MainTex_ST.xy + _MainTex_ST.zw);
					
					rippleTex = (rippleTex * fresnel) * _Intensity;
					rippleTex.a *= 4;
										
					col *= (1-(rippleTex.a) * (fixed4(1,1,1,1) - _Color));
					
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				
				ENDCG
			}
		}

		// ------------------------------------------------------------------
		// Fallback for older cards

		SubShader 
		{
			Blend DstColor Zero
			Pass 
			{
				Name "BASE"
				SetTexture [_MainTex] {	combine texture }
			}
		}
	}
}
