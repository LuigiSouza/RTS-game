Shader "Projector/AdditiveTint" {
    Properties{
        _Color("Tint Color", Color) = (1, 1, 1, 1)
        _Attenuation("Falloff", Range(0.0, 1.0)) = 1.0
        _ShadowTex("Cookie", 2D) = "gray" {}
    }

        Subshader{
            Tags {"Queue" = "Transparent"}
            Pass {
                ZWrite Off
                ColorMask RGB
                Blend SrcAlpha One // Additive blending

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct VertexOutput {
                    float4 uvShadow : TEXCOORD0;
                    float4 clipSpacePosition : SV_POSITION;
                };

                float4x4 unity_Projector;
                float4x4 unity_ProjectorClip;

                VertexOutput vert(float4 objectSpacePosition : POSITION)
                {
                    VertexOutput output;
                    output.clipSpacePosition = UnityObjectToClipPos(objectSpacePosition);
                    output.uvShadow = mul(unity_Projector, objectSpacePosition);
                    return output;
                }

                sampler2D _ShadowTex;
                fixed4 _Color;
                float _Attenuation;

                fixed4 frag(VertexOutput input) : SV_TARGET
                {
                    // Apply alpha mask
                    fixed4 shadowTexColor = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(input.uvShadow));
                    fixed4 tintColor = _Color * shadowTexColor.a;

                    // Attenuation
                    float depth = input.uvShadow.z; // [-1 (near), 1 (far)]
                    return tintColor * clamp(1.0 - abs(depth) + _Attenuation, 0.0, 1.0);
                }
                ENDCG
            }
    }
}
