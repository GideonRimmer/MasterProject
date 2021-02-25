using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CompositeBehaviorDynamic))]
public class CompositeBehaviorEditorDynamic : Editor
{
    public override void OnInspectorGUI()
    {
        CompositeBehaviorDynamic compBehavior = (CompositeBehaviorDynamic)target;

        // Check for behaviours
        if (compBehavior.behaviors == null || compBehavior.behaviors.Length == 0)
        {
            EditorGUILayout.HelpBox("No behaviours in array.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            //EditorGUILayout.LabelField("Number", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
            EditorGUILayout.LabelField("Behaviors", GUILayout.MinWidth(60f));
            EditorGUILayout.LabelField("Weights", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < compBehavior.behaviors.Length; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.MaxWidth(15f));
                compBehavior.behaviors[i] = (FlockBehaviorDynamic)EditorGUILayout.ObjectField(compBehavior.behaviors[i], typeof(FlockBehaviorDynamic), false, GUILayout.MinWidth(60f));
                compBehavior.weights[i] = EditorGUILayout.FloatField(compBehavior.weights[i], GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(compBehavior);
            }
        }

        GUILayout.Space(10f);
        // Buttons to add and remove behaviours in our containers
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Add"))
        {
            AddBehavior(compBehavior);
            EditorUtility.SetDirty(compBehavior);
        }
        if (GUILayout.Button("Remove"))
        {
            RemoveBehavior(compBehavior);
            EditorUtility.SetDirty(compBehavior);
        }
        EditorGUILayout.EndVertical();

        void AddBehavior(CompositeBehaviorDynamic cb)
        {
            int oldCount = (cb.behaviors != null) ? cb.behaviors.Length : 0;
            FlockBehaviorDynamic[] newBehaviors = new FlockBehaviorDynamic[oldCount + 1];
            float[] newWeights = new float[oldCount + 1];
            for (int i = 0; i < oldCount; i++)
            {
                newBehaviors[i] = cb.behaviors[i];
                newWeights[i] = cb.weights[i];
            }
            newWeights[oldCount] = 1f;
            cb.behaviors = newBehaviors;
            cb.weights = newWeights;
        }

        void RemoveBehavior(CompositeBehaviorDynamic cb)
        {
            int oldCount = cb.behaviors.Length;
            if (oldCount == 1)
            {
                cb.behaviors = null;
                cb.weights = null;
                return;
            }
            FlockBehaviorDynamic[] newBehaviors = new FlockBehaviorDynamic[oldCount - 1];
            float[] newWeights = new float[oldCount - 1];
            for (int i = 0; i < oldCount - 1; i++)
            {
                newBehaviors[i] = cb.behaviors[i];
                newWeights[i] = cb.weights[i];
            }
            cb.behaviors = newBehaviors;
            cb.weights = newWeights;
        }
    }
}