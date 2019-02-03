using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private Image[] healthImages;

    [SerializeField]
    private Image[] armorImages;

    private int health = 3;
    private int armor = 3;

    private void SetVisibleItems(Image[] images, int count)
    {
        for (var i = 0; i < images.Length; i++)
        {
            images[i].enabled = i < count;
        }
    }

    private void SetHealth(int val)
    {
        health = val;
        SetVisibleItems(healthImages, val);
    }

    private void SetArmor(int val)
    {
        armor = val;
        SetVisibleItems(armorImages, val);
    }

    public void Reset()
    {
        SetHealth(3);
        SetArmor(3);
    }

    public void Hit()
    {
        if (armor > 0) {
            SetArmor(armor - 1);
        }
        else if (health > 0) {
            SetHealth(health - 1);
        }

        if (health == 0) {
            Debug.LogFormat("RIP {0}", name);
        }
    }

    private void Awake()
    {
        Reset();
    }
}
