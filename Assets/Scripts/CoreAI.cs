using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoreAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    
}
