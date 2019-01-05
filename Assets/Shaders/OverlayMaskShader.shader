Shader "Custom/OverlayMaskShader"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color Albedo", Color) = (1,1,1,1)
		_BumpMap("Normal map", 2D) = "bump" {}
		_Mask("Mask (RGB)", 2D) = "white" {}
		_ColorMask("Color Mask", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	
	SubShader
	{
		Tags
		{ 
			"RenderType" = "Opaque"
		}
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Mask;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_Mask;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _ColorMask;

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo depends on the alpha value of the mask texture
			fixed4 maskSample = tex2D(_Mask, IN.uv_Mask) * _ColorMask;
			fixed4 textureSample = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			fixed4 c = lerp(textureSample, maskSample, maskSample.a);
			o.Albedo = c.rgb;

			// Output alpha depends on the alpha in the albedo color and ignores alpha channels in textures
			o.Alpha = _Color.a;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			// Unpack normals or emission; depends on mask alpha
			// Due to IF condition, this is not the best code for shaders
			o.Emission = maskSample.a * maskSample;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}

		ENDCG
	}
	
	FallBack "Diffuse"
}
