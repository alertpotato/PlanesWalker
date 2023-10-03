using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Battlefield))]
public class BattlefieldGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Field"))
        {
            Battlefield battle = (Battlefield)target;
            battle.InicializeHexField(9,6);
        }
    }
}