using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static InventoryItem;
using static LootManager;
using Random = UnityEngine.Random;

public class EnemyLogic : MonoBehaviour
{
    public float StaySuspiciousForSeconds = 10f;
    public float HitRecoveryTime;
    public string HitAnimation;
    public string IdleState = "Idle";
    public string DeathAnimation = "Die";
    public string WalkAnimation = "Walk";
    public string AttackState = "Attack_1H_Off_WepL";
    public float AttackDuration = 1f;
    public float AttackPrepareTime = 0f;
    public float AttackDistance = 0.4f;
    public GameObject Weapon;
    public LootManager Loot;
    public CharAttributes LastOneShotMe;
    public int BountyCode = 0;

    private DateTime GotSuspiciousAt;
    private DateTime LastSeenTargetAt;
    private FieldOfView Fov;
    private GameObject Player;
    private CharAttributes PlayerAttributes;
    private CharAttributes Attributes;
    private bool Dead;
    private Animator Anim;
    private PathFollower PF;
    private float OlfPFSpeed;
    private bool Attacking;
    private bool CanAttack;
    private Vector3 LastAttackedFrom = Vector3.zero;

    void Start()
    {
        Dead = false;
        Player = null;
        CanAttack = true;
        Attacking = false;

        Attributes = GetComponent<CharAttributes>();
        Attributes.HPRemaining = Attributes.HP;
        PF = GetComponent<PathFollower>();
        GotSuspiciousAt = DateTime.MinValue;
        Fov = GetComponent<FieldOfView>();
        Anim = GetComponent<Animator>();
        OlfPFSpeed = PF.Speed;
        LastOneShotMe = null;
    }

    public bool IsDead()
    {
        return Dead;
    }

    void FixedUpdate()
    {
        if(Dead)
        {
            return;
        }

        // inspection stuff
        // bool foundPlayer = false;

        if (PF.AutoPlay && Player != null)
        {
            float distance;

            if (LastAttackedFrom != Vector3.zero)
            {
                distance = Mathf.Abs(Vector3.Distance(LastAttackedFrom, transform.position));

                if(distance <= 0.1f)
                {
                    LastAttackedFrom = Vector3.zero;
                }
            }
            else
            {
                distance = Mathf.Abs(Vector3.Distance(Player.transform.position, transform.position));

                if (distance <= AttackDistance && PlayerAttributes.HPRemaining > 0)
                {
                    StartCoroutine(Attack());
                } else if (distance > 1.2f * Fov.viewRadius)
                {
                    CalmDown();
                } else
                {
                    PF.MoveTo(Player.transform.position);
                }

                return;
            }
        }

        Fov.FindVisibleTargets();
        foreach (Transform visibleTarget in Fov.visibleTargets)
        {
            if (visibleTarget.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player = visibleTarget.gameObject;
                PlayerAttributes = visibleTarget.gameObject.GetComponent<CharAttributes>();
                if (Player.GetComponent<CharAttributes>().HPRemaining > 0)
                {
                    //foundPlayer = true;

                    if (LastAttackedFrom != Vector3.zero || GotSuspiciousAt == DateTime.MinValue)
                    {
                        LastAttackedFrom = Vector3.zero;

                        Anim.CrossFade(WalkAnimation, 0.01f);
                        PF.SetAutoPlay();
                        PF.Pause();
                        GetSuspicious();

                        return;
                    }
                }
            }
        }

        /*
         * Uncomment below if you will make inspection again
         * ---------------------------------------------------
         * 
         * if(!Dead && !Attacking && !foundPlayer && GotSuspiciousAt != DateTime.MinValue)
        {
            if (LastSeenTargetAt == DateTime.MinValue)
            {
                Debug.Log("Lost him...");

                LastSeenTargetAt = DateTime.Now;
                PF.StopAutoPlay();
                StartCoroutine(InspectAround());
            }
            else
            {
                if ((DateTime.Now - LastSeenTargetAt).TotalSeconds >= StaySuspiciousForSeconds)
                {
                    Debug.Log(Player.name + ": ~");

                    // leave him
                    CalmDown();
                }
            }
        }*/
    }

