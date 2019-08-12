//////////////////////////////////////////////////////
// MK Glow Editor Helper Main  	    	       		//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MK.Glow.Editor
{
    internal static partial class EditorHelper
    {   
        /// <summary>
        /// Creates a empty space with the height of 1
        /// </summary>
        internal static void VerticalSpace()
        {
            GUILayoutUtility.GetRect(1f, EditorGUIUtility.standardVerticalSpacing);
        }

        /// <summary>
        /// Draws a header
        /// </summary>
        /// <param name="text"></param>
        internal static void DrawHeader(string text)
        {
            EditorGUILayout.LabelField(text, UnityEditor.EditorStyles.boldLabel);
        }
	}
}
#endif
