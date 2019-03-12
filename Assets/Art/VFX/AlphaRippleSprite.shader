// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/AlphaRippleSprite"
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

		_Alpha("Alpha Transparency", Range(0,1)) = 1

		// Ripple Speed
		_RippleSpeed("Ripple Speed", Float) = 20
		_RippleFrequency("Ripple Frequency", Float) = 50
		_StartingTime("StartingTime", Float) = 0
		_EndingTime("Ending Time", Float) = 3
		_CTime("Time", Range(0,5)) = 0
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
	float _RippleSpeed;
	float _RippleFrequency;
	float _LineThickness;
	float _StartingTime;
	float _EndingTime;
	float _CTime;
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

	fixed4 frag(v2f IN) : SV_Target
	{
		float2 texcoord = float2(IN.texcoord);
		float2 center = float2(.5, .5);
		float outAlpha = 1;
		float dist = distance(texcoord, center);
		half4 color = IN.color;
		

		// c is how you control thickness

		float xVal = _CTime * -_RippleSpeed + _RippleFrequency * dist;
		xVal = _CTime-dist- _StartingTime > 0 ? xVal : 0;
		xVal = _CTime-dist < _EndingTime ? xVal : 0;

		outAlpha = sin(xVal) * 100;
		outAlpha = clamp(outAlpha, 0, 1);

		// fade out the center
		outAlpha *= pow(dist / 0.45,5);

		// fade out near the edges of the circle
		outAlpha *= 1 - pow(min(dist, 0.65) / 0.65, 1.5); //exp distance

		// apply the alpha
		color.a *= outAlpha * _Alpha;
		
		// multiply by tint
		color *= _Color;

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
