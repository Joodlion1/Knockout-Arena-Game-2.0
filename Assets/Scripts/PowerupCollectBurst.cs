using UnityEngine;

public class PowerupCollectBurst : MonoBehaviour
{
    [SerializeField] private Color burstColor = new Color(1f, 0.85f, 0.3f);
    [SerializeField] private int burstCount = 70;
    [SerializeField] private float ringRadius = 0.8f;
    [SerializeField] private float burstSpeed = 5.5f;
    [SerializeField] private float lifetime = 0.7f;

    private ParticleSystem ps;

    void Awake()
    {
        var go = new GameObject("PowerupBurst");
        // Keep inactive while configuring so the ParticleSystem doesn't auto-play
        // for a frame before playOnAwake = false takes effect.
        go.SetActive(false);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, 0.1f, 0f);

        ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.duration = 0.2f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = lifetime;
        main.startSpeed = burstSpeed;
        main.startSize = 0.28f;
        main.startColor = burstColor;
        main.maxParticles = 400;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, burstCount)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = ringRadius;
        shape.arc = 360f;
        shape.rotation = new Vector3(90f, 0f, 0f); // Lay the ring flat (XZ plane)

        var col = ps.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(burstColor, 0f),
                new GradientColorKey(burstColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        var sizeCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0.1f)
        );
        size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        renderer.material = new Material(shader);

        // Now safe to activate — playOnAwake is already false
        go.SetActive(true);
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Play()
    {
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }
}
