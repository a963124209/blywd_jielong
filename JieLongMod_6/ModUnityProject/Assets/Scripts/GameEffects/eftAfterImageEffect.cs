using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using HanFramework;

/// <summary>
/// 残影特效
/// </summary>
public class eftAfterImageEffect : MonoBehaviour
{
    public class BuModelController
    {
        public SkinnedMeshRenderer[] smrList = null;
        public MeshFilter[] mfList = null;
    }

    class AfterImage
    {
        public Mesh mesh;
        public Matrix4x4 matrix;
        public float curTimer = 0;
        public float duration;
        public float alpha;
        public float startAlpha;
        public bool needRemove = false;
    }

    public float StartAlpha = 0.75f;
    public float GenInterval = 0.15f;
    public float FadeTime = 1.0f;
    public Color EftColor = new Color(1, 1, 1, 1);
    public bool isEnableOnAwake = false;
    public SkinnedMeshRenderer[] DefinedSmrList = null;
    public MeshFilter[] DefinedMfList = null;

    private bool isEnable = false;
    private float curGenTimer = 0;
    private int sidAlpha = 0;
    private int sidColor = 0;

    private List<AfterImage> _imageList = new List<AfterImage>();

    private BuModelController bindController = null;

    private SkinnedMeshRenderer[] smrList = null;
    private MeshFilter[] mfList = null;

    private Shader eftShader = null;
    private Material[] eftMaterialSteps = null;
    private int curMaxMaterialSteps = 5;

    private float curGenImageInterval = 1.0f;
    private bool isInitedEft = false;

    private bool isDisabled = false;

    void Awake()
    {
        if (isEnableOnAwake)
        {
            SetEnable(true);
        }

        if (!isInitedEft)
        {
            InitEffect();
        }
    }

    public void InitEffect()
    {
        if (QualitySettings.GetQualityLevel() <= 0)
        {
            isDisabled = true;
            SetEnable(false);
            return;
        }
        else
        {
            isDisabled = false;
        }

        eftShader = Shader.Find("MyShaders/MyGameBlendVision");
        if (eftShader == null)
        {
            //GlobalUI.Instance.LogError("Shader load failed, ShaderPath = MyShaders/MyGameBlendVision");
        }

        sidAlpha = Shader.PropertyToID("_Alpha");
        sidColor = Shader.PropertyToID("_Color");

        int qualityLevel = QualitySettings.GetQualityLevel();
        int maxQLevel = 0;//GameSettings.MAX_QUALITY_LEVEL;
        curGenImageInterval = GenInterval * (1.0f + (1.0f - Mathf.Clamp((float)qualityLevel / (float)maxQLevel, 0, 1.0f)));
        
        curMaxMaterialSteps = 4 + qualityLevel;

        eftMaterialSteps = new Material[curMaxMaterialSteps];
        float tagAlpha = 0;
        for (int i = 0; i < curMaxMaterialSteps; i++)
        {
            tagAlpha = Mathf.Clamp((1.0f - (float)i / (float)curMaxMaterialSteps), 0, 1.0f);
            tagAlpha = tagAlpha * StartAlpha;
            eftMaterialSteps[i] = new Material(eftShader);
            eftMaterialSteps[i].SetFloat(sidAlpha, tagAlpha);
            eftMaterialSteps[i].SetColor(sidColor, EftColor);
        }

        isInitedEft = true;
    }

    public Material GetStepMaterialByAlpha(float _alpha)
    {
        if (eftMaterialSteps == null) return null;
        int tagInex = (int)(curMaxMaterialSteps * (1.0f - _alpha / StartAlpha));
        if (tagInex < 0) tagInex = 0;
        else if (tagInex > eftMaterialSteps.Length - 1) tagInex = eftMaterialSteps.Length - 1;
        return eftMaterialSteps[tagInex];
    }

    public void SetTagModelController(BuModelController _tagController)
    {
        bindController = _tagController;
        
    }

