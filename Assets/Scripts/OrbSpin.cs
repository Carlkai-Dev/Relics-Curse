using UnityEngine;

public class OrbSpin : MonoBehaviour
{
    public float spinSpeed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }
}
