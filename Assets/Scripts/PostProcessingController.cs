using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [Header("Bloom")]
    [SerializeField] private float bloomIntensity = 1.2f;
    [SerializeField] private float bloomThreshold = 0.9f;
    [SerializeField] private float bloomScatter = 0.7f;

    [Header("Vignette - Idle")]
    [SerializeField] private float baseVignetteIntensity = 0.25f;
    [SerializeField] private Color baseVignetteColor = Color.black;

    [Header("Vignette - Powered Up")]
    [SerializeField] private float poweredVignetteIntensity = 0.5f;
    [SerializeField] private Color poweredVignetteColor = new Color(1f, 0.7f, 0.2f);

    [SerializeField] private float vignetteLerpSpeed = 4f;

    private Volume volume;
    private VolumeProfile profile;
    private Bloom bloom;
    private Vignette vignette;
    private PlayerController player;

    void Awake()
    {
        volume = gameObject.GetComponent<Volume>();
        if (volume == null) volume = gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 10f;
        volume.weight = 1f;

        profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "RuntimePostProcessProfile";
        volume.sharedProfile = profile;

        bloom = profile.Add<Bloom>(true);
        bloom.intensity.Override(bloomIntensity);
        bloom.threshold.Override(bloomThreshold);
        bloom.scatter.Override(bloomScatter);

        vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(baseVignetteIntensity);
        vignette.color.Override(baseVignetteColor);
        vignette.smoothness.Override(0.4f);
    }

    void Start()
    {
        var p = GameObject.Find("Player");
        if (p != null) player = p.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (vignette == null) return;

        bool powered = player != null && player.HasPowerup;
        float targetIntensity = powered ? poweredVignetteIntensity : baseVignetteIntensity;
        Color targetColor = powered ? poweredVignetteColor : baseVignetteColor;

        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetIntensity, Time.deltaTime * vignetteLerpSpeed);
        vignette.color.value = Color.Lerp(vignette.color.value, targetColor, Time.deltaTime * vignetteLerpSpeed);
    }

    void OnDestroy()
    {
        if (profile != null) Destroy(profile);
    }
}
