// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33851,y:32294,varname:node_1873,prsc:2|emission-3523-OUT,alpha-5538-OUT;n:type:ShaderForge.SFN_Tex2d,id:3674,x:32672,y:32805,ptovrint:False,ptlb:TextureRay1,ptin:_TextureRay1,varname:_TextureRay1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1c097ab1eb09aa64c879a6cb6874263f,ntxv:0,isnm:False|UVIN-4372-UVOUT;n:type:ShaderForge.SFN_Color,id:3081,x:32656,y:32400,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:_Color1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07450981,c2:0,c3:0.1803922,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9809,x:32672,y:33004,ptovrint:False,ptlb:TextureRay2,ptin:_TextureRay2,varname:_TextureRay2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1c097ab1eb09aa64c879a6cb6874263f,ntxv:0,isnm:False|UVIN-7971-UVOUT;n:type:ShaderForge.SFN_Lerp,id:6134,x:33044,y:32820,varname:node_6134,prsc:2|A-3674-R,B-9809-R,T-6782-OUT;n:type:ShaderForge.SFN_Vector1,id:7844,x:32475,y:33337,varname:node_7844,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Panner,id:4372,x:32437,y:32805,varname:node_4372,prsc:2,spu:0.1,spv:0|UVIN-9523-UVOUT;n:type:ShaderForge.SFN_Panner,id:7971,x:32437,y:33004,varname:node_7971,prsc:2,spu:-0.1,spv:0|UVIN-1638-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:9523,x:32200,y:32805,varname:node_9523,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:1638,x:32200,y:33004,varname:node_1638,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Sin,id:4185,x:32475,y:33212,varname:node_4185,prsc:2|IN-3995-T;n:type:ShaderForge.SFN_Time,id:3995,x:32301,y:33212,varname:node_3995,prsc:2;n:type:ShaderForge.SFN_Clamp,id:6782,x:32661,y:33212,varname:node_6782,prsc:2|IN-4185-OUT,MIN-7844-OUT,MAX-9453-OUT;n:type:ShaderForge.SFN_Vector1,id:9453,x:32475,y:33401,varname:node_9453,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Color,id:8003,x:32656,y:32587,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:_Color2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07450981,c2:0,c3:0.1803922,c4:1;n:type:ShaderForge.SFN_Lerp,id:1725,x:33296,y:32397,varname:node_1725,prsc:2|A-7863-OUT,B-8371-OUT,T-1131-OUT;n:type:ShaderForge.SFN_Vector1,id:1131,x:33030,y:32528,varname:node_1131,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:8371,x:33017,y:32596,varname:node_8371,prsc:2|A-8003-RGB,B-9809-R;n:type:ShaderForge.SFN_Add,id:7863,x:33001,y:32399,varname:node_7863,prsc:2|A-3081-RGB,B-3674-R;n:type:ShaderForge.SFN_Tex2d,id:3817,x:33044,y:32964,ptovrint:False,ptlb:MaskRay,ptin:_MaskRay,varname:_TextureRay3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1c097ab1eb09aa64c879a6cb6874263f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:5538,x:33441,y:32797,varname:node_5538,prsc:2|A-1182-OUT,B-6134-OUT,T-3817-G;n:type:ShaderForge.SFN_Vector1,id:1182,x:33194,y:32768,varname:node_1182,prsc:2,v1:0;n:type:ShaderForge.SFN_Panner,id:1593,x:32582,y:31407,varname:node_1593,prsc:2,spu:0.07,spv:0.12|UVIN-4244-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:4244,x:32345,y:31407,varname:node_4244,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:8199,x:33022,y:31427,varname:node_8199,prsc:2|A-3739-OUT,B-7592-RGB,T-7208-B;n:type:ShaderForge.SFN_Vector1,id:3739,x:32758,y:31297,varname:node_3739,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:3523,x:33531,y:32383,varname:node_3523,prsc:2|A-6170-OUT,B-1725-OUT;n:type:ShaderForge.SFN_Tex2d,id:7208,x:32758,y:31407,ptovrint:False,ptlb:TextureDot1,ptin:_TextureDot1,varname:node_7208,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1c097ab1eb09aa64c879a6cb6874263f,ntxv:0,isnm:False|UVIN-1593-UVOUT;n:type:ShaderForge.SFN_Color,id:7592,x:32803,y:31038,ptovrint:False,ptlb:ColorDot1,ptin:_ColorDot1,varname:node_7592,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:8709,x:33237,y:31471,varname:node_8709,prsc:2|A-8199-OUT,B-3606-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3606,x:33022,y:31604,ptovrint:False,ptlb:PowerDot1,ptin:_PowerDot1,varname:node_3606,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Panner,id:2669,x:32601,y:32057,varname:node_2669,prsc:2,spu:-0.07,spv:0.12|UVIN-4615-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:4615,x:32364,y:32057,varname:node_4615,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:7146,x:33041,y:32076,varname:node_7146,prsc:2|A-8751-OUT,B-3721-RGB,T-3183-B;n:type:ShaderForge.SFN_Vector1,id:8751,x:32777,y:31946,varname:node_8751,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:3183,x:32777,y:32057,ptovrint:False,ptlb:TextureDot2,ptin:_TextureDot2,varname:_TextureDot2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1c097ab1eb09aa64c879a6cb6874263f,ntxv:0,isnm:False|UVIN-2669-UVOUT;n:type:ShaderForge.SFN_Color,id:3721,x:32822,y:31688,ptovrint:False,ptlb:ColorDot2,ptin:_ColorDot2,varname:_ColorDot2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:8747,x:33256,y:32120,varname:node_8747,prsc:2|A-7146-OUT,B-3986-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3986,x:33025,y:32270,ptovrint:False,ptlb:PowerDot2,ptin:_PowerDot2,varname:_PowerDot2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:6170,x:33470,y:32120,varname:node_6170,prsc:2|A-8709-OUT,B-8747-OUT;proporder:3674-3081-9809-8003-3817-7592-7208-3606-3183-3721-3986;pass:END;sub:END;*/

