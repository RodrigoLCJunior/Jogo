using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Necessário para o uso de Slider

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Monster : MonoBehaviour
{
    [Header("Controller")]
    public Entity entity;
    public GameManager manager;

    [Header("Patrol")]
    public List<Transform> waypointList;
    public float arrivalDistance = 1.0f;
    public float waitTime = 5;
    public int waypointID;

    // Variáveis Privadas
    Transform targetWaypoint;
    int currentWaypoint = 0;
    float lastDistanceToTarget = 0f;
    float currentWaitTime = 0f;

    [Header("Experience Reward")]
    public int rewardExperience = 10;
    public int lootGoldMin = 0;
    public int lootGoldMax = 10;

    [Header("Respawn")]
    public GameObject prefab;
    public bool respawn = true;
    public float respawnTime = 10f;

    /* KALLEDRA - Rodrigo - 06/10/2024 - UI (Barra de Vida do Monstro) */
    [Header("UI")]
    public Slider healthSlider;

    Rigidbody2D rb2D;
    Animator animator;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        /* KALLEDRA - Rodrigo - 06/10/2024 - UI (Barra de Vida do Monstro) */
        healthSlider.maxValue = entity.maxHealth;
        healthSlider.value = healthSlider.maxValue;

        currentWaitTime = waitTime;
        if (waypointList.Count > 0){
            targetWaypoint = waypointList[currentWaypoint];
            lastDistanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);
        }

        /* KALLEDRA - Rodrigo - 06/10/2024 - ID de Waypoint (Waypoint no Prefab) */
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint")){
            int ID = obj.GetComponent<WaypointID>().ID;
            if(ID == waypointID){
                waypointList.Add(obj.transform);
            }
        }
    }

    private void Update(){
        if (entity.dead){
            return;
        }

        if (entity.currentHealth <= 0){
            entity.currentHealth = 0;
            Die();
        }

        /* KALLEDRA - Rodrigo - 06/10/2024 - UI (Barra de Vida do Monstro) */
        healthSlider.value = entity.currentHealth;

        if (!entity.inCombat){
            if (waypointList.Count > 0){
                Patrol();
            } else {
                animator.SetBool("isWalking", false);
            }
        } else {
            if (entity.attackTimer > 0) {
                entity.attackTimer -= Time.deltaTime;
            }

            if (entity.attackTimer < 0){
                entity.attackTimer = 0;
            }

            if (entity.target != null && entity.inCombat){
                if (!entity.combatCoroutine){
                    StartCoroutine(Attack());
                } else {
                    entity.combatCoroutine = false;
                    StopCoroutine(Attack());
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider){
        if (collider.tag == "Player" && !entity.dead){
            entity.inCombat = true;
            entity.target = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collider){
        if (collider.tag == "Player"){
            entity.inCombat = false;
            entity.target = null;
        }
    }

    void Patrol(){
        if (entity.dead){
            return;
        }

        // Calcular a distância até o Waypoint.
        float distanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);
        
        // Mudar a condição para apenas verificar a distância até o waypoint.
        if (distanceToTarget <= arrivalDistance){
            currentWaitTime -= Time.deltaTime; // Use o tempo de espera para criar uma pausa

            if (currentWaitTime <= 0){
                currentWaypoint++;
                if (currentWaypoint >= waypointList.Count){
                    currentWaypoint = 0;
                }

                targetWaypoint = waypointList[currentWaypoint];
                currentWaitTime = waitTime;
            }
            animator.SetBool("isWalking", false); // Parar a animação ao chegar no waypoint
        }else{
            animator.SetBool("isWalking", true);
            Vector2 direction = (targetWaypoint.position - transform.position).normalized; // Definindo a direção aqui

            // Mover o inimigo na direção do waypoint.
            rb2D.MovePosition(rb2D.position + direction * (entity.speed * Time.fixedDeltaTime));

            animator.SetFloat("input_x", direction.x);
            animator.SetFloat("input_y", direction.y);
        }
    }

    IEnumerator Attack(){
        entity.combatCoroutine = true;

        while (true){
            yield return new WaitForSeconds(entity.cooldown);

            if (entity.target != null && !entity.target.GetComponent<Player>().entity.dead){
                animator.SetBool("attack", true);

                float distance = Vector2.Distance(entity.target.transform.position, transform.position);

                if (distance <= entity.attackDistance){
                    int dmg = manager.CalculateDamage(entity, entity.damage);
                    int targetDef = manager.CalculateDefense(entity.target.GetComponent<Player>().entity, entity.target.GetComponent<Player>().entity.defense);
                    int dmgResult = dmg - targetDef;

                    if (dmgResult < 0){
                        dmgResult = 0;
                    }

                    Debug.LogFormat("Inimigo atacou o Player, DMG: " + dmgResult + " (Monster.cs).");
                    entity.target.GetComponent<Player>().entity.currentHealth -= dmgResult;
                }
            }
        }
    }

    void Die(){
        entity.dead = true;
        entity.inCombat = false;
        entity.target = null;

        animator.SetBool("isWalking", false);

        /* KALLEDRA - Rodrigo - 09/10/2024 - Level Up (Animação e Sound) */
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.GainExp(rewardExperience);

        Debug.Log("O inimigo Morreu " + entity.name + " (Monster.cs).");

        StopAllCoroutines();
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn(){
        yield return new WaitForSeconds(respawnTime);

        GameObject newMonster = Instantiate(prefab, transform.position, transform.rotation);
        newMonster.name = prefab.name;
        newMonster.GetComponent<Monster>().entity.dead = false;

        /* KALLEDRA - Rodrigo - 06/10/2024 - UI (Barra de Vida do Monstro) */
        newMonster.GetComponent<Monster>().entity.combatCoroutine = false;

        Destroy(this.gameObject);
    }
}
