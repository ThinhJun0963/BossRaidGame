using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public Button[] answerButtons;

    private bool playerNearby = false;
    private string correctAnswer = "";
    private int selectedOptionIndex = -1;
    private string apiUrl = "https://quizlet-api.onrender.com/api/questions";
    private CanvasGroup dialogueCanvasGroup;
    private EconomyManager EconomyManager;
    void Start()
    {
        dialogueCanvasGroup = dialogueBox.GetComponent<CanvasGroup>() ?? dialogueBox.AddComponent<CanvasGroup>();
        dialogueCanvasGroup.alpha = 0;
        dialogueBox.SetActive(false);
        EconomyManager = new EconomyManager();
        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ShowDialogue();
            StartCoroutine(FetchQuestionFromAPI());
        }

        if (dialogueBox.activeSelf)
        {
            if (Input.GetMouseButtonDown(1)) // Right-click to select answer
            {
                CycleThroughOptions();
            }
            else if (Input.GetKeyDown(KeyCode.F)) // Press Enter to submit answer
            {
                CheckAnswer();
            }
        }
    }

    IEnumerator FetchQuestionFromAPI()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            QuestionData[] questionsArray = JsonHelper.FromJson<QuestionData>(json);

            if (questionsArray.Length > 0)
            {
                QuestionData questionData = questionsArray[Random.Range(0, questionsArray.Length)];
                DisplayQuestion(questionData);
            }
        }
        else
        {
            Debug.LogError("Failed to fetch question: " + request.error);
        }
    }

    void DisplayQuestion(QuestionData questionData)
    {
        dialogueText.text = questionData.question;
        correctAnswer = questionData.answer;
        selectedOptionIndex = -1;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < questionData.options.Length)
            {
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = questionData.options[i];
                answerButtons[i].gameObject.SetActive(true);
                ResetButtonColor(answerButtons[i]);
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void CycleThroughOptions()
    {
        if (selectedOptionIndex < answerButtons.Length - 1)
        {
            selectedOptionIndex++;
        }
        else
        {
            selectedOptionIndex = 0; // Loop back to the first option
        }

        HighlightSelectedOption();
    }

    void HighlightSelectedOption()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            ResetButtonColor(answerButtons[i]);
        }

        if (selectedOptionIndex >= 0 && selectedOptionIndex < answerButtons.Length)
        {
            var cb = answerButtons[selectedOptionIndex].colors;
            cb.normalColor = Color.green;
            answerButtons[selectedOptionIndex].colors = cb;
        }
    }

    void CheckAnswer()
    {
        if (selectedOptionIndex == -1)
        {
            dialogueText.text = "Please select an option first.";
            return;
        }

        string selectedAnswer = answerButtons[selectedOptionIndex].GetComponentInChildren<TMP_Text>().text;

        if (selectedAnswer.Equals(correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            dialogueText.text = "✅ Correct! Fetching new question...";
            EconomyManager.UpdateCurrentGold();
        }
        else
        {
            dialogueText.text = "❌ Wrong answer! Try again.";
        }

        StartCoroutine(NextQuestionAfterDelay(2f));
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FetchQuestionFromAPI());
    }

    void ResetButtonColor(Button button)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        button.colors = cb;
    }

    void ShowDialogue()
    {
        dialogueBox.SetActive(true);
        StartCoroutine(FadeCanvas(dialogueCanvasGroup, 0, 1, 0.3f));
    }

    IEnumerator FadeCanvas(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            StartCoroutine(FadeCanvas(dialogueCanvasGroup, 1, 0, 0.3f));
        }
    }
}

[System.Serializable]
public class QuestionData
{
    public string question;
    public string answer;
    public string[] options;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
