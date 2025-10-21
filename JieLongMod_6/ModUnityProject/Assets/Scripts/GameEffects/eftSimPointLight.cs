using UnityEngine;
using System.Collections;

public class eftSimPointLight : MonoBehaviour {

    public float Radius = 100;

    public float Intensity = 1.0f;

    public float TwinkleTime = 0;

    public float TwinkleRange = 0.1f;

    public float LifeTime = -1;

    public bool IsExploreModeLight = false;

    [HideInInspector]
    public float CurScreenX = 0;
    [HideInInspector]
    public float CurScreenY = 0;
    [HideInInspector]
    public float CurRadius { get { return curRadius; } }
    [HideInInspector]
    public float CurIntensity { get { return curIntensity; } }
    [HideInInspector]
    public bool SetFlag = false;

    private float curRadius = 0;
    private float curIntensity = 0;
    private float curTwinkleScale = 0;

    private float curTimer = 0;

    private bool isOn = false;
    private bool isFading = false;
    private bool isFadin = false;
    private float fadeTime = 0;
    private float fadingTimer = 0;

    private float curLifeTimer = 0;

    private void Awake()
    {
        ResetParam();
    }

    public void ParseFromAdaptor(MapObjPointLightAdaptor lightAdaptor)
    {
        Radius = lightAdaptor.Radius;
        Intensity = lightAdaptor.Intensity;
        TwinkleTime = lightAdaptor.TwinkleTime;
        TwinkleRange = lightAdaptor.TwinkleRange;
        LifeTime = lightAdaptor.LifeTime;
        IsExploreModeLight = lightAdaptor.IsExploreModeLight;
    }

    public void ResetParam()
    {
        SetFlag = false;
        curLifeTimer = 0;
        SetIsOn(true, 0.3f);
    }

    private void Update()
    {
        if (!isOn) return;
        float ct = Time.deltaTime;
        if (isFading && fadeTime > 0)
        {
            fadingTimer += ct;
            if (isFadin)
            {
                curTwinkleScale = -(1.0f - Mathf.Clamp(fadingTimer / fadeTime, 0, 1));
                if (fadingTimer >= fadeTime)
                {
                    isFading = false;
                }
            } else
            {
                curTwinkleScale = -Mathf.Clamp(fadingTimer / fadeTime, 0, 1);
                if (fadingTimer >= fadeTime)
                {
                    isOn = false;
                    isFading = false;
                    return;
                }
            }
        }
        else
        {
            if (TwinkleTime > 0)
            {
                curTimer += ct;
                if (curTimer > 100) curTimer = curTimer - 100;
                curTwinkleScale = TwinkleRange * 0.5f * (Mathf.Sin(Mathf.PI * curTimer / TwinkleTime) - 1);
            }

            if (LifeTime > 0)
            {
                curLifeTimer += ct;
                if (curLifeTimer >= LifeTime)
                {
                    SetIsOn(false, 0.3f);
                }
            }
        }
        curRadius = Radius * (1.0f + curTwinkleScale);
        curIntensity = Mathf.Clamp(Intensity * (1.0f + curTwinkleScale), 0, 1);

        if (!SetFlag)
        {
            CameraEffectController.Instance.SetPointLight(this, IsExploreModeLight);
        }
    }

    public void SetIsOn(bool _isOn, float _fadeTime)
    {
        if (_fadeTime <= 0)
        {
            isOn = _isOn;
        }
        else
        {
            if (_isOn)
            {
                isFadin = true;
                isFading = true;
                isOn = true;
            }
            else
            {
                isFadin = false;
                isFading = true;
            }
            fadeTime = _fadeTime;
            fadingTimer = 0;
        }
    }

    public bool GetIsOn()
    {
        return isOn;
    }

}
