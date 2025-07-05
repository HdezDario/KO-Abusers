using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class MultiplayerCoreAI : NetworkBehaviour
{
    public NavMeshAgent agent;

    [ClientRpc]
    public void SetDestinationClientRpc(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    
}
