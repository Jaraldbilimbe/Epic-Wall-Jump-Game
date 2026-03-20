using Unity.VisualScripting;
using UnityEngine;
using Windows.Kinect;

public class KinectJump : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;

    private KinectSensor _sensor;
    private BodyFrameReader _reader;
    private Body[] _bodies = null;

    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public float jumpVelocityThreshold = 0.03f; // Upward velocity needed per frame to predict a jump
    public float jumpCooldown = 1f;
    public float rotationSpeed = 360f; // Degrees per second during jump

    [Header("Ground Check")]
    public float groundCheckDistance = 1.2f;
    public LayerMask groundLayer;
    public bool isGrounded;
    public float fallLimit = -5f;

    [Header("Character Models")]
   public GameObject pinkCharacter;
   public GameObject redCharacter;

    private float lastJumpTime;
    
    // Variables for more robust jump detection
    private float lastSpineY = 0f;
    private float lastLeftFootY = 0f;
    private float lastRightFootY = 0f;
     public int lives = 3;
     [Header("Life UI")]
    public GameObject[] hearts;
    public Vector3 centerPosition = new Vector3(0, 1, 0);

    [Header("Landing Particle")]
    public GameObject landingParticle;


    void Start()
    {

        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        pinkCharacter.SetActive(true);
        redCharacter.SetActive(false);
         isGrounded = true;

        _sensor = KinectSensor.GetDefault();
        if (_sensor != null)
        {
            _reader = _sensor.BodyFrameSource.OpenReader();
            if (!_sensor.IsOpen)
            {
                _sensor.Open();
                Debug.Log("Kinect sensor opened.");
            }
        }
        else
        {
            Debug.LogError("No Kinect sensor found!");
        }
    }

    public void ShowRed()
{
    Debug.Log("red shows");
    pinkCharacter.SetActive(false);
    redCharacter.SetActive(true);
    redCharacter.transform.localPosition = Vector3.zero;
}

    public void ShowPink()
{
    Debug.Log("pink shows");
    pinkCharacter.SetActive(true);
    redCharacter.SetActive(false);
}

    void Update()
    {  CheckGround();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
        }
            if (transform.position.y < fallLimit)
    {
        GameOver();
         GameManager.instance.GameOver();
    }
        if (_reader != null)
        {
            var frame = _reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_bodies == null)
                {
                    _bodies = new Body[_sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(_bodies);
                frame.Dispose();

                bool bodyTracked = false;

                foreach (var body in _bodies)
                {
                    if (body != null && body.IsTracked)
                    {
                        bodyTracked = true;
                        
                        // We use SpineBase as a reliable center of mass point for jump calculation
                        float currentSpineY = body.Joints[JointType.SpineBase].Position.Y;
                        
                        // We also track feet to ensure they are leaving the ground/moving upwards
                        float currentLeftFootY = body.Joints[JointType.FootLeft].Position.Y;
                        float currentRightFootY = body.Joints[JointType.FootRight].Position.Y;

                        // Only check jump if we have a valid previous position
                        //if (lastSpineY != 0f && lastLeftFootY != 0f && lastRightFootY != 0f)
                        // Initialize first frame
                    if (lastSpineY == 0f)
                     {
                   lastSpineY = currentSpineY;
                   lastLeftFootY = currentLeftFootY;
                   lastRightFootY = currentRightFootY;
                 return;
                    }
                        {
                            // Calculate upward velocity per frame for spine and feet
                            float spineVelocity = currentSpineY - lastSpineY;
                            float leftFootVelocity = currentLeftFootY - lastLeftFootY;
                            float rightFootVelocity = currentRightFootY - lastRightFootY;

                            // Predict jump: Spine MUST move up fast enough.
                            // AND feet must be moving upwards significantly (to ignore hand waves and tracking noise)
                            bool feetJumping = (leftFootVelocity > 0.025f && rightFootVelocity > 0.025f) || (leftFootVelocity > 0.04f || rightFootVelocity > 0.04f);
                            if (spineVelocity > jumpVelocityThreshold && feetJumping)
                            {
                                Debug.Log($"[Kinect] Valid JUMP Motion Detected -> SpineVel: {spineVelocity:F4} | LFootVel: {leftFootVelocity:F4} | RFootVel: {rightFootVelocity:F4} | Grounded: {isGrounded}");
                                
                                if (isGrounded && Time.time > lastJumpTime + jumpCooldown)
                                {
                                    Jump();
                                    lastJumpTime = Time.time;
                                }
                                else if (!isGrounded)
                                {
                                    Debug.LogWarning("[Kinect] Jump motion detected, but player is NOT GROUNDED in Unity.");
                                }
                            }
                            else if (spineVelocity > jumpVelocityThreshold)
                            {
                                Debug.Log($"[Kinect] Spine moved fast ({spineVelocity:F4}), but feet didn't move up. Likely just stretching/leaning.");
                            }
                            else if (spineVelocity > 0.01f) // Log minor movements
                            {
                                Debug.Log($"[Kinect] Minor upward movement: {spineVelocity:F4}");
                            }
                        }

                        lastSpineY = currentSpineY;
                        lastLeftFootY = currentLeftFootY;
                        lastRightFootY = currentRightFootY;
                        break; // Only track the first active body
                    }
                }

                if (!bodyTracked)
                {
                    lastSpineY = 0f;
                    lastLeftFootY = 0f;
                    lastRightFootY = 0f;
                }
            }
        }
    }

    void CheckGround()
    {
        if (isGrounded)
{
    ShowPink();
}
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );
        
        // Draw debug line in Scene view
        Debug.DrawRay(origin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        
        // Log if we hit something that isn't the ground layer
        if (!isGrounded && Physics.Raycast(origin, Vector3.down, out RaycastHit anyHit, groundCheckDistance))
        {
            Debug.Log($"[Kinect] Ground raycast hit '{anyHit.collider.name}' (Layer: {LayerMask.LayerToName(anyHit.collider.gameObject.layer)}) but it is NOT in the GroundLayer mask.");
        }
    }

    void Jump()
    {
        Debug.Log("KINECT JUMP DETECTED!");

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (anim != null)
        {
            anim.SetTrigger("IsJump");
        }
        
        // Increment score
        ScoreManager.score++;
        
        // Start jump rotation
        StartCoroutine(JumpRotationCoroutine());
    }

    private System.Collections.IEnumerator JumpRotationCoroutine()
    {
        float targetRotation = 360f;
        float currentRotation = 0f;

        while (currentRotation < targetRotation && !isGrounded)
        {
            float rotationChange = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationChange, 0);
            currentRotation += rotationChange;
            yield return null;
        }

        // Snap to exact 360 rotation to avoid slight offsets
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void StopKinect()
    {
        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
        }

        if (_sensor != null)
        {
            if (_sensor.IsOpen)
            {
                _sensor.Close();
                Debug.Log("Kinect sensor closed.");
            }
            _sensor = null;
        }
    }

    void OnDestroy()
    {
        StopKinect();
    }
    
    void OnApplicationQuit()
    {
        StopKinect();
    }

    // public void TakeDamage()
    // {
    //     lives--;

    //     Debug.Log("Player hit! Lives left: " + lives);

    //     // Move player back to center
    //     transform.position = centerPosition;

    //     if (lives <= 0)
    //     {
    //         GameOver();
    //         GameManager.instance.GameOver();
    //     }
    // }
    public void TakeDamage()
{
    lives--;

    Debug.Log("Player hit! Lives left: " + lives);

    // Disable heart UI
    if (lives >= 0 && lives < hearts.Length)
    {
        hearts[lives].SetActive(false);
    }

    // Move player back to center
    transform.position = centerPosition;

    ShowRed();

    if (lives <= 0)
    {
        GameOver();
        GameManager.instance.GameOver();
    }
}

    void GameOver()
    {
        Debug.Log("GAME OVER");

        // Stop player movement
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Optional: disable player
        gameObject.SetActive(false);
    }
       void OnCollisionEnter(Collision collision)
    {
        bool isCollide=false;
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isCollide)
            {
                
            }
            if (!isCollide)
            {
                 KinectJump player = collision.gameObject.GetComponent<KinectJump>();
                GameObject particle = Instantiate(landingParticle,transform);
                particle.SetActive(true);
                Destroy(particle,2f);
                isCollide=true;
            }
            

            

            // // Player landed on top
            // if (contact.normal.y < -0.5f)
            // {
            //     Debug.Log("Player landed on cloud");

            //     if (player != null)
            //     {
            //         player.ShowPink();

            //         // ⭐ SPAWN PARTICLE HERE
                   
            //     }
            // }
            // else
            // {
            //     Debug.Log("Player hit cloud side");

            //     if (player != null)
            //     {
            //         player.TakeDamage();
            //         player.ShowRed();
            //     }
            // }
        }
    }   

//    void OnCollisionEnter(Collision collision)
// {
//     if (collision.gameObject.CompareTag("Ground"))
//     {
//         // Spawn particle at player position
//         GameObject particle = Instantiate(landingParticle, transform.position, Quaternion.identity);

//         // Destroy particle after 2 seconds
//         Destroy(particle, 2f);
//     }
//}
}
