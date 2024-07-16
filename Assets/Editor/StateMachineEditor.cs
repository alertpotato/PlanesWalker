using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor
{
    private SerializedProperty currentStateProperty;
    void OnEnable()
    {
        currentStateProperty = serializedObject.FindProperty("_currentState");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(currentStateProperty, new GUIContent("Current State"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            (target as StateMachine).CurrentState = currentStateProperty.objectReferenceValue as StateBehaviour;
        }
    }
}
