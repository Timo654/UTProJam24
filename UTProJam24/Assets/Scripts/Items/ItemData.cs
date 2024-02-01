using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Factory/Items")]
[Serializable]
public class ItemData : ScriptableObject
{
    public string displayName;
    public Sprite sprite;
    public ItemType type;
    // TODO - add more items in case we need em

}
