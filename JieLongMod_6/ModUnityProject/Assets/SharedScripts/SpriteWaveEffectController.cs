using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class SpriteWaveEffectController : MonoBehaviour {

    public SpriteRenderer[] targetSprites;

    public bool enableWaveEft = false;

    public float centerOffsetY = 0;

    public float waveScale = 0;

    public float timeScale = 0;

    public float timeOffsetRange = 0;

    [HideInInspector]
    public bool initialized;

    private bool isUsingEftShader = false;
    
    public void Init()
    {
        if (initialized) return;
        initialized = true;
        

    }

    private void Awake()
    {
        OnEftUpdated();
    }
    
    public void OnEftUpdated()
    {
        if (enableWaveEft)
        {
            if (targetSprites != null && targetSprites.Length > 0)
            {
                for (int i = 0; i < targetSprites.Length; i++)
                {
                    float offsetVal = 0;
                    float offsetRange = timeOffsetRange;
                    if (offsetRange != 0)
                    {
                        offsetVal = Random.Range(0, offsetRange);
                    }
                    eft2DSpritesEffects.AddGrassAnimatToSprite(targetSprites[i], centerOffsetY, waveScale, timeScale, offsetVal);
                }
            }
            isUsingEftShader = true;
        } else
        {
            if (isUsingEftShader)
            {
                if (targetSprites != null && targetSprites.Length > 0)
                {
                    for (int i = 0; i < targetSprites.Length; i++)
                    {
                        eft2DSpritesEffects.ResetSpriteShader(targetSprites[i]);
                    }
                }
            }
            isUsingEftShader = false;
        }
    }

}
