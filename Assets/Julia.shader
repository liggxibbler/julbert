Shader "Unlit/Julia"
{
    Properties
    {
        _Position ("Position", Vector) = (0, 0, 1, 0) // x, y, scalex, scaley
		_MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

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

            float4 _Position;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;// TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			float2 iterate(float2 p, float2 c)
			{
				return float2(p.x * p.x - p.y * p.y + c.x, 2 * p.x * p.y + c.y);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				int k;
				//float2 offset = float2(sin(_Time.y*17 / 50), cos(_Time.y * 19 / 50));
				float2 offset = float2(_Position.x, _Position.y) * _Position.zw;
				float2 ret = (i.uv - .5) * 2.0* _Position.zw;
				ret = ret;
				float2 c = offset;

				for (k = 0; k < 256; ++k)
				{
					ret = iterate(ret, c);
					if (dot(ret, ret) >= 4.0)
						break;
				}

				float j = sqrt(float(k));
				//fixed4 col = fixed4(sin(j), cos(j), 1/j, 1.0);
				fixed4 col = 1 - fixed4(1/j, 1/j, 1 / j, 1.0);
				//float j = 256.0/sqrt(float(k));
				//fixed4 col = fixed4(j,j,j,1);
				//col = fixed4(ret.x, ret.y, 0.0, 1.0);
				return col;
            }
            ENDCG
        }
    }
}
