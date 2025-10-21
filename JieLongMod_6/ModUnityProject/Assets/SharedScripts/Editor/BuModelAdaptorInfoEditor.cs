using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(BuModelAdaptorInfo))]
public class BuModelAdaptorInfoEditor : Editor
{
    BuModelAdaptorInfo bindObj;

    void OnEnable()
    {
        bindObj = (BuModelAdaptorInfo)target;

        if (!bindObj.initialized) bindObj.Init();
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;

        DrawDefaultInspector();

        if (GUILayout.Button("自动绑定模型信息"))
        {
            bindObj.AutoBindModelInfo();

            EditorUtility.SetDirty(target);
        }
        
        if (GUI.changed)
        {
            //bc.drawCurve();
        }
    }

}
