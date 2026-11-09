using TreeEditor;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private float bobHeight = 0.2f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = startPosition + Vector3.up * offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null) GameManager.Instance.ActivateSpeedBoost();

        AudioManager.Instance.PlayOneShot("SpeedBoost");

        Destroy(gameObject);
    }
}
