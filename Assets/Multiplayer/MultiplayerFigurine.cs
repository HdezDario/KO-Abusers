using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.Netcode;

public class MultiplayerFigurine : NetworkBehaviour
{
    public MeshRenderer render;
    private Texture sprite;
    public NavMeshAgent agent;
    public TextMeshPro playerName;

    [Header("Stats")]
    public MeshRenderer healthMat;
    public MeshRenderer powerMat;
    public MeshRenderer speedMat;
    public MeshRenderer actionMat;

    public TextMeshPro health;
    public TextMeshPro power;
    public TextMeshPro speed;
    public TextMeshPro action;

    //Colors
    private Color32 aliveColor;
    private Color32 evadeColor;
    private Color32 stunColor;
    private Color32 deadColor;

    private void Start()
    {
        healthMat.material.mainTexture = Resources.Load<Texture>("Icons/Stagger_icon");
        powerMat.material.mainTexture = Resources.Load<Texture>("Icons/Power_icon");
        speedMat.material.mainTexture = Resources.Load<Texture>("Icons/Speed_icon");
        actionMat.material.mainTexture = Resources.Load<Texture>("Icons/Cooldown_icon");

        aliveColor = new Color32(255, 255, 255, 255);
        evadeColor = new Color32(255, 255, 255, 150);
        stunColor = new Color32(183, 173, 13, 255);
        deadColor = new Color32(75, 75, 75, 255);
    }

    [ClientRpc]
    public void SetPlayerClientRpc(string name)
    {
        playerName.text = name;
    }

    [ClientRpc]
    public void SetHealthClientRpc(int ammount)
    {
        health.text = ammount.ToString();
    }

    [ClientRpc]
    public void SetPowerClientRpc(int ammount)
    {
        power.text = ammount.ToString();
    }

    [ClientRpc]
    public void SetSpeedClientRpc(int ammount)
    {
        speed.text = ammount.ToString();
    }

    [ClientRpc]
    public void SetActionClientRpc(int ammount)
    {
        action.text = ammount.ToString();
    }

    [ClientRpc]
    public void SetStrikerSpriteClientRpc(Strikers striker)
    {
        sprite = Resources.Load<Texture>("CharacterIcons/" + striker + "_icon");
        render.material.mainTexture = sprite;

        SetRenderAliveClientRpc();
    }

    [ClientRpc]
    public void SetDestinationClientRpc(Vector3 position)
    {
        agent.SetDestination(position);
    }

    [ClientRpc]
    public void SetRenderAliveClientRpc()
    {
        render.material.color = aliveColor;
    }

    [ClientRpc]
    public void SetRenderEvadeClientRpc()
    {
        render.material.color = evadeColor;
    }

    [ClientRpc]
    public void SetRenderStunClientRpc()
    {
        render.material.color = stunColor;
    }

    [ClientRpc]
    public void SetRenderDeadClientRpc()
    {
        render.material.color = deadColor;
    }
}
