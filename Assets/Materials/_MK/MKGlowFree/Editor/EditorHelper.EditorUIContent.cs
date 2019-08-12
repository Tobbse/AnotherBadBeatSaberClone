//////////////////////////////////////////////////////
// MK Glow Editor Helper UI Content	           		//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MK.Glow.Editor
{
    internal static partial class EditorHelper
    {
        internal static class EditorUIContent
        {
            internal static class Tooltips
            {
                //Main
                internal static readonly GUIContent debugView = new GUIContent("Debug View", "Displaying of different render steps. \n \n" +
                                                                               "None: Debug view is disabled. \n\n" +
                                                                               "Raw Bloom: Shows extracted bloom map. \n\n" +
                                                                               "Bloom: Shows created bloom without lens surface. \n\n"
                                                                               );
                internal static readonly GUIContent workflow = new GUIContent("Workflow", "Basic definition of the workflow. \n\n" +
                                                                              "Luminance: Glow map is defined by the pixels brightness and threshold setup. Just use the emission of the shaders and raise it up. This should be chosen in most cases. Performs significantly faster than selective workflow.\n\n" +
                                                                               "Selective: Glow map is created by using separate shaders (MK/Glow/Selective).");
                internal static readonly GUIContent selectiveRenderLayerMask = new GUIContent("Render Layer", "In most cases 'Everything' should be chosen to avoid Z issues.");

                //Bloom
                internal static readonly GUIContent bloomThreshold = new GUIContent("Threshold", "Threshold in gamma space for extraction of bright areas. \n\n Min: Minimum brightness until the bloom starts. \n Max: Maximum brightness for cutting off colors.");
                internal static readonly GUIContent bloomScattering = new GUIContent("Scattering", "Scattering of the bloom. A higher value increases the scattered area.");
                internal static readonly GUIContent bloomIntensity = new GUIContent("Intensity", "Intensity of the bloom in gamma space.");
            }

            internal static readonly string mainTitle = "Main";
            internal static readonly string bloomTitle = "Bloom";
            internal static readonly string lensSurfaceTitle = "Lens Surface";
            internal static readonly string dirtTitle = "Dirt:";
            internal static readonly string diffractionTitle = "Diffraction:";
            internal static readonly string lensFlareTitle = "Lens Flare (SM 3.0+)";
            internal static readonly string ghostsTitle = "Ghosts:";
            internal static readonly string haloTitle = "Halo:";
            internal static readonly string glareTitle = "Glare (SM 4.0+)";
            internal static readonly string sample0Title = "Sample 0:";
            internal static readonly string sample1Title = "Sample 1:";
            internal static readonly string sample2Title = "Sample 2:";
            internal static readonly string sample3Title = "Sample 3:";

            internal static void XRUnityVersionWarning()
            {
                #if UNITY_2018_3_OR_NEWER
                #else
                if(PlayerSettings.virtualRealitySupported)
                {
                    EditorGUILayout.HelpBox("Your are currently targeting XR. For best XR support its recommend to update to unity 2018.3 or higher.", MessageType.Warning);
                }
                #endif
            }

            internal static void SelectiveWorkflowVRWarning(Workflow workflow)
            {
                if(UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null && workflow == Workflow.Selective)
                {
                    EditorGUILayout.HelpBox("Selective workflow isn't supported if a scriptable rendering pipeline is active. Please use Luminance workflow instead. ", MessageType.Warning);
                }
                if(PlayerSettings.virtualRealitySupported && workflow == Workflow.Selective)
                {
                    EditorGUILayout.HelpBox("Selective workflow isn't supported in XR. Please use Luminance workflow instead. ", MessageType.Warning);
                }
            }
        }
    }
}
#endif