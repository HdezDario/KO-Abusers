using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NewPlayerManager : Menu
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmNameButton;

    private void Awake()
    {
        nameInputField.onValueChanged.AddListener((string name) =>
        {
            if (name.Length < 1)
                confirmNameButton.interactable = false;
            else
                confirmNameButton.interactable = true;
        });

        confirmNameButton.onClick.AddListener(() =>
        {
            SaveData.current.name = nameInputField.text;
            SaveGame();
            mainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        });
    }
}
