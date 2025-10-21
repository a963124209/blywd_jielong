using UnityEngine;
using System.Collections;

public class eftRotateSprite : MonoBehaviour {
    
    public float rotateSpeed = 100;

    private float curRotate = 0;
    
	void Awake () {
        
    }
    
	// Update is called once per frame
	void Update () {
        float ct = Time.deltaTime;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRotate));
        curRotate += ct * rotateSpeed;
        if (curRotate < 0) curRotate = 360;
        else if (curRotate > 360) curRotate = 0;
    }
    
}
