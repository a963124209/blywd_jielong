using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using HanSquirrel.ResourceManager;

public class BuModelAdaptorInfo : MonoBehaviour {

    public Transform bodyTransf;

    public Transform headTransf;

    public Transform LWeaponTransf;

    public Transform RWeaponTransf;

    public Transform MaskMountTransf;

    public Transform BackMountTransf;

    public ParticleSystem[] modelParticles = null;

    public SkinnedMeshRenderer BodyMesh;

    public SkinnedMeshRenderer RobeMesh;

    public bool isSimpleAvataSystem = false;

    [HideInInspector]
    public bool initialized;

    public void Init()
    {
        if (initialized) return;
        initialized = true;


    }

    private void Awake()
    {
        
    }

    public void AutoBindModelInfo()
    {
        bodyTransf = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1");
        if (bodyTransf != null)
        {
            headTransf = bodyTransf.Find("Bip001 Neck/Bip001 Head");
            LWeaponTransf = bodyTransf.Find("Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/LeftWeaponMount");
            RWeaponTransf = bodyTransf.Find("Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/RightWeaponMount");
            BackMountTransf = bodyTransf.Find("BackWeaponmount");
            if (headTransf != null)
            {
                MaskMountTransf = headTransf.Find("MaskMount");
            }

            if (LWeaponTransf == null)
            {
                LWeaponTransf = bodyTransf.Find("Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/LeftWeaponMount");
            }
            if (RWeaponTransf == null)
            {
                RWeaponTransf = bodyTransf.Find("Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/RightWeaponMount");
            }
        }
    }

}
