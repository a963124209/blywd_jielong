using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class eftPsController : MonoBehaviour {

    public ParticleSystem[] managedPs;

    public GameObject[] managedObj;

    public bool isSetEmissionByEftVal = false;

    public float fadeOutTime = 0;

    private float curPsScale = 1;

    private Dictionary<ParticleSystem, float> oldPsEmissionVals = null;

    public void StartPlayPs()
    {
        if (managedPs != null)
        {
            foreach (ParticleSystem tagPs in managedPs)
            {
                tagPs.Play();
            }
        }

        if (managedObj != null)
        {
            foreach (GameObject tagObj in managedObj)
            {
                tagObj.SetActive(true);
            }
        }
    }

    public void StopPlayPs()
    {
        if (managedPs != null)
        {
            foreach(ParticleSystem tagPs in managedPs)
            {
                tagPs.Stop();
            }
        }

        if (managedObj != null)
        {
            foreach (GameObject tagObj in managedObj)
            {
                tagObj.SetActive(false);
            }
        }

    }

    /// <summary>
    /// 设置粒子效果的强度 0~100
    /// </summary>
    public void SetBuffEftVal(float _eftVal)
    {
        if (managedPs != null)
        {
            float _setVal = Mathf.Clamp(_eftVal / 100.0f, 0, 1);
            float oldRate = 1;
            if (isSetEmissionByEftVal &&  oldPsEmissionVals == null)
            {
                oldPsEmissionVals = new Dictionary<ParticleSystem, float>();
            }
            foreach (ParticleSystem tagPs in managedPs)
            {
                if (isSetEmissionByEftVal)
                {
                    ParticleSystem.EmissionModule emission = tagPs.emission;
                    if (oldPsEmissionVals.ContainsKey(tagPs))
                    {
                        oldRate = oldPsEmissionVals[tagPs];
                    } else
                    {
                        oldRate = tagPs.emission.rateOverTimeMultiplier;
                        oldPsEmissionVals.Add(tagPs, oldRate);
                    }
                    emission.rateOverTimeMultiplier = _setVal * oldRate;
                }
            }
        }
    }

    public void RestoreEftVal()
    {
        if (managedPs != null)
        {
            float oldRate = 1;
            foreach (ParticleSystem tagPs in managedPs)
            {
                if (isSetEmissionByEftVal && oldPsEmissionVals != null)
                {
                    ParticleSystem.EmissionModule emission = tagPs.emission;
                    if (oldPsEmissionVals.ContainsKey(tagPs))
                    {
                        oldRate = oldPsEmissionVals[tagPs];
                    }
                    emission.rateOverTimeMultiplier = oldRate;
                }
            }
        }
    }

}
