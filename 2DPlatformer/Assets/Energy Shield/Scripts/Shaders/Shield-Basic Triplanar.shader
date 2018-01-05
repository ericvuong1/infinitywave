// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Tri-P Shield-Basic" 
{
	Properties 
	{
		_Color("Colour Tint", Color) = (1,1,1,1)
		_MainTex("Colour Texture (RGB)", 2D) = "white" {}
		_SecondTex("Colour Texture 2 (RGB)", 2D) = "white" {}
		
		_Intensity("Colour Intensity", Range(0,1)) = 1
		
		// Uncomment the lines below to be able to alter the values in the inspector
		/*_FresnelBias("Fresnel Bias", float) = 0.95	
		_FresnelScale("Fresnel Scale", float) = 0.45
		_FresnelPower("Fresnel Power", float) = 4.89*/
		
		_Speed("Energy", Range(0,10)) = 1
		
		_Direction("Texture Direction", Vector) = (0,0,0,0)		
		
		_AnimSpeed("Animation Speed", Float) = 10.0
		_AnimFreq("Animation Frequency", Float) = 1.0
		_AnimPowerX("Animation Power X", Float) = 0.0
		_AnimPowerY("Animation Power Y", Float) = 0.1

		_AnimOffsetX("Animation Offset X", Float) = 10.0
		_AnimOffsetY("Animation Offset Y", Float) = 0.0
	}

	Category 
	{

		// We must be transparent, so other objects are drawn before this one.
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		
		Cull off
		
		SubShader
		{
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
					float2 uvmain : TEXCOORD0;
					float2 uvsecond : TEXCOORD1;
					half3 normalDir : TEXCOORD2;
					half3 worldPos : TEXCOORD3;
					half3 viewDir : TEXCOORD4;
					fixed4 colour : TEXCOORD5;
					UNITY_FOG_COORDS(6)
				};

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _SecondTex;
				float4 _SecondTex_ST;
				
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
					
					o.uvmain = _MainTex_ST.xy + _MainTex_ST.zw;
					o.uvsecond = _SecondTex_ST.xy + _SecondTex_ST.zw;
					
					half2 animOffset = half2(_AnimOffsetX, _AnimOffsetY) * o.uvmain;
					half2 animPower = half2(_AnimPowerX, _AnimPowerY);
					o.uvmain = o.uvmain + sin(_Time.x * _AnimSpeed + (animOffset.x + animOffset.y) * _AnimFreq) * animPower.xy;
					o.uvsecond = o.uvsecond + sin(_Time.x * _AnimSpeed + (animOffset.x + animOffset.y) * _AnimFreq) * animPower.xy;
					
					float time = _Time.x * _Speed;
					
					o.uvmain += time * _Direction.xy;
					o.uvsecond += time * _Direction.zw;
					
					o.worldPos = (mul(unity_ObjectToWorld, v.vertex).xyz);
					
					o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos);
					
					o.normalDir = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
					
					o.colour = v.colour;
					
					UNITY_TRANSFER_FOG(o, o.vertex);
					
					return o;
				}

				half4 frag (vertexOut i) : SV_Target
				{
					float3 projNormal = saturate(pow(i.normalDir * 1.4, 4));

					fixed4 x = tex2D(_MainTex, frac(i.worldPos.zy) + i.uvmain) * abs(i.normalDir.x);
					fixed4 y = tex2D(_MainTex, frac(i.worldPos.zx) + i.uvmain) * abs(i.normalDir.y);
					fixed4 z = tex2D(_MainTex, frac(i.worldPos.xy) + i.uvmain) * abs(i.normalDir.z);

					fixed4 tint = z;
					tint = lerp(tint, x, projNormal.x);
					tint = lerp(tint, y, projNormal.y);

					x = tex2D(_SecondTex, frac(i.worldPos.zy) + i.uvsecond) * abs(i.normalDir.x);
					y = tex2D(_SecondTex, frac(i.worldPos.zx) + i.uvsecond) * abs(i.normalDir.y);
					z = tex2D(_SecondTex, frac(i.worldPos.xy) + i.uvsecond) * abs(i.normalDir.z);

					fixed4 tint2 = z;
					tint2 = lerp(tint2, x, projNormal.x);
					tint2 = lerp(tint2, y, projNormal.y);

					fixed4 tintFinal = (tint + tint2) * _Intensity;
					
					#if HARD_FACES
					tintFinal = i.colour;
					#endif
					
					tintFinal *= _Color;
					
					fixed fresnel = 1 - saturate(_FresnelBias + _FresnelScale * pow(dot(i.viewDir, i.normalDir), _FresnelPower));
					
					fixed nDotV = abs(dot(i.viewDir, i.normalDir));
					
					// Uncomment one or both of the lines below to get an idea of what the shield could 
					// look like using the above lines of code
					//tintFinal *= nDotV;
					//tintFinal *= fresnel;
					
					UNITY_APPLY_FOG(i.fogCoord, tintFinal);
					return tintFinal;
				}
				
				ENDCG
			}
		}
	}
}
