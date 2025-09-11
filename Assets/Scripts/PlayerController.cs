using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float jumpForce = 8f;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] bool fallFaster = false;
    [Header("ParticleSystem")]
    [SerializeField] ParticleSystem dirtParticle;
    [SerializeField] ParticleSystem smokeParticle;
    float groundY = 0f; // ground's pos

    float velocityY = 0f;
    bool isGrounded = true;

    Animator playerAnimator;
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        dirtParticle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGameOver())
        {
            PlayerJump();
            ApplyGravity();
            CheckCollisionWithObstacles();
        }
            
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocityY = jumpForce;
            isGrounded = false;

            dirtParticle.Stop();
            playerAnimator.SetTrigger("Jump");
            SoundManager.Instance.PlaySound(SoundType.Jump);
        }
    }

    void ApplyGravity()
    {
        float currentGravity = gravity;

        if(fallFaster)
        {
            // fall faster
            if (velocityY < 0)
                currentGravity *= 2f;
        }

        velocityY += currentGravity * Time.deltaTime; // (v = v + a*t)
        Vector3 playerPos = transform.position;

        playerPos.y += velocityY * Time.deltaTime; // (y = y + v*t)

        if (playerPos.y <= groundY)
        {
            playerPos.y = groundY;
            velocityY = 0f;

            if (!isGrounded)
            {
                dirtParticle.Play();
            }

            isGrounded = true;
        }

        transform.position = playerPos;
    }

    float GetPlayerRadius()
    {
        SkinnedMeshRenderer smr = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
        {
            float diameter = Mathf.Max(smr.bounds.size.x, smr.bounds.size.y, smr.bounds.size.z);
            return diameter * 0.5f;
        }
        return 0.1f; // fallback
    }

    void CheckCollisionWithObstacles()
    {
        float playerRadius = GetPlayerRadius();

        foreach (Transform obstacle in ObstaclePooling.Instance.transform)
        {
            if (!obstacle.gameObject.activeSelf) continue;

            ObstacleController obstacleController = obstacle.GetComponent<ObstacleController>();
            float obstacleRadius = obstacleController.GetObstacleRadius();

            float distance = Vector3.Distance(transform.position, obstacle.position);

            if (distance < playerRadius + obstacleRadius)
            {
                Debug.Log("Hit obstacle: " + obstacle.name);
                dirtParticle.Stop();
                smokeParticle.Play();
                playerAnimator.SetTrigger("IsDeath");
                GameManager.Instance.SetState(GameState.GameOver);
                SoundManager.Instance.StopSound(SoundType.Background);
                SoundManager.Instance.PlaySound(SoundType.Crash);
                GameManager.Instance.ActiveEndGameUI(GameManager.Instance.IsGameOver());
            }
        }
    }
}
