using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    public float test;
    public PlayerData Data;
   	
    //state Parameters
    public Rigidbody2D RB { get; private set; }
    public BoxCollider2D boxCollider2D { get; private set; }
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping {   get; private set;}
    public bool IsSliding { get; private set; }

    //Timers
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float LastPressedJumpTime { get; private set; }


    private bool _isJumpCut;
    private bool _isJumpFalling;
    private bool _hasLanded;

    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    private Vector2 _moveInput;

    public ParticleSystem dust;
    public ParticleSystem landingDust;
    public Animator animator;
    public AudioSource stepAudio;
	public AudioSource stepAudio2;
	public bool piv;//why is it public?
	SoundManager audioManager;

    
    [Header("Checks")]
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);


    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;

    //RaycastDetection
    struct RaycastOrigins 
    {
        
        public Vector2 bottomLeft, bottomRight;
        public Vector2 topLeft, topRight;
    }
    struct CollisionsInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingMaxSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            slidingMaxSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
            slopeNormal = Vector2.zero;
        }
    }
    RaycastOrigins raycastOrigins;
    CollisionsInfo collisions;
    public int verticalRayCastCount = 3;
    public int horizontalRayCastCount = 3;
    public float  skinWidth = .015f;

    float verticalRaySpecing;
    float horizontalRaySpecing;

    [Header("Slopes")]

    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] float rayThreshold;

    Vector3 velocity;

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 boxColliderSize;

    private Vector2 slopeNormalPerp;

    //Moving Platform
    [HideInInspector]
    public bool isOnPlatform;
    [HideInInspector]
    public Rigidbody2D platformRB;


    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        boxColliderSize = boxCollider2D.size;
		
		audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();		
	}

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;

        CalculateRaySpacing();

    }


    private void Update()
    {
        if (GameMananger.instance.isPause) return;

        animator.SetFloat("yVelocity", RB.velocity.y);

        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        if (GameInput.instance == null)
        {
            Debug.LogError("GameInput instance is not initialized.");
            return;
        }
        _moveInput = GameInput.instance.GetMovmentInput();

        if (GameInput.instance.GetJump())
            OnJumpInput();
        if (GameInput.instance.GetUpJump())
            OnJumpUpInput();



        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);
        #region Collision check
        if (!IsJumping)
        {
            UpdateRaycastOrigins();
            collisions.Reset();

            if (RB.velocity.y < 0)
            {
                DescendSlope();
            }
            GroundCheck();
            SlopeCheck();


            //Ground Check

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
           
        }

        #endregion
       


       

        #region JUMP CHECKS
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;

            _isJumpFalling = false;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            _isJumpFalling = false;
        }

        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            IsWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();

            //AnimHandler.startedJumping = true;
            animator.SetBool("isJumping", IsJumping);
            animator.SetBool("isGrounded", !IsJumping);
        }
        //WALL JUMP
        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;

            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
        }
        #endregion
        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        #endregion

        //GRAVITY
        #region GRAVITY
        if (_isJumpCut)
        {
            //Higher gravity if jump button released
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            isOnPlatform = false;
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if (RB.velocity.y < 0)
        {
            //Higher gravity if falling
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
        }
        else 
        {
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(Data.gravityScale);
        }
        #endregion

    }
    void GroundCheck()
    {

        float rayLength = skinWidth + 1f;
        for (int i = 0; i < verticalRayCastCount; i++)
        {

            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpecing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, _groundLayer);
            Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);
            if (hit)
            {

                rayLength = hit.distance;
                if (LastOnGroundTime < -0.1f && !_hasLanded)
                {

                    //TODO:
                    //AnimHandler.justLanded = true;
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isGrounded", true);
                    // Landing Dust abspielen, wenn der Spieler landet
                    if (landingDust != null)
                    {
                        landingDust.Play();
                        audioManager.PlaySFX(audioManager.JumpLand);
                    }

                    _hasLanded = true;
                }
                else
                    _hasLanded = false;

                LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
            }            
        }
        if (collisions.climbingSlope)
        {
            float direction = IsFacingRight ? 1f : -1f;
            Vector2 rayOrigin = ((direction == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.down;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, rayLength, _groundLayer);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }
    private void SlopeCheck()
    {
        float rayLength = skinWidth*2f*rayThreshold;

        float direction = IsFacingRight ? 1f : -1f;

        for (int i = 0; i < horizontalRayCastCount; i++)
        {
            Vector2 rayOrigin = (direction == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpecing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, rayLength, _groundLayer);

            Debug.DrawRay(rayOrigin, Vector2.right * direction * rayLength, Color.red);

            if (hit)
            {

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= Data.maxSlopeAngle)
                {
                    collisions.slopeNormal = hit.normal;
                    ClimbSlope(slopeAngle);
                }
                else if(i == 0 && slopeAngle >= Data.maxSlopeAngle && slopeAngle < 90)
                    collisions.slidingMaxSlope = true;
            }
        }
    }
    void ClimbSlope(float slopeAngle)
    {
        float moveDistance = Mathf.Abs(_moveInput.x * Data.runMaxSpeed);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        float climbVelocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (RB.velocity.y <= climbVelocityY)
        {
            RB.velocity = new Vector2(climbVelocityX * Mathf.Sign(_moveInput.x), climbVelocityY);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;

        }
    }

    void DescendSlope()
    {

        float direction = IsFacingRight ? 1f : -1f;
        Vector2 rayOrigin = (!IsFacingRight) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, _groundLayer);

        if (hit)
        {
           
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle != 0 && slopeAngle <= Data.maxDescentAngle)
            {
                if (Mathf.Sign(hit.normal.x) == direction)
                {
                    float distanceToSlope = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(RB.velocity.x);
                    float skinAdjustment = hit.distance - skinWidth;

                    if (skinAdjustment <= distanceToSlope)
                    {
                        float moveDistance = Mathf.Abs(_moveInput.x * Data.runMaxSpeed);

                        float descendVelocityY = Mathf.Clamp(Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance, 0f, Data.maxFallSpeed);
                        float velocityX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                        //doesnt work still slides dont know the fix
                        if (_moveInput.x == 0f)
                        {
                            // Counteract gravity fully along the slope
                            Vector2 slopeNormal = hit.normal.normalized;

                            // Remove velocity along the slope
                            Vector2 velocityAlongSlope = Vector3.Project(RB.velocity, slopeNormal);
                            RB.velocity -= velocityAlongSlope;

                            // Fully counteract gravity on the slope
                            float gravityForce = Vector2.Dot(Physics2D.gravity, slopeNormal);
                            RB.AddForce(-gravityForce * slopeNormal, ForceMode2D.Force);

                            // Ensure no sliding
                            RB.velocity = Vector2.zero;
                        }
                        else
                        {
                            // Apply calculated velocities
                            RB.AddForce(velocityX * Mathf.Sign(_moveInput.x) * Vector2.right);
                            RB.AddForce(descendVelocityY * Vector2.down, ForceMode2D.Force);
                        }
                        collisions.below = true;
                        collisions.descendingSlope = true;
                        collisions.slopeAngle = slopeAngle;
                        collisions.slopeNormal = hit.normal;
                    }
                }
            }//Max Slope Handling
            else if (slopeAngle != 0 && slopeAngle >= Data.maxSlopeAngle) 
            {
                float distanceToSlope = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(RB.velocity.x);
                float skinAdjustment = hit.distance - skinWidth;

                if (skinAdjustment <= distanceToSlope)
                {
                    //SOMEHOW DOING NOTTHIGN WORKS THE BEST I GUESS
                    collisions.slidingMaxSlope = true;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameMananger.instance.isPause) return;

        //Handle Run
        if (!collisions.slidingMaxSlope)
        {

        if (IsWallJumping)
                Run(Data.wallJumpRunLerp);
            else
                Run(1);

            //Handle Slide
            if (IsSliding)
            {
                Slide();
            }

        }
        if (isOnPlatform)
        {
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(20);
        }
    }

    public void OnJumpInput()
    {
        isOnPlatform = false;
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }

  
    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        //nameof() notation means we don't need to input a string directly.
        //Removes chance of spelling mistakes and will improve error messages if any
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        if (isOnPlatform && platformRB != null)
        {
            RB.gravityScale = 50;
            Vector2 platformVelocity = platformRB.velocity;
            IsJumping = false;

            if (_moveInput.x == 0)
            {
                RB.velocity = new Vector2(platformVelocity.x, RB.velocity.y);

            }
            else
            {
                // Player input: Combine target speed with platform velocity
                RB.velocity = new Vector2(Mathf.Clamp(targetSpeed + platformVelocity.x, -Data.runMaxSpeed, Data.runMaxSpeed), RB.velocity.y);

            }
        }
        else
        {
            // Apply movement normally when not on a platform
            RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }



        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/

        // Animation Parameters ~ Nam
        // Step Sound
        animator.SetFloat("speed", Math.Abs(_moveInput.x));

        if (Math.Abs(movement) >= 0.1 && _moveInput.x != 0){
            //what does piv mean?
            if (piv == false){
                if (!stepAudio.isPlaying && !stepAudio2.isPlaying){
                    stepAudio.Play();
					piv = true;
				}
                if (IsJumping || IsWallJumping || _isJumpFalling || IsSliding) stepAudio.Stop();
            }
            else{
				if (!stepAudio2.isPlaying && !stepAudio.isPlaying) {
					stepAudio2.Play();
					piv = false;
				}
				if (IsJumping || IsWallJumping || _isJumpFalling || IsSliding) stepAudio2.Stop();
			}
        }
    
    }


    private void Turn()
    {
        if (Time.deltaTime != 0)
        {
            //stores scale and flips the player along the x axis, 
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            IsFacingRight = !IsFacingRight;
        }
        
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;
            RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x,-Data.maxFallSpeed,Data.maxFallSpeed),0);

        if (collisions.slidingMaxSlope)
        {
            if (collisions.slopeNormal != Vector2.zero && (_moveInput.x != -Mathf.Sign(collisions.slopeNormal.x)))
            {
                if (RB.velocity.x < Data.runMaxSpeed)
                {
                    RB.AddForce(collisions.slopeNormal * force, ForceMode2D.Impulse);
                }
                else
                    RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);

            }
            return;
        }
        else
           RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        // Staub beim Sprung abspielen
        if (dust != null)
        {
            dust.Play();
        }
        #endregion
        audioManager.PlaySFX(audioManager.JumpStart);
    }

    private void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
            force.x -= RB.velocity.x;

        if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= RB.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        RB.AddForce(force, ForceMode2D.Impulse);
        #endregion
    }
    private void Slide()
    {
        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = Data.slideSpeed - RB.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }
    #endregion
    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {

        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
             (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.velocity.y > 0;
    }
    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCastCount = Mathf.Clamp(horizontalRayCastCount, 2, int.MaxValue);
        verticalRayCastCount = Mathf.Clamp(verticalRayCastCount, 2, int.MaxValue);

        horizontalRaySpecing = bounds.size.y / (horizontalRayCastCount - 1);
        verticalRaySpecing = bounds.size.x / (verticalRayCastCount - 1);
    }

}
