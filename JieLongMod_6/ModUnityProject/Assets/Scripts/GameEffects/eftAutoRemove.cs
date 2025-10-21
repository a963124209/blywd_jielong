using UnityEngine;
using System.Collections;
using HanSquirrel.ResourceManager;

public class eftAutoRemove : LeanPoolObjDelegate {
    public float lifetime = 1.0f;

    private float curLifeTimer = 0;
    private bool ssUpdateBySelf = true;

    private void Update()
    {
        if (ssUpdateBySelf)
        {
            curLifeTimer += Time.deltaTime;
            if (curLifeTimer >= lifetime)
            {
                ResourceLoader.DespawnOrDestory(gameObject);
                return;
            }
        }
    }

    public void SetIsUpdateBySelf(bool _isSelf)
    {
        ssUpdateBySelf = _isSelf;
    }

    override public void ResetObjectDatas()
    {
        curLifeTimer = 0;
        /*
        ParticleSystem tagPs = GetComponent<ParticleSystem>();
        if (tagPs != null)
        {
            tagPs.Play();
        } else
        {
            tagPs = transform.GetComponentInChildren<ParticleSystem>();
            if (tagPs != null)
            {
                tagPs.Play();
            }
        }*/
    }

}
