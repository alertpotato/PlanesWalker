#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
/*
[CustomPropertyDrawer(typeof(Unit))]
public class UnitDrawer : PropertyDrawer
{
}
*/
[CustomPropertyDrawer(typeof(UnitAbility))]
public class AbilityPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        // Foldout with ability name
        var abilityName = property.FindPropertyRelative("AbilityName");
        var displayName = abilityName != null ? abilityName.stringValue : "Ability";
        if (string.IsNullOrEmpty(displayName)) displayName = "Unnamed Ability";
        
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, displayName, true);
        
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            
            EditorGUI.PropertyField(rect, abilityName);
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            
            var description = property.FindPropertyRelative("AbilityDescription");
            if (description != null)
            {
                EditorGUI.PropertyField(rect, description);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }
            var unitCompany = property.FindPropertyRelative("UnitCompany");
            if (unitCompany != null)
            {
                EditorGUI.PropertyField(rect, unitCompany);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                if (unitCompany.isExpanded)
                {
                    rect.y += EditorGUIUtility.singleLineHeight * 3;
                }
            }
            var tags = property.FindPropertyRelative("Tags");
            if (tags != null)
            {
                EditorGUI.PropertyField(rect, tags);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                if (tags.isExpanded)
                {
                    rect.y += EditorGUIUtility.singleLineHeight * 3;
                }
            }
            var retaliationTags = property.FindPropertyRelative("RetaliationTags");
            if (retaliationTags != null)
            {
                EditorGUI.PropertyField(rect, retaliationTags);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                if (retaliationTags.isExpanded)
                {
                    rect.y += EditorGUIUtility.singleLineHeight * 3;
                }
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight * 8 + 10; // 5 fields + foldout + spacing
        }
        return EditorGUIUtility.singleLineHeight;
    }
}
#endif