using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private GameObject destroyEffectPrefab;

    public int ScoreValue => scoreValue;

    public void OnHit()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
