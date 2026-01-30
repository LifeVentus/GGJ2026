Shader "Unlit/NumRain"
{
    Properties
    {
         _MainTex ("Dummy Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,1,0,1)
        _Speed ("Speed", Float) = 1.2
        _Density ("Column Density", Float) = 60
        _Trail ("Trail Length", Float) = 0.35
        _Alpha ("Global Alpha", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float _Speed;
            float _Density;
            float _Trail;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float hash(float n)
            {
                return frac(sin(n) * 43758.5453);
            }

            // 画一个简易的 "0"
            float drawZero(float2 p)
            {
                float d = abs(length(p) - 0.35);
                return smoothstep(0.08, 0.02, d);
            }

            // 画一个简易的 "1"
            float drawOne(float2 p)
            {
                float body = smoothstep(0.05, 0.0, abs(p.x));
                float cap  = smoothstep(0.05, 0.0, abs(p.y - 0.3));
                return max(body, cap);
            }



            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _Rows = 20;   // 行密度（字符高度）

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // ===== 列索引 =====
                float col = floor(uv.x * _Density);

                // 每列不同速度
                float speed = hash(col) * 0.6 + 0.4;

                // 下落
                float y = uv.y + _Time.y * _Speed * speed;

                float row = floor(y * _Rows);

                // 决定 0 / 1
                float bit = step(0.5, hash(col * 100 + row));

                // ===== 关键：字符局部坐标（解耦 Density）=====
                float colCenterX = (col + 0.5) / _Density;

                float2 cellUV;
                cellUV.x = (uv.x - colCenterX) * _Density; // 宽度固定
                cellUV.y = frac(y * _Rows) - 0.5;          // 高度固定

                // ===== 字符形状 =====
                float shape0 = drawZero(cellUV);
                float shape1 = drawOne(cellUV);
                float shape  = lerp(shape0, shape1, bit);

                // 尾巴渐隐
                float trail = frac(y);
                float fade = smoothstep(_Trail, 0, trail);

                float brightness = shape * fade;
                float alpha = brightness * _Alpha;

                return float4(_Color.rgb * brightness, alpha);
            }

            ENDCG
        }
    }
}
