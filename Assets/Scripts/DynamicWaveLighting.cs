using UnityEngine;

[RequireComponent(typeof(Light))]
public class DynamicWaveLighting : MonoBehaviour
{
    [Header("Wave Mood")]
    [SerializeField] private float baseIntensity = 1.0f;
    [SerializeField] private float minIntensity = 0.45f;
    [SerializeField] private int wavesUntilDarkest = 6;
    [SerializeField] private Color baseColor = new Color(1f, 0.96f, 0.84f);
    [SerializeField] private Color lateWaveColor = new Color(0.55f, 0.6f, 0.95f);

    [Header("Flicker")]
    [SerializeField] private float flickerAmplitude = 0.06f;
    [SerializeField] private float flickerSpeed = 8f;

    private Light dirLight;
    private float targetIntensity;
    private Color targetColor;

    void Awake()
    {
        dirLight = GetComponent<Light>();
        targetIntensity = baseIntensity;
        targetColor = baseColor;
    }

    void OnEnable()
    {
        GameManager.OnWaveChanged += HandleWaveChanged;
        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        GameManager.OnWaveChanged -= HandleWaveChanged;
        GameManager.OnGameOver -= HandleGameOver;
    }

    void HandleWaveChanged(int waveNumber)
    {
        float t = Mathf.Clamp01((waveNumber - 1f) / Mathf.Max(1, wavesUntilDarkest - 1));
        targetIntensity = Mathf.Lerp(baseIntensity, minIntensity, t);
        targetColor = Color.Lerp(baseColor, lateWaveColor, t);
    }

    void HandleGameOver()
    {
        targetIntensity = minIntensity * 0.6f;
        targetColor = lateWaveColor;
    }

    void Update()
    {
        // Smooth transition between waves
        float smoothed = Mathf.Lerp(dirLight.intensity, targetIntensity, Time.deltaTime * 2f);
        dirLight.color = Color.Lerp(dirLight.color, targetColor, Time.deltaTime * 2f);

        // Subtle flicker on top of the smoothed value
        float flicker = (Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) - 0.5f) * 2f * flickerAmplitude;
        dirLight.intensity = Mathf.Max(0f, smoothed + flicker);
    }
}
