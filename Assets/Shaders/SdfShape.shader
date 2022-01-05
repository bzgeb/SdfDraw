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

            #include "UnityCG.cginc"

            struct Sdf
            {
                float3 position;
                float size;
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
                float aspect = (_ScreenParams.x / _ScreenParams.y);
                float2 uv = i.uv;
                uv.x *= aspect;

                float l = 0;
                for (int c = 0; c < _NumSdfs; ++c)
                {
                    Sdf sdf = _SdfBuffer[c];
                    float currentLength = length(sdf.position - uv) / sdf.size;
                    //l = smoothstep(0.97, 0.98, l);
                    currentLength = saturate(1 - currentLength);
                    l = max(l, currentLength);
                }

                return fixed4(l, 0, 0, l);
            }
            ENDCG
        }
    }
}