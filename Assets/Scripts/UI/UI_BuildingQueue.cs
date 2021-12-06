using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_BuildingQueue : MonoBehaviour
{
    [SerializeField] Image timerImage;

    UnitSpawner unitSpawner;
    TMP_Text unitQueueText;
    
    // Start is called before the first frame update
    void Start()
    {
        unitSpawner = GetComponentInParent<UnitSpawner>();
        unitQueueText = GetComponentInChildren<TMP_Text>();

        unitSpawner.ServerOnUnitQueueSizeUpdated += SetCurrentQueueText;
        unitSpawner.ServerOnUnitQueued += StartQueueTimer;

        unitQueueText.text = "0";
        timerImage.fillAmount = 0;
    }

    void OnDestroy()
    {
        unitSpawner.ServerOnUnitQueueSizeUpdated -= SetCurrentQueueText;
        unitSpawner.ServerOnUnitQueued -= StartQueueTimer;
    }

    void SetCurrentQueueText(int newQueueSize)
    {
        unitQueueText.text = newQueueSize.ToString();

        if (newQueueSize == 0)
        {
            timerImage.fillAmount = 0;
        }
    }

    void StartQueueTimer(Unit unit)
    {
        if (timerImage == null) { return; }

        StartCoroutine(BuildingCountdown(unit.BuildingTime));
    }
    
    IEnumerator BuildingCountdown(int buildingTime)
    {
        float currentBuildingTime = 0f;

        while (currentBuildingTime <= buildingTime)
        {
            timerImage.fillAmount = currentBuildingTime / buildingTime;
            currentBuildingTime += Time.deltaTime;
            yield return null;
        }
    }
}
