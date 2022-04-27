using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehavior : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float patienceMin;
    [SerializeField] private float patienceMax;
    [SerializeField] private GameObject gameController;
    [SerializeField] private float attackRange;
    [SerializeField] private float maxWalkBack;
    [SerializeField] private float walkSpeedStraight;
    [SerializeField] private float walkSpeedStrafe;

    public int enemyhealth;

    private bool attackTaken = false;
    private float patience;
    private bool animationEnd = true;
    private bool steppingBack = false;
    private Vector3 lastPosition;

    public bool dead = false;
    public bool win = false;
    public float distanceTraveled = 0;

    private bool trueFalse = false;
    private void trueFalseSwap()
    {
        if (trueFalse == true)
        {
            trueFalse = false;
        }
        else
        {
            trueFalse = true;
        }
    }
    private Rigidbody m_Rigidbody;
    private Animator m_Animation;
    // private Animation m_playerAnimation;
    private AudioController m_AudioController;
    private HitReg m_HitReg;


    // Start is called before the first frame update
    void Start()
    {
        enemyhealth = 3;
        patience = Random.Range(patienceMin, patienceMax);

        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animation = GetComponent<Animator>();
        //m_playerAnimation = player.GetComponent<Animation>();
        m_AudioController = gameController.GetComponent<AudioController>();
        m_HitReg = gameController.GetComponent<HitReg>();
    }

    void Update()
    {
        // Debug.Log("Distance Traveled: " + distanceTraveled);
        // Debug.Log("Patience: " + patience);
        distanceTraveled += Vector3.Distance(lastPosition, transform.position);

        if (dead == true)
        {
            startDeathState();
        }
        else if (win == true)
        {
            startWinState();
        }
        if (attackTaken == true)
        {
            attackTaken = false;
            patience = Random.Range(patienceMin, patienceMax);
        }
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Debug.Log(Vector3.Distance(player.transform.position, transform.position));
        if (animationEnd == false) {/*do nothing*/}
        else
        {
            if (steppingBack == true)
            {
                startIdleState();
                endIdleState();
            }
            else if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                patience -= 1;
                if (patience <= 0)
                {
                    startAttackState();
                    return;
                }
                else
                {
                    distanceTraveled = 0;
                    startIdleState();
                    endIdleState();
                }
            }
            else
            {
                Move(Vector3.MoveTowards(transform.position, player.transform.position, walkSpeedStrafe * Time.deltaTime));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("ai body collision");
        m_HitReg.enemyBodyCollide = true;
    }

    void OnCollisionExit(Collision collision)
    {
        m_HitReg.enemyBodyCollide = false;
    }

    private void startIdleState()
    {
        // Debug.Log("Accessing State: Idle");
        //m_Animation.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
        Move(Vector3.MoveTowards(transform.position, player.transform.position, -1 * walkSpeedStrafe * Time.deltaTime));
        // Move(Vector3.MoveTowards(transform.position, player.transform.position, walkSpeedStrafe * Time.deltaTime));
        steppingBack = true;
        if (distanceTraveled >= maxWalkBack)
        {
            steppingBack = false;
            distanceTraveled = 0;
        }
    }

    private void endIdleState()
    {
        // Debug.Log("Ending State: Idle");
        //set neutral
    }

    private void startAttackState()
    {
        Debug.Log("Accessing State: Attack");
        animationEnd = false;

        m_Animation.SetTrigger("Attack");
        m_AudioController.state = "enemy swing";
        endAttackState();
    }

    private void endAttackState()
    {
        Debug.Log("Ending State: Attack");
        animationEnd = true;
        attackTaken = true;
        //set neutral
    }

    private void Move(Vector3 moveVector)
    {
        if (moveVector != Vector3.zero)
        {
            UpdateSpeed(moveVector);
            UpdateStrafe(moveVector);
        }
        m_Rigidbody.MovePosition(moveVector);
    }

    public void UpdateSpeed(Vector3 movementDirection)
    {
        if (movementDirection.z == 0)
        {
            m_Animation.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
        }
        else if (movementDirection.z > 0)
        {
            m_Animation.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
        }
        else if (movementDirection.z < 0)
        {
            m_Animation.SetFloat("Speed", -1, 0.1f, Time.deltaTime);
        }
    }

    public void UpdateStrafe(Vector3 movementDirection)
    {
        if (movementDirection.x == 0)
        {
            m_Animation.SetFloat("LateralDirection", 0, 0.1f, Time.deltaTime);
        }
        else if (movementDirection.x > 0)
        {
            m_Animation.SetFloat("LateralDirection", 1, 0.1f, Time.deltaTime);
        }
        else if (movementDirection.x < 0)
        {
            m_Animation.SetFloat("LateralDirection", -1, 0.1f, Time.deltaTime);
        }
    }

    private void startDeathState()
    {
        Debug.Log("Accessing State: Death");
        dead = false;
        //die
    }

    private void startWinState()
    {

        Debug.Log("Accessing State: Win");
        win = false;
        //win
    }
}
