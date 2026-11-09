using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PowerUpSpawn : MonoBehaviour
{
    [SerializeField] public GameObject powerUpPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float heightOffset = 0.5f;

    [Header("Timing")]
    [SerializeField] private float minDelay = 5f;
    [SerializeField] private float maxDelay = 12f;
    [SerializeField] private float lifeTime = 10f;

    private GameObject current;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

        if (TryGetSpawnPoint(out Vector3 position))
        {
            current = Instantiate(powerUpPrefab, position + Vector3.up * heightOffset, Quaternion.identity);
            
            if (lifeTime >0f ) Destroy(current, lifeTime);

            yield return new WaitUntil(() => current == null);
        }
    }

    private bool TryGetSpawnPoint(out Vector3 result)
    {
        for (int i = 0; i <10; i++)
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPoint = transform.position + new Vector3(circle.x, 0f, circle.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true; 
            }
        }

        result = Vector3.zero;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
