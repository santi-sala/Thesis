using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateTestHUD : MonoBehaviour
{
    private enum Mode
    {
        VSync,    // vSyncCount=1, target=-1
        Uncapped, // vSyncCount=0, target=-1
        Cap20,
        Cap30,
        Cap60,
        Cap120
    }

    [Header("HUD Settings")]
    [SerializeField] private bool _showHud = false;
    [SerializeField] private int _hudFontSize = 14;
    [SerializeField] private Vector2 _hudOffset = new Vector2(10, 10);
    [SerializeField] private int _sampleSize = 600; // ~10s at 60 FPS
    [SerializeField] private float _onePercentWindowSeconds = 1.5f;

    [Header("Canvas Elements")]
    [SerializeField] private TMP_Text _modeText;
    [SerializeField] private TMP_Text _currentFPSText;
    [SerializeField] private TMP_Text _averageFPSText;

    private readonly Queue<float> _frameTimes = new Queue<float>();
    private float _accumTime = 0f;
    private int _framesInAccum = 0;
    private float _avgFps = 0f;
    private float _instFps = 0f;
    private float _avgFrameMs = 0f;
    private float _stdDevFrameMs = 0f;
    private float _worstFrameMs = 0f;
    private float _onePercentLowFps = 0f;

    private GUIStyle _style;
    private Mode _mode = Mode.VSync;

    void Start()
    {
        _style = new GUIStyle
        {
            normal = { textColor = Color.white },
            fontSize = _hudFontSize
        };
        ApplyMode(_mode);
    }

    void Update()
    {
        HandleHotkeys();
        SampleFrameTiming();

        _currentFPSText.text = $"{Mathf.RoundToInt(_instFps)}";
        _averageFPSText.text = $"{Mathf.RoundToInt(_avgFps)}";
    }

    private void HandleHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Q)) SetMode(Mode.VSync);
        if (Input.GetKeyDown(KeyCode.W)) SetMode(Mode.Uncapped);
        if (Input.GetKeyDown(KeyCode.E)) SetMode(Mode.Cap20);
        if (Input.GetKeyDown(KeyCode.R)) SetMode(Mode.Cap30);
        if (Input.GetKeyDown(KeyCode.T)) SetMode(Mode.Cap60);
        if (Input.GetKeyDown(KeyCode.Y)) SetMode(Mode.Cap120);
        if (Input.GetKeyDown(KeyCode.U)) _showHud = !_showHud;
    }

    private void SetMode(Mode m)
    {
        _mode = m;
        ApplyMode(m);
        // Clear stats when switching
        _frameTimes.Clear();
        _accumTime = 0f;
        _framesInAccum = 0;
        _avgFps = _instFps = _avgFrameMs = _stdDevFrameMs = _worstFrameMs = 0f;
        _onePercentLowFps = 0f;
    }

    private void ApplyMode(Mode m)
    {
        switch (m)
        {
            case Mode.VSync:
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1; // let display sync decide
                _modeText.text = "VSync On";
                break;
            case Mode.Uncapped:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
                _modeText.text = "VSync Off";

                break;
            case Mode.Cap20:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 20;
                _modeText.text = "Target 20FPS";

                break;
            case Mode.Cap30:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                _modeText.text = "Target 30FPS";

                break;
            case Mode.Cap60:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;
                _modeText.text = "Target 60FPS";

                break;
            case Mode.Cap120:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 120;
                _modeText.text = "Target 120FPS";

                break;
        }
    }

    private void SampleFrameTiming()
    {
        float dt = Time.deltaTime; // scaled; use unscaled if you manipulate timeScale
        _instFps = dt > 0f ? 1f / dt : 0f;

        // Sliding buffer of recent frame times
        _frameTimes.Enqueue(dt);
        while (_frameTimes.Count > _sampleSize)
            _frameTimes.Dequeue();

        // Running 1s averages
        _accumTime += dt;
        _framesInAccum++;
        if (_accumTime >= 1f)
        {
            _avgFps = _framesInAccum / _accumTime;
            _avgFrameMs = 1000f / Mathf.Max(_avgFps, 0.0001f);
            _accumTime = 0f;
            _framesInAccum = 0;

            // Compute jitter (std dev of frame times in ms) and worst frame
            if (_frameTimes.Count > 1)
            {
                float mean = 0f;
                foreach (var f in _frameTimes) mean += f;
                mean /= _frameTimes.Count;

                float var = 0f;
                float worst = 0f;
                foreach (var f in _frameTimes)
                {
                    float d = f - mean;
                    var += d * d;
                    if (f > worst) worst = f;
                }
                var /= (_frameTimes.Count - 1);
                _stdDevFrameMs = Mathf.Sqrt(var) * 1000f;
                _worstFrameMs = worst * 1000f;
            }

            // Approximate “1% low” over a recent window
            _onePercentLowFps = ComputeOnePercentLowFps(_onePercentWindowSeconds);
        }
    }

    private float ComputeOnePercentLowFps(float windowSeconds)
    {
        // Gather the last N seconds of frame times
        List<float> recent = new List<float>();
        float sum = 0f;
        foreach (var f in _frameTimes) recent.Add(f);

        // Trim from the front until sum <= windowSeconds
        for (int i = 0; i < recent.Count; i++)
        {
            sum += recent[i];
        }
        int start = 0;
        while (sum > windowSeconds && start < recent.Count)
        {
            sum -= recent[start];
            start++;
        }
        if (start >= recent.Count) return 0f;

        var slice = recent.GetRange(start, recent.Count - start);
        slice.Sort(); // small to large frame times (worst at end)
        if (slice.Count == 0) return 0f;

        int count1p = Mathf.Max(1, Mathf.FloorToInt(slice.Count * 0.01f));
        // Take the largest 1% of frame times, average them, invert to FPS
        float worstSum = 0f;
        for (int i = slice.Count - count1p; i < slice.Count; i++)
            worstSum += slice[i];

        float avgWorstFrameTime = worstSum / count1p;
        return avgWorstFrameTime > 0f ? 1f / avgWorstFrameTime : 0f;
    }

    void OnGUI()
    {
        if (!_showHud) return;

        var rect = new Rect(_hudOffset.x, _hudOffset.y, 540, 260);
        GUI.Box(rect, GUIContent.none);

        string modeName = _mode switch
        {
            Mode.VSync => "VSync (vSyncCount=1, target=-1)",
            Mode.Uncapped => "Uncapped (vSyncCount=0, target=-1)",
            Mode.Cap20 => "Cap 20 FPS",
            Mode.Cap30 => "Cap 30 FPS",
            Mode.Cap60 => "Cap 60 FPS",
            Mode.Cap120 => "Cap 120 FPS",
            _ => "Unknown"
        };

        float stutterIndex = (_avgFrameMs > 0f) ? (_stdDevFrameMs / _avgFrameMs) : 0f;

        string text =
            $"Frame Rate Test HUD\n" +
            $"Mode: {modeName}\n" +
            $"Keys: F1 VSync | F2 Uncapped | F3 20 | F4 30 | F5 60 | F6 120 | F7 Toggle HUD\n\n" +
            $"Instant FPS: {Mathf.RoundToInt(_instFps)}\n" +
            $"Avg FPS (≈1s): {Mathf.RoundToInt(_avgFps)}\n" +
            $"1% Low FPS (≈{_onePercentWindowSeconds:0.0}s window): {Mathf.RoundToInt(_onePercentLowFps)}\n" +
            $"Avg Frame Time: {_avgFrameMs:0.00} ms\n" +
            $"Frame-Time Jitter (σ): {_stdDevFrameMs:0.00} ms\n" +
            $"Stutter Index (σ/mean): {stutterIndex:0.00}\n" +
            $"Worst Frame (recent): {_worstFrameMs:0.00} ms";

        GUI.Label(new Rect(_hudOffset.x + 10, _hudOffset.y + 10, rect.width - 20, rect.height - 20), text, _style);
    }
}
