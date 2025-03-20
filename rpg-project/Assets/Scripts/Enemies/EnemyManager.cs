using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;

public class EnemyManager : MonoBehaviour
{
    private int totalEnemies;
    public GameObject areaExit; // Assign in Inspector
    public TextMeshProUGUI victoryMessage; // Assign in Inspector
    private AudioSource audioSource;
    private string updateCoinsURL = "https://quizlet-api.onrender.com/api/users/update-coins"; // Replace with your actual API
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (areaExit == null)
        {
            Debug.LogError("❌ areaExit is NOT assigned! Assign it in Inspector.");
        }
        if (victoryMessage == null)
        {
            Debug.LogError("❌ victoryMessage is NOT assigned! Assign it in Inspector.");
        }
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        audioSource = GetComponent<AudioSource>();
        ResetState(); //Ensure reset on Start
    }

    public void EnemyDefeated()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length - 1;
        Debug.Log("Updated Enemy Count: " + totalEnemies);
        if (totalEnemies <= 0)
        {
            Debug.Log("All enemies defeated! Area Exit unlocked.");
            areaExit.SetActive(true); // Show Area Exit
            PlayVictorySound();
            StartCoroutine(ShowVictoryMessage());
            _ = UpdateCoinsAsync();
        }
    }

    private void PlayVictorySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    private IEnumerator ShowVictoryMessage()
    {
        victoryMessage.gameObject.SetActive(true);
        victoryMessage.text = "All enemies defeated!";
        yield return new WaitForSeconds(3f);
        victoryMessage.gameObject.SetActive(false);
    }

    // ✅ Reset state when entering a new scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
        ResetState();
    }

    private void ResetState()
    {
        Debug.Log("ResetState called!"); // Debugging log
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (areaExit != null)
        {
            areaExit.SetActive(false); // Hide exit
            Debug.Log("areaExit is set to false");
        }

        if (victoryMessage != null)
        {
            victoryMessage.gameObject.SetActive(false); // Hide message
            Debug.Log("Victory message is set to false");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // ✅ Unsubscribe to prevent memory leaks
    }
    private async Task UpdateCoinsAsync()
    {
        string username = PlayerPrefs.GetString("LoggedInUsername", "Guest");
        int currentCoins = EconomyManager.Instance.GetCurrentGold(); // Fetch from EconomyManager

        Debug.Log($"Updating coins for {username}: {currentCoins}");

        string jsonData = $"{{\"username\": \"{username}\", \"coins\": {currentCoins}}}";

        using (UnityWebRequest request = new UnityWebRequest(updateCoinsURL, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); // ✅ Prevent game from freezing
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Coins updated successfully!");
            }
            else
            {
                Debug.LogError("❌ Failed to update coins: " + request.error);
            }
        }
    }
}
