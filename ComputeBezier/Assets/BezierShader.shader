Shader "Custom/BezierLineShader"
{
    Properties
    {
        
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Name "BezierLinePass"
            Cull Off

            CGPROGRAM
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float3 start;
            float3 startControl;
            float3 end;
            float3 endControl;

            float4 color;

            struct Vertex
            {
                float4 position;
            };
            StructuredBuffer<Vertex> vertexBuffer;

            StructuredBuffer<uint> indexBuffer;

            struct GeomIn
            {
                float4 position : POSITION;
            };

            struct FragIn
            {
                float4 position : POSITION;
            };

            FragIn vert(uint id : SV_VertexID)
            {
                int vertId = indexBuffer[id];

                FragIn input;
                input.position = UnityObjectToClipPos(vertexBuffer[vertId].position);
                return input;
            }

            fixed4 frag (FragIn fragIn) : SV_Target
            {
                return color;
            }
            ENDCG
        }
    }

    FallBack "VertexLit"
}