Shader "Shader Forge/M_WinFX" {
    Properties {
        _TextureRay1 ("TextureRay1", 2D) = "white" {}
        _Color1 ("Color1", Color) = (0.07450981,0,0.1803922,1)
        _TextureRay2 ("TextureRay2", 2D) = "white" {}
        _Color2 ("Color2", Color) = (0.07450981,0,0.1803922,1)
        _MaskRay ("MaskRay", 2D) = "white" {}
        _ColorDot1 ("ColorDot1", Color) = (0.5,0.5,0.5,1)
        _TextureDot1 ("TextureDot1", 2D) = "white" {}
        _PowerDot1 ("PowerDot1", Float ) = 2
        _TextureDot2 ("TextureDot2", 2D) = "white" {}
        _ColorDot2 ("ColorDot2", Color) = (0.5,0.5,0.5,1)
        _PowerDot2 ("PowerDot2", Float ) = 2
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _Stencil ("Stencil ID", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilOpFail ("Stencil Fail Operation", Float) = 0
        _StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOpFail]
                ZFail [_StencilOpZFail]
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            uniform sampler2D _TextureRay1; uniform float4 _TextureRay1_ST;
            uniform float4 _Color1;
            uniform sampler2D _TextureRay2; uniform float4 _TextureRay2_ST;
            uniform float4 _Color2;
            uniform sampler2D _MaskRay; uniform float4 _MaskRay_ST;
            uniform sampler2D _TextureDot1; uniform float4 _TextureDot1_ST;
            uniform float4 _ColorDot1;
            uniform float _PowerDot1;
            uniform sampler2D _TextureDot2; uniform float4 _TextureDot2_ST;
            uniform float4 _ColorDot2;
            uniform float _PowerDot2;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_3739 = 0.0;
                float4 node_1262 = _Time;
                float2 node_1593 = (i.uv0+node_1262.g*float2(0.07,0.12));
                float4 _TextureDot1_var = tex2D(_TextureDot1,TRANSFORM_TEX(node_1593, _TextureDot1));
                float node_8751 = 0.0;
                float2 node_2669 = (i.uv0+node_1262.g*float2(-0.07,0.12));
                float4 _TextureDot2_var = tex2D(_TextureDot2,TRANSFORM_TEX(node_2669, _TextureDot2));
                float2 node_4372 = (i.uv0+node_1262.g*float2(0.1,0));
                float4 _TextureRay1_var = tex2D(_TextureRay1,TRANSFORM_TEX(node_4372, _TextureRay1));
                float2 node_7971 = (i.uv0+node_1262.g*float2(-0.1,0));
                float4 _TextureRay2_var = tex2D(_TextureRay2,TRANSFORM_TEX(node_7971, _TextureRay2));
                float3 node_1725 = lerp((_Color1.rgb+_TextureRay1_var.r),(_Color2.rgb*_TextureRay2_var.r),0.5);
                float3 emissive = (((lerp(float3(node_3739,node_3739,node_3739),_ColorDot1.rgb,_TextureDot1_var.b)*_PowerDot1)+(lerp(float3(node_8751,node_8751,node_8751),_ColorDot2.rgb,_TextureDot2_var.b)*_PowerDot2))+node_1725);
                float3 finalColor = emissive;
                float4 node_3995 = _Time;
                float4 _MaskRay_var = tex2D(_MaskRay,TRANSFORM_TEX(i.uv0, _MaskRay));
                return fixed4(finalColor,lerp(0.0,lerp(_TextureRay1_var.r,_TextureRay2_var.r,clamp(sin(node_3995.g),0.2,0.8)),_MaskRay_var.g));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
