// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Static_Spheree_shader" {
	Properties{
		_MainTex("Diffuse Texture", 2D) = "White" {}
	}
		SubShader{
			Pass{
				Tags{"LightMode" = "ForwardBase"}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				uniform sampler2D _Maintex;
	uniform sampler2D _MainTex;
	uniform float4 _LightColor0;
	uniform float4x4 calib_MVP;
	
	struct vertexInput {
		float4 vertex : POSITION;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 tex : TEXCOORD0;
		float4 cal_pos : TEXCOORD1;
		};

	vertexOutput vert(vertexInput v) {
		vertexOutput o;
		UNITY_INITIALIZE_OUTPUT(vertexOutput, o);
		o.tex = v.vertex;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}

	float4 frag(vertexOutput i) : COLOR{
		float4 temp = mul(calib_MVP, mul(unity_ObjectToWorld, i.tex));
		temp = temp / temp.w;
		float4 tex = tex2D(_MainTex, temp.xy / 2 + float2(0.5, 0.5));
		return float4(tex.xyz, 1.0);
	}
		ENDCG
		}
		//FallBack "Diffuse"
	}
}