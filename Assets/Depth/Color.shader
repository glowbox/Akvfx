Shader "Unlit/Color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "Black" {}
    } 
     SubShader {
        Cull Off
        ZWrite Off
        ZTest Always

        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM

            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //the rendered screen so far
            sampler2D _MainTex;

            //the object data that's put into the vertex shader
            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            //the data that's used to generate fragments and can be read by the fragment shader
            struct v2f{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            //the vertex shader
            v2f vert(appdata v){
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET{

                fixed4 col = tex2D(_MainTex, i.uv);

                return col;
            }
          
            ENDCG
        }
    }
}
