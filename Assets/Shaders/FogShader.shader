Shader "Custom/FogProjection"
{
	Properties
	{
		_FullTexture("Full Texture", 2D) = "white" {}
		_SemiTexture("Semi Texture", 2D) = "white" {}
		_Color("Color", Color) = (0, 0, 0, 0)
		_SemiOpacity("Semi Opacity", Float) = 1.0
	}
		SubShader
		{
			Tags { "Queue" = "Transparent+100" } // to cover other transparent non-z-write things

			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha
				ZTest Equal

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};


				float4x4 unity_Projector;
				sampler2D _FullTexture;
				sampler2D _SemiTexture;
				fixed4 _Color;
				float _SemiOpacity;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = mul(unity_Projector, v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float aFull = tex2Dproj(_FullTexture, i.uv).a;
					float aSemi = tex2Dproj(_SemiTexture, i.uv).a;

					float a = aFull + _SemiOpacity * aSemi;

					// weird things happen to minimap if alpha value gets negative
					_Color.a = max(0, _Color.a - a);
					return _Color;
				}
				ENDCG
			}
		}
}