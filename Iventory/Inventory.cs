/*
// Autor.: Rodrigo Luiz Cardoso Junior
// Data..: 18/10/2024
// Motivo: Metodo de Iventorio, e visual
*/

using UnityEngine;
using UnityEngine.UI; // Necessário para trabalhar com o componente Button

public class Inventory : MonoBehaviour
{
    // Singleton para facilitar o acesso global à instância da classe
    public static Inventory Singleton;

    // Item atualmente sendo carregado pelo cursor (drag-and-drop)
    public static InventoryItem carriedItem;

    // Slots de inventário e equipamento para armazenar os itens
    [SerializeField] InventorySlots[] inventorySlots;
    [SerializeField] InventorySlots[] equipamentSlots;

    // Transform para armazenar o objeto atualmente sendo arrastado
    [SerializeField] Transform draggableTransform;

    // Prefab do item que será instanciado
    [SerializeField] InventoryItem itemPrefab;

    // Lista de itens que podem ser utilizados ou gerados no inventário
    [SerializeField] Item[] items;

    // Botão que, ao ser clicado, gera um novo item no inventário
    [SerializeField] Button giveItemBtn;

    public Player player;
    private Item helmetTempItem;
    private Item chestTempItem;
    private Item legsTempItem;
    private Item feetTempItem;

    // Inicialização do Singleton e registro do evento de clique no botão
    private void Awake(){
        player = GameObject.Find("Player").GetComponent<Player>();
        Singleton = this;
        giveItemBtn.onClick.AddListener(delegate { SpawnInventoryItem(); });
    }

    // Atualiza a posição do item carregado para seguir o cursor do mouse
    private void Update() {
        if(carriedItem == null){
            return; // Se não houver item carregado, não faz nada
        }

        carriedItem.transform.position = Input.mousePosition; // Move o item para a posição do cursor
    }

    // Define o item que está sendo carregado (drag-and-drop)
    public void SetCarriedItem(InventoryItem item){
        if(carriedItem != null){
            // Verifica se o item pode ser colocado no slot atual, baseado na tag
            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag){
                return; // Se a tag não for compatível, não faz nada
            }

            // Coloca o item carregado no slot atual
            item.activeSlot.SetItem(carriedItem);
        } 
        
        // Se o slot tiver uma tag específica, remove o equipamento antigo
        if(item.activeSlot.myTag != SlotTag.None){
            EquipEquipament(item.activeSlot.myTag, null);
        }

        // Atualiza o item sendo carregado e impede que interfira na detecção de colisão
        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggableTransform);
    }

    // Equipamento ou remoção de itens baseados na tag do slot
    public void EquipEquipament(SlotTag tag, InventoryItem item = null){
        switch(tag){
            case SlotTag.Head:
                if(item == null){
                    player.entity.strength -= helmetTempItem.str;
                    player.entity.resistence -= helmetTempItem.res;
                    helmetTempItem = null;
                    Debug.Log("Removeu o Item da Cabeça"); // Mensagem para remoção de item
                } else{
                    player.entity.strength -= helmetTempItem.str;
                    player.entity.resistence -= helmetTempItem.res;
                    helmetTempItem = item.myItem;
                    Debug.Log("Equipou o Item na Cabeça"); // Mensagem para equipamento de item
                }
                break;
            case SlotTag.Chest:
                // Implementar lógica para equipamento no peito
                if(item == null){
                    chestTempItem = null;
                    Debug.Log("Removeu o Item da Peito"); // Mensagem para remoção de item
                } else{
                    Debug.Log("Equipou o Item na Peito"); // Mensagem para equipamento de item
                    chestTempItem = item.myItem;
                }
                break;
            case SlotTag.Legs:
                // Implementar lógica para equipamento nas pernas
                if(item == null){
                    legsTempItem = null;
                    Debug.Log("Removeu o Item da Penas"); // Mensagem para remoção de item
                } else{
                    Debug.Log("Equipou o Item na Pernas"); // Mensagem para equipamento de item
                    legsTempItem = item.myItem;
                }
                break;
            case SlotTag.Feet:
                // Implementar lógica para equipamento nos pés
                if(item == null){
                    feetTempItem = null;
                    Debug.Log("Removeu o Item da Pernas"); // Mensagem para remoção de item
                } else{
                    Debug.Log("Equipou o Item na Pernas"); // Mensagem para equipamento de item
                    feetTempItem = item.myItem;
                }
                break;
        }
    }

    // Gera um novo item e o coloca em um slot disponível do inventário
    public void SpawnInventoryItem(Item item = null){
        Item _item = item;
        if(_item == null){
            _item = PickRandomItem(); // Seleciona um item aleatório se nenhum for especificado
        }

        // Procura um slot vazio no inventário para colocar o novo item
        for(int i = 0; i < inventorySlots.Length; i++){
            if(inventorySlots[i].myItem == null){
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }

    // Retorna um item aleatório da lista de itens disponíveis
    Item PickRandomItem(){
        int random = Random.Range(0, items.Length);
        return items[random];
    }

    Item PickItem(Item pickItem){
        Item selectItem = null;
        foreach(var item in items){
            if(item.name == pickItem.name){
                selectItem = item;
                break;
            }
        }
        return selectItem;
    }

    public void PickupItem(Item item){
        Item _item = item;
        if(_item == null){
            _item = PickItem(item); // Seleciona um item especificado
        }

        // Procura um slot vazio no inventário para colocar o novo item
        for(int i = 0; i < inventorySlots.Length; i++){
            if(inventorySlots[i].myItem == null){
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }
    public void DropItem(InventoryItem item){
        Debug.Log("Drop item (Iventory.cs)");
        SpawnObjectNearPlayer(item);
        Destroy(item.gameObject);
    }

    public void SpawnObjectNearPlayer(InventoryItem item){
        Transform player = GameObject.Find("Player").transform;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(0.1f, 0.5f);
        Vector2 spawnPosition = (Vector2)player.position + randomDirection * randomDistance;

        GameObject dropItemPrefab = Instantiate(item.myItem.prefab, spawnPosition, Quaternion.identity);
        dropItemPrefab.GetComponent<SpriteRenderer>().sprite = item.myItem.sprite;
        dropItemPrefab.GetComponent<PickupItem>().item = item.myItem;
    }
}
