using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    //private void Start() {
    //    MonoBehaviour currenActiveWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
    //    damageAmount = (currenActiveWeapon as IWeapon).GetWeaponInfo().weaponDamage;
    //}

    private void OnTriggerEnter2D(Collider2D other) {
        //EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        //enemyHealth?.TakeDamage(damageAmount);
        if (other.gameObject.GetComponent<EnemyAI>())
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount);
        }
    }
}
