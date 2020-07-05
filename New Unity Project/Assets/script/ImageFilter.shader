Shader "Workshop/ImageFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _WaveSpeed("WaveSpeed", float) = 0
        _WaveFrequency("WaveSpeed", float) = 0
        _WaveStrength("WaveSpeed", float) = 0
            


    }
        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                uniform sampler2D _MainTex;
                uniform float4 _Color;
                uniform float _WaveSpeed, _WaveFrequency, _WaveStrength;
                uniform float _Should;

            struct attribute
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct varying
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            varying vert (attribute v)
            {
                varying o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            fixed4 frag(varying i) : SV_Target
            {
                float2 uv = i.uv;
                uv += _Should * sin(_Time.y * _WaveSpeed + uv.y * _WaveFrequency) * _WaveStrength;
                fixed4 col = tex2D(_MainTex, uv) /* _Color*/;
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
