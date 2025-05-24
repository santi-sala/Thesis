using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class BombSpawner : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int columns = 8;
    [SerializeField] private int rows = 8;
    [SerializeField] private float spacing = 1.5f;

    [Header("Animation Offset Settings")]
    [SerializeField] private string animatorParameter = "RandomStartOffset";
    [SerializeField] private bool randomizeAnimatorOffset = true;

    private void Start()
    {
        if (bombPrefab == null)
        {
            Debug.LogError("Bomb Prefab not assigned.");
            return;
        }
        Vector3 startPos = transform.position;

        for(int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate position: left to right, then down
                Vector3 position = startPos + new Vector3(col * spacing, -row * spacing, 0);
                GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity, transform);


                if (randomizeAnimatorOffset)
                {
                    Animator animator = bomb.GetComponent<Animator>();
                    if (animator != null)
                    {
                        // Adds a random normalized time offset to the Animator's playback
                        float randomTime = Random.Range(0f, 1f);
                        animator.Play(0, -1, randomTime);
                    }
                }
            }
        }
    }
}