    public void GetSuspicious()
    {
        LastSeenTargetAt = DateTime.MinValue;
        GotSuspiciousAt = DateTime.Now;
    }

    public void CalmDown()
    {
        if(Dead)
        {
            return;
        }

        LastSeenTargetAt = DateTime.MinValue;
        GotSuspiciousAt = DateTime.MinValue;

        PF.StopAutoPlay();
        PF.Resume();

        Anim.CrossFade(WalkAnimation, 0.01f);
    }

    public void Die()
    {
        if(Dead)
        {
            return;
        }

        //Debug.Log(name + " died");
        LastOneShotMe.Experience += Attributes.HP;

        Attributes.HPRemaining = 0;
        Dead = true;

        PF.StopAutoPlay();
        PF.Pause();
        PF.Speed = 0f;
        PF.enabled = false;
        
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        ScreamLouder();

        Anim.CrossFade(DeathAnimation, 0.01f);

        StartCoroutine(DropBounties());
        StartCoroutine(BurryToUnderground());
    }

    IEnumerator BurryToUnderground()
    {
        yield return new WaitForSeconds(10);

        transform.DOLocalMoveY(transform.localPosition.y - 1f, 3f);
        transform.Find("Point Light").GetComponent<Light>().DOIntensity(0, 3f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(3.1f);

        //Debug.Log(name + " has been destroyed");
        Destroy(gameObject);
    }

    IEnumerator DropBounties()
    {
        yield return new WaitForSeconds(1f);

        switch(BountyCode)
        {
            // Demon warrior 1 in cursed forest
            case 101:
                Loot.LootGoldPile(transform.position + Vector3.up * 0.1f, Attributes.Level * UnityEngine.Random.Range(12, 18));
                Loot.LootPotion(transform.position + Vector3.up * 0.1f + Vector3.left * 1f, 0f, PotionSize.Small, PotionType.Life);
                break;
            // Mushrooms in cursed forest
            case 102:
                if(Random.Range(1, 10) < 2)
                {
                    // 20% possibility
                    Loot.LootGoldPile(transform.position + Vector3.up * 0.1f, Attributes.Level * UnityEngine.Random.Range(5, 15));
                }

                if (Random.Range(1, 10) < 1)
                {
                    // 10% possibility
                    if (Random.Range(1, 2) < 1)
                    {
                        Loot.LootPotion(transform.position + Vector3.up * 0.1f + Vector3.left * 1f, 0f, PotionSize.Tiny, PotionType.Life);
                    } else
                    {
                        Loot.LootPotion(transform.position + Vector3.up * 0.1f + Vector3.left * 1f, 0f, PotionSize.Tiny, PotionType.Mana);
                    }
                }

                break;
        }
        //Loot.LootItem(transform.position + Vector3.up * 0.1f + Vector3.left * 2f, 0f, 801);
    }

    public void Damage(int damage, Vector3 shotFrom, CharAttributes shooter)
    {
        if(Dead)
        {
            return;
        }

        LastOneShotMe = shooter;
        if (Attributes.HPRemaining <= damage)
        {
            Die();
        } else
        {
            PF.StopAutoPlay();

            Attributes.HPRemaining -= damage;
            Hit(shotFrom);
        }
    }

    public void Hit(Vector3 shotFrom)
    {
        if(Dead)
        {
            return;
        }

        OlfPFSpeed = PF.Speed;
        PF.Speed = 0f;

        Scream();
        Anim.CrossFade(HitAnimation, 0.01f);

        if (GotSuspiciousAt == DateTime.MinValue)
        {
            GotSuspiciousAt = DateTime.Now;
            LastAttackedFrom = shotFrom;
        }

        StartCoroutine(BackToWalk(HitRecoveryTime));
        // Move to last position of attack
    }

    public void Scream()
    {
        GetComponents<AudioSource>()[0].Play();
    }

    public void ScreamLouder()
    {
        GetComponents<AudioSource>()[1].Play();
    }

    IEnumerator BackToWalk(float duration)
    {
        yield return new WaitForSeconds(duration);

        if(Dead)
        {
            yield break;
        }

        Anim.CrossFade(WalkAnimation, 0.01f);

        PF.SetAutoPlay();
        PF.Pause();
        PF.Speed = OlfPFSpeed;
        GetSuspicious();

        PF.MoveTo(LastAttackedFrom);
    }

    /*
    IEnumerator InspectAround()
    {
        if (!Attacking && !Dead)
        {
            Debug.Log("Inspecting around...");
            GetComponent<Animator>().SetBool("isStanding", true);
            GetComponent<Animator>().SetBool("isWalking", false);
            GetComponent<Animator>().SetBool("isAttacking", false);
            GetComponent<Animator>().CrossFade(IdleState, 0.01f);

            yield return new WaitForSeconds(StaySuspiciousForSeconds / 5f);

            if (Dead)
            {
                yield break;
            }

            // Take a look at left
            Debug.Log("Inspecting look left");
            transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 90, 0), 0.15f);

            yield return new WaitForSeconds(StaySuspiciousForSeconds / 5f);

            if (Dead)
            {
                yield break;
            }

            // Take a look at right
            Debug.Log("Inspecting look right");
            transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 180, 0), 0.3f);

            yield return new WaitForSeconds(StaySuspiciousForSeconds / 5f);

            if (Dead)
            {
                yield break;
            }

            // Take a look at behind
            Debug.Log("Inspecting look behind");
            transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, -90, 0), 0.3f);

            yield return new WaitForSeconds(StaySuspiciousForSeconds / 5f);

            if (Dead)
            {
                yield break;
            }

            // Take a look at forward
            Debug.Log("Inspecting look forward");
            transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 180, 0), 0.3f);

            yield return new WaitForSeconds(StaySuspiciousForSeconds / 5f);
            // go back
        }
    }
    */

    IEnumerator Attack()
    {
        nextAttack:

        if(Dead)
        {
            yield break;
        }

        if(Weapon != null)
        {
            Weapon.GetComponent<CapsuleCollider>().enabled = false;
        }

        Attacking = true;
        PF.Pause();
        PF.StopAutoPlay();
        transform.LookAt(Player.transform.position);
        Anim.CrossFade(AttackState, 0.01f);
        Anim.Play(AttackState, -1, 0);

        yield return new WaitForSeconds(AttackPrepareTime);

        if (Dead)
        {
            yield break;
        }

        CanAttack = true;
        if (Weapon != null)
        {
            Weapon.GetComponent<CapsuleCollider>().enabled = true;
        }

        yield return new WaitForSeconds(AttackDuration - AttackPrepareTime);

        if(Dead)
        {
            yield break;
        }

        if (PlayerAttributes.HPRemaining <= 0)
        {
            // player seems to be dead
            CalmDown();

            Attacking = false;
            PF.Resume();
            PF.StopAutoPlay();

            Anim.CrossFade(PF.WalkState, 0.01f);
            yield break;
        }

        // still in attack distance? and not dead?
        if (Mathf.Abs(Vector3.Distance(Player.transform.position, transform.position)) <= AttackDistance)
        {
            // yes: attack once more
            goto nextAttack;
        }
        else
        {
            if (!Dead)
            {
                // no: follow him
                Anim.CrossFade(PF.WalkState, 0.01f);

                PF.SetAutoPlay();
                PF.Pause();
                Attacking = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!Dead && Attacking && CanAttack)
            {
                CanAttack = false;
                int damage = UnityEngine.Random.Range(GetComponent<CharAttributes>().AttackMin, GetComponent<CharAttributes>().AttackMax + 1);
                other.gameObject.GetComponent<PlayerMovement>().Hit(damage);
            }
        }
    }
}
