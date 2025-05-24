using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimationEvents : MonoBehaviour
{
    private AudioSource _audioSource;
    private GameObject _bombsContainer;

    [SerializeField] private GameObject _bombPrefab;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _bombsContainer = transform.parent.gameObject;

        if (_bombPrefab == null)
            _bombPrefab = gameObject;
    }
    public void BoomSfx()
    {
        _audioSource.Play();
    }

    public void InstantiateBomb() 
    { 
        Instantiate(_bombPrefab, transform.position, Quaternion.identity, _bombsContainer.transform);
    }

    public void DestroyBomb() 
    { 
        Destroy(gameObject);
    }
}
