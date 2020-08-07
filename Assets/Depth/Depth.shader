/*

below works but the depth texture does not match the color texture, something is up with the projection

_cameradepthtexture sizes with the windows, which can make it seem to align incorrectly

https://forum.unity.com/threads/decodedepthnormal-linear01depth-lineareyedepth-explanations.608452/


https://www.reddit.com/r/GraphicsProgramming/comments/9ukp2z/unity3d_engine_im_trying_to_calculate_the/
https://github.com/zezba9000/UnityMathReference/blob/master/Assets/Demos/Shaders/DepthBuffToWorldPos/DepthBuffToWorldPos.shader

https://forum.unity.com/threads/world-position-from-depth.151466/#post-3065516
https://www.reddit.com/r/Unity3D/comments/9g2hup/comparing_cameradepthtexture_to_depth_in_shader/
*/

Shader "Unlit/Depth"
{
    Properties
    {
        _DepthTex ("Texture", 2D) = "Black" {}
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
            sampler2D _DepthTex;


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

            float3 rgb2hsv(float3 c) {
              float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
              float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

              float d = q.x - min(q.w, q.y);
              float e = 1.0e-10;
              return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c) {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            //https://forum.unity.com/threads/getting-scene-depth-z-buffer-of-the-orthographic-camera.601825/#post-4966334
            float CorrectDepth(float rawDepth)
            {
                float persp = LinearEyeDepth(rawDepth);
                float ortho = (_ProjectionParams.z - _ProjectionParams.y) * (1 - rawDepth) + _ProjectionParams.y;
                return lerp(persp, ortho, unity_OrthoParams.w);
            }


            fixed4 frag(v2f i) : SV_TARGET{
                //get depth from depth texture
                float depth = tex2D(_DepthTex, i.uv).r;

                //linear depth between camera and far clipping plane
                //depth = Linear01Depth(depth);

                //https://forum.unity.com/threads/getting-scene-depth-z-buffer-of-the-orthographic-camera.601825/#post-4966334
                depth = CorrectDepth(depth) / _ProjectionParams.z;

                float3 rgb = hsv2rgb( float3(depth,1.0,1.0) );

                return float4(rgb,1.0);
            }
          
            ENDCG
        }
    }
}
