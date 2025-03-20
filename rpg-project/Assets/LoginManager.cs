using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    private string loginURL = "https://quizlet-api.onrender.com/api/users/login"; // Adjust as needed

    public void OnLoginButtonClick()
    {
        StartCoroutine(LoginUser());
    }

    public void OnSignInButtonClick()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    IEnumerator LoginUser()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string jsonData = $"{{\"username\": \"{username}\", \"password\": \"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(loginURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                try
                {
                    LoginResponse responseData = JsonUtility.FromJson<LoginResponse>(jsonResponse);
                    if (responseData != null && responseData.user != null)
                    {
                        PlayerPrefs.SetString("LoggedInUsername", responseData.user.username);
                        PlayerPrefs.SetInt("UserCoins", responseData.user.coins);
                        PlayerPrefs.Save();

                        messageText.text = $"✅ Login Successful! Entering town...";
                    }
                    else
                    {
                        messageText.text = "❌ Login Failed! Invalid server response.";
                        yield break; // Stop execution
                    }
                }
                catch
                {
                    messageText.text = "❌ Server error. Please try again later.";
                    yield break; // Stop execution
                }
            }
            else
            {
                string errorMessage = "❌ Login Failed!";
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    LoginResponse responseData = JsonUtility.FromJson<LoginResponse>(jsonResponse);
                    if (!string.IsNullOrEmpty(responseData.message))
                    {
                        errorMessage = "❌ " + responseData.message;
                    }
                }
                catch
                {
                    errorMessage = "❌ Server error. Please try again later.";
                }

                messageText.text = errorMessage;
                yield break; // Stop execution
            }
        }

        // ✅ Now wait AFTER exiting the using block
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Town");
    }

}

// ✅ JSON Response Classes
[System.Serializable]
public class LoginResponse
{
    public string message;
    public UserData user;
}

[System.Serializable]
public class UserData
{
    public string id;
    public string username;
    public string email;
    public int coins;
}
