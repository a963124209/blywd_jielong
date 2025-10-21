using UnityEngine;
using System.Collections;

public class eftClickablePsController : MonoBehaviour {

    public ParticleSystem[] normalPs;

    public ParticleSystem[] hoverPs;

    public ParticleSystem[] clickPs;

    public bool isPlayOnStart = true;

    private bool isHovering = false;

    private void Awake()
    {
        if (isPlayOnStart)
        {
            StartPlayPs();
        } else
        {
            StopPlayPs();
        }
    }

    public void StartPlayPs()
    {
        if (normalPs != null)
        {
            foreach (ParticleSystem tagPs in normalPs)
            {
                tagPs.Play();
            }
        }
    }

    public void StopPlayPs()
    {
        if (normalPs != null)
        {
            foreach(ParticleSystem tagPs in normalPs)
            {
                tagPs.Stop();
            }
        }
        SetIsHover(false);
    }

    public void PlayClickedPs()
    {
        if (clickPs != null)
        {
            foreach (ParticleSystem tagPs in clickPs)
            {
                tagPs.Play();
            }
        }
    }

    public void SetIsHover(bool _isHover)
    {
        isHovering = _isHover;
        if (isHovering)
        {
            if (hoverPs != null)
            {
                foreach (ParticleSystem tagPs in hoverPs)
                {
                    tagPs.Play();
                }
            }
        } else
        {
            if (hoverPs != null)
            {
                foreach (ParticleSystem tagPs in hoverPs)
                {
                    tagPs.Stop();
                }
            }
        }
    }

}
