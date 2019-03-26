using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileSetup : MonoBehaviour
{

  [SerializeField]
  private float minTimeBetweenSelections = 0.2f;

  public float MinTimeBetweenSelections
  {
    get
    {
      return minTimeBetweenSelections;
    }
  }
}
