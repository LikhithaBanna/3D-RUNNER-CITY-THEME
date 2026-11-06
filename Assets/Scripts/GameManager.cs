
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins;
    public int gems;
    public float distance;
    public bool gameOver;

    [Header("UI References")]
    public Text hudText;
    public Text messageText;
    public GameObject restartButton;

    Texture2D whiteTexture;

    void Awake()
    {
        Instance = this;
        gameOver = false;
        Time.timeScale = 1f;
    }

    public void SetupUI(Text hud, Text message, GameObject restart)
    {
        hudText = hud;
        messageText = message;
        restartButton = restart;

        if (hudText != null)
            hudText.text = "";

        if (messageText != null)
            messageText.text = "";

        if (restartButton != null)
            restartButton.SetActive(false);
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        gameOver = false;
        coins = 0;
        gems = 0;
        distance = 0f;

        Time.timeScale = 1f;

        if (hudText != null)
            hudText.text = "Distance: 0 m    Coins: 0    Gems: 0";

        if (messageText != null)
            messageText.text = "";

        if (restartButton != null)
            restartButton.SetActive(false);
    }

    public void AddCoin()
    {
        coins++;
        UpdateHUD();
    }

    public void AddGem()
    {
        gems++;
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (hudText != null)
        {
            hudText.text =
                "Distance: " + Mathf.FloorToInt(distance) + " m" +
                "    Coins: " + coins +
                "    Gems: " + gems;
        }
    }

    void Update()
    {
        if (!gameOver && Time.timeScale > 0f)
        {
            distance += Time.deltaTime * 5f;
            UpdateHUD();
        }
    }

    public void EndGame()
    {
        if (gameOver)
            return;

        gameOver = true;

        if (messageText != null)
        {
            messageText.text =
                "GAME OVER\n\n" +
                "Distance: " + Mathf.FloorToInt(distance) + " m\n" +
                "Coins Collected: " + coins + "\n" +
                "Gems Collected: " + gems;
        }

        if (restartButton != null)
            restartButton.SetActive(false);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        if (gameOver)
        {
            // Large red GAME OVER heading
            GUIStyle headingStyle = new GUIStyle();
            headingStyle.fontSize = 60;
            headingStyle.fontStyle = FontStyle.Bold;
            headingStyle.alignment = TextAnchor.MiddleCenter;
            headingStyle.normal.textColor = Color.red;

            GUI.Label(
                new Rect(
                    0,
                    Screen.height / 2 - 220,
                    Screen.width,
                    90
                ),
                "GAME OVER",
                headingStyle
            );

            // Large red score details
            GUIStyle scoreStyle = new GUIStyle();
            scoreStyle.fontSize = 38;
            scoreStyle.fontStyle = FontStyle.Bold;
            scoreStyle.alignment = TextAnchor.MiddleCenter;
            scoreStyle.normal.textColor = Color.red;

            GUI.Label(
                new Rect(
                    0,
                    Screen.height / 2 - 120,
                    Screen.width,
                    230
                ),
                "Distance: " + Mathf.FloorToInt(distance) + " m\n\n" +
                "Coins Collected: " + coins + "\n\n" +
                "Gems Collected: " + gems,
                scoreStyle
            );

            // White Restart button with bold black letters
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 30;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.textColor = Color.black;
            buttonStyle.normal.background = GetWhiteTexture();

            if (GUI.Button(
                new Rect(
                    Screen.width / 2 - 120,
                    Screen.height / 2 + 150,
                    240,
                    75
                ),
                "RESTART",
                buttonStyle
            ))
            {
                RestartGame();
            }
        }
    }

    Texture2D GetWhiteTexture()
    {
        if (whiteTexture == null)
        {
            whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
        }

        return whiteTexture;
    }
}
