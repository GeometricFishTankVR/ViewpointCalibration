// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/VertexColor" {
	Properties{
	}
	SubShader{
		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct vertexInput {
				float4 vertex : POSITION;
				float4 color : COLOR;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 world_pos : TEXCOORD0;
			};

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				//Calculate vertex position in camera space
				o.pos = UnityObjectToClipPos(v.vertex);
				o.world_pos = mul(unity_ObjectToWorld, v.vertex);
				o.col = v.color;
				return o;
			}

			float4 frag(vertexOutput i) : COLOR{
				
				// Make a white strip along x and z axis
				if (abs(i.world_pos.x) < 0.01f || abs(i.world_pos.z) < 0.01f) {
					return float4(1, 1, 1, 1);
}
				return i.col;

			}
			ENDCG
		}
		
	}
}