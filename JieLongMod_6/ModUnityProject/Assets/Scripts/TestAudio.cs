using UnityEngine;
using System.Collections;

public class TestAudio : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/audios");
        foreach(var audioController in ab.LoadAllAssets<GameObject>())
        {
            var ac = Instantiate(audioController);






        }
        foreach (var ass in ab.GetAllAssetNames())
        {
            Debug.Log(ass);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
