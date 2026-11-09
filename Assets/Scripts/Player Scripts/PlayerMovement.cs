using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movespeed = 5f;

    private Vector2 moveInput; 

    // Update is called once per frame
    private void Update()
    {
       Vector3 movement = new Vector3(moveInput.x,0f, moveInput.y);
        float speedMultiplier = GameManager.Instance != null ? GameManager.Instance.SpeedMultiplier : 1f;
        transform.Translate(movement* movespeed * speedMultiplier * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
       moveInput = value.Get<Vector2>();
    }
}
