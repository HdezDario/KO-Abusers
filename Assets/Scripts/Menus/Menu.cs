using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void LoadGame()
    {
        SaveData.current = (SaveData)SaveSystem.Load(Application.persistentDataPath + "/SaveFiles/SaveData.ko");
    }

    public static void SaveGame()
    {
        SaveSystem.Save(SaveData.current);
    }

    public void DeleteGame()
    {
        SaveSystem.Delete(Application.persistentDataPath + "/SaveFiles/SaveData.ko");
    }

    public void UnlockPlayerCard(string name)
    {
        try
        {
            PlayerCard player = Resources.Load<PlayerCard>("PlayerCards/" + name);

            switch (player.role)
            {
                case (Role.Goalie):
                    SaveData.current.unlockedGoalies.Add(player.playerName);
                    break;
                case (Role.Midfielder):
                    SaveData.current.unlockedMidfielders.Add(player.playerName);
                    break;
                case (Role.Forward):
                    SaveData.current.unlockedForwards.Add(player.playerName);
                    break;
            }

            //Debug.Log("Unlocked " + name);
            SaveData.current.unlockedPlayerCards.Add(player.playerName);
        }

        catch
        {
            Debug.Log("Fail Unlocking Player. Card Already Unlocked or Non Existent");
        }
    }

    public virtual void SetTeamMember(string name, Role role)
    {
        switch (role)
        {
            case (Role.Goalie):
                SaveData.current.goalie = name;
                break;
            case (Role.Midfielder):
                SaveData.current.midfielder = name;
                break;
            case (Role.Forward):
                SaveData.current.forward = name;
                break;
        }
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}
