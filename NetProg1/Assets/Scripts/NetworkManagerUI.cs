using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    private void OnGUI()
    {
        if (GUILayout.Button("Host"))
        {
            networkManager.StartHost();
        }
        if (GUILayout.Button("Join"))
        {
            networkManager.StartClient();
        }
        if (GUILayout.Button("Exit"))
        {
            Application.Quit();
        }
    }
}
