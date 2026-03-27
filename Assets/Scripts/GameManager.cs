using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int startingAmmo = 30;
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private int maxReloads = 1;
    [SerializeField] private float reloadDuration = 2f;

    private int currentAmmo;
    private int currentHealth;
    private int currentScore;
    private int reloadsRemaining;
    private bool isGameOver;
    private bool isReloading;

    public int CurrentAmmo => currentAmmo;
    public int CurrentHealth => currentHealth;
    public int CurrentScore => currentScore;
    public int ReloadsRemaining => reloadsRemaining;
    public float ReloadDuration => reloadDuration;
    public bool IsGameOver => isGameOver;
    public bool IsReloading => isReloading;

    public System.Action OnAmmoChanged;
    public System.Action OnHealthChanged;
    public System.Action OnScoreChanged;
    public System.Action OnGameOver;
    public System.Action OnReloadStart;
    public System.Action OnReloadEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentAmmo = startingAmmo;
        currentHealth = startingHealth;
        currentScore = 0;
        reloadsRemaining = maxReloads;
        isGameOver = false;
        isReloading = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool TryUseAmmo()
    {
        if (isGameOver || isReloading || currentAmmo <= 0) return false;
        currentAmmo--;
        OnAmmoChanged?.Invoke();

        if (currentAmmo <= 0 && reloadsRemaining > 0)
        {
            StartReload();
        }
        else if (currentAmmo <= 0 && reloadsRemaining <= 0)
        {
            TriggerGameOver();
        }

        return true;
    }

    public bool CanReload()
    {
        return !isGameOver && !isReloading && reloadsRemaining > 0 && currentAmmo < startingAmmo;
    }

    public void StartReload()
    {
        if (!CanReload()) return;
        isReloading = true;
        OnReloadStart?.Invoke();
        StartCoroutine(ReloadCoroutine());
    }

    private System.Collections.IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadDuration);
        FinishReload();
    }

    private void FinishReload()
    {
        reloadsRemaining--;
        currentAmmo = startingAmmo;
        isReloading = false;
        OnReloadEnd?.Invoke();
        OnAmmoChanged?.Invoke();
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;
        currentScore += points;
        OnScoreChanged?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        OnHealthChanged?.Invoke();
        if (currentHealth <= 0)
            TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        isReloading = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
