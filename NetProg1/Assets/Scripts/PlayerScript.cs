using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private GameObject winnerSign;
    [SerializeField] private GameObject loserSign;

    [SerializeField] private bool win = false;
    [SerializeField] private bool lose = false;
    [SerializeField] private float force = 10f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private bool playerHit = false;
    [SerializeField] private bool playerDead = false;
    [SerializeField] public int playerHealth = 3,playerMaxHealth = 3;
    [SerializeField] public GameObject emoteObject;

    private Vector3 mousePosition;

    private NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>();
    private NetworkVariable<bool> jumpInput = new NetworkVariable<bool>();
    private NetworkVariable<bool> emoteInput = new NetworkVariable<bool>();
    [SerializeField]private int jumpsCount = 3;
    private List<int> jumpsToAdd = new List<int>();
    private bool isDone1 = false;
    private bool isDone2 = false;
    private bool emoting = false;



    private GameObject player;
    private IEnumerator _enumerator;

    void Start()
    {
        Mathf.Clamp(jumpsCount, 0, 3);

        if (inputReader != null && IsLocalPlayer)
        {
            inputReader.MoveEvent += OnMove;
            inputReader.ShootEvent += SpawnInternal;
            inputReader.JumpEvent += OnJump;
            inputReader.EmoteEvent += EmoteInternal;
        }

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        lineRenderer = objectToSpawn.GetComponent<LineRenderer>();
    }

    public void TakeDamage()
    {
        if (playerHealth >= 0)
        {
            playerHealth--;
        }
        else
        {
                if(!NetworkObject.IsSpawned)
                    return;
        
                NetworkObject.Despawn();
        }
            
    }

    private void EmoteInternal()
    {
        EmoteRPC();
    }
    private void SpawnInternal()
    {
        SpawnRPC();
    }

    private void OnMove(Vector2 input)
    {
        HelloRPC(input);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("KillBrick"))
        {
            if(!NetworkObject.IsSpawned)
                return;
            lose = true;
            
        }
    }

    private void OnLose()
    {
        NetworkObject loserObject = Instantiate(loserSign).GetComponent<NetworkObject>();
        loserObject.Spawn();
        NetworkObject.Despawn();
    }
    private void OnWin()
    {
        NetworkObject winnerObject = Instantiate(winnerSign).GetComponent<NetworkObject>();
        winnerObject.Spawn();
    }
    private void Update()
    {
        if (IsLocalPlayer)
        {
            if (emoting)
            {
                
                emoting = false;
            }
        }
        if (IsLocalPlayer)
        {
            if (lose)
            {
                if (isDone1 == false)
                { 
                    isDone1 = true;
                    OnLose();
                }
            }
        } else if (!IsLocalPlayer)
        {
            if (lose)
            {
                if (!isDone2)
                {
                    isDone2 = true;
                    OnWin();
                }
            }
        }

        
        if (IsServer)
        {
            for (int i = 0; i < 10; i++)
            {
                transform.position += (Vector3)moveInput.Value * Time.deltaTime;
            }
            
            if (jumpInput.Value)
            {
                gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * force,ForceMode2D.Impulse);
                jumpInput.Value = false;
            }
        }

        if (IsLocalPlayer)
        {
            mousePosition = FindTarget();
            UpdateMouseRPC(mousePosition);
            
            
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateMouseRPC(Vector3 mpos)
    {
        mousePosition = mpos;
    }

    private Vector3 FindTarget()
    {
        var mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        
        if (!mainCamera)
            return Vector3.zero;
        
        var screenPosition = Input.mousePosition;
        var contains = mainCamera.rect.Contains(mainCamera.ScreenToViewportPoint(screenPosition));
        return contains ? mainCamera.ScreenToWorldPoint(screenPosition) : Vector3.zero;
    }
    
    private void OnJump()
    {
        if (jumpsCount != 0)
        {
            JumpRPC(true);
            jumpsCount--;
            jumpsToAdd.Add(0);
            StartCoroutine(jumpRefillTimer(1));
        }
    }
    
    [Rpc(SendTo.Server)]
    private void EmoteRPC()
    {
        NetworkObject emoteSpawn = Instantiate(emoteObject, transform.position+(Vector3.up*2), transform.rotation).GetComponent<NetworkObject>();
        emoteSpawn.Spawn();
        emoteSpawn.GetComponent<KillScript>().Destroy();
    }


    [Rpc(SendTo.Server)]
    private void SpawnRPC()
    {
        NetworkObject ob = Instantiate(objectToSpawn, transform.position, transform.rotation).GetComponent<NetworkObject>();
        Projectile projectileScript = ob.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.playerVariable = gameObject;
            projectileScript.SetFacing();
            projectileScript.targetPosition = mousePosition;
            
            ob.Spawn();
        }
    }

    [Rpc(SendTo.Server)]
    private void HelloRPC(Vector2 data)
    {
        moveInput.Value = data;
    }
    [Rpc(SendTo.Server)]
    private void JumpRPC(bool data)
    {
        jumpInput.Value = data;
    }

    IEnumerator jumpRefillTimer(int timeToWait)
    {
        yield return new WaitForSeconds(3);
        foreach (var i in jumpsToAdd)
        {
            yield return new WaitForSeconds(timeToWait);
            jumpsCount++;
            Debug.Log("Added 1 jump");
            jumpsToAdd.Remove(0);

        }
    }
    
}
