using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    private void Awake()
    {
        if (audioMixer == null)
            audioMixer = FindFirstObjectByType<AudioManager>().GetComponent<AudioMixer>();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20f);
    }
}
