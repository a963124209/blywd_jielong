using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ClickablePolyObj : MonoBehaviour {

    public SpriteRenderer[] HighlightTargets;

    public Vector2[] detectPoly = new Vector2[] { new Vector2(-20, -20), new Vector2(20, -20), new Vector2(20, 40), new Vector2(-20, 40) };

    public float PolyMinX = -20;

    public float PolyMinY = -20;

    public float PolyMaxX = 20;

    public float PolyMaxY = 40;

    [HideInInspector]
    public bool initialized;

    private bool isReplacedShader = false;

    private static bool isInitedShaderPropID = false;
    private static int sidSpriteBright;

    private float curBrightVal = 1;

    private Tween brightTween = null;
    private Sequence seqHighlightAni = null;

    private bool isHighlight = false;

    private List<Shader> oldShaders = null;

    private bool isDestroyed = false;

    [HideInInspector]
    public float BrightVal
    {
        get
        {
            return curBrightVal;
        }
        set
        {
            SetBrightVal(value);
        }
    }

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

        foreach (Vector2 tagPt in detectPoly)
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
        if (instIndex < 0 || instIndex >= detectPoly.Length) return;
        List<Vector2> newPtList = new List<Vector2>();
        newPtList.AddRange(detectPoly);
        newPtList.Insert(instIndex + 1, nodePos);
        detectPoly = newPtList.ToArray();

        PolyonUpdated();
    }

    public void RemoveNodePoint(int tagIndex)
    {
        if (tagIndex < 0 || tagIndex >= detectPoly.Length) return;
        List<Vector2> newPtList = new List<Vector2>();
        newPtList.AddRange(detectPoly);
        newPtList.RemoveAt(tagIndex);
        detectPoly = newPtList.ToArray();

        PolyonUpdated();
    }

    private void ReplaceShader()
    {
        if (isDestroyed) return;
        if (!isReplacedShader)
        {
            if (HighlightTargets != null && HighlightTargets.Length > 0)
            {
                Shader tagShader = Shader.Find("MyShaders/HighlightSprite");
                if (tagShader != null) {
                    oldShaders = new List<Shader>();
                    for (int i = 0; i < HighlightTargets.Length; i++)
                    {
                        if (HighlightTargets[i] == null) continue;
                        oldShaders.Add(HighlightTargets[i].material.shader);
                        HighlightTargets[i].material.shader = tagShader;
                    }
                }
                else
                {
                    Debug.LogError("Shader load failed, ShaderPath = MyShaders/HighlightSprite");
                }
            }

            if (!isInitedShaderPropID)
            {
                sidSpriteBright = Shader.PropertyToID("_Bright");
                isInitedShaderPropID = true;
            }

            isReplacedShader = true;
        }
    }

    private void ResetOldShader()
    {
        if (isDestroyed) return;
        if (isReplacedShader)
        {
            if (oldShaders != null && oldShaders.Count > 0)
            {
                for (int i = 0; i < HighlightTargets.Length; i++)
                {
                    if (HighlightTargets[i] == null) continue;
                    oldShaders.Add(HighlightTargets[i].material.shader);
                    if (i < oldShaders.Count)
                    {
                        HighlightTargets[i].material.shader = oldShaders[i];
                    }
                }
            }
            oldShaders = null;
            isReplacedShader = false;
        }
    }

    public void SetBrightVal(float tagVal)
    {
        if (isDestroyed) return;
        curBrightVal = tagVal;
        if (HighlightTargets != null && HighlightTargets.Length > 0)
        {
            ReplaceShader();
            for (int i = 0; i < HighlightTargets.Length; i++)
            {
                if (HighlightTargets[i] == null) continue;
                HighlightTargets[i].material.SetFloat(sidSpriteBright, tagVal);
            }
        }
    }

    public void SetHighlight(bool _isLight)
    {
        if (isDestroyed) return;
        if (brightTween != null)
        {
            brightTween.Kill(true);
            brightTween = null;
        }
        float tagVal = 1;
        if (_isLight)
        {
            tagVal = 1.5f;
        }
        brightTween = DOTween.To(() => BrightVal, x => BrightVal = x, tagVal, 0.3f).OnComplete(onSetHighlightDone);
        isHighlight = _isLight;
    }

    public void onSetHighlightDone()
    {
        if (isDestroyed) return;
        if (isHighlight)
        {

        } else
        {
            ResetOldShader();
        }
    }

    public bool IsHighlight()
    {
        return isHighlight;
    }

    public void PlayHighlightAnimat()
    {
        isHighlight = false;
        BrightVal = 1;
        if (seqHighlightAni != null)
        {
            seqHighlightAni.Kill(true);
            seqHighlightAni = null;
        }
        seqHighlightAni = DOTween.Sequence();
        seqHighlightAni.Append(DOTween.To(() => BrightVal, x => BrightVal = x, 2, 0.3f));
        seqHighlightAni.Append(DOTween.To(() => BrightVal, x => BrightVal = x, 1, 0.3f));
        seqHighlightAni.SetLoops(1);
        seqHighlightAni.OnComplete(onSetHighlightDone);
    }

    private void OnDestroy()
    {
        if (seqHighlightAni != null)
        {
            seqHighlightAni.Kill();
            seqHighlightAni = null;
        }
        isDestroyed = true;
    }
}
