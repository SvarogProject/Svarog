Shader "Sprites/ColorChange" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("_Color", Color) = (1,1,1,1)
        
        // color 1
        _ColorX ("Tint", Color) = (1,1,1,1)
        _HueShift ("Hue", Range (0,1)) = 1.0
        _Alpha ("Alpha", Range (0,1)) = 1.0
        _Tolerance ("Tolerance", Range (0,1)) = 1.0
        _Sat("Saturation", Float) = 1
        _Val("Value", Float) = 1
        // color 2
        _ColorX2 ("Tint2", Color) = (1,1,1,1)
        _HueShift2 ("Hue2", Range (0,1)) = 1.0
        _Alpha2 ("Alpha2", Range (0,1)) = 1.0
        _Tolerance2 ("Tolerance2", Range (0,1)) = 1.0
        _Sat2("Saturation2", Float) = 1
        _Val2("Value2", Float) = 1
        // color 3
        _ColorX3 ("Tint3", Color) = (1,1,1,1)
        _HueShift3 ("Hue3", Range (0,1)) = 1.0
        _Alpha3 ("Alpha3", Range (0,1)) = 1.0
        _Tolerance3 ("Tolerance3", Range (0,1)) = 1.0
        _Sat3("Saturation3", Float) = 1
        _Val3("Value3", Float) = 1
    }
    
    SubShader
    {   
        Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off
        
        Pass
        {
        
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma target 3.0
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                half2 texcoord  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;          
            };
            
            sampler2D _MainTex;
            float _Size;
            // Color 1
            float _HueShift;
            float _Tolerance;
            fixed4 _Color;
            fixed4 _ColorX;
            fixed _Alpha;
            float _Sat;
            float _Val;
            // Color 2
            float _HueShift2;
            float _Tolerance2;
            fixed4 _ColorX2;
            fixed _Alpha2;
            float _Sat2;
            float _Val2;
            // Color 3
            float _HueShift3;
            float _Tolerance3;
            fixed4 _ColorX3;
            fixed _Alpha3;
            float _Sat3;
            float _Val3;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }
            
            float3 shift_col(float3 RGB, float3 shift)
            {
                
                float3 RESULT = float3(RGB);
                float a1= shift.z*shift.y;
                float a2= shift.x*3.14159265/180;
                float VSU = a1*cos(a2);
                float VSW = a1*sin(a2);
                
                RESULT.x = (.299*shift.z+.701*VSU+.168*VSW)*RGB.x
                + (.587*shift.z-.587*VSU+.330*VSW)*RGB.y
                + (.114*shift.z-.114*VSU-.497*VSW)*RGB.z;
                
                RESULT.y = (.299*shift.z-.299*VSU-.328*VSW)*RGB.x
                + (.587*shift.z+.413*VSU+.035*VSW)*RGB.y
                + (.114*shift.z-.114*VSU+.292*VSW)*RGB.z;
                
                RESULT.z = (.299*shift.z-.3*VSU+1.25*VSW)*RGB.x
                + (.587*shift.z-.588*VSU-1.05*VSW)*RGB.y
                + (.114*shift.z+.886*VSU-.203*VSW)*RGB.z;
                
                return (RESULT);
            }
            
            
            float4 frag (v2f i) : COLOR
            {
                fixed4 c = tex2D(_MainTex, i.texcoord)*i.color;
                
                // Color 1
                float3 shift = float3(_HueShift, _Sat, _Val);
                float3 shifted = shift_col(c, shift);      
                float3 c1 = c.rgb;
                c1 = c1-_ColorX.rgb;
                c1 = abs(c1);
                if (c1.r<_Tolerance && c1.g<_Tolerance && c1.b<_Tolerance) 
                    c.rgb=shifted;             
                c.a = c.a-_Alpha;
                
                // Color 2
                float3 shift2 = float3(_HueShift2, _Sat2, _Val2);
                float3 shifted2 = shift_col(c, shift2);      
                float3 c2 = c.rgb;
                c2 = c2-_ColorX2.rgb;
                c2 = abs(c2);
                if (c2.r<_Tolerance && c2.g<_Tolerance && c2.b<_Tolerance) 
                    c.rgb=shifted2;             
                c.a = c.a-_Alpha2;
                
                // Color 3
                float3 shift3 = float3(_HueShift3, _Sat3, _Val3);
                float3 shifted3 = shift_col(c, shift3);      
                float3 c3 = c.rgb;
                c3 = c3-_ColorX3.rgb;
                c3 = abs(c3);
                if (c3.r<_Tolerance && c3.g<_Tolerance && c3.b<_Tolerance) 
                    c.rgb=shifted3;              
                c.a = c.a-_Alpha3;
                
                return c;
            }
            
        ENDCG
        }
    }
    Fallback "Sprites/Default"
}