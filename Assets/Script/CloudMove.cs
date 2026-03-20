using UnityEngine;

public class CloudMove : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float destroyY = -6f;
    public bool hasReachedTarget=false;
    Transform player;
    void Start()
    {
        player=GameObject.FindGameObjectWithTag("ReachPoint").gameObject.transform;
    }
    void Update()
    {
        if (!hasReachedTarget)
        {
            transform.position=Vector3.MoveTowards(transform.position,player.transform.position,moveSpeed*Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * 1 * Time.deltaTime);
        }
        if(Vector3.Distance(player.transform.position,transform.position)<.5f)
        {
            hasReachedTarget=true;
        }


       // transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         ContactPoint contact = collision.contacts[0];

    //         // Player landed on top
    //         if (contact.normal.y < -0.5f)
    //         {
    //             Debug.Log("Player landed on cloud");
    //         }
    //         else
    //         {
    //             Debug.Log("Player hit cloud side");

    //             KinectJump player = collision.gameObject.GetComponent<KinectJump>();

    //             if (player != null)
    //             {
    //                 player.TakeDamage();
    //             }
    //         }
    //     }
    // }
    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        ContactPoint contact = collision.contacts[0];
        KinectJump player = collision.gameObject.GetComponent<KinectJump>();

        // Player landed on top
        if (contact.normal.y < -0.5f)
        {
            Debug.Log("Player landed on cloud");

            if (player != null)
            {
                player.ShowPink();   // ADD THIS
            }
        }
        else
        {
            Debug.Log("Player hit cloud side");

            if (player != null)
            {
                player.TakeDamage(); // already exists
                player.ShowRed();    // ADD THIS
            }
        }
    }
}
}