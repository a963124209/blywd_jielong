using UnityEngine;
using System.Collections;

public class eftRotateAndWave : MonoBehaviour {
    
    public Vector3 rotateSpeedVec = new Vector3(0, 0, 100);

    public Vector3 wavePosOffset = new Vector3(0, 0, 0);

    public float waveCycleTime = 0;

    private Vector3 initPos = Vector3.zero;

    private float curTimer = 0;
    
	void Awake () {
        initPos = transform.localPosition;
    }
    
	// Update is called once per frame
	void Update () {
        float ct = Time.deltaTime;
        transform.Rotate(rotateSpeedVec * ct);
        if (waveCycleTime > 0)
        {
            curTimer += ct;
            transform.localPosition = initPos + (wavePosOffset * Mathf.Sin(180.0f / Mathf.Rad2Deg * curTimer / waveCycleTime));
            if (curTimer > waveCycleTime * 100)
            {
                curTimer = curTimer - (waveCycleTime * 100);
            }
        }
    }
    
}
