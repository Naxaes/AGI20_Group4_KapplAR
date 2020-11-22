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
    _CameraPos("Camera Position", vector) = (0.0, 0.0, 0.0)

  }
    SubShader
    {
      Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
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
                 float3 normal : NORMAL;
             };

    // vertex shader outputs ("vertex to fragment")
    struct v2f
    {
        float2 uv : TEXCOORD0; // texture coordinate
        float4 vertex : SV_POSITION; // clip space position
        float3 worldPos : TEXCOORD1;
        float3 normalDir : TEXCOORD2;
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
        o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        o.normalDir = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
        return o;
    }

    // texture we will sample
    sampler2D _MainTex;
    float3 _CameraPos;
    float3 _Color;

    float fbm(in float2 x, in float H)
    {
      int numOctaves = 16;
      float t = 0.0;
      for (int i = 0; i < numOctaves; i++)
      {
        float f = pow(2.0, float(i));
        float a = pow(f, -H);
        t += a * tex2D(_MainTex, f * x);
      }
      return t;
    }

    float sunSurface(in float2 x) {
     //x += 0.2 * sin(float2(0.11, 0.13) * _Time.x*4.0f + length(x) * 2.0);

    
      float r = fbm(x, 1.0f)*length(x)*2.0f;
     // r = fbm(r, 3.0f);

      r = fbm(r + float2(frac(cos(_Time.x)*0.7f + sin(_Time.x)*0.3f), length(x)), 5.0f);
      return r*2.0f;//*2.0f;
    }

    // pixel shader; returns low precision ("fixed4" type)
    // color ("SV_Target" semantic)
    fixed4 frag(v2f i) : SV_Target
    {
      float2 tex = i.uv;
      // sample texture and return it
      float4 col = float4(_Color.x, _Color.y, _Color.z, 1.0f); // float4(0.0f, 0.0f, 0.0f, 1.0f);
      float pi = 3.1415;
      // "Fresnell"
      // Vertex to camera
      float3 vToC = normalize(_CameraPos - i.worldPos);
      float v_dot_n = max(dot(vToC, i.normalDir), 0.0f);
      col.rgb *= sunSurface(tex);
      col.rgb += float3(1.0f, 0.5f, 0.4f)*pow((1.0f - v_dot_n), 4.0f);
      col.rgb = pow(col.rgb, 2.2f);
      return col;
  }
  ENDCG
  }
  }
}