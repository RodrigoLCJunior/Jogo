/*
// Autor.: Rodrigo Luiz Cardoso Junior
// Data..: 18/10/2024
// Motivo: Slots unicos de Iventario de itens
*/
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlots : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set; }
    public SlotTag myTag;
    public void OnPointerClick(PointerEventData eventData){
        if(eventData.button == PointerEventData.InputButton.Left ){
            //Todo, Remover o comentario quando tiver o Panel do Inventario
            /*
            if(Inventory.carriedItem == null){
                return;
            }

            if(myTag != SlotTag.None && Inventory.carriedItem.myItem.itemTag != myTag){
                return;
            }
            */

        }

    }

    public void SetItem(InventoryItem item){
        //Todo, Remover o comentario quando tiver o Panel do Inventario
        /*
        Inventory.carriedItem = null;
        item.activeSlot.myItem = null;

        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;

        if(myTag != SlotTag.None){
            Inventory.Singleton.EquipEquipament(myTag, myItem);
        }
        */
    }
    
}
