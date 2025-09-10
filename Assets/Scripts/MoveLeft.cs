using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float moveLeftSpeed = 8f;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGameOver())
        {
            transform.position += Vector3.left * moveLeftSpeed * GameManager.Instance.GetDifficultyMultiplier() * Time.deltaTime;
        }
    }
}
