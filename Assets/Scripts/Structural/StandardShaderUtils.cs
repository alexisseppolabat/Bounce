 using System;
 using UnityEngine;

 namespace Structural {
     public static class StandardShaderUtils {
         private static readonly int Color = Shader.PropertyToID("_Color");
         private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
         private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
         private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

         public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode) {
             switch (blendMode) {
                 case BlendMode.Opaque:
                     standardShaderMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                     standardShaderMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
                     standardShaderMaterial.SetInt(ZWrite, 1);
                     standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                     standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                     standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                     standardShaderMaterial.renderQueue = -1;
                     break;
                 case BlendMode.Cutout:
                     standardShaderMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                     standardShaderMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
                     standardShaderMaterial.SetInt(ZWrite, 1);
                     standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                     standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                     standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                     standardShaderMaterial.renderQueue = 2450;
                     break;
                 case BlendMode.Fade:
                     standardShaderMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                     standardShaderMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                     standardShaderMaterial.SetInt(ZWrite, 0);
                     standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                     standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                     standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                     standardShaderMaterial.renderQueue = 3000;
                     break;
                 case BlendMode.Transparent:
                     standardShaderMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                     standardShaderMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                     standardShaderMaterial.SetInt(ZWrite, 0);
                     standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                     standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                     standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                     standardShaderMaterial.renderQueue = 3000;
                     break;
             }
         }

         public static void SetTransparency(Material standardShaderMaterial, float a) {
             bool isOpaque = Math.Abs(a - 1f) < 0.1f;
             
             ChangeRenderMode(standardShaderMaterial, isOpaque ? BlendMode.Opaque : BlendMode.Transparent);
             standardShaderMaterial.SetColor(
                 Color,
                 new Color(
                     standardShaderMaterial.color.r,
                     standardShaderMaterial.color.g,
                     standardShaderMaterial.color.b, 
                     isOpaque ? 1 : a
                 )
             );
         }
     }
 }