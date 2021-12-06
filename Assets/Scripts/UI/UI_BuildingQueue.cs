using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_BuildingQueue : MonoBehaviour
{
    UnitSpawner unitSpawner;
    TMP_Text unitQueueText;
    
    // Start is called before the first frame update
    void Start()
    {
        unitSpawner = GetComponentInParent<UnitSpawner>();
        unitQueueText = GetComponentInChildren<TMP_Text>();

        unitQueueText.text = "0";

        unitSpawner.ServerOnUnitQueueSizeUpdated += SetCurrentQueueText;
        unitSpawner.ServerOnUnitQueued += StartQueueTimer;
    }

    void OnDestroy()
    {
        unitSpawner.ServerOnUnitQueueSizeUpdated -= SetCurrentQueueText;
        unitSpawner.ServerOnUnitQueued -= StartQueueTimer;
    }

    void SetCurrentQueueText(int newQueueSize)
    {
        unitQueueText.text = newQueueSize.ToString();
    }

    void StartQueueTimer(Unit unit)
    {

    } 
}
