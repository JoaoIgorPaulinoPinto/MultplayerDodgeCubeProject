using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WallMovimentController : NetworkBehaviour
{
    public float wallspeed;
    public bool IsMoving;
    public Vector3 center;
    private Vector3 dir;

    private Rigidbody body;

    // Conjunto para armazenar objetos já detectados
    private HashSet<Transform> detectedObjects = new HashSet<Transform>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        body = GetComponent<Rigidbody>();

    }


    [ServerRpc(RequireOwnership = false)]
    public void MoveWallServerRpc(Vector3 centerPosition, float _wallspeed)
    {
        wallspeed = _wallspeed;
        center = centerPosition;
        IsMoving = true;

        // Calcular direção limitada a um eixo (X ou Z)
        Vector3 difference = center - transform.position;
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.z))
        {
            // Mover no eixo X
            dir = new Vector3(difference.x, 0, 0).normalized;
        }
        else
        {
            // Mover no eixo Z
            dir = new Vector3(0, 0, difference.z).normalized;
        }

        // Atualiza os clientes
        UpdateWallDirectionClientRpc(dir);
        Destroy(gameObject, 30);
    }

    [ClientRpc]
    private void UpdateWallDirectionClientRpc(Vector3 direction)
    {
        dir = direction;
    }

    private void Update()
    {
        if (IsMoving)
        {
            // Aplicar velocidade ao Rigidbody na direção calculada
            body.linearVelocity = dir * wallspeed;
        }

        if (IsServer)
        {
            RayDetection();
        }
    }

    private bool VerifyPlayerDetected(Transform target)
    {
        if (target.gameObject.CompareTag("Player"))
        {
            // Verificar se o objeto já foi detectado
            if (!detectedObjects.Contains(target))
            {
                detectedObjects.Add(target); // Adicionar ao conjunto
                target.TryGetComponent(out PlayerNetwork player);
                if (player != null) player.AddPointToPoints();
                return true;
            }
        }
        return false;
    }

    private void RayDetection()
    {
        RaycastHit hit;

        // Verificar direção principal do movimento (X ou Z)
        if (Mathf.Abs(transform.eulerAngles.y) < 90 || Mathf.Abs(transform.eulerAngles.y - 360) < 90)
        {
            // Raios para a direita e esquerda (eixo X)
            Vector3 rayOriginRight = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
            Vector3 rayOriginLeft = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);

            if (IsMoving)
            {
                if (Physics.Raycast(rayOriginRight, Vector3.right, out hit, 10, 1 << LayerMask.NameToLayer("Player")))
                {
                    if (VerifyPlayerDetected(hit.transform))
                    {
                        Debug.Log("COLIDIIU");
                        return;
                    }
                }

                if (Physics.Raycast(rayOriginLeft, Vector3.left, out hit, 10, 1 << LayerMask.NameToLayer("Player")))
                {
                    if (VerifyPlayerDetected(hit.transform))
                    {
                        Debug.Log("COLIDIIU");
                        return;
                    }
                }
            }
        }
        else
        {
            // Raios para frente e para trás (eixo Z)
            Vector3 rayOriginForward = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            Vector3 rayOriginBack = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);

            if (IsMoving)
            {
                if (Physics.Raycast(rayOriginForward, Vector3.forward, out hit, 10, 1 << LayerMask.NameToLayer("Player")))
                {
                    if (VerifyPlayerDetected(hit.transform))
                    {
                        Debug.Log("COLIDIIU");
                        return;
                    }
                }

                if (Physics.Raycast(rayOriginBack, Vector3.back, out hit, 10, 1 << LayerMask.NameToLayer("Player")))
                {
                    if (VerifyPlayerDetected(hit.transform))
                    {
                        Debug.Log("COLIDIIU");
                        return;
                    }
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (Mathf.Abs(transform.eulerAngles.y) < 90 || Mathf.Abs(transform.eulerAngles.y - 360) < 90)
        {
            Vector3 rayOriginRight = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 rayOriginLeft = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(rayOriginRight, Vector3.right * 10);
            Gizmos.DrawRay(rayOriginLeft, Vector3.left * 10);
        }
        else
        {
            Vector3 rayOriginForward = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 rayOriginBack = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rayOriginForward, Vector3.forward * 10);
            Gizmos.DrawRay(rayOriginBack, Vector3.back * 10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerNetwork>().ReduceLifes();
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
