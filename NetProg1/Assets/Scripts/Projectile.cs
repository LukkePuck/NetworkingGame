using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;


public class Projectile : NetworkBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 mouse;
    [SerializeField] private float speed = 2f;
    public Vector3 screenPosition;
    public Vector3 targetPosition;
    private NetworkVariable<Vector2> projectilePosition = new NetworkVariable<Vector2>();

    public GameObject playerVariable;
    
    public void SetFacing()
    {
        if (playerVariable != null)
        {
            gameObject.transform.LookAt(playerVariable.transform);
        }
    }

    private void Start()
    { 
          if(IsServer)
            StartCoroutine(TimerUntilDestruction(3));
    }
    
    IEnumerator TimerUntilDestruction(int timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Despawn();
        Debug.Log("Destroyed: "+ gameObject);
    }

    private void Despawn()
    {
        if(!NetworkObject.IsSpawned)
            return;
        
        NetworkObject.Despawn();
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject == IsLocalPlayer)
            return;
    }
   private void Update()
   {
       if(!playerVariable)
           return;
       
        Vector2 direction = (targetPosition - transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + direction * (speed * Time.deltaTime);

        transform.position = newPosition;
        if (projectilePosition != null)
        {
            ProjectileRPC(newPosition);
        }
    }
    [Rpc(SendTo.Server)]
    private void ProjectileRPC(Vector2 position)
    {
        projectilePosition.Value = position;
    }
}
