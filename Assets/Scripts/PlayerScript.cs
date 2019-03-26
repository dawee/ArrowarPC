using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
  [SerializeField]
  private Animator[] healthAnimators;

  [SerializeField]
  private Animator[] armorAnimators;

  private int health = 3;
  private int armor = 3;

  private void SetVisibleItems(Animator[] animators, int count)
  {
    for (var i = 0; i < animators.Length; i++)
    {
      animators[i].SetBool("visible", i < count);
    }
  }

  private void SetHealth(int val)
  {
    health = val;
    SetVisibleItems(healthAnimators, val);
  }

  private void SetArmor(int val)
  {
    armor = val;
    SetVisibleItems(armorAnimators, val);
  }

  public void Reset()
  {
    SetHealth(3);
    SetArmor(3);
  }

  public void Hit()
  {
    if (armor > 0)
    {
      SetArmor(armor - 1);
    }
    else if (health > 0)
    {
      SetHealth(health - 1);
    }

    if (health == 0)
    {
      Debug.LogFormat("RIP {0}", name);
    }
  }

  public void PullItems(PushedItemsEventNode.Data data)
  {
    foreach (var item in data.Origin.Items)
    {
      item.Move(data.Origin, this);
    }

    data.Origin.Items.Clear();
  }

  public void OnItemDropped(Item item)
  {
    Hit();
    Object.Destroy(item.gameObject);
  }

  private void Awake()
  {
    Reset();
  }
}
