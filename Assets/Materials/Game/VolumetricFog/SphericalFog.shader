// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Spherical Fog" {
	Properties {
		_FogBaseColor ("Fog Base Color", Color) = (0,1,1,1)
		_FogDenseColor ("Fog Dense Color", Color) = (1,1,1,1)
		_InnerRatio ("Inner Ratio", Range (0.0, 0.9999)) = 0.5
		_Density ("Density", Range (0.0, 10.0)) = 10.0
		_ColorFalloff ("Color Falloff", Range (0.0, 50.0)) = 16.0
	}
	 
	Category {
		Tags { "Queue"="Transparent+99" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off
		ZTest Always
	 	
		SubShader {
			Pass {
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				 
				inline float CalcVolumeFogIntensity (
					float3 sphereCenter,
					float sphereRadius,
					float innerRatio,
					float density,
					float3 cameraPosition,
					float3 viewDirection,
					float maxDistance
				) {
					// Local is the cam position in local space of the sphere.
					float3 local = cameraPosition - sphereCenter;
					
					// Calculate ray-sphere intersection
					float fA = dot (viewDirection, viewDirection);
					float fB = 2 * dot (viewDirection, local);
					float fC = dot(local, local) - sphereRadius * sphereRadius;
					float fD = fB * fB - 4 * fA * fC;
					
					// Early out of no intersection.
					if (fD <= 0.0f)
						return 0;
				 	
					float recpTwoA = 0.5 / fA;
					float DSqrt = sqrt (fD);
					// Distance to front of sphere (0 if inside sphere).
					// This is the distance from the camera where sampling should begin.
					float dist = max ((-fB - DSqrt) * recpTwoA, 0);
					// Distance to back of sphere
					float dist2 = max ((-fB + DSqrt) * recpTwoA, 0);
				 	
				 	// Max sampling depth should be minimum of back of sphere or solid surface hit.
					float backDepth = min (maxDistance, dist2);
					// Calculate initial sample dist and distance between samples.
					float sample = dist;
					float step_distance = (backDepth - dist) / 10;
					
					// The step_contribution is a value where 0 means no reduction in clarity and
					// 1 means 100% reduction in clarity.
					// The step_contribution approaches 1 as the sample distance increases.
					float step_contribution = (1 - 1 / pow (2, step_distance)) * density;
					
					// Calculate value at the center needed to make the value be 1 at the desired inner ratio.
					// This high value does not actually produce high density in the center, since it's clamped to 1.
					float centerValue = 1 / (1 - innerRatio);
					
					// Initially there's no fog, which is full clarity.
					float clarity = 1;
					for ( int seg = 0; seg < 10; seg++ )
					{
						float3 position = local + viewDirection * sample;
						float val = saturate (centerValue * (1 - length (position) / sphereRadius));
						float sample_fog_amount = saturate (val * step_contribution);
						clarity *= (1 - sample_fog_amount);
						sample += step_distance;
					}
					return 1 - clarity;
				}
				 
				fixed4 _FogBaseColor;
				fixed4 _FogDenseColor;
				float _InnerRatio;
				float _Density;
				float _ColorFalloff;
				sampler2D _CameraDepthTexture;
				uniform float4 FogParam;
				 
				struct v2f {
					float4 pos : SV_POSITION;
					float3 view : TEXCOORD0;
					float4 projPos : TEXCOORD1;
				};
				 
				v2f vert (appdata_base v) {
					v2f o;
					float4 wPos = mul (unity_ObjectToWorld, v.vertex);
					o.pos = UnityObjectToClipPos (v.vertex);
					o.view = wPos.xyz - _WorldSpaceCameraPos;
					o.projPos = ComputeScreenPos (o.pos);
				 	
					// Move projected z to near plane if point is behind near plane.
					float inFrontOf = ( o.pos.z / o.pos.w ) > 0;
					o.pos.z *= inFrontOf;
					return o;
				}
				 
				half4 frag (v2f i) : COLOR {
					half4 color = half4 (1,1,1,1);
					float depth = LinearEyeDepth (UNITY_SAMPLE_DEPTH (tex2Dproj (_CameraDepthTexture, UNITY_PROJ_COORD (i.projPos))));
					float3 viewDir = normalize (i.view);
					
					// Calculate fog density.
					// Scale by factor 1000 to avoid precision errors for large volumes.
					float fog = CalcVolumeFogIntensity (
						FogParam.xyz * 0.001,
						FogParam.w * 0.001,
						_InnerRatio,
						_Density * 1000,
						_WorldSpaceCameraPos * 0.001,
						viewDir,
						depth * 0.001);
					
					// Calculate ratio of dense color.
					float denseColorRatio = pow (fog, _ColorFalloff);
					
					// Set color based on denseness and alpha based on raw calculated fog density.
					color.rgb = lerp (_FogBaseColor.rgb, _FogDenseColor.rgb, denseColorRatio);
					color.a = fog * lerp (_FogBaseColor.a, _FogDenseColor.a, denseColorRatio);
					return color;
				}
				ENDCG
			}
		}
	}
	Fallback "VertexLit"
}
