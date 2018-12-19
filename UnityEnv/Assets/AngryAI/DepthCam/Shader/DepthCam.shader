// Code adapted from https://github.com/keijiro/KinoVision

Shader "Custom/DepthCam"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
    }
    Subshader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "DepthAndMotion.cginc"
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #pragma vertex CommonVertex
            #pragma fragment DepthAndMotionFragment
            #pragma target 3.0
            ENDCG
        }
    }
}
