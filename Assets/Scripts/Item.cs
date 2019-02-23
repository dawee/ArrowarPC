using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {
        Heart,
        Shield,
        Bomb
    };

    [SerializeField]
    private Type type;
}
