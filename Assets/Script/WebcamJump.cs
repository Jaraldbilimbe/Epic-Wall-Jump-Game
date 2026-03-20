
// using UnityEngine;

// public class WebcamJump : MonoBehaviour
// {
//     Animator anim;
//     private WebCamTexture webcam;
//     private Color32[] previousFrame;
//     private Rigidbody rb;

//     private float cameraWarmupTime = 2f;
//     private float startTime;

//     [Header("Jump Settings")]
//     public float jumpForce = 6f;
//     public float motionThreshold = 20f;
//     public float jumpCooldown = 0.8f;

//     [Header("Ground Check")]
//     public float groundCheckDistance = 1.2f;
//     public LayerMask groundLayer;

//     public bool isGrounded;

//     private float lastJumpTime;

//     void Start()
//     {
//         anim = GetComponent<Animator>();
//         rb = GetComponent<Rigidbody>();
        
//         WebCamDevice[] devices = WebCamTexture.devices;

//         if (devices.Length == 0)
//         {
//             Debug.LogError("NO CAMERA FOUND");
//             return;
//         }

//         // Print all cameras
//         for (int i = 0; i < devices.Length; i++)
//         {
//             Debug.Log("Camera " + i + ": " + devices[i].name);
//         }

//         // Select last camera (usually external)
//         int cameraIndex = devices.Length - 1;

//         webcam = new WebCamTexture(devices[cameraIndex].name, 320, 240, 30);
//         webcam.Play();

//         startTime = Time.time;

//         Debug.Log("Using Camera: " + devices[cameraIndex].name);
//     }

//     void Update()
//     {
//         Debug.Log("Update Working");
//         if (webcam == null || !webcam.isPlaying || !webcam.didUpdateThisFrame)
//             return;

//         // Camera warmup
//         if (Time.time < startTime + cameraWarmupTime)
//         {
//             previousFrame = webcam.GetPixels32();
//             return;
//         }

//         CheckGround();

//         Color32[] currentFrame = webcam.GetPixels32();

//         if (previousFrame == null)
//         {
//             previousFrame = currentFrame;
//             return;
//         }

//         float motion = DetectMotion(currentFrame);

//         if (motion > motionThreshold &&
//             isGrounded &&
//             Time.time > lastJumpTime + jumpCooldown)
//         {
//             Jump();
//             lastJumpTime = Time.time;
//         }

//         previousFrame = currentFrame;
//     }

//     float DetectMotion(Color32[] currentFrame)
//     {
//         float total = 0f;
//         int count = 0;

//         for (int i = 0; i < currentFrame.Length; i += 200)
//         {
//             float diff =
//                 Mathf.Abs(currentFrame[i].r - previousFrame[i].r) +
//                 Mathf.Abs(currentFrame[i].g - previousFrame[i].g) +
//                 Mathf.Abs(currentFrame[i].b - previousFrame[i].b);

//             total += diff;
//             count++;
//         }

//         return total / count;
//     }

//     void CheckGround()
//     {
//         Vector3 origin = transform.position + Vector3.up * 0.1f;

//         isGrounded = Physics.Raycast(
//             origin,
//             Vector3.down,
//             groundCheckDistance,
//             groundLayer
//         );
//     }

//     void Jump()
//     {
//         Debug.Log("WEBCAM JUMP!");

//         rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
//         rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

//         if (anim != null)
//         {
//             anim.SetTrigger("IsJump");
//         }
//     }

//     public void StopCamera()
//     {
//         if (webcam != null)
//         {
//             webcam.Stop();
//             webcam = null;
//             Debug.Log("Webcam stopped");
//         }
//     }
// }
using UnityEngine;

public class WebcamJump : MonoBehaviour
{
    Animator anim;
    private WebCamTexture webcam;
    private Color32[] previousFrame;
    private Rigidbody rb;

    private float cameraWarmupTime = 2f;
    private float startTime;

    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public float motionThreshold = 35f;
    public float jumpCooldown = 1f;

    [Header("Motion Detection")]
    public int requiredMotionFrames = 3;
    private int motionFrames = 0;

    [Header("Ground Check")]
    public float groundCheckDistance = 1.2f;
    public LayerMask groundLayer;

    public bool isGrounded;

    private float lastJumpTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("NO CAMERA FOUND");
            return;
        }

        // Print cameras
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Camera " + i + ": " + devices[i].name);
        }

        int cameraIndex = devices.Length - 1;

        webcam = new WebCamTexture(devices[cameraIndex].name, 320, 240, 30);
        webcam.Play();

        startTime = Time.time;

        Debug.Log("Using Camera: " + devices[cameraIndex].name);
    }

    void Update()
    {
        if (webcam == null || !webcam.isPlaying || !webcam.didUpdateThisFrame)
            return;

        // Camera warmup
        if (Time.time < startTime + cameraWarmupTime)
        {
            previousFrame = webcam.GetPixels32();
            return;
        }

        CheckGround();

        Color32[] currentFrame = webcam.GetPixels32();

        if (previousFrame == null)
        {
            previousFrame = currentFrame;
            return;
        }

        float motion = DetectMotion(currentFrame);

        // Motion frame filter
        if (motion > motionThreshold)
        {
            motionFrames++;
        }
        else
        {
            motionFrames = 0;
        }

        // Jump condition
        if (motionFrames >= requiredMotionFrames &&
            isGrounded &&
            Time.time > lastJumpTime + jumpCooldown)
        {
            Jump();
            lastJumpTime = Time.time;
            motionFrames = 0;
        }

        previousFrame = currentFrame;
    }

    float DetectMotion(Color32[] currentFrame)
    {
        float total = 0f;
        int count = 0;

        for (int i = 0; i < currentFrame.Length; i += 200)
        {
            float diff =
                Mathf.Abs(currentFrame[i].r - previousFrame[i].r) +
                Mathf.Abs(currentFrame[i].g - previousFrame[i].g) +
                Mathf.Abs(currentFrame[i].b - previousFrame[i].b);

            total += diff;
            count++;
        }

        return total / count;
    }

    void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        isGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    void Jump()
    {
        Debug.Log("WEBCAM JUMP!");

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (anim != null)
        {
            anim.SetTrigger("IsJump");
        }

        // Increment score
        ScoreManager.score++;
    }

    public void StopCamera()
    {
        if (webcam != null && webcam.isPlaying)
        {
            webcam.Stop();
            Debug.Log("Webcam stopped");
        }
    }
}