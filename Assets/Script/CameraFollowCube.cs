using UnityEngine;

public class CameraFollowCube : MonoBehaviour
{
    public Transform target;
    public float offsetY = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPosition = transform.position;
        newPosition.y = target.position.y + offsetY;

        transform.position = newPosition;
    }
}