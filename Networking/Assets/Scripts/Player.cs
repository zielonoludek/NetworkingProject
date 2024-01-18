using UnityEngine;
public class Player : MonoBehaviour
{
    public string name;
    public Vector3 targetPosition;
    public CharacterController characterController;

    public virtual void Update()
    {
        characterController.Move(targetPosition * Time.deltaTime);
    }

    public virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
}
