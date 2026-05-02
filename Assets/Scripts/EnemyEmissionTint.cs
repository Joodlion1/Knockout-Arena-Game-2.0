using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EnemyEmissionTint : MonoBehaviour
{
    [SerializeField] private Color tintColor = new Color(1f, 0.25f, 0.1f);
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField, Min(0f)] private float maxIntensity = 2.5f;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private Material instanceMat;
    private PlayerController player;
    private float phaseOffset;

    void Start()
    {
        var rend = GetComponent<Renderer>();
        instanceMat = rend.material;
        instanceMat.EnableKeyword("_EMISSION");
        instanceMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        var p = GameObject.Find("Player");
        if (p != null) player = p.GetComponent<PlayerController>();

        phaseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (instanceMat == null) return;

        bool powered = player != null && player.HasPowerup;
        Color emission;
        if (powered)
        {
            float pulse01 = (Mathf.Sin(Time.time * pulseSpeed + phaseOffset) + 1f) * 0.5f;
            float intensity = Mathf.Lerp(0.5f, maxIntensity, pulse01);
            emission = tintColor * intensity;
        }
        else
        {
            emission = Color.black;
        }
        instanceMat.SetColor(EmissionColorId, emission);
    }

    void OnDestroy()
    {
        if (instanceMat != null) Destroy(instanceMat);
    }
}
