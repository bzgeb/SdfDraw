Shader "SdfShape"
{
    Properties
    {
        _Color ("Color", Color) = (0.8,0.2,0.2,1)
        [Toggle] _EnableMovement("Enable movement", float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //source: https://iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm
            float smin(float a, float b, float k)
            {
                float h = max(k - abs(a - b), 0.0) / k;
                return min(a, b) - h * h * k * (1.0 / 4.0);
            }

            //source: https://iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm
            float sdCircle(float2 p, float r)
            {
                return length(p) - r;
            }


            #define MAX_SHAPES 64

            fixed4 _Color;
            float _EnableMovement;

            float _SdfSizes[MAX_SHAPES];
            float _SdfStartTimes[MAX_SHAPES];
            float4 _SdfPositions[MAX_SHAPES];
            float4 _SdfDirections[MAX_SHAPES];

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = (v.uv * 2.0f) - 1.0f;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float2 p = i.uv;

                float d = 10000000;
                for (int c = 0; c < MAX_SHAPES; ++c)
                {
                    if (_SdfStartTimes[c] == -1)
                        continue;

                    const float size = _SdfSizes[c];
                    const float3 position = _SdfPositions[c].xyz;

                    float startTime = _SdfStartTimes[c];
                    float2 direction = _SdfDirections[c].xy;
                    float aliveTime = (startTime - _Time[1]) / 20.0;

                    float2 movement = (aliveTime * direction * _EnableMovement);
                    float2 pos = (p - position) + movement;
                    pos.x = ((pos.x + 1.0) % 2) - 1.0;
                    pos.y = ((pos.y + 1.0) % 2) - 1.0;
                    d = smin(d, sdCircle(pos, size), 0.1);
                }

                d = smoothstep(0.02, 0.03, d);
                d = saturate(1 - d);
                return d * _Color;
            }
            ENDCG
        }
    }
}