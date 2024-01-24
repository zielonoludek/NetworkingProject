using UnityEngine;

public class LocalPlayer : Player
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    public void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        transform.Rotate(200 * mouseX * Time.deltaTime * Vector3.up);

        Vector3 rotation = cameraTransform.rotation.eulerAngles + new Vector3(-mouseY, 0, 0);
        rotation.x = ClampAngle(rotation.x, -90, 90);
        cameraTransform.eulerAngles = rotation;

        characterController.Move(targetPosition * Time.deltaTime);

        SendPlayerInput();
    }

    private void SendPlayerInput()
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
        };
        PacketSenders.PlayerMovement(inputs, transform.position, transform.rotation);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < 0)
        {
            angle = 360 + angle;
        }
        if (angle > 180)
        {
            return Mathf.Max(angle, 360 + min);
        }
        return Mathf.Min(angle, max);
    }
}
