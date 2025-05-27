using UnityEngine;

public class StarGlowPulse : MonoBehaviour
{
    [Header("Glow Settings")]
    public Color colorA = Color.red;
    public Color colorB = Color.blue;
    public float speed = 2f;
    public float intensity = 2f;

    [Header("Toggle")]
    public bool glowActive = true;

    private Material starMaterial;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        starMaterial = renderer.material; // Instantiates a unique material instance

        starMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!glowActive) return;

        float t = Mathf.PingPong(Time.time * speed, 1f);
        Color emissionColor = Color.Lerp(colorA, colorB, t) * intensity;
        starMaterial.SetColor("_EmissionColor", emissionColor);
    }
}
