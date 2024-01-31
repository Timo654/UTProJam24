using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player_Walk : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D body;
    private PlayerControls playerControls;
    private InputAction move;
    private InputAction jump;
    Player_GroundDetection ground;

    [Header("Calculations")]
    public float directionX;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;
    private float acceleration;
    private float deceleration;
    private float turnSpeed;

    [Header("Movement Stats")]
    [SerializeField, Range(0f, 20f)][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed")] public float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop after letting go")] public float maxDecceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed when in mid-air")] public float maxAirAcceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop in mid-air when no direction is used")] public float maxAirDeceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction when in mid-air")] public float maxAirTurnSpeed = 80f;
    [SerializeField] private Animator playerAnimator;
    [Header("Current State")]
    public bool onGround;
    public bool pressingKey;
    private bool isPaused = false;
    //public ParticleSystem dust;
    // Audio
    public EventInstance playerFootsteps;
    public CurrentPlayer player = CurrentPlayer.Present;
    private void Start()
    {
        switch (player)
        {
            case CurrentPlayer.Present:
                playerFootsteps = AudioManager.Instance.CreateInstance(FMODEvents.Instance.StepsFuture);
                break;
            case CurrentPlayer.Past:
                playerFootsteps = AudioManager.Instance.CreateInstance(FMODEvents.Instance.StepsPast);
                break;
        }
    }

    //public void CreateDust()
    //{
    //    dust.Play();
    //}

    private void PauseToggled(bool paused)
    {
        isPaused = paused;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<Player_GroundDetection>();
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        move = playerControls.Gameplay.Move;
        move.Enable();
        move.started += OnMovement;
        move.canceled += OnMovement;
        PauseMenuController.GamePaused += PauseToggled;
        jump = playerControls.Gameplay.Jump;
        jump.Enable();
    }
    private void OnDisable()
    {
        directionX = 0f;
        PauseMenuController.GamePaused -= PauseToggled;
        move.started -= OnMovement;
        move.canceled -= OnMovement;
        move.Disable();
        jump.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
        //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
        if (!isPaused)
        {
            directionX = context.ReadValue<float>();
        }
        else
        {
            directionX = 0;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Used to flip the character's sprite when they change direction
        //Also tells us that we are currently pressing a direction button
        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);
            pressingKey = true;
        }
        else
        {
            pressingKey = false;
        }
        playerAnimator.SetBool("Running", pressingKey);

        desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(maxSpeed, 0f);
    }

    private void FixedUpdate()
    {
        //Fixed update runs in sync with Unity's physics engine
        onGround = ground.GetOnGround();
        velocity = body.velocity;
        runWithAcceleration();
    }

    private void runWithAcceleration()
    {
        //Set our acceleration, deceleration, and turn speed stats, based on whether we're on the ground on in the air

        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        deceleration = onGround ? maxDecceleration : maxAirDeceleration;
        turnSpeed = onGround ? maxTurnSpeed : maxAirTurnSpeed;

        if (pressingKey)
        {
            //If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around and so should use the turn speed stat.
            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                //If they match, it means we're simply running along and so should use the acceleration stat
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            //And if we're not pressing a direction at all, use the deceleration stat
            maxSpeedChange = deceleration * Time.deltaTime;
        }
        //Move our velocity towards the desired velocity, at the rate of the number calculated above
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        // audio stuff. might rewrite this, not sure yet
        if (velocity.x != 0 & pressingKey)
        {
            // get the playback state
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
        //Update the Rigidbody with this new velocity
        body.velocity = velocity;
    }

}
