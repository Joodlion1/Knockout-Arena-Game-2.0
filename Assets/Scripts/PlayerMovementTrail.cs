using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementTrail : MonoBehaviour
{
    [SerializeField] private float speedThreshold = 0.6f;
    [SerializeField] private Color trailColor = new Color(0.35f, 0.75f, 1f);
    [SerializeField] private float startSize = 0.22f;
    [SerializeField] private float lifetime = 0.55f;
    [SerializeField] private float emitPerMeter = 28f;

    private ParticleSystem ps;
    private Rigidbody rb;
    private bool playing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = BuildTrailParticles();
        ps.Stop();
    }

    void Update()
    {
        if (rb == null || ps == null) return;
        if (GameManager.Instance == null || !GameManager.Instance.isGameActive)
        {
            if (playing) { ps.Stop(); playing = false; }
            return;
        }

        bool moving = rb.linearVelocity.magnitude > speedThreshold;
        if (moving && !playing) { ps.Play(); playing = true; }
        else if (!moving && playing) { ps.Stop(); playing = false; }
    }

    ParticleSystem BuildTrailParticles()
    {
        var go = new GameObject("MovementTrail");
        // Keep inactive while configuring so playOnAwake's default doesn't fire one frame.
        go.SetActive(false);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, -0.4f, 0f);

        var system = go.AddComponent<ParticleSystem>();

        var main = system.main;
        main.duration = 5f;
        main.loop = true;
        main.startLifetime = lifetime;
        main.startSpeed = 0.4f;
        main.startSize = startSize;
        main.startColor = trailColor;
        main.gravityModifier = -0.05f;
        main.maxParticles = 300;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;

        var emission = system.emission;
        emission.rateOverTime = 0f;
        emission.rateOverDistance = emitPerMeter;

        var shape = system.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var col = system.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(trailColor, 0f),
                new GradientColorKey(trailColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.75f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        col.color = new ParticleSystem.MinMaxGradient(grad);

        var size = system.sizeOverLifetime;
        size.enabled = true;
        var sizeCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)
        );
        size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var renderer = system.GetComponent<ParticleSystemRenderer>();
        renderer.material = CreateParticleMaterial();

        go.SetActive(true);
        return system;
    }

    static Material CreateParticleMaterial()
    {
        var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        return new Material(shader);
    }
}
