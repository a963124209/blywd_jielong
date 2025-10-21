/// <summary>  
/// 功能:添加单例模板类,在想要创建单例的脚本中后面继承Singleton<T>并添加下面的两行代码,就可以实现单例功能  例子(继承): public class GameManager :    Singleton<GameManager>  
///         private void Init(){}  
///         void Awake(){ this.Init();}  
///  创建时间: 2015年11月1日 15:03  
/// </summary> 
/// 
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        if (!Application.isPlaying)
                            throw new System.Exception("不允许在编辑器模式下调用此单例！");

                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        try
                        {
                            DontDestroyOnLoad(singleton);
                        }
                        catch (System.Exception ex)
                        {
                            //编辑模式下禁止调用Singleton!
                            Debug.LogException(ex);
                        }
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    public static bool IsDestroy()
    {
        return applicationIsQuitting;
    }
}
