using UnityEngine;

public class MovingCube : MonoBehaviour
{
    public float speed = 10f;

    private Vector3 targetPosition;
    private bool reachedTarget = false;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        reachedTarget = false;
    }

    void Update()
    {
        if (!reachedTarget)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                reachedTarget = true;
            }
        }
    }
}