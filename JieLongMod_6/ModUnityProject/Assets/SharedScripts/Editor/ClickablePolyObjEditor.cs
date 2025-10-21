using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ClickablePolyObj))]
public class ClickablePolyObjEditor : Editor
{
    public class InsertNodePointSolution
    {
        public Vector2 resultPos;
        public int nodeIndexA;
        public int nodeIndexB;

        public InsertNodePointSolution(Vector2 _pos, int _a, int _b)
        {
            resultPos = _pos;
            nodeIndexA = _a;
            nodeIndexB = _b;
        }
    }

    ClickablePolyObj bindObj;

    private bool isEditPolygon = false;

    private bool isAddingNode = false;
    private bool isRemovingNode = false;

    private const float ControlPointSize = 5.0f;

    private bool isPointPosMoved = false;

    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;

    private InsertNodePointSolution curTestInsertSolution = null;

    private Color testNodeColor = new Color(1, 0, 0, 0.6f);
    private float testNodeSize = 5;
    private Vector2 testNodeSnap = Vector2.zero;
    private Handles.CapFunction testNodeCap = Handles.DotHandleCap;

    public const float EDITOR_POINT_THRESHOLD = 8.0f;

    void OnEnable()
    {
        bindObj = (ClickablePolyObj)target;

        if (!bindObj.initialized) bindObj.Init();
    }

