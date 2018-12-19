// Code adapted from https://github.com/keijiro/KinoVision

#include "Common.cginc"

sampler2D_float _CameraDepthTexture;
sampler2D_half _CameraMotionVectorsTexture;

float LinearizeDepth(float z)
{
    float isOrtho = unity_OrthoParams.w;
    float isPers = 1 - unity_OrthoParams.w;
    z *= _ZBufferParams.x;
    return (1 - isOrtho * z) / (isPers * z + _ZBufferParams.y);
}

half4 VectorToColor(float2 mv, float depthInv)
{
    // Horizontal motion -> red/green.
    // Amplify motion proportional to distance.
    half r = mv.x > 0 ? mv.x * depthInv : 0;
    half g = mv.x < 0 ? -mv.x * depthInv : 0;
    // Threshold motion yes/no -> alpha.
    half a = length(mv) > 0.0001 ? 1 : 0;

    return saturate(half4(r, g, 0, a));
}

half3 DepthAndMotionFragment(CommonVaryings input) : SV_Target
{
    half4 src = tex2D(_MainTex, input.uv0);

    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.uv1);
    // Continuous: 0 -> far, 1 -> near.
    depth = 1 - LinearizeDepth(depth);

    // Amplify motion by fixed factor.
    half2 mv = tex2D(_CameraMotionVectorsTexture, input.uv1).rg * 100;
    half4 mc = VectorToColor(mv, 1 / depth);
    // Discrete: 1/0 -> moving object yes/no.
    half a = mc.a;
    
    // Horizontal dimming gradient.
    // depth *= sqrt((0.5 - abs(0.5 - input.uv0)) * 2);

    // Combine with static objects (blue).
    half3 rgb = half3(depth * a * mc.r, 
                      depth * a * mc.g, 
                      depth * (1 - a));

#if !UNITY_COLORSPACE_GAMMA
    rgb = GammaToLinearSpace(rgb);
#endif

    return rgb;
}
