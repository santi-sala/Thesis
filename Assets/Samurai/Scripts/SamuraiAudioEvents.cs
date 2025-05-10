using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SamuraiAudioEvents : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(string sfx) 
    {
        switch (sfx)
        {
            case "Walk":
            case "Run":
                _audioSource.PlayOneShot(_audioClips[0]);
                break;
            case "Jump":
                _audioSource.PlayOneShot(_audioClips[1]);
                break;
            case "Attack_One":
                _audioSource.PlayOneShot(_audioClips[2]);
                break;
            default:
                Debug.LogWarning($"Audio clip for {sfx} not found.");
                break;
        }
    }
}