    public override void OnInspectorGUI()
    {
        GUI.changed = false;

        if (ToggleButtonStyleNormal == null)
        {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
            ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
        }

        DrawDefaultInspector();

        if (GUILayout.Button("编辑多边形", isEditPolygon ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
        {
            isEditPolygon = !isEditPolygon;
            isAddingNode = false;
            isRemovingNode = false;
            if (isEditPolygon)
            {
                Undo.RegisterFullObjectHierarchyUndo(bindObj, "Start Edit Polygon");
            }
            isPointPosMoved = false;
            EditorUtility.SetDirty(target);
        }

        if (isEditPolygon)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("添加节点", isAddingNode ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                isAddingNode = !isAddingNode;
                isRemovingNode = false;
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("移除节点", isRemovingNode ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                isRemovingNode = !isRemovingNode;
                isAddingNode = false;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();
        }
        
        if (GUI.changed)
        {
            //bc.drawCurve();
        }
    }

    public void GetTestInsertPoint(Vector2 tagMapPos, float _rad)
    {
        Vector2 nodePos;
        for (int i = 0; i < bindObj.detectPoly.Length; i++)
        {
            nodePos = bindObj.detectPoly[i];
            if ((nodePos - tagMapPos).magnitude <= EDITOR_POINT_THRESHOLD)
            {
                curTestInsertSolution = null;
                return;
            }
        }

        Vector3 orgPos = bindObj.transform.position;
        Vector3 tagPointPos;
        Vector3 nextPointPos;
        int nextIndex = 0;
        for (int i = 0; i < bindObj.detectPoly.Length; i++)
        {
            tagPointPos = bindObj.detectPoly[i];
            tagPointPos = tagPointPos + orgPos;
            if (i < bindObj.detectPoly.Length - 1)
            {
                nextIndex = i + 1;
            }
            else
            {
                nextIndex = 0;
            }
            nextPointPos = bindObj.detectPoly[nextIndex];

            Vector2 posA = tagPointPos;
            Vector2 posB = nextPointPos;
            Vector2 footPt = SimpleMathLib.GetFootPoint(tagMapPos, posA, posB);
            footPt = new Vector2(Mathf.Clamp(footPt.x, Mathf.Min(posA.x, posB.x), Mathf.Max(posA.x, posB.x)),
                Mathf.Clamp(footPt.y, Mathf.Min(posA.y, posB.y), Mathf.Max(posA.y, posB.y)));
            if ((footPt - tagMapPos).magnitude <= _rad)
            {
                curTestInsertSolution = new InsertNodePointSolution(footPt, i, nextIndex);
                return;
            }
        }
        curTestInsertSolution = null;
    }

    public int GetTestPoint(Vector2 tagMapPos)
    {
        Vector2 nodePos;
        for (int i = 0; i < bindObj.detectPoly.Length; i++)
        {
            nodePos = bindObj.detectPoly[i];
            if ((nodePos - tagMapPos).magnitude <= EDITOR_POINT_THRESHOLD)
            {
                return i;
            }
        }
        return -1;
    }

    void Input()
    {
        if (!isEditPolygon)
            return;
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        mousePos = bindObj.transform.InverseTransformPoint(mousePos);

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            if (isPointPosMoved)
            {
                Undo.RecordObject(bindObj, "Move point");
                isPointPosMoved = false;
            }
            return;
        }

        if (isAddingNode)
        {
            GetTestInsertPoint(mousePos, 8);
            if (curTestInsertSolution != null)
            {
                if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
                {
                    Vector3 tagRetPos = curTestInsertSolution.resultPos;
                    Vector2 tagOffsetPos = tagRetPos - bindObj.transform.position;
                    bindObj.AddNodePoint(tagOffsetPos, curTestInsertSolution.nodeIndexA);
                    Undo.RecordObject(bindObj, "Insert point");

                    Repaint();
                    return;
                }
                else
                {
                    Handles.color = testNodeColor;
                    testNodeSize = HandleUtility.GetHandleSize(curTestInsertSolution.resultPos) * 0.1f;
                    Handles.FreeMoveHandle(GUIUtility.GetControlID(FocusType.Passive), curTestInsertSolution.resultPos, Quaternion.identity, testNodeSize, testNodeSnap, testNodeCap);
                }
            }
        }
        else if (isRemovingNode)
        {
            int tagIndex = GetTestPoint(mousePos);
            if (tagIndex >= 0)
            {
                if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
                {
                    bindObj.RemoveNodePoint(tagIndex);
                    Undo.RecordObject(bindObj, "Remove point");

                    Repaint();
                    return;
                } else
                {
                    Handles.color = testNodeColor;
                    Vector2 orgPos = bindObj.transform.position;
                    Vector2 tagPointPos = bindObj.detectPoly[tagIndex] + orgPos;
                    testNodeSize = HandleUtility.GetHandleSize(tagPointPos) * 0.1f;
                    Handles.FreeMoveHandle(GUIUtility.GetControlID(FocusType.Passive), tagPointPos, Quaternion.identity, testNodeSize, testNodeSnap, testNodeCap);
                }
            }
        }
    }

    void Draw()
    {
        var snap = Vector2.zero;
        Handles.CapFunction cap = Handles.DotHandleCap;
        Handles.color = Color.yellow;

        Vector3 orgPos = bindObj.transform.position;
        Vector3 tagPointPos;
        Vector3 nextPointPos;
        float handleSize = 1;
        for (int i = 0; i < bindObj.detectPoly.Length; i++)
        {
            tagPointPos = bindObj.detectPoly[i];
            tagPointPos = tagPointPos + orgPos;
            if (i < bindObj.detectPoly.Length - 1)
            {
                nextPointPos = bindObj.detectPoly[i + 1];
            }
            else
            {
                nextPointPos = bindObj.detectPoly[0];
            }
            nextPointPos = nextPointPos + orgPos;
            Handles.DrawLine(tagPointPos, nextPointPos);

            if (isEditPolygon)
            {
                handleSize = HandleUtility.GetHandleSize(tagPointPos) * .05f;
                int ctrlId = GUIUtility.GetControlID(FocusType.Passive);
                Vector3 newPos = Handles.FreeMoveHandle(ctrlId, tagPointPos, Quaternion.identity, handleSize, snap, cap);
                if (newPos.x != tagPointPos.x || newPos.y != tagPointPos.y)
                {
                    bindObj.detectPoly[i] = newPos - orgPos;
                    isPointPosMoved = true;
                    PolyUpdated();
                    Repaint();
                }
            }
        }
        
    }

    private void PolyUpdated()
    {
        bindObj.PolyonUpdated();
        Vector2[] tempList = new Vector2[bindObj.detectPoly.Length];
        for (int i = 0; i < bindObj.detectPoly.Length; i++)
        {
            tempList[i] = bindObj.detectPoly[i];
        }
        var so = serializedObject.FindProperty("detectPoly");
        so.ClearArray();
        for (int i = 0; i < tempList.Length; i++)
        {
            so.InsertArrayElementAtIndex(i);
            var newSp = so.GetArrayElementAtIndex(i);
            newSp.vector2Value = tempList[i];
        }
        serializedObject.FindProperty("PolyMinX").floatValue = bindObj.PolyMinX;
        serializedObject.FindProperty("PolyMinY").floatValue = bindObj.PolyMinY;
        serializedObject.FindProperty("PolyMaxX").floatValue = bindObj.PolyMaxX;
        serializedObject.FindProperty("PolyMaxY").floatValue = bindObj.PolyMaxY;
        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        Input();
        Draw();
    }


}
