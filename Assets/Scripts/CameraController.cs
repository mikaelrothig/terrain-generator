using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
