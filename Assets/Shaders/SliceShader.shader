// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SimpleUnlitTexturedShader"
{
  Properties
  {
    // we have removed support for texture tiling/offset,
    // so make them not be displayed in material inspector
    [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
    _AnimTimeStart("Animation Time Start", Float) = 0
      _AnimTimeEnd("Animation Time End", Float) = 0
    _Color("Color (RGBA)", Color) = (1, 1, 1, 1)
      
  }
    SubShader
  {
    Tags {"Queue"="Transparent" "RenderType"="Transparent"}
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
   // Cull front
      Pass
      {
          CGPROGRAM
          // use "vert" function as the vertex shader
          #pragma vertex vert
          // use "frag" function as the pixel (fragment) shader
          #pragma fragment frag alpha

          // vertex shader inputs
          struct appdata
          {
              float4 vertex : POSITION; // vertex position
              float2 uv : TEXCOORD0; // texture coordinate
          };

  // vertex shader outputs ("vertex to fragment")
  struct v2f
  {
      float2 uv : TEXCOORD0; // texture coordinate
      float4 vertex : SV_POSITION; // clip space position
  };

  // vertex shader
  v2f vert(appdata v)
  {
      v2f o;
      // transform position to clip space
      // (multiply with model*view*projection matrix)
      o.vertex = UnityObjectToClipPos(v.vertex);
      // just pass the texture coordinate
      o.uv = v.uv;
      return o;
  }

  // texture we will sample
  sampler2D _MainTex;
  float _AnimTimeStart;
  float _AnimTimeEnd;

  // pixel shader; returns low precision ("fixed4" type)
  // color ("SV_Target" semantic)
  fixed4 frag(v2f i) : SV_Target
  {
    float2 tex = i.uv;
    tex.x += _Time.x*60;
    // sample texture and return it
    float4 col = float4(0.0f, 0.0f, 0.0f, 1.0f);
    float pi = 3.1415;

    float s = 1.0f; // smoothstep(0.0f, 1.0f, (_Time.y - _AnimTimeStart) * 3.0f);

    float s2 = 1.0f;// smoothstep(0.0f, -0.2f, _Time.y - _AnimTimeEnd);

    // Fade with max intensity at y = 0.5
    float t = i.uv.y > 0.5f ? smoothstep(1.0f, 0.0f, i.uv.y * 2.0f - 1.0f) : smoothstep(0.0f, 1.0f, i.uv.y * 2.0f);
    // s2 is 1.0 before animation end fade begins.
    col.a = t * s * s2;
    // fallof / attenuation from max intensity.
    float t_fallof = pow(t, 2.0);
    col.rgb = float3(t_fallof * 1.0f, t_fallof * 0.33f, t_fallof * 0.02f) * tex2D(_MainTex, tex).x * 2.5f;
    col.rgb = col.rgb + float3(t_fallof, t_fallof, t_fallof) / 1.5f;

    return col;
}
ENDCG
}
  }
}