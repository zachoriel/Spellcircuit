using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Jumping & Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float verticalVelocity = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
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

        bool running = Input.GetKey(Globals.KeyBinds.RUN_KEY);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float speed = 0f;

        if (direction.magnitude >= 0.1f)
        {
            speed = running ? runSpeed : walkSpeed;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move((moveDir.normalized * speed + Vector3.up * verticalVelocity) * Time.deltaTime);
        }
        else
        {
            // Apply gravity even when idle
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        float speedPercent = direction.magnitude * (running ? 1f : 0.5f);
        animator.SetFloat("Speed", speedPercent);
        animator.SetFloat("Horizontal", horizontal);
    }
}
