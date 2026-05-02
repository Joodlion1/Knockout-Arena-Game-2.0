using UnityEngine;

public class PowerupGlow : MonoBehaviour
{
    [SerializeField] private Color lightColor = new Color(1f, 0.85f, 0.3f);
    [SerializeField] private float baseIntensity = 6f;
    [SerializeField] private float flickerAmount = 1.5f;
    [SerializeField] private float flickerSpeed = 12f;
    [SerializeField] private float lightRange = 6f;

    private Light pointLight;

    void Start()
    {
        var go = new GameObject("PowerupPointLight");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, 0.6f, 0f);

        pointLight = go.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.color = lightColor;
        pointLight.range = lightRange;
        pointLight.intensity = baseIntensity;
        pointLight.shadows = LightShadows.None;
    }

    void Update()
    {
        if (pointLight == null) return;
        float n = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.3f);
        pointLight.intensity = baseIntensity + (n - 0.5f) * 2f * flickerAmount;
    }
}
