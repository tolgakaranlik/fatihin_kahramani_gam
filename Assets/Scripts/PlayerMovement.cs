using cakeslice;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static LootManager;

public class PlayerMovement : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject Bow;
    public LootManager Loot;
    public GUIController GUI;
    public DialogueManager Dialogue;
    public string HitAnimation = "Archer_Hit";
    public string DieAnimation = "Archer_Death";
    public string LoadAnimation = "Archer_Load";
    public string AttackAnimation = "Archer_Attack_Bow";
    public string RunAnimation = "Archer_Run";
    public string IdleAnimation = "Archer_Idle";
    public float PreAttackDuration = 0.6f;
    public float PostAttackDuration = 0.2f;

    TerrainCollider terrainCollider;
    NavMeshAgent agent;
    Animator anim;
    AudioSource AudioHit;
    AudioSource AudioDeath;
    cakeslice.Outline lastOutline;
    GameObject lastCanvas;
    CharAttributes attr;
    bool isShooting;
    bool isWalking;
    bool canMove;
    int lastXP;
    GameObject CubePhysics;

    void Start()
    {
        CubePhysics = transform.Find("/Cube_physics").gameObject;
        terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        attr = GetComponent<CharAttributes>();
        AudioHit = GetComponents<AudioSource>()[2];
        AudioDeath = GetComponents<AudioSource>()[1];

        if (transform.parent.GetComponent<PlayerSkinSelector>().GetPlayerSkin() == name)
        {
            MainCamera.GetComponent<CameraFollow>().ObjectToFollow = transform;
        }

        lastOutline = null;
        lastCanvas = null;
        isWalking = false;
        isShooting = false;
        canMove = false;
        lastXP = attr.Experience;

        GUI.SetLifeTo(1);
        GUI.SetManaTo(1);
        GUI.SetXP(0);
    }

    void Update()
    {
        if (attr.HPRemaining <= 0)
        {
            return;
        }

        if (lastXP != attr.Experience)
        {
            GUI.SetXP((attr.Experience - attr.XPLevelBase()) / (float)(attr.Level * attr.XPPerLevel));
            lastXP = attr.Experience;

            bool leveledUp = false;
            while (attr.XPNeededToLevelUp() <= 0)
            {
                leveledUp = true;
                Debug.Log("Level up (" + lastXP + ", " + (float)(attr.Level * attr.XPPerLevel) + ")");

                attr.Level += 1;
                GUI.SetXP((attr.Experience - attr.XPLevelBase()) / (float)(attr.Level * attr.XPPerLevel));
            }

            if(leveledUp)
            {
                GUI.ShowLevelUpDisplay();
            }
        }

        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        DisableCubePhysics();

        // Handle hover effect for monsters
        if (Physics.Raycast(ray, out hitData, 1000))
        {
            cakeslice.Outline component;
            //Debug.Log(hitData.collider.gameObject.name);

            // Enemies
            if (LayerMask.NameToLayer("Enemy") == hitData.collider.gameObject.layer)
            {
                try
                {
                    component = hitData.collider.gameObject.transform.Find("Body").GetComponent<cakeslice.Outline>();
                } catch
                {
                    component = null;
                }

                if (component != null)
                {
                    component.eraseRenderer = false;
                    lastOutline = component;
                    lastCanvas = hitData.collider.gameObject.transform.Find("Canvas").gameObject;

                    lastCanvas.SetActive(true);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    // shoot him!
                    if (!isShooting)
                    {
                        StartCoroutine(ShootToTarget(hitData.collider.gameObject));
                    }

                    return;
                }
            }
            else if (LayerMask.NameToLayer("ChestsNLevers") == hitData.collider.gameObject.layer)
            {
                // Open sesame!
                if (Input.GetMouseButtonDown(0))
                {
                    hitData.collider.gameObject.transform.parent.GetComponent<ChestController>().OpenChest();
                    var item = hitData.collider.gameObject.transform.parent.GetComponent<InventoryItem>();
                    item.SellValue = UnityEngine.Random.Range(18, 34);
                    item.Code = 100;

                    // Are you sure if it is gold?
                    Loot.LootGoldPile(hitData.collider.gameObject.transform.parent.Find("GoldSpawner").position, 1.15f, item.SellValue);
                    Loot.LootPotion(hitData.collider.gameObject.transform.parent.Find("GoldSpawner").position + Vector3.up * 0.1f + Vector3.left * 1f, 1.25f, PotionSize.Small, PotionType.Mana);

                    return;
                }
                else
                {
                    component = hitData.collider.gameObject.GetComponent<cakeslice.Outline>();
                    if (component != null)
                    {
                        component.eraseRenderer = false;
                        lastOutline = component;
                    }
                }
            }
            else if (LayerMask.NameToLayer("NPCs") == hitData.collider.gameObject.layer)
            {
                // Open sesame!
                if (Input.GetMouseButtonDown(0))
                {
                    var dialogStarter = hitData.collider.gameObject.GetComponent<NPCDialogue>();
                    if (dialogStarter != null)
                    {
                        Dialogue.StartDialogue(dialogStarter.DialogStarter);
                    }

                    return;
                }
                else
                {
                    component = hitData.collider.gameObject.GetComponentInChildren<cakeslice.Outline>();
                    if (component != null)
                    {
                        component.eraseRenderer = false;
                        lastOutline = component;
                    }
                }
            }
            else if (lastOutline != null)
            {
                lastOutline.eraseRenderer = true;
                lastOutline = null;

                if (lastCanvas != null)
                {
                    lastCanvas.SetActive(false);
                    lastCanvas = null;
                }
            }
        } else if (lastOutline != null)
        {
            lastOutline.eraseRenderer = true;
            lastOutline = null;
        }

        EnableCubePhysics();

        // Handle movement
        if (isWalking)
        {
            float dist = agent.remainingDistance;

            if (dist != Mathf.Infinity && dist <= agent.stoppingDistance)
            {
                StopAgent();
            }
            else
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(agent.destination, path);
                if (path.status == NavMeshPathStatus.PathPartial)
                {
                    // seems stuck
                    StopAgent();
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            canMove = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            canMove = false;
        }

        if (canMove && !GUI.IsQuestsOpen() && !GUI.IsDialogueOpen() && Input.GetMouseButton(0))
        {
            if (GUI.GUIItemClicked)
            {
                GUI.GUIItemClicked = false;
                canMove = false;
            }
            else
            {
                MoveToMousePosition();
            }
        }

        if (GUI.GUIItemClicked)
        {
            GUI.GUIItemClicked = false;
            StopAgent();
        }
    }

    private void DisableCubePhysics()
    {
        CubePhysics.SetActive(false);
    }

    private void EnableCubePhysics()
    {
        CubePhysics.SetActive(true);
    }

    private void MoveToMousePosition()
    {
        bool b1;
        bool b2 = false;

        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        b1 = Physics.Raycast(ray, out hitData, LayerMask.NameToLayer("Walkable"), 1000);
        if (!b1)
        {
            b2 = terrainCollider.Raycast(ray, out hitData, 1000);
        }

        if (b1 || b2)
        {
            StartCoroutine(SetDestination(hitData.point));
        }
    }

    private IEnumerator ShootToTarget(GameObject target)
    {
        if(isShooting || target.GetComponent<EnemyLogic>() == null || target.GetComponent<EnemyLogic>().IsDead())
        {
            yield break;
        }

        isShooting = true;
        StopAgent();

        transform.DOLookAt(target.transform.position, 0.3f);

        if (attr.HPRemaining <= 0)
        {
            yield break;
        }

        anim.CrossFade(AttackAnimation, 0.01f);
        yield return new WaitForSeconds(PreAttackDuration);

        if (attr.HPRemaining <= 0)
        {
            yield break;
        }

        var projectile = Instantiate(Bow, transform.position + Vector3.up * 1.25f, transform.rotation);
        projectile.transform.LookAt(target.transform.position + Vector3.up * 1.25f);
        projectile.transform.Rotate(new Vector3(1, 0, 0), -90f);
        projectile.SetActive(true);

        float distance = Vector3.Distance(transform.position, target.transform.position);
        projectile.transform.DOMove(target.transform.position + Vector3.up * 1.25f, distance * 0.03f);
        Destroy(projectile.gameObject, distance * 0.03f);
        GetComponent<AudioSource>().Play();

        isShooting = false;

        yield return new WaitForSeconds(distance * 0.03f);

        if(attr.HPRemaining <= 0)
        {
            yield break;
        }

        CharAttributes targetAttributes = target.GetComponent<CharAttributes>();
        CharAttributes attributes = GetComponent<CharAttributes>();
        PlayerEquipment equipment = GetComponent<PlayerEquipment>();

        // Calculate enemy armor
        int enemyArmor = targetAttributes.Armor;

        // Calculate attack damage
        int attackDamage = 0;

        if (equipment.LeftHand != null)
        {
            attackDamage += UnityEngine.Random.Range(equipment.LeftHand.MinDamage, equipment.LeftHand.MaxDamage + 1);
        }

        if (equipment.RightHand != null)
        {
            attackDamage += UnityEngine.Random.Range(equipment.RightHand.MinDamage, equipment.RightHand.MaxDamage + 1);
        }

        int actualDamage = Math.Max(1, attackDamage - enemyArmor);

        target.GetComponent<EnemyLogic>().Damage(actualDamage, transform.position, attr);
        target.transform.Find("Canvas/HealthBar").GetComponent<Slider>().value = targetAttributes.HPRemaining;

        yield return new WaitForSeconds(PostAttackDuration);
    }

    private IEnumerator SetDestination(Vector3 point)
    {
        agent.destination = point;

        while(agent.pathPending)
        {
            yield return null;
        }

        if (agent.hasPath && agent.remainingDistance >= agent.stoppingDistance)
        {
            isWalking = true;
            transform.DOLookAt(point, 0.35f);
            anim.CrossFade(RunAnimation, 0.01f);
        } else
        {
            //Debug.Log("No path found");
        }
    }

    private void StopAgent()
    {
        isWalking = false;
        anim.CrossFade(IdleAnimation, 0.01f);
        agent.isStopped = true;
        agent.ResetPath();

        canMove = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("CubePhysics"))
        {
            StopAgent();
        }
    }

    public void Hit(int damage)
    {
        if (attr.HPRemaining <= damage)
        {
            Die();
        }
        else
        {
            AudioHit.Play();
            anim.CrossFade(HitAnimation, 0.01f);

            attr.HPRemaining -= damage;
            GUI.SetLifeTo(attr.HPRemaining / (float)attr.HP);
        }
    }

    public void Die()
    {
        AudioDeath.Play();

        anim.CrossFade(DieAnimation, 0.01f);
        attr.HPRemaining = 0;

        GUI.SetLifeTo(0);
    }
}
