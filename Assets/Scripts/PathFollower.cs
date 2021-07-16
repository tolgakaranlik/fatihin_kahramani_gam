using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    public GameObject Path = null;
    public int StartFrom = 0;
    public float Speed = 0.1f;
    public float Closeness = 0.1f;
    public string WalkState = "Walk";
    public string WalkStateParam = "isWalking";
    public string SpeedParam = "animSpeed";
    public float SpeedCoefficient = 1.0f;
    public PlayerSkinSelector Selector;

    [HideInInspector]
    public bool AutoPlay;

    GameObject Player;
    CharAttributes PlayerAttributes;
    NavMeshAgent NMA;
    Animator Anim;
    Transform[] PathList;
    bool Paused;

    // Start is called before the first frame update
    void Start()
    {
        Player = Selector.GetPlayerObject();

        PlayerAttributes = Player.GetComponent<CharAttributes>();
        Anim = GetComponent<Animator>();
        NMA = GetComponent<NavMeshAgent>();
        NMA.enabled = false;

        AutoPlay = false;
        Paused = false;

        if (Path != null)
        {
            PathList = Path.GetComponentsInChildren<Transform>();
            if (PathList.Length > 0)
            {
                if (SpeedParam != "")
                {
                    Anim.SetFloat(SpeedParam, SpeedCoefficient);
                }

                transform.position = PathList[0].position;
                if (WalkStateParam != "")
                {
                    Anim.SetBool(WalkStateParam, true);
                }

                Anim.CrossFade(WalkState, 0.01f);
            }
        }
    }

    public void SetAutoPlay()
    {
        NMA.enabled = true;
        AutoPlay = true;
    }

    public void StopAutoPlay()
    {
        NMA.enabled = false;
        AutoPlay = false;
        //Paused = true; ??

        // This is not enough, NPC may have to find its way pack to the last path item, or to the closest path item
    }

    public void MoveTo(Vector3 p)
    {
        NMA.destination = p;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Path == null || Paused)
        {
            return;
        }

        try
        {
            int next = StartFrom + 1;
            if(next >= PathList.Length)
            {
                next = 0;
            }

            transform.position = Vector3.MoveTowards(transform.position, PathList[next].position, Time.deltaTime * Speed);
            transform.DOLookAt(new Vector3(PathList[next].transform.position.x, transform.position.y, PathList[next].transform.position.z), 0.35f);

            float dist = Vector3.Distance(transform.position, PathList[next].position);
            if(Mathf.Abs(dist) <= Closeness)
            {
                // destination has been reached
                StartFrom = (StartFrom + 1) % PathList.Length;
            }
        } catch(Exception ex)
        {
            Debug.LogError("Failure on move to path: " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    public bool IsPaused()
    {
        return Paused;
    }

    public void Pause()
    {
        Paused = true;
    }

    public void Resume()
    {
        Paused = false;
    }


}
