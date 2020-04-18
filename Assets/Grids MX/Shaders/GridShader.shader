// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GridShader"
{
	SubShader
	{
		Pass
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off
			Cull Back
			ZWrite Off
			ZTest LEqual
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
	FallBack "Off"
}