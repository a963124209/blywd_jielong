using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SpriteWaveEffectController))]
public class SpriteWaveEffectControllerEditor : Editor
{
    SpriteWaveEffectController bindObj;

    void OnEnable()
    {
        bindObj = (SpriteWaveEffectController)target;

        if (!bindObj.initialized) bindObj.Init();
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;

        DrawDefaultInspector();

        if (GUILayout.Button("更新设置"))
        {
            bindObj.OnEftUpdated();

            EditorUtility.SetDirty(target);
        }
        
        if (GUI.changed)
        {
            //bc.drawCurve();
        }
    }

}
