// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SphereSurfaceShader" {
	Properties{
		_MainTex("Projector Info", 2D) = "White" {}
	}
	SubShader{
		Pass{
			Cull Back
			Tags{ "Queue"="Transparent" "LightMode" = "ForwardBase" "RenderType"="Transparent"}
	        ZWrite Off
		    Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#include "UnityCG.cginc" 
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _MainTex;
			uniform float4x4 view_P;
			uniform float4x4 view_V;
			uniform float4x4 inv_view_V;

			struct vertexInput {
				float4 vertex : POSITION;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float3 norm : NORMAL;
			};

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				//calculate the vertex location in both camera's viewport
				o.tex = mul(view_P, mul(view_V, mul(unity_ObjectToWorld, v.vertex)));
				o.tex = o.tex / o.tex.w;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.norm = v.vertex.xyz;
				return o;
			}

			float4 frag(vertexOutput i) : COLOR{
				// Set fragment black if not visible to viewport
				float3 normal = normalize(i.norm);
				float4 view_pos = mul(unity_WorldToObject,mul(inv_view_V, float4(0, 0, 0, 1)));
				view_pos = view_pos / view_pos.w;
				if (dot(normalize(view_pos.xyz), normal) < 0) {
					return float4(0.0, 0.0, 0.0, 0.2);
				}

				// Sample from texture at location of fragment in view camera space
				float4 tex = tex2D(_MainTex, i.tex.xy / 2 + float2(0.5,0.5));
				return float4(tex.xyz, 0.3);
			}
			ENDCG
		}
	}
}