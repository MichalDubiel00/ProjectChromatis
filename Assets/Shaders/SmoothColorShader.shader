Shader "Custom/ColorSpread"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _HitPoint("Hit Point (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Spread("Spread Radius", Float) = 0.0
        _TargetColor("Target Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;   // Main sprite texture
            float4 _MainTex_ST;

            float2 _HitPoint;     // Hit point in UV coordinates
            float _Spread;        // Spread radius
            float4 _TargetColor;  // Target color to apply

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the sprite's original color and alpha
                fixed4 originalColor = tex2D(_MainTex, i.uv);

                // Calculate distance from the hit point
                float dist = distance(i.uv, _HitPoint);

                // Calculate spread factor (modulates influence of _TargetColor)
                float spreadFactor = saturate(1.0 - dist / _Spread);

                // Modulate the target color by the original color
                fixed4 colorWithAlpha = _TargetColor * originalColor.a;

                // Interpolate between the original color and the target color
                fixed4 blendedColor = lerp(originalColor, colorWithAlpha, spreadFactor);

                // Preserve the sprite's original transparency
                blendedColor.a = originalColor.a;

                return blendedColor;
            }
            ENDCG
        }
    }
}
