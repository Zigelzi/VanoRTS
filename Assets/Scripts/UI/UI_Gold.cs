using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class UI_Gold : MonoBehaviour
{
    TMP_Text goldText;
    RtsNetworkBank bank;
    RtsNetworkPlayer player;

    // Start is called before the first frame update
    void Start()
    {
        goldText = GetComponentInChildren<TMP_Text>();     
    }
    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RtsNetworkPlayer>();
            
            bank = player.GetComponent<RtsNetworkBank>();
            bank.ClientOnGoldUpdate += SetCurrentGold;

            SetCurrentGold(bank.StartingGold);
        }
    }

    void OnDestroy()
    {
        bank.ClientOnGoldUpdate -= SetCurrentGold;
    }

    void SetCurrentGold(int newGoldValue)
    {
        Debug.Log("Setcurrentgold ran");
        goldText.text = newGoldValue.ToString();
    }
}
