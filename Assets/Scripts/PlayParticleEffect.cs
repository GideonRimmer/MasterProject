using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleEffect : MonoBehaviour
{
    public ParticleSystem particleEffect;

    public void PlayParticleSystem()
    {
        //particleEffect.Play();
        Instantiate(particleEffect, transform.position, Quaternion.identity);
    }
}
