using UnityEngine;

public class PlayParticleEffect : MonoBehaviour
{
    public Transform explosionOrigin;

    public void PlayParticleSystem(ParticleSystem effect)
    {
        Instantiate(effect, explosionOrigin.position, explosionOrigin.rotation);
    }
}
