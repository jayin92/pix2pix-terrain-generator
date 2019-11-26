
Shader "Unlit/s1"
{
	Properties
	{
		_Gradient("Gradient",2D)="white" {}
		_Diffuse("Diffuse",Color) = (1,1,1,1)
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(8.0,256)) = 20
		_YScale("YScale",Float)=1.0
	}
		SubShader
	{
		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"

			fixed4 _Diffuse,_Specular;
			float _Gloss,_YScale;
			sampler2D _Gradient;

            struct a2v
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };

            struct v2f
            {
				float3 wnorm : TEXCOORD0;
				float3 wpos : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.wnorm = mul(v.normal, (float3x3)unity_WorldToObject);
				o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 albedo = tex2D(_Gradient,fixed2(i.wpos.y*_YScale,i.wnorm.y)).rgb;
			
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 worldNormal = normalize(i.wnorm);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _Diffuse.rgb * _LightColor0.rgb * saturate(dot(worldNormal, worldLightDir));
				fixed3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.wpos.xyz);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss* tex2D(_Gradient, fixed2(i.wpos.y * _YScale, i.wpos.y * _YScale)).a  );
				
                return fixed4(albedo,1.0);
            }
            ENDCG
        }
    }
}
