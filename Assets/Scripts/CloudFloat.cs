using UnityEngine;

public class CloudFloat : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float floatRange = 0.5f;
    public Vector3 moveDirection = Vector3.right;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Gentle floating movement
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPos + moveDirection * offset;
    }
}
