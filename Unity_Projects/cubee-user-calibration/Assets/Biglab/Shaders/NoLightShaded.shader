// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/Soft Shaded" 
{
	Properties 
	{
		_MainTex ( "Main Texture", 2D ) = "white" {}
		_Cutoff ( "Alpha Cutoff", Range( 0.01, 1.0 ) ) = 0.05
		_Color ( "Color", Color ) = ( 1,1,1,1 )
	}

	SubShader 
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType" = "TransparentCutout"
			"IgnoreProjector" = "True"
		}

		ZWrite On
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		// Cull On
		LOD 100	

		Pass 
		{
			CGPROGRAM

			#pragma fragment frag
			#pragma vertex vert

			#include "UnityCG.cginc"
			
			// ===============================================
			// Structs
			// ===============================================

			struct Input {
				float2 uv_MainTex;
			};

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL0;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL0;
				float2 uv : TEXCOORD0;
			};

			// ===============================================
			// Uniforms / Constants / Parameters
			// ===============================================

			sampler2D _MainTex;
			uniform fixed _Cutoff;
			fixed4 _Color;

			// ===============================================
			// Constants
			// ===============================================

			static const float Pi = 3.141592653;
			static const float PiHalf = Pi / 2.0;
			static const float TwoPi = Pi * 2.0;

			// ===============================================
			// Functions
			// ===============================================

			// Adjusts the lower bound of x
			float LowerDomain( float x, float e )
				{ return e + x * ( 1.0 - e ); }

			// Adjusts the upper bound of x
			float CompressDomain( float x, float e )
				{ return e + x * ( 1.0 - e * 2.0 ); }

			// Vertex Shader Program
			// Operates on every vertex independantly
			v2f vert( appdata v ) 
			{
				v2f o;
				o.vertex = mul( UNITY_MATRIX_MVP, v.vertex );
				o.normal = normalize( mul( UNITY_MATRIX_M, float4( v.normal, 0.0 ) ).xyz );
				o.uv = v.uv;
				return o;
			}

			// Fragment Shader Program
			// Operates on every pixel independantly
			fixed4 frag( v2f i ) : SV_Target
			{
				// Sample texture
				fixed4 texel = tex2D( _MainTex, i.uv );
				
				// Color, discard transparent pixels
				fixed4 color = texel * _Color;

				// Alpha test clipping
				clip( color.a - _Cutoff );

				// Compute lambert term
				float l = dot( i.normal, normalize( float3( 1, 3, 2 ) ) );
				
				// Computes an 'unlit' but 'non-flat' shading
				l = ( l + 1.0 ) / 2.0;     // Wrap Lighting
				l = LowerDomain( l, 0.5 ); // Softens lighting
				l = max( 0.0, l );         // Clamps to 0 - 1 range
				
				return float4( color.rgb * l, color.a );
			}

			ENDCG
		}
	}
}
