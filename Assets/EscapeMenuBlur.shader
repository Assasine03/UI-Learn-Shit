Shader "Hidden/EscapeMenuBlur"
{
    Properties
    {
        _MainTex ("Background", 2D) = "white" {}
        _Radius ("Blur radius", Float) = 2
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        float _Radius;

        half4 Blur(float2 uv, float2 direction)
        {
            half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv) * 0.227027;
            color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + direction * 1.384615) * 0.316216;
            color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - direction * 1.384615) * 0.316216;
            color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + direction * 3.230769) * 0.070270;
            color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - direction * 3.230769) * 0.070270;
            color.a = 1;
            return color;
        }
        half4 Horizontal(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            return Blur(input.texcoord, float2(_BlitTexture_TexelSize.x * _Radius, 0));
        }
        half4 Vertical(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            return Blur(input.texcoord, float2(0, _BlitTexture_TexelSize.y * _Radius));
        }
        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Horizontal
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Vertical
            ENDHLSL
        }
    }
    Fallback Off
}
