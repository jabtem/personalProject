Shader "Custom/RimLight" {
	Properties {
		[HDR]_Color ("Color", Color) = (1,1,1,1)
		[HDR]_Color2 ("Color2", Color) = (1,1,1,1)
		[HideInInspector]_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RimLightMul("RimLightMul", Range(0, 10)) = 1
		_RimLightPow("RimLightPow", Range(0, 10)) = 1
		_CumstomValue("CumstomValue", float) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting alpha:fade noshadow  noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd nolppv noshadowmask interpolateview

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 2.0

		//sampler2D _MainTex;

		struct Input {
			fixed2 uv_MainTex;
			fixed3 viewDir;
		};

		fixed4 _Color;
		fixed4 _Color2;
		fixed _RimLightMul;
		fixed _RimLightPow;
		float _CumstomValue;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {

			fixed NdotV = dot(IN.viewDir, o.Normal);
			fixed3 RimLight = saturate(pow(((1 - NdotV) * _RimLightMul), _RimLightPow));

			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) ;
			o.Albedo = lerp(_Color, _Color2, _CumstomValue);
			o.Alpha = saturate(RimLight * _Color.a);
		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha * (1 - _CumstomValue);
			return c;
		}

		ENDCG

	}
	FallBack ""
}
