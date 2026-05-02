using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WaveTextAnimator : MonoBehaviour
{
    [SerializeField] private float punchScale = 1.6f;
    [SerializeField] private float punchDuration = 0.45f;
    [SerializeField] private Color flashColor = new Color(1f, 0.85f, 0.2f);

    private TextMeshProUGUI text;
    private Color baseColor;
    private Vector3 baseScale;
    private Coroutine running;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        baseColor = text.color;
        baseScale = transform.localScale;
    }

    void OnEnable()
    {
        GameManager.OnWaveChanged += HandleWaveChanged;
    }

    void OnDisable()
    {
        GameManager.OnWaveChanged -= HandleWaveChanged;
    }

    void HandleWaveChanged(int waveNumber)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(PunchRoutine());
    }

    IEnumerator PunchRoutine()
    {
        float t = 0f;
        Vector3 punched = baseScale * punchScale;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            float k = t / punchDuration;
            // Ease-out from punched -> base
            float eased = 1f - (1f - k) * (1f - k);
            transform.localScale = Vector3.Lerp(punched, baseScale, eased);
            text.color = Color.Lerp(flashColor, baseColor, eased);
            yield return null;
        }
        transform.localScale = baseScale;
        text.color = baseColor;
        running = null;
    }
}
