using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eftFixedRotateToCamra : MonoBehaviour
{
    public bool enableAxisX = true;

    public bool enableAxisY = true;

    public bool enableAxisZ = true;

    public Vector3 offsetRotate;

    private Vector3 eulerAngles;
    private Vector3 oldAngles;

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        oldAngles = this.transform.rotation.eulerAngles;
        eulerAngles = Camera.main.transform.rotation.eulerAngles + offsetRotate;
        if (enableAxisX) oldAngles.x = eulerAngles.x;
        if (enableAxisY) oldAngles.y = eulerAngles.y;
        if (enableAxisZ) oldAngles.z = eulerAngles.z;
        this.transform.rotation = Quaternion.Euler(oldAngles);
    }
}
