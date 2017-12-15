// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/ColorSphere" {
	Properties{
		_MainTex("Projector Info", 2D) = "White" {}
		_ViewTex("Sampled Image", 2D) = "White" {}
	}
	SubShader{
		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _ViewTex;
			sampler2D _MainTex;
			uniform float3 view_pos;
			uniform float4x4 view_P;
			uniform float4x4 view_V;
			//QZ: sphere-related params are not in use; should be avoided in frag shader
			uniform float4x4 sphere_transform; 
			uniform float3 sphere_scale; 
			uniform float3 sphere_center; 

			struct vertexInput {
				float4 vertex : POSITION;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				//Calculate vertex position in camera space
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_V, mul(unity_ObjectToWorld, v.vertex)));
				return o;
			}

			float4 frag(vertexOutput i) : COLOR{
								
				// Flip the y direction (Not sure why, maybe reading in a binary file into image works differently in c# than c++?) //QZ: depends graphic API: DirectX vs OpenGL core
				i.tex = i.tex / i.tex.w;
				i.tex.y = -i.tex.y;
				
				// Get fragment information from texture
				float4 frag_info = tex2D(_MainTex, i.tex.xy / 2 + float2(0.5, 0.5));
				float3 frag_pos = frag_info.xyz;
				float frag_alpha = frag_info.w;

						

				// Set pixel black if it is not on the sphere: QZ this should be fixed in the screen calibration and removed here
				if (length(frag_pos) > 1.01 || length(frag_pos) < 0.99) {
					return float4(0, 0, 0, 1);
				}
				
				float4 new_frag_pos = mul(sphere_transform, float4(frag_pos, 1));

				// Create a white strip along x and z axis
				if (abs(new_frag_pos.x) < 0.01 || abs(new_frag_pos.z) < 0.01 || abs(new_frag_pos.y) < 0.01) {
					return float4(1, 1, 1, 1);
				}
				// Color based on fragment position
				return float4(frag_alpha*new_frag_pos.xyz, 1);
			}
			ENDCG
		}
		
	}
}