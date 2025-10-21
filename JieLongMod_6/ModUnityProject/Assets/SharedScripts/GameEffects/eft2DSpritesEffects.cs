using UnityEngine;
using System.Collections;

public class eft2DSpritesEffects
{
    static bool isInitedShaderPropID = false;
    static int sidTimeScale;
    static int sidSpeedScale;
    static int sidUvCenterY;
    static int sidUvHeight;
    static int sidTimeOffset;

    public static void InitShaderPropID()
    {
        if (!isInitedShaderPropID)
        {
            sidTimeScale = Shader.PropertyToID("_TimeScale");
            sidSpeedScale = Shader.PropertyToID("_SpeedScale");
            sidUvCenterY = Shader.PropertyToID("_uvCenterY");
            sidUvHeight = Shader.PropertyToID("_uvHeight");
            sidTimeOffset = Shader.PropertyToID("_TimeOffset");
            isInitedShaderPropID = true;
        }
    }

    /// <summary>
    /// 对一个贴图实现水草摆动效果
    /// </summary>
    /// <param name="sr">原贴图</param>
    /// <param name="acOffsetY">摆动中心点在Y方向上的偏移（0.5，0.5为默认中心）</param>
    /// <param name="waveScale">摆动幅度</param>
    /// <param name="speedScale">摆动速率</param>
    /// <param name="timeOffset">时间偏移</param>
	public static void AddGrassAnimatToSprite(SpriteRenderer sr, float acOffsetY, float waveScale, float speedScale, float timeOffset)
    {
        if (sr == null || sr.sprite == null)
            return;
        InitShaderPropID();
        Shader tagShader = Shader.Find("MyShaders/2DGrassAnimat");
        Sprite sprite = sr.sprite;
        if (tagShader != null)
        {
            sr.material.shader = tagShader;
            float uvCenter = 0.5f;
            float uvHeight = 1.0f;
            if (sprite.textureRect != null)
            {
                float uvMinY = sprite.textureRect.min.y / sprite.texture.height;
                float texHeight = (sprite.textureRect.max.y - sprite.textureRect.min.y) / sprite.texture.height;
                float centerY = uvMinY + 0.5f * texHeight + (acOffsetY / (sprite.rect.height * 0.5f) * texHeight);
                uvCenter = centerY;
                uvHeight = texHeight;
            }
            sr.material.SetFloat(sidTimeScale, waveScale);
            sr.material.SetFloat(sidSpeedScale, speedScale);
            sr.material.SetFloat(sidUvCenterY, uvCenter);
            sr.material.SetFloat(sidUvHeight, uvHeight);
            sr.material.SetFloat(sidTimeOffset, timeOffset);
        }
        else
        {
            Debug.LogError("Can't find shader MyShaders/2DGrassAnimat");
        }
    }

    public static void SetAdditiveSprite(SpriteRenderer tagSr)
    {
        if (tagSr.sprite == null)
            return;
        InitShaderPropID();
        Shader tagShader = Shader.Find("Mobile/Particles/Multiply");
        Sprite sprite = tagSr.sprite;
        if (tagShader != null && tagSr != null)
        {
            tagSr.material.shader = tagShader;
        }
    }

    public static void ResetSpriteShader(SpriteRenderer sr)
    {
        Shader tagShader = Shader.Find("Sprites/Default");
        Sprite sprite = sr.sprite;
        if (tagShader != null && sr != null && sr.material.shader != tagShader)
        {
            sr.material.shader = tagShader;
        }
    }

}
