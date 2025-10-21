using UnityEngine;
using System.Collections.Generic;

using HanSquirrel.ResourceManager;

public class AudioManager : MonoBehaviour
{

    private static AudioManager m_instance = null;
    private static object _lock = new object();//定义一个静态的objec类型的_lock并实例化  
    private static bool applicationIsQuitting = false;//定义bool类型的变量applicationIsQuitting 判断程序是否关闭

    private float curSoundVolum = 1.0f;
    private float curMusicVolum = 1.0f;

    public float SoundVolum
    {
        set {
            curSoundVolum = Mathf.Clamp(value, 0, 1.0f);
        }
        get
        {
            return curSoundVolum;
        }
    }

    public float MusicVolum
    {
        set
        {
            curMusicVolum = Mathf.Clamp(value, 0, 1.0f);
            if (curMusicVolum <= 0)
            {
                
            }
        }
        get
        {
            return curMusicVolum;
        }
    }

    public static AudioManager Instance
    {
        get
        {
            if (applicationIsQuitting)//如果applicationIsQuitting为真 则返回null  以下的代码不执行  
            {
                return null;
            }
            lock (_lock)//lock 关键字可以用来确保代码块完成运行，而不会被其他线程中断。  
            {
                if (null == m_instance)//如果_instance等于空 执行一下代码  
                {
                    GameObject singleton = new GameObject("AudioManager");
                    m_instance = singleton.AddComponent<AudioManager>();
                    DontDestroyOnLoad(singleton);

                    
                }
                return m_instance;//返回出_instance单例   
            }
        }
    }

}