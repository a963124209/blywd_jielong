using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class MapBlockAreaPoly : MonoBehaviour {
    [SerializeField]
    public Vector2[] areaPoly = new Vector2[] { new Vector2(-20, 0), new Vector2(0, -10), new Vector2(20, 0), new Vector2(0, 10) };

    public float PolyMinX = -20;

    public float PolyMinY = -20;

    public float PolyMaxX = 20;

    public float PolyMaxY = 40;

    [HideInInspector]
    public bool initialized;

    private bool isDestroyed = false;

    public void Init()
    {
        if (initialized) return;
        initialized = true;
        

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PolyonUpdated()
    {
        //计算新的AABB边界
        float minX = -1;
        float minY = -1;
        float maxX = -1;
        float maxY = -1;
        bool isSet = true;

        float addRadius = 2;

        foreach (Vector2 tagPt in areaPoly)
        {
            if (isSet || tagPt.x < minX)
                minX = tagPt.x;
            if (isSet || tagPt.y < minY)
                minY = tagPt.y;
            if (isSet || tagPt.x > maxX)
                maxX = tagPt.x;
            if (isSet || tagPt.y > maxY)
                maxY = tagPt.y;
            isSet = false;
        }
        PolyMinX = minX - addRadius;
        PolyMinY = minY - addRadius;
        PolyMaxX = maxX + addRadius;
        PolyMaxY = maxY + addRadius;
    }

    public void AddNodePoint(Vector2 nodePos, int instIndex)
    {
        if (instIndex < 0 || instIndex >= areaPoly.Length) return;
        List<Vector2> newPtList = new List<Vector2>();
        newPtList.AddRange(areaPoly);
        newPtList.Insert(instIndex + 1, nodePos);
        areaPoly = newPtList.ToArray();

        PolyonUpdated();
    }

    public void RemoveNodePoint(int tagIndex)
    {
        if (tagIndex < 0 || tagIndex >= areaPoly.Length) return;
        List<Vector2> newPtList = new List<Vector2>();
        newPtList.AddRange(areaPoly);
        newPtList.RemoveAt(tagIndex);
        areaPoly = newPtList.ToArray();

        PolyonUpdated();
    }

}
