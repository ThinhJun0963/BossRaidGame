using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool IsDead { get; private set; }

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockbackThrust = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private Slider healthSlider;
    private int currentHealth;
    private bool canTakeDamage = true;

    private Knockback knockback;
    private Flash flash;

    const string HEALTH_SLIDER_TEXT = "Health Slider";
    const string TOWN_TEXT = "Town";
    readonly int DEATH_HASH = Animator.StringToHash("Death");
    public DeathQuizManager quizManager;
    protected override void Awake()
    {
        base.Awake();

        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        IsDead = false;
        currentHealth = maxHealth;
        if (quizManager == null)
        {
            quizManager = FindObjectOfType<DeathQuizManager>();
            if (quizManager == null)
            {
                Debug.LogError("DeathQuizManager not found in the scene!");
            }
        }
        UpdateHealthSlider();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy)
        {
            TakeDamage(1, other.transform);
        }
    }

    public void HealPlayer()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += 1;
            UpdateHealthSlider();
        }
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) { return; }

        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockbackThrust);
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        currentHealth -= damageAmount;
        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfPlayerDeath();
    }

    private void CheckIfPlayerDeath()
    {
        if (currentHealth <= 0 && !IsDead)
        {
            IsDead = true;
            currentHealth = 0;
            StartCoroutine(TriggerDeathQuiz());
        }
    }
    private IEnumerator TriggerDeathQuiz()
    {
        yield return new WaitForSeconds(2f);
        if (quizManager == null)
        {
            quizManager = FindObjectOfType<DeathQuizManager>();
        }
        if (quizManager != null)
        {
            quizManager.ShowDialogue();
        }
        else
        {
            Debug.LogError("DeathQuizManager not found in the scene!");
        }

    }
    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Restore HP but not exceed max
        UpdateHealthSlider();
        IsDead = false; // Allow the player to continue playing
    }

    public void TriggerDeathRoutine()
    {
        GetComponent<Animator>().SetTrigger(DEATH_HASH);
        if (ActiveWeapon.Instance != null)
        {
            Destroy(ActiveWeapon.Instance.gameObject);
        }

        Debug.Log("Weapon destroyed after quiz.");
        StartCoroutine(DeathLoadSceneRoutine());
    }

    private IEnumerator DeathLoadSceneRoutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        Stamina.Instance.ReplenishStaminaOnDeath();
        SceneManager.LoadScene(TOWN_TEXT);
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find(HEALTH_SLIDER_TEXT).GetComponent<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}
