using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float shootRange = 100f;



    // Update is called once per frame
    private void Update()
    {
        AimAtMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            AudioManager.Instance.PlayOneShot("Shooting");
        }
    }

    private void AimAtMouse()
    {
        Ray ray = playerCamera.ScreenPointToRay (Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPostion = ray.GetPoint (distance);

            Vector3 direction = mouseWorldPostion - transform.position;

            direction.y = 0f; 

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    private void Shoot()
    {
       Vector3 origin = transform.position + transform.forward * 0.5f;
        Vector3 direction = transform.forward; 

        Debug.DrawRay(origin, direction * shootRange, Color.blue, 2f);

        Debug.Log("Shooting!");

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange))
        {
            Debug.Log ("Hit: " + hit.collider.name);

            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.Die();
                return; 
            }

            Asteroid asteroid = hit.collider.GetComponent<Asteroid>();
            if (asteroid != null)
            {
                asteroid.DestroyAsteroid();
            }
        }

    }
}
