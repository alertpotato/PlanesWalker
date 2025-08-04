using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    private const int cellSize = 30;
    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }
    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawGrid();
    }
    private void DrawGrid()
    {
        base.OnInspectorGUI(); // Draw default inspector

        GridManager gridManager = (GridManager)target;
        var grid = gridManager.GetGrid();
        int width = gridManager.Width;
        int height = gridManager.Height;


        EditorGUILayout.LabelField("Grid Preview", EditorStyles.boldLabel);
        var bound = gridManager.GetGridBounds();
        for (int y = bound.maxY+1; y >= bound.minY-1; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = bound.minX-1; x <= bound.maxX+1; x++)
            {
                (int x, int y) pos = (x, y);
                string label = $"{pos.x}:{pos.y}";

                if (grid.TryGetValue(pos, out var company))
                {
                    if (company.Unit != null)
                        label = company.Unit.name.Length > 3 ? company.Unit.name.Substring(0, 3) : company.Unit.name;
                    else label="EMP";
                }
                
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fixedWidth = cellSize;
                style.fixedHeight = cellSize;
                style.fontSize = 8;

                if (grid.ContainsKey(pos))
                {
                    Color originalColor = GUI.backgroundColor;
                    GUI.backgroundColor =
                        company.unitOwner.heroName == "Planeswalker" ? Color.red :
                        Color.blue;
                    GUILayout.Button(label, style);
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    GUILayout.Button(label, style);
                }

            }
            EditorGUILayout.EndHorizontal();
        }
    }
}