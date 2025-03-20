using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    private string registerURL = "https://quizlet-api.onrender.com/api/users/register"; // Adjust as needed

    public void OnRegisterButtonClick()
    {
        StartCoroutine(RegisterUser());
    }
    public void OnLoginButtonClick()
    {
        SceneManager.LoadScene("LoginScene");
    }
    IEnumerator RegisterUser()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        string jsonData = $"{{\"username\": \"{username}\", \"email\": \"{email}\", \"password\": \"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(registerURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                messageText.text = "Registration Successful! Redirecting...";
                yield return new WaitForSeconds(1.5f);
                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                string errorMessage = "❌ Registration Failed!";
                try
                {
                    //Parse JSON response to extract the "message" field
                    string jsonResponse = request.downloadHandler.text;
                    var responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);
                    if (!string.IsNullOrEmpty(responseData.message))
                    {
                        errorMessage = "❌ " + responseData.message; // Display API error message
                    }
                }
                catch
                {
                    errorMessage = "❌ Server error. Please try again later.";
                }

                messageText.text = errorMessage;
            }
        }
    }
}
[System.Serializable]
public class ResponseData
{
    public string message;
}