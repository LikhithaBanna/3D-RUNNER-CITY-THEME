using UnityEngine;

public class SpinCollectible : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0f, 150f * Time.deltaTime, 0f);
    }
}
