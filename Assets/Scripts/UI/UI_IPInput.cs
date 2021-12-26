using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class UI_IPInput : MonoBehaviour
{
    TMP_InputField ipInputField;
    // Start is called before the first frame update
    void OnEnable()
    {
        ipInputField = GetComponent<TMP_InputField>();
        ipInputField.text = NetworkManager.singleton.networkAddress;
    }
}
