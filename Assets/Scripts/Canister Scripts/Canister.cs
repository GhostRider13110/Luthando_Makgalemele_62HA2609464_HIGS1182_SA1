using UnityEngine;

public class Canister : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Canister Picked up");

            GameManager.Instance.CanisterCollected();

            AudioManager.Instance.PlayOneShot("Canister");

            Destroy(gameObject);
        }
    }
}
