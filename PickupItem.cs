/*
// Autor.: Rodrigo Luiz Cardoso Junior
// Data..: 18/10/2024
// Motivo: Metodo de pegar itens do Chão
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;
    bool alreadyPickup = false;

    public void OnTriggerStay2D(Collider2D collision) {
        if (collision.transform.tag == "Player"){
            if(Input.GetKeyDown(KeyCode.E) && !alreadyPickup){
               // Só remover do comentario quando tiver um Panel de Iventario Pronto
               /* //pickup Item 
                Iventory.Singleton.PickupItem(item);
                alreadyPickup = true;

                Destroy(this.gameObject);
                */
            }
        }
    }
}
