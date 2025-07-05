using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCard))]
public class PlayerCardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        PlayerCard data = (PlayerCard)target;

        base.OnInspectorGUI();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Generate Stats"))
        {
            GenerateBaseStats(data);
            GenerateExtraStats(data);
            EditorUtility.SetDirty(data);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateBaseStats(PlayerCard data)
    {
        switch (data.rank)
        {
            case Rank.Rookie:
                data.health = 10;
                break;
            case Rank.Bronze:
                data.health = 12;
                data.power = 0;
                data.speed = 0;
                data.actionMetter = 5;
                break;
            case Rank.Silver:
                data.health = 14;
                data.power = 0;
                data.speed = 0;
                data.actionMetter = 5;
                break;
            case Rank.Gold:
                data.health = 16;
                data.power = 2;
                data.speed = 2;
                data.actionMetter = 10;
                break;
            case Rank.Platinum:
                data.health = 18;
                data.power = 2;
                data.speed = 2;
                data.actionMetter = 10;
                data.strikePower = 1;
                break;
            case Rank.Diamond:
                data.health = 20;
                data.power = 4;
                data.speed = 4;
                data.actionMetter = 15;
                data.strikePower = 1;
                break;
            case Rank.Challenger:
                data.health = 22;
                data.power = 4;
                data.speed = 4;
                data.actionMetter = 15;
                data.strikePower = 1;
                break;
            case Rank.Omega:
                data.health = 24;
                data.power = 6;
                data.speed = 6;
                data.actionMetter = 20;
                data.strikePower = 2;
                break;
            case Rank.ProLeague:
                data.health = 26;
                data.power = 6;
                data.speed = 6;
                data.actionMetter = 20;
                data.strikePower = 2;
                break;
        }
    }

    private void GenerateExtraStats(PlayerCard data)
    {
        int remaining = 0; int maxStat = 4; int index; int type; List<int> statType = new List<int>() { 0, 1, 2 }; int[] extra = new int[3];

        switch (data.rank)
        {
            case Rank.Bronze:
                remaining = 4;
                break;
            case Rank.Silver:
                remaining = 4;
                break;
            case Rank.Gold:
                remaining = 6;
                break;
            case Rank.Platinum:
                remaining = 6;
                break;
            case Rank.Diamond:
                remaining = 8;
                break;
            case Rank.Challenger:
                remaining = 8;
                break;
            case Rank.Omega:
                remaining = 10;
                break;
            case Rank.ProLeague:
                remaining = 10;
                break;
        }

        while (remaining > 0)
        {
            index = Random.Range(0, statType.Count);

            type = statType[index];

            switch(type)
            {
                case 0:
                    extra[0]++;
                    break;
                case 1:
                    extra[1]++;
                    break;
                case 2:
                    extra[2]++;
                    break;
            }

            if (extra[type] >= maxStat)
                statType.Remove(type);

            remaining--;
        }

        Debug.Log("Health: " + extra[0] + ", Power: " + extra[1] + ", Speed: " + extra[2]);

        data.health += extra[0];
        data.power += extra[1];
        data.speed += extra[2];
    }
}
