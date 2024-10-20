/*
// Autor.: Rodrigo Luiz Cardoso Junior
// Data..: 18/10/2024
// Motivo: Entidades dos Itens
*/

using UnityEngine;
public enum SlotTag { None, Head, Chest, Legs, Feet}

[CreateAssetMenu(menuName = "RPG 2D/Item")]

public class Item : ScriptableObject
{
    public Sprite sprite;
    public SlotTag itemTag;
    public GameObject prefab;
    public int str;
    public int res;
}
