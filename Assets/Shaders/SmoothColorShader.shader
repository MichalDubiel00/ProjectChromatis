Shader "Custom/SplashColor"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ImpactPoint ("Impact Point", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Float) = 0.0
        _Color ("Splash Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        float4 _ImpactPoint;  // Impact Point in UV space
        float _Radius;        // Radius of the splash
        fixed4 _Color;        // Splash color

        struct Input
        {
            float2 uv_MainTex; // UV coordinates of the sprite
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Sample the base texture
            fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex);

            // Calculate distance from the impact point
            float dist = distance(float2(_ImpactPoint.x, _ImpactPoint.y), IN.uv_MainTex);

            // Smooth spread using the radius
            float t = smoothstep(_Radius, _Radius - 0.1, dist);

            // Blend splash color with the base color
            fixed4 splashColor = lerp(_Color, baseColor, t);

            // Output the result
            o.Albedo = splashColor.rgb;
            o.Alpha = splashColor.a;
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}
