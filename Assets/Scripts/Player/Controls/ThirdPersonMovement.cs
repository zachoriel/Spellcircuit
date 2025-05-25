using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class ThirdPersonMovement : BaseAnimationController
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private SpellCaster spellCaster;
    [SerializeField] private GameObject defaultCinemachineCam;
    [SerializeField] private GameObject aimingCinemachineCam;
    [SerializeField] private CinemachineBrain brain;
    private CinemachineOrbitalFollow defaultCMCamOrbitFollow;
    private CinemachineOrbitalFollow aimingCMCamOrbitFollow;

    [Header("Movement Settings")]
    [SerializeField] private float aimingSpeed = 1.5f;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Jumping & Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float verticalVelocity = 0f;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultCMCamOrbitFollow = defaultCinemachineCam.GetComponent<CinemachineOrbitalFollow>();
        aimingCMCamOrbitFollow = aimingCinemachineCam.GetComponent<CinemachineOrbitalFollow>();
    }

    private void Update()
    {
        HandleGravity();
        HandleAiming();
        HandleMovement();
    }

    private void HandleGravity()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void HandleAiming()
    {
        isAiming = Input.GetKey(Globals.KeyBinds.AIM_BUTTON);
        SetAiming(isAiming);

        // Manually sync CM cameras to the correct orientation when transitioning in/out of aiming.
        if (!isAiming && !defaultCinemachineCam.gameObject.activeSelf)
        {
            defaultCMCamOrbitFollow.HorizontalAxis.Value = aimingCMCamOrbitFollow.HorizontalAxis.Value;
            defaultCMCamOrbitFollow.VerticalAxis.Value = aimingCMCamOrbitFollow.VerticalAxis.Value;
        }
        else if (isAiming && defaultCinemachineCam.gameObject.activeSelf)
        {
            aimingCMCamOrbitFollow.HorizontalAxis.Value = defaultCMCamOrbitFollow.HorizontalAxis.Value;
            aimingCMCamOrbitFollow.VerticalAxis.Value = defaultCMCamOrbitFollow.VerticalAxis.Value;
        }

        defaultCinemachineCam.SetActive(!isAiming);
        aimingCinemachineCam.SetActive(isAiming);
        brain.ManualUpdate();

        // Handle player orientation and attack detection.
        if (isAiming)
        {
            HandleAimingInput();
            if (Input.GetKeyDown(Globals.KeyBinds.ATTACK_BUTTON))
            {
                TriggerCastAnimation();
            }
        }
    }

    private void HandleAimingInput()
    {
        Vector3 camFwd = playerCamera.transform.forward;
        camFwd.y = 0f;

        if (camFwd != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(camFwd);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    private void TriggerCastAnimation()
    {
        TriggerSpellcast();
    }

    public void SpawnSpellFromAnimation()
    {
        spellCaster.CastTestSpell();
    }

    private void HandleMovement()
    {
        // Movement
        bool running = Input.GetKey(Globals.KeyBinds.RUN_KEY) && !isAiming;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float speed = 0f;

        if (direction.magnitude >= 0.1f)
        {
            speed = running ? runSpeed : (isAiming ? aimingSpeed : walkSpeed);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            if (!isAiming)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move((moveDir.normalized * speed + Vector3.up * verticalVelocity) * Time.deltaTime);
        }
        else
        {
            // Apply gravity even when idle
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        float speedPercent = direction.magnitude * (running ? 1f : 0.5f);
        UpdateMovement(speedPercent);
    }
}
