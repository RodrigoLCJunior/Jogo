using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Ele inclui componentes como botões, textos, imagens, sliders, painéis e muito mais que são usados para construir a interface visual do jogo.

public class Player : MonoBehaviour
{
    public Entity entity;

    [Header("Player Regen System")]
    public bool regenEnabled = true;
    public float regenHpTime = 5f;
    public int regenHPValue = 5;

    //Regeneração da Mana
    public float regenMPTime = 10f;
    public int regenMPValue = 5;

    //Regeneração da Stamina
    public float regenStaminaTime = 1f;
    public int regenStaminaValue = 1;

    [Header("Game Manager")]
    public GameManager manager;

    [Header("Player UI")]

    //Slider é um Objeto padrão do Unity
    public Slider health;
    public Slider mana;
    public Slider stamina;

    public Slider exp;
    // Start is called before the first frame update
    void Start()
    {
        if(manager == null){
            Debug.LogError("Você precisa anexar o Game Manger aqui no Player (Player.cs).");
            return;
        }

        entity.maxHealth  = manager.CalculateHealth(entity);
        entity.maxMana    = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        /*
        //Teste
        int dmg = manager.CalculateDamage(this, 10);
        int def = manager.CalculateDefense(this, 5);
        */

        entity.currentHealth  = entity.maxHealth;
        entity.currentMana    = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        health.maxValue = entity.maxHealth;
        health.value    = health.maxValue;

        mana.maxValue = entity.maxMana;
        mana.value    = mana.maxValue;

        stamina.maxValue = entity.maxStamina;
        stamina.value    = stamina.maxValue;

        exp.value = 0;

        //Iniciar Regen Health pela rotina;
        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
        StartCoroutine(RegenStamina());
    }

    private void Update()
    {
        health.value  = entity.currentHealth;
        mana.value    = entity.currentMana;
        stamina.value = entity.currentStamina;

        /*
        //Input.GetKeyDown(KeyCode.): Faz a ativação para usar uma tecla do teclado. Reduz o Proprio HP
        if(Input.GetKeyDown(KeyCode.B))
            entity.currentHealth -= 10;

        //Input.GetKeyDown(KeyCode.): Faz a ativação para usar uma tecla do teclado. Reduz o Proprio MP
        if(Input.GetKeyDown(KeyCode.N))
            entity.currentMana -= 10;

        //Input.GetKeyDown(KeyCode.): Faz a ativação para usar uma tecla do teclado. Reduz a Propria Stamina
        if(Input.GetKeyDown(KeyCode.M))
            entity.currentStamina -= 10;  
        */  
    } 

    IEnumerator RegenHealth()
    {
        while(true){  //Loop Infinito
            if(regenEnabled){
                if(entity.currentHealth < entity.maxHealth){
                    Debug.LogFormat("Recuperando HP do Jogador (Player.cs).");
                    entity.currentHealth += regenHPValue;
                    yield return new WaitForSeconds(regenHpTime);
                } else {
                    yield return null;
                }
            } else{
                yield return null;
            }
        } 
    } 

    IEnumerator RegenMana()
    {
        while(true){  //Loop Infinito
            if(regenEnabled){
                if(entity.currentMana < entity.maxMana){
                    Debug.LogFormat("Recuperando Mana do Jogador (Player.cs).");
                    entity.currentMana += regenMPValue;
                    yield return new WaitForSeconds(regenMPTime);
                } else {
                    yield return null;
                }
            } else{
                yield return null;
            }
        } 
    }

    IEnumerator RegenStamina()
    {
        while(true){  //Loop Infinito
            if(regenEnabled){
                if(entity.currentStamina < entity.maxStamina){
                    Debug.LogFormat("Recuperando Stamina do Jogador (Player.cs).");
                    entity.currentStamina += regenStaminaValue;
                    yield return new WaitForSeconds(regenStaminaTime);
                } else {
                    yield return null;
                }
            } else{
                yield return null;
            }
        } 
    }

}
