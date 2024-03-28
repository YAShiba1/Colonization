using System.Collections;
using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    [SerializeField] private Gold _goldPrefab;
    [SerializeField][Min(0)] private float _spawnAreaRadius;
    [SerializeField][Range(0, 30)] private int _amount;
    [SerializeField][Min(0)] private float _delay;

    private void Start()
    {
        StartCoroutine(SpawnGoldWithDelay());
    }

    private void Update()
    {
        RandomGoldSpawnByClick();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnAreaRadius);
    }

    private IEnumerator SpawnGoldWithDelay()
    {
        var waitForeSeconds = new WaitForSeconds(_delay);

        for (int i = 0; i < _amount; i++)
        {
            yield return waitForeSeconds;

            RandomGoldSpawn();
        }
    }

    private void RandomGoldSpawn()
    {
        Vector2 randomPointInsideCircle = Random.insideUnitCircle * _spawnAreaRadius;

        float randomX = transform.position.x + randomPointInsideCircle.x;
        float randomZ = transform.position.z + randomPointInsideCircle.y;

        Instantiate(_goldPrefab, new Vector3(randomX, _goldPrefab.transform.position.y, randomZ), Quaternion.identity);
    }

    private void RandomGoldSpawnByClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RandomGoldSpawn();
        }
    }
}
