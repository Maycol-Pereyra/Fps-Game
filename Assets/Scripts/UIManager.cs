using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text scoreText;
    private Text ammoText;
    private Text healthText;
    private Text reloadText;
    private Text reloadingIndicator;
    private GameObject gameOverPanel;
    private Text gameOverScoreText;
    private Text gameOverMessageText;
    private Button restartButton;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        BuildUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAmmoChanged += UpdateAmmoUI;
            GameManager.Instance.OnHealthChanged += UpdateHealthUI;
            GameManager.Instance.OnScoreChanged += UpdateScoreUI;
            GameManager.Instance.OnGameOver += ShowGameOver;
            GameManager.Instance.OnReloadStart += ShowReloading;
            GameManager.Instance.OnReloadEnd += HideReloading;
        }

        UpdateAllUI();
    }

    private void BuildUI()
    {
        scoreText = CreateTextElement("ScoreText", new Vector2(20, -20), new Vector2(300, 50), TextAnchor.UpperLeft, 32, Color.white);
        ammoText = CreateTextElement("AmmoText", new Vector2(20, -70), new Vector2(300, 50), TextAnchor.UpperLeft, 32, Color.yellow);
        healthText = CreateTextElement("HealthText", new Vector2(20, -120), new Vector2(300, 50), TextAnchor.UpperLeft, 32, Color.red);
        reloadText = CreateTextElement("ReloadText", new Vector2(20, -170), new Vector2(400, 50), TextAnchor.UpperLeft, 24, new Color(0.6f, 0.8f, 1f));

        // Center "RELOADING..." indicator
        reloadingIndicator = CreateTextElement("ReloadingIndicator", new Vector2(0, -100), new Vector2(400, 60), TextAnchor.MiddleCenter, 36, Color.yellow);
        RectTransform riRt = reloadingIndicator.GetComponent<RectTransform>();
        riRt.anchorMin = new Vector2(0.5f, 0.5f);
        riRt.anchorMax = new Vector2(0.5f, 0.5f);
        reloadingIndicator.text = "RELOADING...";
        reloadingIndicator.gameObject.SetActive(false);

        CreateCrosshair();
        BuildGameOverPanel();
    }

    private void CreateCrosshair()
    {
        GameObject crosshairObj = new GameObject("Crosshair");
        crosshairObj.transform.SetParent(transform, false);
        Text crosshair = crosshairObj.AddComponent<Text>();
        crosshair.text = "+";
        crosshair.fontSize = 40;
        crosshair.color = Color.white;
        crosshair.alignment = TextAnchor.MiddleCenter;
        crosshair.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        RectTransform rt = crosshairObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(50, 50);
    }

    private void BuildGameOverPanel()
    {
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(transform, false);
        Image panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRt = gameOverPanel.GetComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;

        gameOverMessageText = CreateTextElement("GameOverMessage", new Vector2(0, 80), new Vector2(600, 80), TextAnchor.MiddleCenter, 60, Color.red, gameOverPanel.transform);
        gameOverMessageText.text = "GAME OVER";

        gameOverScoreText = CreateTextElement("GameOverScore", new Vector2(0, 0), new Vector2(600, 60), TextAnchor.MiddleCenter, 40, Color.white, gameOverPanel.transform);

        GameObject btnObj = new GameObject("RestartButton");
        btnObj.transform.SetParent(gameOverPanel.transform, false);
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        restartButton = btnObj.AddComponent<Button>();
        restartButton.onClick.AddListener(OnRestartClicked);
        RectTransform btnRt = btnObj.GetComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0.5f);
        btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        btnRt.anchoredPosition = new Vector2(0, -80);
        btnRt.sizeDelta = new Vector2(250, 60);

        Text btnText = CreateTextElement("BtnText", Vector2.zero, new Vector2(250, 60), TextAnchor.MiddleCenter, 30, Color.white, btnObj.transform);
        btnText.text = "RESTART";
        RectTransform btnTextRt = btnText.GetComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.offsetMin = Vector2.zero;
        btnTextRt.offsetMax = Vector2.zero;
    }

    private Text CreateTextElement(string objectName, Vector2 position, Vector2 size, TextAnchor alignment, int fontSize, Color color, Transform parentOverride = null)
    {
        GameObject obj = new GameObject(objectName);
        obj.transform.SetParent(parentOverride != null ? parentOverride : transform, false);
        Text text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        RectTransform rt = obj.GetComponent<RectTransform>();
        if (parentOverride == null)
        {
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
        }
        else
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
        }
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        return text;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAmmoChanged -= UpdateAmmoUI;
            GameManager.Instance.OnHealthChanged -= UpdateHealthUI;
            GameManager.Instance.OnScoreChanged -= UpdateScoreUI;
            GameManager.Instance.OnGameOver -= ShowGameOver;
            GameManager.Instance.OnReloadStart -= ShowReloading;
            GameManager.Instance.OnReloadEnd -= HideReloading;
        }
    }

    private void UpdateAllUI()
    {
        UpdateScoreUI();
        UpdateAmmoUI();
        UpdateHealthUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null && GameManager.Instance != null)
            scoreText.text = "SCORE: " + GameManager.Instance.CurrentScore;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null && GameManager.Instance != null)
            ammoText.text = "AMMO: " + GameManager.Instance.CurrentAmmo;
        if (reloadText != null && GameManager.Instance != null)
            reloadText.text = "RELOADS: " + GameManager.Instance.ReloadsRemaining + "  [R]";
    }

    private void UpdateHealthUI()
    {
        if (healthText != null && GameManager.Instance != null)
            healthText.text = "LIVES: " + GameManager.Instance.CurrentHealth;
    }

    private void ShowReloading()
    {
        if (reloadingIndicator != null)
            reloadingIndicator.gameObject.SetActive(true);
    }

    private void HideReloading()
    {
        if (reloadingIndicator != null)
            reloadingIndicator.gameObject.SetActive(false);
        UpdateAmmoUI();
    }

    private void ShowGameOver()
    {
        if (reloadingIndicator != null)
            reloadingIndicator.gameObject.SetActive(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (GameManager.Instance != null)
            {
                bool outOfAmmo = GameManager.Instance.CurrentAmmo <= 0 && GameManager.Instance.ReloadsRemaining <= 0;
                bool dead = GameManager.Instance.CurrentHealth <= 0;

                if (gameOverMessageText != null)
                {
                    if (dead)
                        gameOverMessageText.text = "ELIMINATED";
                    else if (outOfAmmo)
                        gameOverMessageText.text = "OUT OF AMMO";
                    else
                        gameOverMessageText.text = "GAME OVER";
                }

                if (gameOverScoreText != null)
                    gameOverScoreText.text = "Final Score: " + GameManager.Instance.CurrentScore;
            }
        }
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}
