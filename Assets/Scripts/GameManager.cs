using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    None,
    GamePlay,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Properties")]
    [SerializeField] private Transform spawnPoint; // obstacles spawn pos
    [SerializeField] private string[] obstacleTags;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Difficulty")]
    [SerializeField] private float increaseRate = 0.2f;
    [SerializeField] private float interval = 10f; // countdown timer to increase difficulty
    [SerializeField] private float spawnDelay = 5f; // countdown timer to spawn obstacle


    private float difficultyMultiplier = 1f;
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
        StartCoroutine(ScaleDifficulty());

        SoundManager.Instance.PlaySound(SoundType.Background, true);
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

    private IEnumerator ScaleDifficulty()
    {
        while (!IsGameOver())
        {
            yield return new WaitForSeconds(interval);
            difficultyMultiplier += increaseRate;
        }
    }

    public float GetDifficultyMultiplier()
    {
        return difficultyMultiplier;
    }

    public void ActiveEndGameUI(bool isActive)
    {
        gameOverPanel.SetActive(isActive);
    }

    public void PlayAgainBtnClicked()
    {
        SceneManager.LoadScene("EndLessRunner");
    }

    public void QuitGameBtnClicked()
    {
        UnityEditor.EditorApplication.isPlaying = false; // quit game in editor

        Application.Quit(); // quit game in build
    }
}
