using UnityEngine;
using System.Collections;

public class eftObjSoundEftControl : MonoBehaviour {
    
    public float startDelayTime = 0;

    public string soundEftID = null;

    /// <summary>
    /// 播放次数 <=0:持续期间无限循环 >=1:有限次播放 
    /// </summary>
    public int loops = 1;

    /// <summary>
    /// 单次播放时间
    /// </summary>
    public float soundEftTime = 1;

    private float curTimer = 0;

    private int curLoop = 0;

    /// <summary>
    /// 当前状态 0：初始等待  1：正在播放  2：播放结束
    /// </summary>
    private int curState = 0;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        curState = 0;
        curLoop = 0;
        if (startDelayTime <= 0)
        {
            curState = 1;
        }
    }

    // Update is called once per frame
    void Update () {
        if (curState >= 2) return;
        float ct = Time.deltaTime;
        curTimer += ct;
        if (curState == 0)
        {
            if (curTimer >= startDelayTime)
            {
                curTimer = 0;
                curState = 1;
                PlaySoundEft();
            }
        } else
        {
            if (soundEftTime > 0) {
                if (curTimer >= soundEftTime)
                {
                    curTimer = 0;
                    if (loops > 0)
                    {
                        curLoop++;
                        if (curLoop >= loops)
                        {
                            curState = 2;
                        } else
                        {
                            PlaySoundEft();
                        }
                    }
                }
            }  else
            {
                curState = 2;
            }
        }
    }

    private void PlaySoundEft()
    {
        //AudioManager.Instance.PlayAtEmitter(soundEftID, this.gameObject);
    }
    
}
