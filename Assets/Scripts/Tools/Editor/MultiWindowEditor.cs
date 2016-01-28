using UnityEngine;
using UnityEditor;
using System.Collections;

public class MultiWindowEditor : Editor
{
    private Rect _headerRect;
    private Rect _screenRect;
    private Rect _avalibleRect;
    private Rect _mainRect;
    private Rect _auxRect;

    private float _footerHeight = 45;
    private float _inspectorPercent = 0.7f;
    private float _windowOffset = 2;
    private float _padding = 4;

    protected void SetupMultiWindow(float percent)
    {
        _inspectorPercent = percent;

        _footerHeight = 45;
        _windowOffset = 2;
        _padding = 4;
    }

    protected override void OnHeaderGUI()
    {
        EditorGUILayout.BeginVertical();
        OnMultiHeaderGUI();
        EditorGUILayout.EndVertical();

        if (Event.current.type == EventType.Repaint)
        {
            _headerRect = GUILayoutUtility.GetLastRect();
        }
    }

    public override void OnInspectorGUI()
    {
        if(_inspectorPercent > 0.95f)
        {
            _inspectorPercent = 0.95f;
        }

        if (Event.current.type == EventType.Layout)
        {
            _screenRect     = new Rect(0, 0, Screen.width, Screen.height);
            _avalibleRect   = new Rect(_screenRect.x + _padding, _screenRect.y + _headerRect.height + _padding, _screenRect.width - _padding * 2, _screenRect.height - _headerRect.height - _footerHeight - _padding * 2);

            float mainInspectorHeight = Mathf.CeilToInt(_avalibleRect.height * _inspectorPercent)- _windowOffset;
            float auxInspectorHeight  = Mathf.CeilToInt(_avalibleRect.height * (1 - _inspectorPercent)- _windowOffset);

            _mainRect       = new Rect(_avalibleRect.x, _avalibleRect.y, _avalibleRect.width, mainInspectorHeight);
            _auxRect        = new Rect(_avalibleRect.x, _avalibleRect.y + mainInspectorHeight+ _windowOffset*2, _avalibleRect.width, auxInspectorHeight);
        }


        OnMultiInspectorMainGUI(_mainRect);
        OnMultiInspectorAuxGUI(_auxRect);

    }

    protected virtual void OnMultiHeaderGUI()
    {
        //Overridable
    }
    protected virtual void OnMultiInspectorMainGUI(Rect totalArea)
    {
        //Overridable
    }
    protected virtual void OnMultiInspectorAuxGUI(Rect totalArea)
    {
        //Overridable
    }
}