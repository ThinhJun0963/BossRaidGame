using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private float swordAttackCD = .3f;
    [SerializeField] private WeaponInfo weaponInfo;

    [SerializeField] private Transform weaponCollider;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;
    private PlayerControls playerControls;
    private GameObject slashAnim;
    private bool attackButtonDown, isAttacking = false;
    private bool attackBuffered = false;
    private void Awake() {
        playerControls = new PlayerControls();
        playerController = GetComponent<PlayerController>();
        activeWeapon = GetComponent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start() {
        playerControls.Combat.Attack.started += _ => BufferAttack();
        playerControls.Combat.Attack.started -= _ => StopAttack();
       // weaponCollider = PlayerController.Instance.GetWeaponCollider();
      //slashAnimSpawnPoint = GameObject.Find("SlashSpawnPoint").transform;
    }
    private void OnDisable()
    {
        playerControls.Disable();
        
    }
    private void Update() {
        MouseFollowWithOffset();
    }

    private void StartAttack()
    {
        attackButtonDown = true;
        PerformAttack();
    }
    private void StopAttack()
    {
        attackButtonDown = false;
    }
    public WeaponInfo GetWeaponInfo() {
        return weaponInfo;
    }

    public void PerformAttack()
    {
        if (isAttacking) return; 

        isAttacking = true;
        attackBuffered = false; 

        myAnimator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);

        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;

        StartCoroutine(AttackCDRoutine());
    }
    private void BufferAttack()
    {
        attackBuffered = true; 
        if (!isAttacking)
        {
            PerformAttack();
        }
    }
    private void CancelBuffer()
    {
        attackBuffered = false;
    }
    private IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(swordAttackCD);
        isAttacking = false;

        if (attackBuffered)
        {
            PerformAttack();
        }
    }
    public void DoneAttackingAnimEvent() {
        weaponCollider.gameObject.SetActive(false);
    }


    public void SwingUpFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft) { 
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void MouseFollowWithOffset() {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x) {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        } else {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }
}
