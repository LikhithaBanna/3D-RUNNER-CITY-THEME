using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 6, -9);
    public float smoothSpeed = 6f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 wanted = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, wanted, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.2f + Vector3.forward * 5f);
    }
}
