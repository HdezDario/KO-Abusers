using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestMultiplayerUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            Debug.Log("Starting Host");
            NetworkManager.Singleton.StartHost();
        });
        startClientButton.onClick.AddListener(() =>
        {
            Debug.Log("Starting Client");
            NetworkManager.Singleton.StartClient();
        });
    }
}
