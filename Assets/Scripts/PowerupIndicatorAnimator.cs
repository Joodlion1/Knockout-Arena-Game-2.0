using UnityEngine;

public class PowerupIndicatorAnimator : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 180f;
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobFrequency = 3f;
    [SerializeField] private float pulseScaleAmount = 0.1f;
    [SerializeField] private float pulseSpeed = 5f;

    private Vector3 baseLocalScale;
    private float t;

    void Awake()
    {
        baseLocalScale = transform.localScale;
    }

    void OnEnable()
    {
        t = 0f;
    }

    // LateUpdate runs after PlayerController.Update has placed the indicator
    // at the player's position, so our bob/scale apply on top.
    void LateUpdate()
    {
        if (!gameObject.activeInHierarchy) return;
        t += Time.deltaTime;

        // Spin around Y
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        // Bob (added in world space on top of the position set by PlayerController)
        float bob = Mathf.Sin(t * bobFrequency) * bobAmplitude;
        var p = transform.position;
        transform.position = new Vector3(p.x, p.y + bob, p.z);

        // Pulsing scale
        float pulse = 1f + Mathf.Sin(t * pulseSpeed) * pulseScaleAmount;
        transform.localScale = baseLocalScale * pulse;
    }

    void OnDisable()
    {
        transform.localScale = baseLocalScale;
    }
}
