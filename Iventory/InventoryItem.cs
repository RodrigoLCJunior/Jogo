/*
// Autor.: Rodrigo Luiz Cardoso Junior
// Data..: 18/10/2024
// Motivo: Itens unicos para colocar nos Slots(Ainda não Estocam, ficam em Slots separados)
*/ 

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }
    public Item myItem { get; set; }
    public InventorySlots activeSlot { get; set; }

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
        
    }

    public void Initialize(Item item, InventorySlots parent){
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        itemIcon.sprite = item.sprite;

    }
    
    public void OnPointerClick(PointerEventData eventData){
        if(eventData.button == PointerEventData.InputButton.Left){
            //ToDo
            Inventory.Singleton.SetCarriedItem(this);
            
        }else if(eventData.button == PointerEventData.InputButton.Right){
            //ToDo
            Inventory.Singleton.DropItem(this);
            
        }

    }
}
