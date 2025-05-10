using UnityEngine;
using TMPro;


public class TimeDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    public bool showTime = true;
    public bool showMilliseconds = true;

    private float timePassed = 0f;
    [SerializeField] private TextMeshProUGUI _timeText;

    void Update()
    {
        _timeText.text = showMilliseconds
            ? $"Time: {timePassed:F3} s"
            : $"Time: {Mathf.FloorToInt(timePassed)} s";

        timePassed += Time.deltaTime;
    }
}

