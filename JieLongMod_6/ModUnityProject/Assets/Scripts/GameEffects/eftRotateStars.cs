using UnityEngine;
using System.Collections;

public class eftRotateStars : MonoBehaviour {

    public GameObject[] starObjs;
    public float rotateRadius;

    private short curState;
    private float curRotation;
    private int starCount;
    private float rotateSpeed1;
    private float rotateSpeed2;

    private void Awake()
    {
        curState = 0;
        curRotation = 0;
        rotateSpeed1 = 180.0f;
        rotateSpeed2 = 360.0f;
    }

    // Use this for initialization
    void Start () {
        if (starObjs != null && starObjs.Length > 0)
        {
            starCount = starObjs.Length;
            curState = 1;
        }
    }
	
	// Update is called once per frame
	void Update () {
        float ct = Time.deltaTime;
        if (curState == 1 && starCount > 0)
        {
            float dtAng = 360.0f / (float)starCount;
            for (int i = 0; i < starCount; i++)
            {
                Vector3 newPos = new Vector3(0, 0, 0);
                newPos.z = Mathf.Cos(Mathf.Deg2Rad * (curRotation + i * dtAng)) * rotateRadius;
                newPos.y = Mathf.Sin(Mathf.Deg2Rad * (curRotation + i * dtAng)) * rotateRadius;
                newPos.x = Mathf.Cos(Mathf.Deg2Rad * (curRotation*2.0f + i * dtAng)) * rotateRadius * 0.1f;
                starObjs[i].transform.localPosition = newPos;
                starObjs[i].transform.localRotation *= Quaternion.Euler(new Vector3(rotateSpeed2 * ct, 0, 0));
            }

            curRotation += rotateSpeed1 * ct;
            if (curRotation >= 360.0f)
                curRotation = curRotation - 360.0f;
        }
    }
}
