using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleEffect : MonoBehaviour
{
    public ParticleSystem particleEffect;
    public Transform explosionOrigin;

    public void PlayParticleSystem()
    {
        //particleEffect.Play();
        Instantiate(particleEffect, explosionOrigin.position, Quaternion.identity);
    }
}
