using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemDistributor : MonoBehaviour {

    private float lastDistributionTime;

    [SerializeField]
    private float period = 5f;

    [SerializeField]
    private ItemTypeEvent distributed = new ItemTypeEvent();

    public ItemTypeEvent Distributed {
        get {
            return distributed;
        }
    }

    private void Update() {
        if (Time.fixedTime - lastDistributionTime >= period) {
            Debug.Log("distribute item");
            distributed.Invoke(Item.ItemType.Bomb);
            lastDistributionTime = Time.fixedTime;
        }
    }

}