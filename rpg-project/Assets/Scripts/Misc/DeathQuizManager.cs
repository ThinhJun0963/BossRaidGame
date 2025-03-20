using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathQuizManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public Button[] answerButtons;

    private string correctAnswer = "";
    private int selectedOptionIndex = -1;
    private string apiUrl = "https://quizlet-api.onrender.com/api/questions";
    private CanvasGroup dialogueCanvasGroup;
    private bool questionFetched = false;
    public void Start()
    {
        dialogueCanvasGroup = dialogueBox.GetComponent<CanvasGroup>() ?? dialogueBox.AddComponent<CanvasGroup>();
        dialogueCanvasGroup.alpha = 0;
        dialogueBox.SetActive(false);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        StartCoroutine(FetchQuestionFromAPI());
    }

    public void ShowDialogue()
    {
        dialogueBox.SetActive(true);
        StartCoroutine(FadeCanvas(dialogueCanvasGroup, 0, 1, 0.3f));
        StartCoroutine(FetchQuestionFromAPI());
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

    void Update()
    {
        if (!questionFetched) return;

        if (Input.GetMouseButtonDown(1)) // Right-click to cycle options
        {
            CycleThroughOptions();
        }
        else if (Input.GetKeyDown(KeyCode.F)) // Press 'F' to submit answer
        {
            CheckAnswer();
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
                DisplayQuestion(questionsArray[Random.Range(0, questionsArray.Length)]);
                questionFetched = true;
            }
        }
        else
        {
            Debug.LogError("Failed to fetch question: " + request.error);
            dialogueText.text = "Error loading question. Please try again.";
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
            selectedOptionIndex = 0; // Loop back to first option
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

    private void CheckAnswer()
    {
        if (selectedOptionIndex == -1)
        {
            dialogueText.text = "Please select an option first.";
            return;
        }

        string selectedAnswer = answerButtons[selectedOptionIndex].GetComponentInChildren<TMP_Text>().text;

        if (selectedAnswer.Equals(correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Correct Answer! Restoring 1 HP.");
            StartCoroutine(HideQuizAndRestoreHealth());
        }
        else
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Town")
            {
                Debug.Log("Wrong Answer in Town! Gaining coins instead of respawning.");
                
            }
            else
            {
                Debug.Log("Wrong Answer! Respawning in town.");
                StartCoroutine(HideQuizAndRespawn());
            }
        }
    }
    IEnumerator HideQuizAndRestoreHealth()
    {
        StartCoroutine(FadeCanvas(dialogueCanvasGroup, 1, 0, 0.3f));
        yield return new WaitForSeconds(0.3f);
        dialogueBox.SetActive(false);
        FindObjectOfType<PlayerHealth>().RestoreHealth(1);
        ClearOldQuestion();
        StartCoroutine(FetchQuestionFromAPI());
    }

    IEnumerator HideQuizAndRespawn()
    {
        StartCoroutine(FadeCanvas(dialogueCanvasGroup, 1, 0, 0.3f));
        yield return new WaitForSeconds(0.3f);
        dialogueBox.SetActive(false);
        FindObjectOfType<PlayerHealth>().TriggerDeathRoutine();
    }
    private void ClearOldQuestion()
    {
        dialogueText.text = "";
        correctAnswer = "";
        selectedOptionIndex = -1;
        questionFetched = false;

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
            ResetButtonColor(btn);
        }
    }
    void ResetButtonColor(Button button)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        button.colors = cb;
    }
}
