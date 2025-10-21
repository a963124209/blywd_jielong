using UnityEngine;
using System.Collections;

public class eftModelRotate : MonoBehaviour {
    
    public float rotateSpeed = 100;

    public Vector3 rotateAxi = new Vector3(0, 0, 1);


    void Awake () {
        
    }
    
	// Update is called once per frame
	void Update () {
        float ct = Time.deltaTime;
        transform.Rotate(rotateAxi, rotateSpeed * ct);
    }
    
}
