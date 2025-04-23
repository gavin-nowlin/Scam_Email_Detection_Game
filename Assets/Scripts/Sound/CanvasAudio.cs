using UnityEngine;
using UnityEngine.Audio;

public class CanvasAudio : MonoBehaviour
{
    // Reference to the Audio Manager
    public AudioManager audioManager;

    // Reference to main audio mixer
    [SerializeField]
    private AudioMixer audioMixer;

    private void Start()
    {
        // Gets a reference to the Audio Manager at the start of this scene
        audioManager = FindAnyObjectByType<AudioManager>();
        // Defualting volume to half
        audioMixer.SetFloat("Volume", Mathf.Log10(0.5f) * 20f);
    }

    // Plays the click sound from the Audio Manager
    public void PlayClickSound() {
        audioManager.PlayClickSound();
    }
}
