// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/RefractionShader" {
	Properties{
		planeInfo("Plane Info", Vector) = (0,0,0,0)
		tankScale("Fish Tank Scale", Vector) = (0,0,0,0)
		_MainTex("Texture", 2D) = "White" {}
	}
	SubShader{




		Pass{
			//Cull Front
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _MainTex;
			float4 planeInfo;
			float4 tankScale;
			uniform float indexOfRefraction;
			uniform float4 _LightPos;
			uniform float4 _LightColor;
			uniform float4 ambient;
			uniform float3 sphere_center;
			uniform float3 view_pos;
			uniform float4x4 view_P;
			uniform float4x4 view_V;
			uniform float4x4 inv_view_V;
			uniform float4x4 main_MVP;
			uniform float4x4 sphereTransform;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 refractedPosition : TEXCOORD3;
				float3 normal : TEXCOORD1;
				float3 screenCenter : TEXCOORD2;
				

			};
			float3 Refract(float3 position, float4 surface, float refractionIndex) {

				//ray origin is the camera position
				float3 viewerPosition = _WorldSpaceCameraPos.xyz;

				//ray end is the vertex's undistorted position
				float3 vertexPosition = position;

				//get the vector from the camera to the vertex
				float3 worldRay = vertexPosition - viewerPosition;

				//normalize it for direction
				float3 worldRayDir = normalize(worldRay);

				//surface is a vector4 that defines a plane
				float3 worldPlaneNormal = surface.xyz;

				//define a known position on the plane
				float3 worldPlaneOrigin = worldPlaneNormal * surface.w;

				//get the vector result of the worldRay entering the water
				float3 refraction = refract(worldRayDir, normalize(worldPlaneNormal), refractionIndex);

				//raycast from the vertex, backwards along the refraction vector
				float denom = dot(-worldPlaneNormal, -refraction);
				float3 p010 = worldPlaneOrigin - vertexPosition;
				float t = dot(p010, -worldPlaneNormal) / denom;
				float3 intersection = vertexPosition + refraction * -t;

				//get the vector from the camera to the intersection, this is the perceived position
				float3 originToIntersection = intersection - viewerPosition;

				//starting from the camera, move the vector along the perceived position vector by the original ray length
				return viewerPosition + normalize(originToIntersection) * length(worldRay);
			}

			bool IsValidPixel(float3 pos, float3 center) {
				
				float3 viewRay = normalize(_WorldSpaceCameraPos - pos);
				float dist = dot(center - pos, planeInfo.xyz) / dot(viewRay, planeInfo.xyz);
				float3 intersectionPoint = pos + dist*viewRay;
				float3 offset = intersectionPoint - center;

				if (abs(planeInfo.x) > 0.5) {
					if (viewRay.x * planeInfo.x < 0) {
						return false;
					}
					if (abs(offset.z) > tankScale.z/2 || abs(offset.y) > tankScale.y/2) {
						return false;
					}
				}

				if (abs(planeInfo.y) > 0.5) {
					if (viewRay.y * planeInfo.y < 0) {
						return false;
					}
					if (abs(offset.z) > tankScale.z/2 || abs(offset.x) > tankScale.x/2) {
						return false;
					}
				}

				if (abs(planeInfo.z) > 0.5) {
					if (viewRay.z * planeInfo.z < 0) {
						return false;
					}
					if (abs(offset.x) > tankScale.x/2 || abs(offset.y) > tankScale.y/2) {
						return false;
					}
				}
				return true;
			}

			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				//Calculate vertex position in camera space
				float4 vertexWorldSpace = mul(unity_ObjectToWorld, v.vertex);
				vertexWorldSpace = vertexWorldSpace / vertexWorldSpace.w;
				float3 viewRay = _WorldSpaceCameraPos - vertexWorldSpace.xyz;

				float3 refractedPosition = Refract(vertexWorldSpace, planeInfo, 1/indexOfRefraction);
				//refractedPosition = vertexWorldSpace;
				o.refractedPosition = refractedPosition;
				o.pos = mul(UNITY_MATRIX_VP, float4(refractedPosition, 1));
				o.screenCenter = planeInfo.xyz * planeInfo.w;
				o.uv = v.uv;
				o.normal = mul(transpose(unity_WorldToObject), float4(v.normal,0)).xyz;
				return o;
			}

			float4 frag(vertexOutput i) : COLOR{
				if(!IsValidPixel(i.refractedPosition, i.screenCenter))				discard;
				float3 lightDir;
				if (_LightPos.w == 1) {
					lightDir = normalize(i.refractedPosition - _LightPos.xyz);
				}
				else {
					lightDir = _LightPos.xyz;
				}
				
				float diffuseCoefficient = dot(-lightDir, i.normal)/length(i.refractedPosition - _LightPos.xyz);
				return tex2D(_MainTex, i.uv) * diffuseCoefficient;


			}
		ENDCG
		}
		


	}
}