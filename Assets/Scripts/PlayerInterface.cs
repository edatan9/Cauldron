using UnityEngine;
using UnityEngine.SceneManagement;

// Runtime interface for the playable test scene; the existing gameplay HUD stays in place.
public class PlayerInterface : MonoBehaviour
{
    const string VolumeKey = "Cauldron.MasterVolume";
    const string BestKey = "Cauldron.BestScore";
    enum ScreenState { Main, Playing, Paused, Settings, GameOver }
    static bool startPlaying;
    ScreenState screen = ScreenState.Main, settingsReturn;
    GameManager game;
    IngredientDrag[] ingredients;
    float volume;
    int bestScore, finalScore;
    GUIStyle title, label, button;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (SceneManager.GetActiveScene().name != "Level1_Test") return;
        new GameObject("PlayerInterface").AddComponent<PlayerInterface>();
    }

    void Awake()
    {
        volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
        AudioListener.volume = volume;
        bestScore = PlayerPrefs.GetInt(BestKey, 0);
        ingredients = FindObjectsByType<IngredientDrag>();
        SetScreen(startPlaying ? ScreenState.Playing : ScreenState.Main);
        startPlaying = false;
    }

    void Start()
    {
        game = GameManager.Instance;
        if (game == null) { Debug.LogError("PlayerInterface: GameManager bulunamadi."); return; }
        // Suppress the temporary GameManager OnGUI panel while preserving its game-over flow.
        var placeholder = new GameObject("GameOverPanelPlaceholder");
        placeholder.transform.SetParent(transform);
        placeholder.SetActive(false);
        game.gameOverPanel = placeholder;
        game.OnGameOver += GameOver;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (screen == ScreenState.Playing) SetScreen(ScreenState.Paused);
        else if (screen == ScreenState.Paused) SetScreen(ScreenState.Playing);
        else if (screen == ScreenState.Settings) SetScreen(settingsReturn);
    }

    void SetScreen(ScreenState next)
    {
        screen = next;
        bool playing = next == ScreenState.Playing;
        Time.timeScale = playing ? 1f : 0f;
        if (ingredients != null)
            foreach (var ingredient in ingredients) if (ingredient != null) ingredient.enabled = playing;
    }

    void GameOver(int score)
    {
        finalScore = score;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestKey, bestScore);
            PlayerPrefs.Save();
        }
        SetScreen(ScreenState.GameOver);
    }

    void Restart(bool playImmediately)
    {
        startPlaying = playImmediately;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        if (title == null)
        {
            title = new GUIStyle(GUI.skin.label) { fontSize = 28, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            label = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
            button = new GUIStyle(GUI.skin.button) { fontSize = 18 };
        }
        if (screen == ScreenState.Playing)
        {
            if (GUI.Button(new Rect(Screen.width - 120, 12, 108, 36), "Duraklat")) SetScreen(ScreenState.Paused);
            return;
        }
        float w = Mathf.Min(400, Screen.width - 30), h = Mathf.Min(410, Screen.height - 30);
        Rect box = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
        GUI.Box(box, "");
        GUILayout.BeginArea(new Rect(box.x + 20, box.y + 15, box.width - 40, box.height - 30));
        if (screen == ScreenState.Main)
        {
            GUILayout.Label("CAULDRON", title, GUILayout.Height(50));
            GUILayout.Label("En yuksek puan: " + bestScore, label, GUILayout.Height(45));
            if (GUILayout.Button("Oyna", button, GUILayout.Height(48))) SetScreen(ScreenState.Playing);
            if (GUILayout.Button("Ayarlar", button, GUILayout.Height(48))) OpenSettings(ScreenState.Main);
            if (GUILayout.Button("Cikis", button, GUILayout.Height(48))) Application.Quit();
        }
        else if (screen == ScreenState.Paused)
        {
            GUILayout.Label("DURAKLATILDI", title, GUILayout.Height(60));
            if (GUILayout.Button("Devam et", button, GUILayout.Height(48))) SetScreen(ScreenState.Playing);
            if (GUILayout.Button("Ayarlar", button, GUILayout.Height(48))) OpenSettings(ScreenState.Paused);
            if (GUILayout.Button("Yeniden baslat", button, GUILayout.Height(48))) Restart(true);
        }
        else if (screen == ScreenState.Settings)
        {
            GUILayout.Label("AYARLAR", title, GUILayout.Height(60));
            GUILayout.Label("Genel ses: %" + Mathf.RoundToInt(volume * 100), label, GUILayout.Height(40));
            float changed = GUILayout.HorizontalSlider(volume, 0f, 1f, GUILayout.Height(35));
            if (!Mathf.Approximately(changed, volume))
            {
                volume = changed;
                AudioListener.volume = volume;
                PlayerPrefs.SetFloat(VolumeKey, volume);
                PlayerPrefs.Save();
            }
            if (GUILayout.Button("Geri", button, GUILayout.Height(48))) SetScreen(settingsReturn);
        }
        else if (screen == ScreenState.GameOver)
        {
            GUILayout.Label("OYUN BITTI", title, GUILayout.Height(60));
            GUILayout.Label("Puan: " + finalScore + "    En yuksek: " + bestScore, label, GUILayout.Height(55));
            if (GUILayout.Button("Tekrar oyna", button, GUILayout.Height(48))) Restart(true);
            if (GUILayout.Button("Ana menu", button, GUILayout.Height(48))) Restart(false);
        }
        GUILayout.EndArea();
    }

    void OpenSettings(ScreenState previous) { settingsReturn = previous; SetScreen(ScreenState.Settings); }
    void OnDestroy()
    {
        if (game != null) game.OnGameOver -= GameOver;
        Time.timeScale = 1f;
    }
}
