Shader "Unlit/Butterfly.shader"
{
    Properties
    {
		_BendScale("Bend Scale", Range(0.0, 1.0)) = 0.1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        //LOD 100

        Pass
        {
			Blend SrcAlpha One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#define PI 3.14159
            // make fog work
            // #pragma multi_compile_fog

            #include "UnityCG.cginc"

			uniform float _BendScale;
			uniform sampler2D _MainTex;

            /*struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };*/

            struct v2f
            {
				float4 position : SV_POSITION;
				fixed4 color	: COLOR;
                float2 uv		: TEXCOORD0;
            };


            v2f vert (appdata_full v)
            {
				float bend = sin(PI * _Time.x * 1000 / 45 + v.vertex.y + v.vertex.x);
				float x = sin(v.texcoord.x * PI) - 1.0;
				float y = sin(v.texcoord.y * PI) - 1.0;
				v.vertex.y += _BendScale * bend * (x + y);

				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
				fixed4 tex = tex2D(_MainTex, i.uv);
				tex.rgb *= i.color.rgb;
				tex.a *= i.color.a;
				return tex;
            }
            ENDCG
        }
    }
}
