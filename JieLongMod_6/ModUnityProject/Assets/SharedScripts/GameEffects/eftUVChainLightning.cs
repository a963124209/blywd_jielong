using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// uv贴图闪电链
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class eftUVChainLightning : MonoBehaviour
{
    //美术资源中进行调整
    public float detail = 1;//增加后，线条数量会减少，每个线条会更长。
    public float displacement = 15;//位移量，也就是线条数值方向偏移的最大值
    public float dpf = 0.05f;
    public float inTime = 0.5f;
    public float outTime = 0.5f;
    public float keepTime = 1.0f;
    public Transform target;//链接目标
    public Transform start;
    public Vector3 offsetPos = new Vector3(0, 0, 0);

    private LineRenderer _lineRender;
    private List<Vector3> _linePosList;
    private float curFrameTimer = 0;
    private float curTilingYOffset = 0;
    private Vector2 curTiling;
    private Material curMat;
    private int curPlayState = 0;
    private float curPlayTimer = 0;

    private void Awake()
    {
        _lineRender = GetComponent<LineRenderer>();
        _linePosList = new List<Vector3>();
        if (_lineRender != null && _lineRender.material != null)
        {
            curMat = _lineRender.material;
            Vector2 texScale = curMat.GetTextureScale("_MainTex");
            curTilingYOffset = texScale.y;
        }
        curTiling = new Vector2(0, 0);
        ResetEftInfo();
    }

    private void OnEnable()
    {
        ResetEftInfo();
    }

    public void ResetEftInfo()
    {
        curPlayState = 0;
        curPlayTimer = 0;
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Time.timeScale != 0 && curPlayState < 3)
        {
            float ct = Time.deltaTime;
            if (curFrameTimer > 0)
            {
                curFrameTimer -= ct;
            }
            curPlayTimer += ct;
            if (curFrameTimer <= 0)
            {
                _linePosList.Clear();
                Vector3 startPos = Vector3.zero;
                Vector3 endPos = Vector3.zero;
                if (target != null)
                {
                    endPos = target.position + offsetPos;
                }
                if (start != null)
                {
                    startPos = start.position + offsetPos;
                }
                Vector3 curStartPos = startPos;
                Vector3 curTagPos = endPos;
                if (curPlayState == 0)
                {
                    if (curPlayTimer < inTime && inTime > 0)
                    {
                        curTagPos = Vector3.Lerp(startPos, endPos, curPlayTimer / inTime);
                    }
                    else
                    {
                        curPlayState = 1;
                        curPlayTimer = 0;
                    }
                } else if (curPlayState == 1)
                {
                    if (curPlayTimer >= keepTime)
                    {
                        curPlayState = 2;
                        curPlayTimer = 0;
                    }
                } else if (curPlayState == 2)
                {
                    if (curPlayTimer < outTime && inTime > 0)
                    {
                        curStartPos = Vector3.Lerp(startPos, endPos, curPlayTimer / inTime);
                    }
                    else
                    {
                        curPlayState = 3;
                        curPlayTimer = 0;
                        this.gameObject.SetActive(false);
                        return;
                    }
                }

                CollectLinPos(curStartPos, curTagPos, displacement);
                _linePosList.Add(curTagPos);

                _lineRender.positionCount = _linePosList.Count;
                for (int i = 0, n = _linePosList.Count; i < n; i++)
                {
                    _lineRender.SetPosition(i, _linePosList[i]);
                }
                curFrameTimer = dpf;
                curTiling.y += curTilingYOffset;
                curMat.SetTextureOffset("_MainTex", curTiling);
                if (curTiling.y >= 1)
                {
                    curTiling.y = 0;
                }
            }
        }
    }

    //收集顶点，中点分形法插值抖动
    private void CollectLinPos(Vector3 startPos, Vector3 destPos, float displace)
    {
        if (displace < detail)
        {
            _linePosList.Add(startPos);
        }
        else
        {

            float midX = (startPos.x + destPos.x) / 2;
            float midY = (startPos.y + destPos.y) / 2;
            float midZ = (startPos.z + destPos.z) / 2;

            midX += (float)(UnityEngine.Random.value - 0.5) * displace;
            midY += (float)(UnityEngine.Random.value - 0.5) * displace;
            midZ += (float)(UnityEngine.Random.value - 0.5) * displace;

            Vector3 midPos = new Vector3(midX,midY,midZ);

            CollectLinPos(startPos, midPos, displace / 2);
            CollectLinPos(midPos, destPos, displace / 2);
        }
    }


}    
