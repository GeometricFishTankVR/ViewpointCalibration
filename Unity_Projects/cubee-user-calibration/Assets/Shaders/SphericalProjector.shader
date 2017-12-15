// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SphericalProjector"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			uniform float4 _projPosition;
			uniform float4 _P1p;
			uniform float4 _P1n;
			uniform float4 _P2p;
			uniform float4 _P2n;
			uniform float4 _P3p;
			uniform float4 _P3n;
			uniform float4 _P4p;
			uniform float4 _P4n;
			uniform float4 _P5p;
			uniform float4 _P5n;
			uniform float4 _P6p;
			uniform float4 _P6n;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vd : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				// d is unit vector from p to the sphere's origin
				o.vd = v.vertex;
				o.uv = float2(0, 0);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 dd = _projPosition - i.vd;
			    float distance = sqrt(dd.x*dd.x + dd.y*dd.y + dd.z*dd.z);
				float4 d = abs(normalize(dd));
				float theta = 0.1 / distance;
				//float phi = UNITY_PI / 4;
				float x = dot(d.xyz, float3(1, 0, 0));
				float y = dot(d.xyz, float3(0, 1, 0));
				float z = dot(d.xyz, float3(0, 0, 1));
				if(x < theta || y < theta || z < theta) return float4(x, y, z, 1); // draw the lines
				else
				{

					/*float t = fmod(asin(dot(d.xyz, float3(1, 1, 1) / 3)), UNITY_PI/5.5);
					if (atan2(t, -phi) > 2.65) return 1 - t * float4(t/acos(d.x), 1/phi*acos(d.y), phi*asin(d.z)/t, 1);
					else return float4(0.2, 0.2, 0.2, 1);*/

					if (abs(dot(i.vd - _P1p, _P1n)) < 0.1 ||
						abs(dot(i.vd - _P2p, _P2n)) < 0.1 ||
						abs(dot(i.vd - _P3p, _P3n)) < 0.1 ||
						abs(dot(i.vd - _P4p, _P4n)) < 0.1) return float4(1, 1, 1, 1);
					else if (abs(dot(i.vd - _P5p, _P5n)) < 0.1 ||
						abs(dot(i.vd - _P6p, _P6n)) < 0.1) return float4(0, 0, 0, 1);
					else return float4(0.5, 0.5, 0.5, 1);
				}
				
			}
			ENDCG
		}
	}
}
