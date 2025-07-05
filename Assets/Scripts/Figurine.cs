using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Figurine : NetworkBehaviour
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
        healthMat.material.mainTexture  = Resources.Load<Texture>("Icons/Stagger_icon");
        powerMat.material.mainTexture   = Resources.Load<Texture>("Icons/Power_icon");
        speedMat.material.mainTexture   = Resources.Load<Texture>("Icons/Speed_icon");
        actionMat.material.mainTexture  = Resources.Load<Texture>("Icons/Cooldown_icon");

        aliveColor = new Color32(255, 255, 255, 255);
        evadeColor = new Color32(255, 255, 255, 150);
        stunColor = new Color32(183, 173, 13, 255);
        deadColor = new Color32(75, 75, 75, 255);

        SetRenderAlive();
    }

    public void SetPlayer(string name)
    {
        playerName.text = name;
    }

    public void SetHealth(int ammount)
    {
        health.text = ammount.ToString();
    }

    public void SetPower(int ammount)
    {
        power.text = ammount.ToString();
    }

    public void SetSpeed(int ammount)
    {
        speed.text = ammount.ToString();
    }

    public void SetAction(int ammount)
    {
        action.text = ammount.ToString();
    }

    public void SetStrikerSprite(Strikers striker)
    {
        sprite = Resources.Load<Texture>("CharacterIcons/" + striker + "_icon");
        render.material.mainTexture = sprite;
    }

    [ClientRpc]
    public void SetStrikerSpriteClientRpc(Strikers striker)
    {
        SetStrikerSprite(striker);
    }

    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
    }

    public void SetRenderAlive()
    {
        render.material.color = aliveColor;
    }

    public void SetRenderEvade()
    {
        render.material.color = evadeColor;
    }

    public void SetRenderStun()
    {
        render.material.color = stunColor;
    }

    public void SetRenderDead()
    {
        render.material.color = deadColor;
    }
}
