using System.Collections;
using UnityEngine;

public enum GameState
{
    None,
    GamePlay,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Transform spawnPoint; // obstacles spawn pos
    [SerializeField] private string[] obstacleTags;

    private float spawnDelay = 5f;
    private GameState currentState;

    public void SetState(GameState state)
    {
        currentState = state;
    }

    public bool IsGamePlay()
    {
        if (currentState == GameState.GamePlay)
            return true;
        else
            return false;
    }

    public bool IsGameOver()
    {
        if (currentState == GameState.GameOver)
            return true;
        else
            return false;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(InitializeAndSpawn());
    }

    private IEnumerator InitializeAndSpawn()
    {
        while (!ObstaclePooling.Instance.IsInitialized())
        {
            yield return null;
        }
        if (obstacleTags.Length > 0)
        {
            StartCoroutine(SpawnObstacles());
        }
    }

    private IEnumerator SpawnObstacles()
    {
        while (!IsGameOver())
        {
            // get randomTag from obstacleTags
            string randomTag = obstacleTags[Random.Range(0, obstacleTags.Length)];

            // get obstacle from pool by randomTag
            GameObject obstacle = ObstaclePooling.Instance.GetObject(randomTag);
            if (obstacle != null)
            {
                Vector3 newPosition = obstacle.transform.position;
                newPosition.x = spawnPoint.position.x;
                obstacle.transform.position = newPosition;

                ObstacleController obstacleController = obstacle.GetComponent<ObstacleController>();
                if (obstacleController != null)
                {
                    obstacleController.SetPoolTag(randomTag);
                }
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
