using UnityEngine;

public class BallAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float m_duration = 1f;         // Total duration of the animation
    [SerializeField] private int m_fps = 20;                // Sampling rate (frames per second)
    [SerializeField] private Vector3 m_startPos;
    [SerializeField] private Vector3 m_endPos;

    private float m_timePerFrame;
    private float m_timer;
    private int m_currentFrame;
    private int m_totalFrames;

    void Start()
    {
        // Calculate how many total frames we need
        m_totalFrames = Mathf.Max(1, Mathf.RoundToInt(m_duration * m_fps));
        m_timePerFrame = m_duration / m_totalFrames;
        m_timer = 0f;
        m_currentFrame = 0;

        // Immediately apply the first frame (frame 0)
        float t = (float)m_currentFrame / m_totalFrames;
        transform.position = Vector3.Lerp(m_startPos, m_endPos, t);

        // Prepare to step to the next frame in Update
        m_currentFrame++;
    }

    void Update()
    {
        if (m_currentFrame <= m_totalFrames)
        {
            m_timer += Time.deltaTime;

            // Advance one or more frames if enough time has passed
            while (m_timer >= m_timePerFrame && m_currentFrame <= m_totalFrames)
            {
                m_timer -= m_timePerFrame;

                float t = (float)m_currentFrame / m_totalFrames;
                transform.position = Vector3.Lerp(m_startPos, m_endPos, t);

                m_currentFrame++;
            }
        }
    }
}
