using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HeartBar : NetworkBehaviour
{
    public GameObject heartPrefab;
    public PlayerScript playerScript;
    private List<HealthHeart> hearts = new List<HealthHeart>();

    

    public void DrawHearts()
    {
        ClearHearts();
        int heartsToMake = playerScript.playerMaxHealth;
        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(playerScript.playerHealth - (i * 1), 0, 1);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);
        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach (Transform t  in transform)
        {
            Destroy(t.gameObject);
        }

        hearts = new List<HealthHeart>();
    }
}
