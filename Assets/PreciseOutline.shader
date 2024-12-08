Shader "Custom/PreciseOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0.01, 0.1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 mainColor = tex2D(_MainTex, i.texcoord);

                // Если пиксель полностью прозрачный, проверяем обводку
                if (mainColor.a == 0)
                {
                    float2 offsets[8] = {
                        float2(-_OutlineThickness, -_OutlineThickness),
                        float2(-_OutlineThickness,  _OutlineThickness),
                        float2( _OutlineThickness, -_OutlineThickness),
                        float2( _OutlineThickness,  _OutlineThickness),
                        float2(-_OutlineThickness, 0),
                        float2( _OutlineThickness, 0),
                        float2(0, -_OutlineThickness),
                        float2(0,  _OutlineThickness)
                    };

                    for (int j = 0; j < 8; j++)
                    {
                        float4 neighbor = tex2D(_MainTex, i.texcoord + offsets[j]);
                        if (neighbor.a > 0.1)
                        {
                            return _OutlineColor;
                        }
                    }
                }

                return mainColor;
            }
            ENDCG
        }
    }
}
