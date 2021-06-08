using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    // See video for reference:
    // https://www.youtube.com/watch?v=Bnm8mzxnwP8&ab_channel=JasonWeimannJasonWeimann

    [SerializeField] private AudioClip[] clips;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomClip()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
