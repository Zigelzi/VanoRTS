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

        unitSpawner.ClientOnUnitQueueSizeUpdated += SetCurrentQueueText;
        unitSpawner.ClientOnUnitBuildingStarted += StartBuildingTimer;

        unitQueueText.text = "0";
        timerImage.fillAmount = 0;
    }

    void OnDestroy()
    {
        unitSpawner.ClientOnUnitQueueSizeUpdated -= SetCurrentQueueText;
        unitSpawner.ClientOnUnitBuildingStarted -= StartBuildingTimer;
    }

    void SetCurrentQueueText(int newQueueSize)
    {
        unitQueueText.text = newQueueSize.ToString();

        if (newQueueSize == 0)
        {
            timerImage.fillAmount = 0;
        }
    }

    void StartBuildingTimer(Unit unit)
    {
        if (timerImage == null) { return; }

        StartCoroutine(BuildingCountdown(unit.BuildingTime));
    }
    
    IEnumerator BuildingCountdown(int buildingTime)
    {
        float currentBuildingTime = 0f;
        timerImage.fillAmount = 0;

        while (currentBuildingTime <= buildingTime)
        {
            float currentProgress = currentBuildingTime / buildingTime;
            timerImage.fillAmount = currentProgress;
            currentBuildingTime += Time.deltaTime;
            yield return null;
        }
    }
}
