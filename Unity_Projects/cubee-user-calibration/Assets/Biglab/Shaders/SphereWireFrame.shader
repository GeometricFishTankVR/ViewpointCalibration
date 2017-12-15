Shader "Custom/Unlit/Sphere Wireframe" {
	Properties 
	{
		width("Line Width", Float) = 0.01
		numLines("Number of Lines", Int) = 20
	}

	SubShader
	{
		Pass
		{
			Tags { "Queue" = "Opaque" }
			ZWrite On
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float width;
			int numLines;

			#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D10)
			#define FACING SV_IsFrontFace
			#define FACE_TYPE uint
			#else
			#define FACING FACE 
			#define FACE_TYPE float
			#endif      

			struct vertexInput {
				float4 vertex : POSITION;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 vertex : TEXCOORD0;
			};

			float2 cartesianToSpherical(float3 pos) {
				return float2(acos(pos.y) / 3.14159, atan2(pos.z,pos.x) / 3.14159);
			}

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				// Calculate vertex position in camera space
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vertex = v.vertex;
				return o;
			}

			float4 frag(vertexOutput i, FACE_TYPE face : FACING) : COLOR{
				float2 spherePos = cartesianToSpherical(normalize(i.vertex.xyz));
				// Scale the sphere to determine which fragments are coloured
				float2 scaledSpherePos = spherePos * numLines;
				if (!(abs(scaledSpherePos.x - round(scaledSpherePos.x)) < width || abs(scaledSpherePos.y - round(scaledSpherePos.y)) < width)) {
					discard;
				}
				return float4(0.4, 0.5, 0.5, 1) + face * float4(0.5, 0.5, 0.5, 0);
			}
			ENDCG
		} 
	}
}