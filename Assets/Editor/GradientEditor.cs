using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class GradientEditor : EditorWindow
{
    CustomGradient gradient;
    const int borderSize = 10;
    const float keyWidth = 10;
    const float keyHeight = 20;

    Rect gradientPreviewRect;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    int selectedKeyIndex;
    bool needsRepaint;

    private void OnGUI()
    {
        Draw();
        HandleInput();

        if (needsRepaint)
        {
            needsRepaint = false;
            Repaint();
        }

    }

    void Draw()
    {
        gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, 25);
        GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));

        keyRects = new Rect[gradient.numKeys];
        for (int i = 0; i < gradient.numKeys; i++)
        {
            CustomGradient.ColorKey key = gradient.GetKey(i);
            Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - keyWidth / 2f, gradientPreviewRect.yMax + borderSize, keyWidth, keyHeight);
            if (i == selectedKeyIndex)
            {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.white);
            }
            else
            {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
            }
            EditorGUI.DrawRect(keyRect, key.Color);
            keyRects[i] = keyRect;
        }

        Rect settingsRect = new Rect(borderSize, keyRects[0].yMax + borderSize, position.width - borderSize * 2, position.height);
        GUILayout.BeginArea(settingsRect);
        EditorGUI.BeginChangeCheck();
        Color newColor = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Color);
        if (EditorGUI.EndChangeCheck())
        {
            gradient.UpdateKeyColor(selectedKeyIndex, newColor);
        }
        gradient.blendMode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", gradient.blendMode);
        gradient.randomizeColor = EditorGUILayout.Toggle("Randomize Color", gradient.randomizeColor);
        GUILayout.EndArea();
    }

    void HandleInput()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            for (int i = 0; i < keyRects.Length; i++)
            {
                if (keyRects[i].Contains(guiEvent.mousePosition))
                {
                    mouseIsDownOverKey = true;
                    selectedKeyIndex = i;
                    needsRepaint = true;
                    break;
                }
            }

            if (!mouseIsDownOverKey)
            {
                float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                Color interpolatedColor = gradient.Evaluate(keyTime);

                selectedKeyIndex = gradient.AddKey((gradient.randomizeColor)?randomColor:interpolatedColor, keyTime);
                mouseIsDownOverKey = true;
                needsRepaint = true;
            }
        }
        if(guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            mouseIsDownOverKey = false;
        }

        if(mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
        {
            float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
            selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
            needsRepaint = true;
        }

        if(guiEvent.keyCode == KeyCode.Backspace && guiEvent.type == EventType.KeyDown)
        {
            gradient.RemoveKey(selectedKeyIndex);
            if(selectedKeyIndex >= gradient.numKeys)
            {
                selectedKeyIndex--;
            }
            needsRepaint = true;
        }
    }

    public void SetGradient(CustomGradient gradient)
    {
        this.gradient = gradient;
    }

    private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
        position.Set(position.x, position.y, 400, 150);
        minSize = new Vector2(250,150);
        maxSize = new Vector2(1920, 150);
    }

    private void OnDisable()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
