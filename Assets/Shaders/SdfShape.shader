Shader "SdfShape"
{
    Properties {}
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Sdf.hlsl"

            struct Sdf
            {
                float3 position;
                float size;
                float2 direction;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float size : PSIZE;
            };

            int _NumSdfs;
            StructuredBuffer<Sdf> _SdfBuffer;
            StructuredBuffer<float3> _VertexBuffer;

            v2f vert(uint vertex_id : SV_VertexID)
            {
                v2f o;

                const float3 vertexLocalPos = _VertexBuffer[vertex_id];

                o.vertex = float4(vertexLocalPos.xy, 0.0, 1.0);
                o.uv = float2(vertexLocalPos.xy);
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                const float aspect = (_ScreenParams.x / _ScreenParams.y);
                float2 uv = i.uv;
                uv.x *= aspect;

                float2 p = uv;

                float d = 10000000;
                for (int c = 0; c < _NumSdfs; ++c)
                {
                    const Sdf sdf = _SdfBuffer[c];
                    const float2 pos = p - sdf.position;
                    d = smin(d, sdCircle(pos, sdf.size), 0.1);
                }

                d = smoothstep(0.01, 0.02, d);
                d = saturate(1 - d);
                return fixed4(d, 0, 0, 1);
            }
            ENDCG
        }
    }
}