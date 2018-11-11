// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:34115,y:32685,varname:node_1873,prsc:2|emission-691-OUT;n:type:ShaderForge.SFN_Color,id:5983,x:33036,y:33011,ptovrint:False,ptlb:ColorLines,ptin:_ColorLines,varname:_ColorLines,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1054282,c2:0.920691,c3:0.9558824,c4:1;n:type:ShaderForge.SFN_Tex2d,id:5394,x:32678,y:32824,ptovrint:False,ptlb:Panner,ptin:_Panner,varname:_Panner,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d7381a6ceab222b48aae9e57a6578d9b,ntxv:0,isnm:False|UVIN-7704-UVOUT;n:type:ShaderForge.SFN_Lerp,id:8906,x:32880,y:32824,varname:node_8906,prsc:2|A-6047-OUT,B-5394-R,T-7365-G;n:type:ShaderForge.SFN_Vector1,id:6047,x:32619,y:32737,varname:node_6047,prsc:2,v1:0;n:type:ShaderForge.SFN_Panner,id:7704,x:32506,y:32824,varname:node_7704,prsc:2,spu:0,spv:-0.1|UVIN-6614-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:6614,x:32288,y:32824,varname:node_6614,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:7365,x:32683,y:33094,ptovrint:False,ptlb:Lines,ptin:_Lines,varname:_Lines,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d7381a6ceab222b48aae9e57a6578d9b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5880,x:33051,y:32824,varname:node_5880,prsc:2|A-8906-OUT,B-6948-OUT;n:type:ShaderForge.SFN_Multiply,id:9424,x:33236,y:32824,varname:node_9424,prsc:2|A-5880-OUT,B-5983-RGB;n:type:ShaderForge.SFN_Lerp,id:7533,x:33480,y:32784,varname:node_7533,prsc:2|A-1684-RGB,B-9024-OUT,T-9424-OUT;n:type:ShaderForge.SFN_Color,id:1684,x:33208,y:32614,ptovrint:False,ptlb:BG color,ptin:_BGcolor,varname:_BGcolor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07461074,c2:0.4357266,c3:0.5073529,c4:1;n:type:ShaderForge.SFN_Vector1,id:9024,x:33219,y:32768,varname:node_9024,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:6277,x:33397,y:33155,varname:node_6277,prsc:2|A-7365-B,B-6572-RGB;n:type:ShaderForge.SFN_Color,id:6572,x:33204,y:33218,ptovrint:False,ptlb:Colorglow,ptin:_Colorglow,varname:_ColorLines_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1793361,c2:0.7867647,c3:0.434875,c4:1;n:type:ShaderForge.SFN_Add,id:691,x:33661,y:32784,varname:node_691,prsc:2|A-7533-OUT,B-6277-OUT;n:type:ShaderForge.SFN_Slider,id:6948,x:32645,y:33009,ptovrint:False,ptlb:LineIntensity,ptin:_LineIntensity,varname:node_6948,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;proporder:7365-5394-5983-1684-6572-6948;pass:END;sub:END;*/

Shader "Shader Forge/M_background" {
    Properties {
        _Lines ("Lines", 2D) = "white" {}
        _Panner ("Panner", 2D) = "white" {}
        _ColorLines ("ColorLines", Color) = (0.1054282,0.920691,0.9558824,1)
        _BGcolor ("BG color", Color) = (0.07461074,0.4357266,0.5073529,1)
        _Colorglow ("Colorglow", Color) = (0.1793361,0.7867647,0.434875,1)
        _LineIntensity ("LineIntensity", Range(0, 5)) = 2
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
            "RenderType"="Opaque"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            uniform float4 _ColorLines;
            uniform sampler2D _Panner; uniform float4 _Panner_ST;
            uniform sampler2D _Lines; uniform float4 _Lines_ST;
            uniform float4 _BGcolor;
            uniform float4 _Colorglow;
            uniform float _LineIntensity;
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
                float node_9024 = 1.0;
                float4 node_1815 = _Time;
                float2 node_7704 = (i.uv0+node_1815.g*float2(0,-0.1));
                float4 _Panner_var = tex2D(_Panner,TRANSFORM_TEX(node_7704, _Panner));
                float4 _Lines_var = tex2D(_Lines,TRANSFORM_TEX(i.uv0, _Lines));
                float3 emissive = (lerp(_BGcolor.rgb,float3(node_9024,node_9024,node_9024),((lerp(0.0,_Panner_var.r,_Lines_var.g)*_LineIntensity)*_ColorLines.rgb))+(_Lines_var.b*_Colorglow.rgb));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
