Shader "Unlit/shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainColor ("Main Color", color) = (1.0, 0, 0, 1.0)
		_RayColor ("Ray Color", color) = (1.0, 0, 0, 1.0)
		_RayPower ("Ray Power", Range(0, 3)) = 1.0
    }
    SubShader
    {
        
        LOD 100

		CGINCLUDE

		#include "UnityCG.cginc"

		struct appdataXRay
        {
            float4 vertex : POSITION;
			float3 normal : NORMAL;
        };

        struct v2fXRay
        {
            float4 vertex : SV_POSITION;
			fixed4 color : TEXCOORD0;
		};

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

        sampler2D _MainTex;
        float4 _MainTex_ST;
		fixed4 _MainColor;
		fixed4 _RayColor;
		float _RayPower;

		v2fXRay vertXRay (appdataXRay v)
		{
			v2fXRay o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			float3 worldNormal = UnityObjectToWorldNormal(v.normal);
			fixed3 worldNormalDir = normalize(worldNormal);
			fixed3 worldViewDir = normalize(WorldSpaceViewDir(v.vertex));

			float rim = 1.0 - dot(worldNormalDir, worldViewDir);
			o.color = _RayColor * pow(rim, _RayPower);
			return o;
		}

		fixed4 fragXRay (v2fXRay i) : SV_Target
		{
			return i.color;
		}

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);
            return col * _MainColor;
        }

		ENDCG

		// 两个Pass顺序不能交换，一定要先渲染X光再正常渲染物体
		Pass
		{
			Tags { "RenderType" = "Opaque" "Queue"="Geometry+1" }
			ZTest Greater
			ZWrite Off
			Blend SrcColor OneMinusSrcColor

			CGPROGRAM
			#pragma vertex vertXRay
			#pragma fragment fragXRay
			ENDCG
		}

		Pass
        {
			Tags { "RenderType"="Opaque" "Queue"="Geometry"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}