    private void CreateImage()
    {
        if (bindController != null)
        {
            smrList = bindController.smrList;
            mfList = bindController.mfList;
        }
        else
        {
            smrList = DefinedSmrList;
            mfList = DefinedMfList;
        }

        if (eftShader != null && (smrList != null || mfList != null))
        {
            List<CombineInstance> combineInstances = new List<CombineInstance>();

            List<Mesh> meshesNeedToRelease = new List<Mesh>(100);
            if (smrList != null)
            {
                for (int i = 0; i < smrList.Length; ++i)
                {
                    var tagMesh = TryGetMeshFromPool();
                    meshesNeedToRelease.Add(tagMesh);
                    smrList[i].BakeMesh(tagMesh);
                    Matrix4x4 tagMatrix = smrList[i].gameObject.transform.localToWorldMatrix;
                    tagMatrix = tagMatrix * Matrix4x4.Scale(new Vector3(1 / tagMatrix.lossyScale.x, 1 / tagMatrix.lossyScale.y, 1 / tagMatrix.lossyScale.z));
                    combineInstances.Add(new CombineInstance
                    {
                        mesh = tagMesh,
                        transform = transform.worldToLocalMatrix * tagMatrix,
                        subMeshIndex = 0,
                    });
                }
            }

            if (mfList != null)
            {
                for (int i = 0; i < mfList.Length; ++i)
                {

                    Mesh tagMesh = mfList[i].sharedMesh; //(Mesh)Instantiate(mfList[i].sharedMesh);
                    combineInstances.Add(new CombineInstance
                    {
                        mesh = tagMesh,
                        transform = transform.worldToLocalMatrix * mfList[i].gameObject.transform.localToWorldMatrix,
                        subMeshIndex = 0,
                    });
                }
            }

            if (combineInstances.Count <= 0) return;

            Mesh combinedMesh = TryGetMeshFromPool();
            combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
            combineInstances.Clear();
            foreach(Mesh tagMesh in meshesNeedToRelease)
            {
                tagMesh.Clear();
                ReleaseMeshToPool(tagMesh);
            }

            _imageList.Add(new AfterImage
            {
                mesh = combinedMesh,
                curTimer = 0,
                matrix = transform.localToWorldMatrix,
                startAlpha = StartAlpha,
                alpha = StartAlpha,
                duration = FadeTime,
            });
        }
    }

    private static Queue<Mesh> cachedMeshPool = new Queue<Mesh>();

    private Mesh TryGetMeshFromPool()
    {
        Mesh tagMesh = null;
        if (cachedMeshPool.Count > 0)
        {
            tagMesh = cachedMeshPool.Dequeue();
        } else
        {
            tagMesh = new Mesh();
        }
        return tagMesh;
    }

    private void ReleaseMeshToPool(Mesh _tagMesh)
    {
        cachedMeshPool.Enqueue(_tagMesh);
    }

    public void SetEnable(bool _enable)
    {
        isEnable = _enable;
    }

    void Update()
    {
        if (isEnable && !isDisabled)
        {
            curGenTimer += Time.deltaTime;
            if (curGenTimer >= curGenImageInterval)
            {
                CreateImage();
                curGenTimer = 0;
            }
        }
    }

    void LateUpdate()
    {
        if (_imageList.Count <= 0) return;
        bool hasRemove = false;
        Material tagMat;
        foreach (var item in _imageList)
        {
            item.curTimer += Time.deltaTime;

            //设置透明度
            item.alpha = Mathf.Lerp(0, item.startAlpha, 1 - item.curTimer / item.duration);
            tagMat = GetStepMaterialByAlpha(item.alpha);
            if (tagMat != null)
            {
                Graphics.DrawMesh(item.mesh, item.matrix, tagMat, gameObject.layer);
            }
            //移除对象
            if (item.curTimer > item.duration)
            {
                if (item.mesh != null)
                {
                    item.mesh.Clear();
                    ReleaseMeshToPool(item.mesh);
                }
                item.needRemove = true;
                hasRemove = true;
                continue;
            }
        }

        if (hasRemove)
        {
            _imageList.RemoveAll(x => x.needRemove);
        }
    }
    
}