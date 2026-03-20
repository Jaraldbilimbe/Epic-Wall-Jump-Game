using UnityEngine;

public class Platform : MonoBehaviour
{
    bool falling = false;

    void Update()
    {
        if (falling)
        {
            // Move cube down toward camera
            transform.Translate(new Vector3(0, -2f, -2f) * Time.deltaTime);

            if (transform.position.y < -10f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Fall()
    {
        falling = true;
    }
}