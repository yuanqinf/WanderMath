// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ForceField"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
		_Size("Size", Range( 0 , 10)) = 1
		_HitRampTex("HitRampTex", 2D) = "white" {}
		_HitSpread("HitSpread", Float) = 0
		_HitNoise("HitNoise", 2D) = "white" {}
		_HitNoiseTilling("HitNoiseTilling", Vector) = (1,1,1,0)
		_HitNoiseIntensity("HitNoiseIntensity", Float) = 0
		_HitFadePower("HitFadePower", Float) = 1
		_HitFadeDistance("HitFadeDistance", Float) = 6
		_RimBias("RimBias", Float) = 0
		_RimScale("RimScale", Float) = 1
		_RimPower("RimPower", Float) = 2
		_EmissColor("EmissColor", Color) = (0,0,0,0)
		_EmissIntensity("EmissIntensity", Float) = 1
		_FlowStrength("FlowStrength", Vector) = (0.2,0.2,0,0)
		_FlowSpeed("FlowSpeed", Float) = 0.2
		_FlowLight("FlowLight", 2D) = "white" {}
		_FlowMap("FlowMap", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform float _CullMode;
		uniform float AffectorAmount;
		uniform float HitSize[20];
		uniform float4 HitPosition[20];
		uniform float4 _EmissColor;
		uniform float _EmissIntensity;
		uniform float _RimBias;
		uniform float _RimScale;
		uniform float _RimPower;
		uniform sampler2D _FlowLight;
		uniform float _Size;
		uniform sampler2D _FlowMap;
		uniform float2 _FlowStrength;
		uniform float _FlowSpeed;
		uniform sampler2D _HitRampTex;
		uniform sampler2D _HitNoise;
		uniform float3 _HitNoiseTilling;
		uniform float _HitNoiseIntensity;
		uniform float _HitSpread;
		uniform float _HitFadeDistance;
		uniform float _HitFadePower;


		inline float4 TriplanarSampling42( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		float HitWaveFunction35( sampler2D RampTex, float3 WorldPos, float HitNoise, float HitSpread, float HitFadeDistance, float HitFadePower )
		{
			float hit_result;
			for(int j = 0;j < AffectorAmount;j++)
			{
			float distance_mask = distance(HitPosition[j].xyz,WorldPos);
			float hit_range = -clamp((distance_mask - HitSize[j] + HitNoise) / HitSpread,-1,0);
			float2 ramp_uv = float2(hit_range,0.5);
			float hit_wave = tex2D(RampTex,ramp_uv).r; 
			float hit_fade = saturate((1.0 - distance_mask / HitFadeDistance) * HitFadePower);
			hit_result = hit_result + hit_fade * hit_wave;
			}
			return saturate(hit_result);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV132 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode132 = ( _RimBias + _RimScale * pow( 1.0 - fresnelNdotV132, _RimPower ) );
			float RimFactor138 = fresnelNode132;
			float2 temp_output_4_0_g1 = (( i.uv_texcoord / _Size )).xy;
			float2 temp_cast_0 = (0.5).xx;
			float2 temp_output_41_0_g1 = ( ( (tex2D( _FlowMap, i.uv_texcoord )).rg - temp_cast_0 ) + 0.5 );
			float2 temp_output_17_0_g1 = _FlowStrength;
			float mulTime22_g1 = _Time.y * _FlowSpeed;
			float temp_output_27_0_g1 = frac( mulTime22_g1 );
			float2 temp_output_11_0_g1 = ( temp_output_4_0_g1 + ( temp_output_41_0_g1 * temp_output_17_0_g1 * temp_output_27_0_g1 ) );
			float2 temp_output_12_0_g1 = ( temp_output_4_0_g1 + ( temp_output_41_0_g1 * temp_output_17_0_g1 * frac( ( mulTime22_g1 + 0.5 ) ) ) );
			float4 lerpResult9_g1 = lerp( tex2D( _FlowLight, temp_output_11_0_g1 ) , tex2D( _FlowLight, temp_output_12_0_g1 ) , ( abs( ( temp_output_27_0_g1 - 0.5 ) ) / 0.5 ));
			float4 FlowColor157 = lerpResult9_g1;
			float4 temp_output_162_0 = ( RimFactor138 * FlowColor157 );
			sampler2D RampTex35 = _HitRampTex;
			float3 WorldPos35 = ase_worldPos;
			float4 triplanar42 = TriplanarSampling42( _HitNoise, ( ase_worldPos * _HitNoiseTilling ), ase_worldNormal, 5.0, float2( 1,1 ), 1.0, 0 );
			float WaveNoise123 = triplanar42.x;
			float HitNoise35 = ( WaveNoise123 * _HitNoiseIntensity );
			float HitSpread35 = _HitSpread;
			float HitFadeDistance35 = _HitFadeDistance;
			float HitFadePower35 = _HitFadePower;
			float localHitWaveFunction35 = HitWaveFunction35( RampTex35 , WorldPos35 , HitNoise35 , HitSpread35 , HitFadeDistance35 , HitFadePower35 );
			float HitWave47 = localHitWaveFunction35;
			float4 temp_output_165_0 = ( temp_output_162_0 + ( ( temp_output_162_0 + 0.5 ) * HitWave47 ) );
			o.Emission = ( ( _EmissColor * _EmissIntensity ) * temp_output_165_0 ).rgb;
			float grayscale168 = Luminance(temp_output_165_0.rgb);
			float clampResult169 = clamp( grayscale168 , 0.0 , 1.0 );
			o.Alpha = clampResult169;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
128.8;148.8;1536;731.8;1673.172;-309.1888;2.002936;True;False
Node;AmplifyShaderEditor.CommentaryNode;161;-2572.768,1754.217;Inherit;False;1753.819;965.485;FlowLightColor;11;153;155;156;160;151;152;159;150;149;148;157;FlowLightColor;0.9811321,0.05923811,0.6143794,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;41;-3131.96,-305.3445;Inherit;False;2291.384;1279.597;HitWave;18;37;35;40;39;28;13;30;36;16;5;17;42;43;46;44;45;47;123;HitWave;0.489655,1,0,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;153;-2522.768,2201.823;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;45;-3049.321,519.8216;Inherit;False;Property;_HitNoiseTilling;HitNoiseTilling;7;0;Create;True;0;0;False;0;False;1,1,1;0.1,0.1,0.1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;44;-3076.321,351.8216;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;155;-2236.875,2193.703;Inherit;True;Property;_FlowMap;FlowMap;19;0;Create;True;0;0;False;0;False;-1;None;9b48a18fc62d06f4f8a6ddf499474f5d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-2801.322,344.8214;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;43;-3075.321,117.8215;Inherit;True;Property;_HitNoise;HitNoise;6;0;Create;True;0;0;False;0;False;None;df8b1f0a74f57f3449f28606fb6ca497;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.CommentaryNode;137;-3013.011,1250.531;Inherit;False;740.6978;397.3641;RimFactor;5;134;133;135;132;138;RimFactor;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-1851.342,2346.132;Inherit;False;Constant;_Float1;Float 1;18;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;156;-1885.437,2219.477;Inherit;False;FLOAT2;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-2998.647,1339.597;Inherit;False;Property;_RimBias;RimBias;11;0;Create;True;0;0;False;0;False;0;1.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;42;-2637.082,315.3507;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;5;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;151;-1789.949,2455.385;Inherit;False;Property;_FlowStrength;FlowStrength;16;0;Create;True;0;0;False;0;False;0.2,0.2;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;134;-2985.718,1443.028;Inherit;False;Property;_RimScale;RimScale;12;0;Create;True;0;0;False;0;False;1;0.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;149;-1868.55,2032.818;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;152;-1776.297,2604.302;Inherit;False;Property;_FlowSpeed;FlowSpeed;17;0;Create;True;0;0;False;0;False;0.2;0.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-2988.591,1522.038;Inherit;False;Property;_RimPower;RimPower;13;0;Create;True;0;0;False;0;False;2;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;150;-1869.805,1804.217;Inherit;True;Property;_FlowLight;FlowLight;18;0;Create;True;0;0;False;0;False;None;24ac6c804c82bb44db835ed9b69d2315;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleSubtractOpNode;159;-1686.762,2222.134;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2296.969,555.3174;Inherit;False;Property;_HitNoiseIntensity;HitNoiseIntensity;8;0;Create;True;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;-2233.676,344.3781;Inherit;False;WaveNoise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;132;-2746.745,1297.144;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;148;-1378.967,2090.43;Inherit;False;Flow;1;;1;acad10cc8145e1f4eb8042bebe2d9a42;2,50,0,51,0;5;5;SAMPLER2D;;False;2;FLOAT2;0,0;False;18;FLOAT2;0,0;False;17;FLOAT2;1,1;False;24;FLOAT;0.2;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;5;-1945.976,208.7538;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;28;-1914.803,643.3719;Inherit;False;Property;_HitFadeDistance;HitFadeDistance;10;0;Create;True;0;0;False;0;False;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2014.138,467.8264;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;36;-1925.238,-16.56707;Inherit;True;Property;_HitRampTex;HitRampTex;4;0;Create;True;0;0;False;0;False;None;8275a2e5db2df9247b15dd4929e7bed6;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-2488.673,1335.287;Inherit;False;RimFactor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;-1043.748,2141.434;Inherit;False;FlowColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1876.669,533.0898;Inherit;False;Property;_HitSpread;HitSpread;5;0;Create;True;0;0;False;0;False;0;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1871.803,793.3723;Inherit;False;Property;_HitFadePower;HitFadePower;9;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;35;-1570.084,162.3929;Inherit;False;float hit_result@$$for(int j = 0@j < AffectorAmount@j++)${$$float distance_mask = distance(HitPosition[j].xyz,WorldPos)@$$float hit_range = -clamp((distance_mask - HitSize[j] + HitNoise) / HitSpread,-1,0)@$$float2 ramp_uv = float2(hit_range,0.5)@$$float hit_wave = tex2D(RampTex,ramp_uv).r@ $$float hit_fade = saturate((1.0 - distance_mask / HitFadeDistance) * HitFadePower)@$$hit_result = hit_result + hit_fade * hit_wave@$}$$return saturate(hit_result)@;1;False;6;True;RampTex;SAMPLER2D;;In;;Inherit;False;True;WorldPos;FLOAT3;0,0,0;In;;Inherit;False;True;HitNoise;FLOAT;0;In;;Inherit;False;True;HitSpread;FLOAT;0;In;;Inherit;False;True;HitFadeDistance;FLOAT;0;In;;Inherit;False;True;HitFadePower;FLOAT;0;In;;Inherit;False;HitWaveFunction;True;False;0;6;0;SAMPLER2D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;-762.8199,879.3596;Inherit;False;138;RimFactor;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;158;-760.9162,969.22;Inherit;False;157;FlowColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-473.8333,1020.684;Inherit;False;Constant;_Float2;Float 2;18;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-1198.341,147.9986;Inherit;False;HitWave;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-506.9166,881.6103;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-281.624,966.0105;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-279.8457,1108.116;Inherit;False;47;HitWave;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;166;-61.08372,972.202;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;165;100.1554,855.208;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;144;-466.0898,537.5254;Inherit;False;Property;_EmissColor;EmissColor;14;0;Create;True;0;0;False;0;False;0,0,0,0;0.9199355,0.3716981,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;141;-535.6262,758.3606;Inherit;False;Property;_EmissIntensity;EmissIntensity;15;0;Create;True;0;0;False;0;False;1;1.71;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;131;-718.2159,-183.3618;Inherit;False;228;165;Properties;1;130;Properties;1,0,0,1;0;0
Node;AmplifyShaderEditor.TFHCGrayscale;168;407.9164,876.6149;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;-210.7772,681.5661;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-668.2159,-133.3618;Inherit;False;Property;_CullMode;CullMode;0;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1890.736,-246.0867;Inherit;False;Global;AffectorAmount;AffectorAmount;6;0;Create;False;0;0;True;0;False;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;37;-2376.522,-254.2805;Inherit;False;HitPosition;0;20;2;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;169;640.1151,883.1907;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GlobalArrayNode;39;-2118.304,-255.3445;Inherit;False;HitSize;0;20;0;False;False;0;1;True;Object;-1;4;0;INT;0;False;2;INT;0;False;1;INT;0;False;3;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;235.4468,688.6158;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;859.7938,604.0956;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;ForceField;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;130;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;155;1;153;0
WireConnection;46;0;44;0
WireConnection;46;1;45;0
WireConnection;156;0;155;0
WireConnection;42;0;43;0
WireConnection;42;9;46;0
WireConnection;159;0;156;0
WireConnection;159;1;160;0
WireConnection;123;0;42;1
WireConnection;132;1;133;0
WireConnection;132;2;134;0
WireConnection;132;3;135;0
WireConnection;148;5;150;0
WireConnection;148;2;149;0
WireConnection;148;18;159;0
WireConnection;148;17;151;0
WireConnection;148;24;152;0
WireConnection;16;0;123;0
WireConnection;16;1;17;0
WireConnection;138;0;132;0
WireConnection;157;0;148;0
WireConnection;35;0;36;0
WireConnection;35;1;5;0
WireConnection;35;2;16;0
WireConnection;35;3;13;0
WireConnection;35;4;28;0
WireConnection;35;5;30;0
WireConnection;47;0;35;0
WireConnection;162;0;139;0
WireConnection;162;1;158;0
WireConnection;170;0;162;0
WireConnection;170;1;171;0
WireConnection;166;0;170;0
WireConnection;166;1;48;0
WireConnection;165;0;162;0
WireConnection;165;1;166;0
WireConnection;168;0;165;0
WireConnection;140;0;144;0
WireConnection;140;1;141;0
WireConnection;169;0;168;0
WireConnection;146;0;140;0
WireConnection;146;1;165;0
WireConnection;0;2;146;0
WireConnection;0;9;169;0
ASEEND*/
//CHKSM=C5BB55842E879DF8D2FC26A952F1EE6CA7D77D72