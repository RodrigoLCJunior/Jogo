using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Ele inclui componentes como botões, textos, imagens, sliders, painéis e muito mais que são usados para construir a interface visual do jogo.
using System;
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

    /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */
    [Header("Player Shortcuts")]
    public KeyCode attributesKey = KeyCode.C;
    public KeyCode inventoryKey = KeyCode.I;

    [Header("Player UI Panels")]
    public GameObject playerAttributesPanel;
    public GameObject playerInventoryPanel;

    /* KALLEDRA - Rodrigo - 07/10/2024 - Morte do Player */
    [Header("Respawn")]
    public float respawnTime = 5;
    public GameObject prefab;

    /* KALLEDRA - Rodrigo - 09/10/2024 - Level Up (Animação e Sound) */
    [Header("Exp")]
    public int currentExp;
    public int expBase;
    public int expLeft;
    public float expMod;
    public GameObject levelUpFX;
    public AudioClip levelUpSound;
    public int givePoints = 5; /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */

    [Header("Player UI")]
    //Slider é um Objeto padrão do Unity
    public Slider health;
    public Slider mana;
    public Slider stamina;
    public Slider exp;
    /* KALLEDRA - Rodrigo - 10/10/2024 - Level Up (Animação, Sound e Texto) */
    public Text expText;
    public Text LevelText;
    /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */
    public Text strText;
    public Text resText;
    public Text intText;
    public Text wilText;
    public Button strPositiveBtn;
    public Button resPositiveBtn;
    public Button intPositiveBtn;
    public Button wilPositiveBtn;
    /*
    public Button strNegativeBtn;
    public Button resNegativeBtn;
    public Button intNegativeBtn;
    public Button wilNegativeBtn;
    */

    public Text pointsTxt;
    
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

        /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */
        /*if (Input.GetKeyDown(KeyCode.C)){
            attributesPanel.SetActive(attributesPanel.activeSelf);        
        }*/


        entity.currentHealth  = entity.maxHealth;
        entity.currentMana    = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        health.maxValue = entity.maxHealth;
        health.value    = health.maxValue;

        mana.maxValue = entity.maxMana;
        mana.value    = mana.maxValue;

        stamina.maxValue = entity.maxStamina;
        stamina.value    = stamina.maxValue;

        exp.value = currentExp; /* KALLEDRA - Rodrigo - 09/10/2024 - Level Up (Animação e Sound) */
        exp.value = expLeft;    /* KALLEDRA - Rodrigo - 09/10/2024 - Level Up (Animação e Sound) */

        /* KALLEDRA - Rodrigo - 10/10/2024 - Level Up (Animação, Sound e Texto) */
        expText.text = string.Format("Exp: {0}/{1}", currentExp, expLeft);
        LevelText.text = entity.level.ToString();

        //Iniciar Regen Health pela rotina;
        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
        StartCoroutine(RegenStamina());

        /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */
        UpdatePoint();
        SetUpButtons();
    }

    private void Update()
    {
        /* Rodrigo - 05/10/2024 */
        if (entity.dead){
            return;
        }

        if(entity.currentHealth <= 0){
            
            /* KALLEDRA - Rodrigo - 06/10/2024 - Morte do Player */
            /*entity.currentHealth = 0;
            entity.dead = true;*/
            Die();

        }


        
        health.value  = entity.currentHealth;
        mana.value    = entity.currentMana;
        stamina.value = entity.currentStamina;

        /* KALLEDRA - Rodrigo - 09/10/2024 - Level Up (Animação e Sound) */
        exp.value = currentExp;
        exp.maxValue = expLeft;

        /* KALLEDRA - Rodrigo - 10/10/2024 - Level Up (Animação, Sound e Texto) */
        expText.text = string.Format("Exp: {0}/{1}", currentExp, expLeft);
        LevelText.text = entity.level.ToString();


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

        if(Input.GetKeyDown(KeyCode.M))
            GainExp(currentExp += 50);

        if (Input.GetKeyDown(KeyCode.C)){
            Debug.Log("Tecla C pressionada (Player.cs)");
            ShowPanel();       
        }

        /* KALLEDRA - Rodrigo - 19/10/2024 - Inventorio (Panel, e Mudança de Status) */
        if (Input.GetKeyDown(inventoryKey)){
            playerInventoryPanel.SetActive(!playerInventoryPanel.activeSelf);     
        }

        if(playerAttributesPanel.activeSelf){
            strText.text = entity.strength.ToString();
            resText.text = entity.resistence.ToString();
            intText.text = entity.inteligence.ToString();
            wilText.text = entity.willpower.ToString();
        }

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

    /* KALLEDRA - Rodrigo - 06/10/2024 - Morte do Player */
    void Die(){
        entity.currentHealth = 0;
        entity.dead = true;
        entity.target = null;
        StopAllCoroutines(); // Função do Unity que para todas as rotinas
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn(){
        GetComponent<PlayerController>().enabled = false;
        yield return new WaitForSeconds(respawnTime);

        GameObject newPlayer = Instantiate(prefab, transform.position, transform.rotation);
        newPlayer.name = prefab.name;
        newPlayer.GetComponent<Player>().entity.dead = false;
        newPlayer.GetComponent<Player>().entity.combatCoroutine = false;
        newPlayer.GetComponent<PlayerController>().enabled = true;

        Destroy(this.gameObject);
    }
    
    /* KALLEDRA - Rodrigo - 09/10/2024 - Level Up (Animação e Sound) */
    /*public void GainExp(int amount)
    {
        Debug.Log(amount);
        currentExp += amount;
        if(currentExp >= expLeft)
        {
            LevelUp();
        }
    }*/

    public void GainExp(int amount){
        Debug.Log(amount);
        currentExp += amount;
    
        // Verifica se a experiência atual é suficiente para passar de nível
        while (currentExp >= expLeft)
        {
            LevelUp(); // Chama o método para subir de nível
        }
    }

 
    public void LevelUp()
    {
        currentExp -= expLeft;
        entity.level++;
        entity.points += givePoints; /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */
        UpdatePoint();               /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */
 
        entity.currentHealth = entity.maxHealth;
 
        float newExp = Mathf.Pow((float)expMod, entity.level);
        expLeft = (int)Mathf.Floor((float)expBase * newExp);
 
        entity.entityAudio.PlayOneShot(levelUpSound); // Vai ser chamado 1 Vez para o som
        Instantiate(levelUpFX, this.gameObject.transform);
    }

    /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */
    public void UpdatePoint(){
        strText.text = entity.strength.ToString();
        resText.text = entity.resistence.ToString();
        intText.text = entity.inteligence.ToString();
        wilText.text = entity.willpower.ToString();
        pointsTxt.text = entity.points.ToString();
    }

    public void SetUpButtons(){
        strPositiveBtn.onClick.AddListener(() => AddPoints(1));
        resPositiveBtn.onClick.AddListener(() => AddPoints(2));
        intPositiveBtn.onClick.AddListener(() => AddPoints(3));
        wilPositiveBtn.onClick.AddListener(() => AddPoints(4));

        /*strNegativeBtn.onClick.AddListener(() => RemovePoints(1));
        resNegativeBtn.onClick.AddListener(() => RemovePoints(2));
        intNegativeBtn.onClick.AddListener(() => RemovePoints(3));
        wilNegativeBtn.onClick.AddListener(() => RemovePoints(4));*/
    }


    public void AddPoints(int index){
        if(entity.points > 0){
            if(index == 1){
                entity.strength++;
            }else if(index == 2){
                entity.resistence++;
            }else if(index == 3){
                entity.inteligence++;
            }else if(index == 4){
                entity.willpower++;
            }
            entity.points--;
            UpdatePoint();
        }
    }

    public void RemovePoints(int index){
        if(entity.points > 0){
            if(index == 1 && entity.strength > 0){
                entity.strength--;
            }else if(index == 2 && entity.resistence > 0){
                entity.resistence--;
            }else if(index == 3 && entity.inteligence > 0){
                entity.inteligence--;
            }else if(index == 4 && entity.willpower > 0){
                entity.willpower--;
            }
            entity.points++;
            UpdatePoint();
        }
    }

    private void ShowPanel(){
        if (playerAttributesPanel != null)
        {
            bool isActive = playerAttributesPanel.activeSelf; // Verifica se o painel está ativo
            playerAttributesPanel.SetActive(!isActive); // Alterna a visibilidade do painel
            Debug.Log("Painel agora está " + (playerAttributesPanel.activeSelf ? "ativo" : "inativo"));
        }
        else
        {
            Debug.LogError("playerAttributesPanel não está definido (Player.cs)");
        }
    }

}