Shader "MyModShaders/MyGameBlendNoOutline"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		_Alpha("Alpha", range(0,1)) = 1
		_Bright("Bright", range(0, 5)) = 1
	}

	SubShader{
		LOD 200

		//关闭光照
		Lighting Off

		//裁剪背面
		Cull Off

		//标记（队列：透明）
		Tags{ Queue = Transparent }

		Pass
		{

			ZWrite On

			//选取Alpha混合方式  
			//Blend  SrcAlpha SrcAlpha
			Blend SrcAlpha OneMinusSrcAlpha

			//===========开启CG着色器语言编写模块============  
			CGPROGRAM

			//编译指令:告知编译器顶点和片段着色函数的名称 
	#pragma vertex vert
	#pragma fragment frag  
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile_instancing

	#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform half _Alpha;
			uniform half _Bright;
			uniform sampler2D _MatCap;

			struct vIn {
				half4 vertex:POSITION;
				half3 normal : NORMAL;
				half2 texcoord:TEXCOORD0;
				half2 cap:TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct vOut {
				half4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				half2 cap : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			//--------------------------------【顶点着色函数】-----------------------------  
			// 输入：POSITION语义（坐标位置）  
			// 输出：SV_POSITION语义（像素位置）  
			//---------------------------------------------------------------------------------  
			vOut vert(vIn v)
			{
				//坐标系变换  
				//输出的顶点位置（像素位置）为模型视图投影矩阵乘以顶点位置，也就是将三维空间中的坐标投影到了二维窗口  
				vOut o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(vOut, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				half2 capCoord;

				half3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
				worldNorm = mul((half3x3)UNITY_MATRIX_V, worldNorm);
				o.cap.xy = worldNorm.xy * 0.5 + 0.5;

				return o;
			}

			//--------------------------------【片段着色函数】-----------------------------  
			// 输入：无  
			// 输出：COLOR语义（颜色值）  
			//---------------------------------------------------------------------------------  
			fixed4 frag(vOut i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 mc = tex2D(_MatCap, i.cap) * tex * (1.0 + _Bright);
				mc.a = mc.a * _Alpha;

				UNITY_SETUP_INSTANCE_ID(i);
				return mc;
			}

			//===========结束CG着色器语言编写模块===========  
			ENDCG

		}
	}

	FallBack "Unlit/Texture"
}