using UnityEngine;
using TMPro;


public class TimeDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private bool m_showTime = true;
    [SerializeField] private bool m_showMilliseconds = true;
    [SerializeField] private TextMeshProUGUI m_timeText;

    private float m_timePassed = 0f;

    void Update()
    {
        m_timeText.text = m_showMilliseconds
            ? $"Time: {m_timePassed:F3} s"
            : $"Time: {Mathf.FloorToInt(m_timePassed)} s";

        m_timePassed += Time.deltaTime;
    }
}

