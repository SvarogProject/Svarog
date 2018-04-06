Shader "Sprites/MasaShader" {

    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        
        // 斗篷红
        _KeyColor01("Key Color #1", Color) = (0.8039, 0.3098, 0.3921, 1)
        _TargetColor01("Target Color #1", Color) = (0.3176, 0.7451, 0.8039, 1)
        // 斗篷红线
        _KeyColor02("Key Color #2", Color) = (0.3411, 0.003, 0.003, 1)
        _TargetColor02("Target Color #2", Color) = (0, 0, 0 ,1)
        // 斗篷红中间色
        _KeyColor03("Key Color #1", Color) = (0.7843, 0.3215, 0.4, 1)
        _TargetColor03("Target Color #1", Color) = (0.3176, 0.7451, 0.8039, 1)
        
        _Threshold("Threshold", Float) = 0.01
    }
	
    SubShader {

        Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
        Pass {
        CGPROGRAM
        
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            //  =============================================
            //        IO structures
            //  =============================================
    
			struct appdata_t {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
            
			struct v2f {
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
            //  =============================================
            //        uniforms / varyings
            //  =============================================
        
            sampler2D _MainTex;
            sampler2D _AlphaTex;
            fixed4 _Color;
			float _AlphaSplitEnabled;
            float4 _KeyColor01;
            float4 _TargetColor01;
            float4 _KeyColor02;
            float4 _TargetColor02;
            float4 _KeyColor03;
            float4 _TargetColor03;
            float  _Threshold;

            //  =============================================
            //        vertex shader
            //  =============================================
            v2f vert(appdata_t i) {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.texcoord = i.texcoord;
                o.color = i.color * _Color;
				#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap (o.vertex);
				#endif
				
                return o;
            }
        
            //  =============================================
            //        pixel shader
            //  =============================================
        
            float4 frag(v2f i) : SV_Target {
                // fetch pixel color from texture
                float4 texColor = tex2D(_MainTex, i.texcoord);
        
                if (abs(_KeyColor01.r - texColor.r) < _Threshold && abs(_KeyColor01.g - texColor.g) < _Threshold && abs(_KeyColor01.b - texColor.b) < _Threshold) {
                    float4 newColor = float4(_TargetColor01.rgb, 1.0);
                    float alpha = texColor.a * 2.0;
                    return lerp(float4(texColor.rgb, alpha), newColor, alpha - 1.0);
                } 
                else if (abs(_KeyColor02.r - texColor.r) < _Threshold && abs(_KeyColor02.g - texColor.g) < _Threshold && abs(_KeyColor02.b - texColor.b) < _Threshold) {
                    float4 newColor = float4(_TargetColor02.rgb, 1.0);
                    float alpha = texColor.a * 2.0;
                    return lerp(float4(texColor.rgb, alpha), newColor, alpha - 1.0);
                } 
                else if (abs(_KeyColor03.r - texColor.r) < _Threshold && abs(_KeyColor03.g - texColor.g) < _Threshold && abs(_KeyColor03.b - texColor.b) < _Threshold) {
                    float4 newColor = float4(_TargetColor03.rgb, 1.0);
                    float alpha = texColor.a * 2.0;
                    return lerp(float4(texColor.rgb, alpha), newColor, alpha - 1.0);
                } 
                else {
                    return float4(texColor.rgb, texColor.a);
                }
            }
    
        ENDCG
        }
	}
    FallBack "Sprites/Default"
}