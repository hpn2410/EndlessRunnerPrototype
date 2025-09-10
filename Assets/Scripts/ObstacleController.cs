using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private float obstacleSpeed = 5f;

    private float minX = -20f;

    private string poolTag;

    public void SetPoolTag(string tag)
    {
        poolTag = tag;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGameOver())
        {
            transform.position += Vector3.left * obstacleSpeed * Time.deltaTime;

            // return to pool when out of range
            if (transform.position.x < minX)
            {
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        if (!string.IsNullOrEmpty(poolTag))
        {
            ObstaclePooling.Instance.ReturnObject(poolTag, gameObject);
        }
    }

    public float GetObstacleRadius()
    {
        MeshRenderer mr = gameObject.GetComponentInChildren<MeshRenderer>();

        if (mr != null)
        {
            float diameter = Mathf.Max(mr.bounds.size.x, mr.bounds.size.y, mr.bounds.size.z);
            return diameter * 0.5f;
        }
        return 0.1f; // fallback
    }
}
