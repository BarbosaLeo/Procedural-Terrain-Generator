using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomGradient))]
public class GradientDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Event guiEvent = Event.current;

        CustomGradient gradient = (CustomGradient)fieldInfo.GetValue(property.serializedObject.targetObject);
        float labelWidth = GUI.skin.label.CalcSize(label).x + 5;
        Rect textureRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

        if (guiEvent.type == EventType.Repaint) {
            GUI.Label(position, label);
            GUIStyle gradientStyle = new GUIStyle();
            gradientStyle.normal.background = gradient.GetTexture((int)position.width);
            GUI.Label(textureRect, GUIContent.none, gradientStyle);
        }
        else
        {
            if(guiEvent.button == 0)
            {
                if (textureRect.Contains(guiEvent.mousePosition))
                {
                    GradientEditor window = EditorWindow.GetWindow<GradientEditor>();
                    window.SetGradient(gradient);
                }
            }
        }

        
        
    }
}
