using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEffects : MonoBehaviour
{
    [Header("Shake")]
    [SerializeField] private float shakeMagnitude = 0.35f;
    [SerializeField] private float shakeDecay = 4f;

    [Header("Zoom (Perspective FOV)")]
    [SerializeField] private float baseFov = 60f;
    [SerializeField] private float poweredFov = 72f;

    [Header("Zoom (Orthographic Size)")]
    [SerializeField] private float baseOrthoSize = 10f;
    [SerializeField] private float poweredOrthoSize = 12f;

    [SerializeField] private float zoomLerpSpeed = 4f;

    private Camera cam;
    private PlayerController player;
    private Vector3 baseLocalPosition;
    private float currentShake;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam.orthographic)
        {
            // Capture whatever the scene was authored with so the rest pose isn't disturbed.
            baseOrthoSize = cam.orthographicSize;
        }
        else
        {
            baseFov = cam.fieldOfView;
        }
        baseLocalPosition = transform.localPosition;
    }

    void Start()
    {
        var p = GameObject.Find("Player");
        if (p != null) player = p.GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        GameManager.OnEnemyKnockedOff += HandleEnemyKnockedOff;
    }

    void OnDisable()
    {
        GameManager.OnEnemyKnockedOff -= HandleEnemyKnockedOff;
    }

    void HandleEnemyKnockedOff(int points)
    {
        currentShake = Mathf.Max(currentShake, shakeMagnitude);
    }

    void LateUpdate()
    {
        // Zoom based on powerup state — works on both orthographic and perspective cameras.
        bool powered = player != null && player.HasPowerup;
        if (cam.orthographic)
        {
            float target = powered ? poweredOrthoSize : baseOrthoSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, target, Time.deltaTime * zoomLerpSpeed);
        }
        else
        {
            float target = powered ? poweredFov : baseFov;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, target, Time.deltaTime * zoomLerpSpeed);
        }

        // Camera shake (offset from the rest local position)
        if (currentShake > 0.001f)
        {
            Vector3 offset = Random.insideUnitSphere * currentShake;
            // Avoid pushing forward/backward — keep shake on the camera's local right/up plane
            offset.z *= 0.3f;
            transform.localPosition = baseLocalPosition + offset;
            currentShake = Mathf.Max(0f, currentShake - shakeDecay * Time.deltaTime);
        }
        else
        {
            transform.localPosition = baseLocalPosition;
            currentShake = 0f;
        }
    }
}
