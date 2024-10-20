//São as Entidades, valores de vida, mana, Status

using System.Collections;
using System.Collections.Generic;
using UnityEngine; // Necessário para usar as classes e tipos específicos do Unity, como MonoBehaviour, GameObject, e outros recursos.
using System;      // Fornece acesso a classes e funções do .NET Framework, como Serializable, DateTime, e outras utilidades básicas.

// Transforma o estado de um objeto em um formato que pode ser facilmente armazenado e reconstruído mais tarde.
[Serializable]
public class Entity
{
    //É um atributo usado para adicionar um cabeçalho no Inspector da Unity.
    [Header("Name")]
    public string name;
    public int level;

    [Header("Health")]
    public int currentHealth;
    public int maxHealth;

    [Header("Mana")]
    public int currentMana;
    public int maxMana;

    [Header("Stamina")]
    public int currentStamina;
    public int maxStamina;

    [Header("Stats")]
    public int strength = 1;
    public int resistence = 1;
    public int inteligence = 1;
    public int willpower = 1;
    public int damage = 1;
    public int defense = 1;
    public float speed = 2f;
    public int points = 0; /* KALLEDRA - Rodrigo - 13/10/2024 - P. Atributos(Mostrar e Administrar) */

    [Header("Combat")]
    public float attackDistance = 0.5f;
    public float attackTimer = 1;
    public float cooldown = 2;
    public bool inCombat = false;
    public GameObject target;
    public bool combatCoroutine = false;
    public bool dead = false;

    /* KALLEDRA - Rodrigo - 09/10/2024 - Level Up (Animação e Sound) */
    [Header("Component")]
    public AudioSource entityAudio;

}