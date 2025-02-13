using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;
    [SerializeField] private float respawnTime = 2f;
    private int currentHealth;
    private Knockback knockback;
    private Flash flash;
    private Vector3 spawnPosition;
    private void Awake() {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        spawnPosition = transform.position;
    }

    private void Start() {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        knockback.GetKnockedBack(PlayerController.Instance.transform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine());
        //StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine() {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath() {
        if (currentHealth <= 0) {
            GameObject vfxInstance = Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            //GetComponent<PickUpSpawner>().DropItems();
            Destroy(vfxInstance, 2f);
            gameObject.SetActive(false);
            Invoke(nameof(Respawn), respawnTime);
        }
    }
    private void Respawn()
    {
        transform.position = spawnPosition; 
        currentHealth = startingHealth;
        gameObject.SetActive(true);
    }
}
