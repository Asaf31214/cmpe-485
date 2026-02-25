using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            audioSource.mute = !audioSource.mute;
        }
    }
}