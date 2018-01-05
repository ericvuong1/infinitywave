// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Shield-Refractive" 
{
	Properties 
	{
		_Color("Colour Tint", Color) = (1,1,1,1)
		_MainTex("Colour Texture (RGB)", 2D) = "white" {}
		_SecondTex("Colour Texture 2 (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		
		_Intensity("Colour Intensity", Range(0,1)) = 1

		_BumpAmt("Distortion", range (0,20)) = 3.28
		
		_FresnelBias("Fresnel Bias", float) = 0.95	
		_FresnelScale("Fresnel Scale", float) = 0.45
		_FresnelPower("Fresnel Power", float) = 4.89
		
//		_ImpactPos("Impact Center", Vector) = (0,0,0,0)
		
		_Speed("Energy", Range(0,10)) = 1
		
		_Direction("Texture Direction", Vector) = (0,0,0,0)
		
		_AnimSpeed ("Animation Speed", Float) = 10.0
		_AnimFreq ("Animation Frequency", Float) = 1.0
		_AnimPowerX ("Animation Power X", Float) = 0.0
		_AnimPowerY ("Animation Power Y", Float) = 0.1

		_AnimOffsetX ("Animation Offset X", Float) = 10.0
		_AnimOffsetY ("Animation Offset Y", Float) = 0.0
	}

	Category 
	{

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		
		Cull off
		
		SubShader
		{
			// This pass grabs the screen behind the object into a texture.
			// We can access the result in the next pass as _GrabTexture
			GrabPass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
			}
			
			Pass
			{
				ZWrite On
				ColorMask 0
			}
			
			// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
			// on to the screen
			Pass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
				
				Blend One OneMinusSrcAlpha
				
				ZTest Equal
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#pragma shader_feature HARD_FACES
				#include "UnityCG.cginc"

				struct vertexIn
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					half3 normal : NORMAL;
					fixed4 colour : COLOR; // I'm Canandian
				};

				struct vertexOut
				{
					float4 vertex : SV_POSITION;
					float4 uvgrab : TEXCOORD0;
					float4 uvbump : TEXCOORD1;
					float2 uvmain : TEXCOORD2;
					float2 uvsecond : TEXCOORD3;
					half3 normalDir : TEXCOORD4;
					half3 viewDir : TEXCOORD5;
					fixed4 colour : TEXCOORD6;
					UNITY_FOG_COORDS(7)
				};

				float _BumpAmt;
				fixed4 _Color;
				sampler2D _BumpMap;
				float4 _BumpMap_ST;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _SecondTex;
				float4 _SecondTex_ST;
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				float _FresnelBias;
				float _FresnelScale;
				float _FresnelPower;
				fixed _Intensity;

				float _Speed;
				float4 _Direction;
				
				half _AnimSpeed;
		        half _AnimFreq;
		        half _AnimPowerX;
		        half _AnimPowerY;

		        half _AnimOffsetX;
		        half _AnimOffsetY;
				
				
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
					
					o.uvbump.xy = o.uvbump.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
					
					o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uvsecond = TRANSFORM_TEX(v.texcoord, _SecondTex);
					
					half2 animOffset = half2(_AnimOffsetX, _AnimOffsetY) * o.uvmain;
					half2 animPower = half2(_AnimPowerX, _AnimPowerY);
					o.uvmain = o.uvmain + sin(_Time.x * _AnimSpeed + (animOffset.x + animOffset.y) * _AnimFreq) * animPower.xy;
					o.uvsecond = o.uvsecond + sin(_Time.x * _AnimSpeed + (animOffset.x + animOffset.y) * _AnimFreq) * animPower.xy;
					
					float time = _Time.x * _Speed;
					
					o.uvmain += time * _Direction.xy;
					o.uvsecond += time * _Direction.zw;
					
					o.uvbump.xy += time * -_Direction.xy;
					o.uvbump.zw += time * -_Direction.zw;
					
					half3 posWorld = (mul(unity_ObjectToWorld, v.vertex).xyz);
					
					o.viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld);
					
					o.normalDir = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
					
					o.colour = v.colour;
					
					UNITY_TRANSFER_FOG(o, o.vertex);
					
					return o;
				}

				half4 frag (vertexOut i) : SV_Target
				{
					fixed fresnel = 1 - saturate(_FresnelBias + _FresnelScale * pow(dot(i.viewDir, i.normalDir), _FresnelPower));
					
					fixed nDotV = saturate(dot(i.viewDir, i.normalDir));
					
					// calculate perturbed coordinates				
					half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump.xy)).rg + UnpackNormal(tex2D(_BumpMap, i.uvbump.zw)).rg; // we could optimize this by just reading the x & y without reconstructing the Z
					
					#if HARD_FACES
					bump = half2(0,0);
					#endif
					
					float2 offset = (-refract(i.viewDir, i.normalDir + bump, 0.9).xy * nDotV) * fresnel;
					
					offset *= _BumpAmt;
					
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
					
					half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
					
					half4 tint = tex2D(_MainTex, i.uvmain);
					
					half4 tint2 = tex2D(_SecondTex, i.uvsecond);
					
					half4 tintFinal = (tint + tint2) * _Intensity;
					
					#if HARD_FACES
					tintFinal = i.colour * _Intensity;
					#else
					tintFinal = (tint + tint2) * _Intensity;
					#endif
					
					col *= (1-(tintFinal) * (fixed4(1,1,1,1) - _Color));
					
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
