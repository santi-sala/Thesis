using UnityEngine;

public class BallAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float duration = 1f;         // Total duration of the animation
    public int fps = 20;                // Sampling rate (frames per second)
    public Vector3 startPos;
    public Vector3 endPos;

    private float timePerFrame;
    private float timer;
    private int currentFrame;
    private int totalFrames;

    void Start()
    {
        // Calculate how many total frames we need
        totalFrames = Mathf.Max(1, Mathf.RoundToInt(duration * fps));
        timePerFrame = duration / totalFrames;
        timer = 0f;
        currentFrame = 0;

        // Immediately apply the first frame (frame 0)
        float t = (float)currentFrame / totalFrames;
        transform.position = Vector3.Lerp(startPos, endPos, t);

        // Prepare to step to the next frame in Update
        currentFrame++;
    }

    void Update()
    {
        if (currentFrame <= totalFrames)
        {
            timer += Time.deltaTime;

            // Advance one or more frames if enough time has passed
            while (timer >= timePerFrame && currentFrame <= totalFrames)
            {
                timer -= timePerFrame;

                float t = (float)currentFrame / totalFrames;
                transform.position = Vector3.Lerp(startPos, endPos, t);

                currentFrame++;
            }
        }
    }
}
