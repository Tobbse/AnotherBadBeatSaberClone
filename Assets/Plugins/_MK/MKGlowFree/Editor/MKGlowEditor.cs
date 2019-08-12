//////////////////////////////////////////////////////
// MK Glow Editor Legacy	    			        //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MK.Glow.Editor.Legacy
{
	using Tooltips = EditorHelper.EditorUIContent.Tooltips;

    [CustomEditor(typeof(MK.Glow.Legacy.MKGlow))]
    public class MKGlowEditor : UnityEditor.Editor
	{
		//Main
		private SerializedProperty _debugView;
		private SerializedProperty _workflow;
		private SerializedProperty _selectiveRenderLayerMask;

		//Bloom
		private SerializedProperty _bloomThreshold;
		private SerializedProperty _bloomScattering;
		private SerializedProperty _bloomIntensity;
		
		public void OnEnable()
		{
			//Main
			_debugView = serializedObject.FindProperty("debugView");
			_workflow = serializedObject.FindProperty("workflow");
			_selectiveRenderLayerMask = serializedObject.FindProperty("selectiveRenderLayerMask");

			//Bloom
			_bloomThreshold = serializedObject.FindProperty("bloomThreshold");
			_bloomScattering = serializedObject.FindProperty("bloomScattering");
			_bloomIntensity = serializedObject.FindProperty("bloomIntensity");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorHelper.VerticalSpace();

			EditorHelper.EditorUIContent.XRUnityVersionWarning();
			
			EditorGUILayout.PropertyField(_debugView, Tooltips.debugView);
			EditorGUILayout.PropertyField(_workflow, Tooltips.workflow);
			EditorHelper.EditorUIContent.SelectiveWorkflowVRWarning((Workflow)_workflow.enumValueIndex);
			if(_workflow.enumValueIndex == 1)
			{
				EditorGUILayout.PropertyField(_selectiveRenderLayerMask, Tooltips.selectiveRenderLayerMask);
			}
			
			if(_workflow.enumValueIndex == 0)
				EditorGUILayout.PropertyField(_bloomThreshold, Tooltips.bloomThreshold);
			EditorGUILayout.PropertyField(_bloomScattering, Tooltips.bloomScattering);
			EditorGUILayout.PropertyField(_bloomIntensity, Tooltips.bloomIntensity);
			_bloomIntensity.floatValue = Mathf.Max(0, _bloomIntensity.floatValue);

			serializedObject.ApplyModifiedProperties();
		}
    }
}
#endif