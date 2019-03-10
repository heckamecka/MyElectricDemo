// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/HologramSprite"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

		_Alpha("Alpha Transparency", Range(0,1)) = 0.8

		// Gradient Tint
		_TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_GradientPower("Gradient Power", Range(0,2)) = 1

		// Glow
		_Glow("Glow", Float) = 0.3

		// Scan Lines
		_ScanWidth ("Scan Line Width", Range(0, 1)) = 0.3
		_ScanTiling ("Scan Tiling", Float) = 20
		_ScanSpeed("Scan Speed", Float) = 20

		// Glitch
		_GlitchFrequency("Glitch Frequency", Range(0, 5)) = 1.0
		_GlitchLeniency("Glitch Leniency", Range(0, 1)) = 0.8
		_GlitchIntensity("Glitch Wave Intensity", Float) = 0
		_GlitchFragmentation("Glitch Wave Fragmentation", Float) = 0
		_ChromAbOffset("Glitch Chromatic Abberation Offset", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
	{
		Name "Default"
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"
#include "UnityUI.cginc"

#pragma multi_compile __ UNITY_UI_ALPHACLIP

		struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
		float4 worldPosition : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	fixed4 _Color;
	fixed4 _TextureSampleAdd;
	float4 _ClipRect;
	float _ScanTiling;
	float _ScanWidth;
	float _ScanSpeed;
	float _GlitchFrequency;
	float _GlitchLeniency;
	float _GlitchIntensity;
	float _GlitchFragmentation;
	float _ChromAbOffset;
	float _Glow;
	half4 _TopColor;
	half4 _BottomColor;
	float _GradientPower;
	float _Alpha;

	v2f vert(appdata_t IN)
	{
		v2f OUT;

		UNITY_SETUP_INSTANCE_ID(IN);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		OUT.worldPosition = IN.vertex;
		OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

		OUT.texcoord = IN.texcoord;

		OUT.color = _Color * IN.color;
		return OUT;
	}

	sampler2D _MainTex;

	half4 GlitchEffect(float2 texcoord) {
		texcoord.x += _GlitchIntensity / 1000 * sin(10 * texcoord.y + _Time.y * 9 * _GlitchFragmentation) * sin(25 * texcoord.y + _Time.y * _GlitchFragmentation);

		float colR = tex2D(_MainTex, float2(texcoord.x - _ChromAbOffset, texcoord.y - _ChromAbOffset)).r;
		float colG = tex2D(_MainTex, texcoord).g;
		float colB = tex2D(_MainTex, float2(texcoord.x + _ChromAbOffset, texcoord.y + _ChromAbOffset)).b;
		float colA = tex2D(_MainTex, texcoord).a;

		return half4(colR, colG, colB, colA);
	}

	half4 NoGlitch(float2 texcoord) {
		return tex2D(_MainTex, texcoord);
	}

	fixed4 frag(v2f IN) : SV_Target
	{
		float2 texcoord = float2(IN.texcoord);

		// occasionally glitch
		float glitchfreq = sin(_Time.y*_GlitchFrequency* -1.1) * sin(2.8 * _Time.y * _GlitchFrequency) * sin(4.9 * _Time.y * _GlitchFrequency);
		half4 color = lerp(NoGlitch(texcoord), GlitchEffect(texcoord),  step(_GlitchLeniency, glitchfreq ));

		// add tint color
		color *= lerp(_BottomColor, _TopColor,  pow(texcoord.y, _GradientPower));

		// add glow
		half4 glow = half4(_Glow, _Glow, _Glow, 1);
		color *= glow;

		// add scan lines
		float scan = 0.0;
		scan = step(frac( texcoord.y * _ScanTiling + _Time.w * _ScanSpeed), _ScanWidth) * 0.35 + 0.65;
		color.a = color.a * (scan) * _Alpha;

		// multiply by tint
		color *= IN.color;

		// Other default unity stuff that i don't understand
		color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

#ifdef UNITY_UI_ALPHACLIP
		clip(color.a - 0.001);
#endif

		return color;
	}
		ENDCG
	}
	}
}
