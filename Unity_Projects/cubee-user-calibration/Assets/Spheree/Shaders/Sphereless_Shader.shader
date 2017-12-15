// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Sphereeless_Shader" {
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
			uniform float4x4 sphere_scale; 
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
				// Get units in meters
				//frag_pos *= 0.15f;

				// Determine fragment position in vertically aligned display space
				float4 new_frag_pos = mul(sphere_scale,mul(sphere_transform,float4(frag_pos, 1.0)));
				//new_frag_pos = float4(frag_pos, 1);
				//float4 new_frag_pos = float4(frag_pos, 1);
				float3 view_ray = normalize( view_pos - new_frag_pos.xyz);
				
				
				// only display pixels facing the viewer
				if (dot(new_frag_pos.xyz, view_ray) < 0) {
					return float4(0, 0, 0, 1);
				}

				
				// Sample Render Texture at location of fragment in viewport canonical space
				float4 pos_VP = mul(view_P, mul(view_V, new_frag_pos));
				pos_VP = pos_VP / pos_VP.w;
				float4 tex = tex2D(_ViewTex, pos_VP.xy / 2 + float2(0.5,0.5));
				//return new_frag_pos;
				return float4(tex.xyz*frag_alpha, 1);

			}
			ENDCG
		}
		
	}
}