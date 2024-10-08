//Script de movimentação do Player

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]

public class PlayerController : MonoBehaviour
{
    public Player player; //Puxa o Objeto Player direto da Hierarquia
    public Animator PlayerAnimator; 
    float input_x = 0;
    float input_y = 0;

    bool isWalking = false;
    Rigidbody2D rb2D;

    //Vector2 é uma struct de modelo 2D composto por x e y, Vector2D.zero são os valores zerados. Ex: Vector2D(0, 0)
    Vector2 movement = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;

        // Como temos [RequireComponent], garantimos que o Rigidbody2D e o Player existam
        rb2D   = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Captura a entrada do jogador (teclas WASD, por exemplo)
        input_x = Input.GetAxisRaw("Horizontal");
        input_y = Input.GetAxisRaw("Vertical");

        isWalking = (input_x != 0 || input_y != 0); 
        movement = new Vector2(input_x, input_y);

        if(isWalking){

            // Define o parâmetro "input_x" no Animator para controlar a animação
            PlayerAnimator.SetFloat("input_x", input_x);
            PlayerAnimator.SetFloat("input_y", input_y);

        }

        PlayerAnimator.SetBool("isWalking",isWalking);

        /* KALLEDRA - Rodrigo - 05/10/2024 - Timer de ataque */
        if(player.entity.attackTimer < 0){
            player.entity.attackTimer = 0;
        } else {
            player.entity.attackTimer = Time.deltaTime;
        };

        if(player.entity.attackTimer == 0 && !isWalking){
            //Mapea para o clique do botão esquerdo do mouse, a tecla Ctrl esquerda, ou a tecla Ctrl direita.
            if(Input.GetButtonDown("Fire1")){
                //SetTriger inicia uma transição
                PlayerAnimator.SetTrigger("atack");
                player.entity.attackTimer = player.entity.cooldown;

                Attack();
            }
        }
        
    }

    /*FixedUpdate: Ele é chamado automaticamente, e é executado em intervalos de tempo fixos, 
      tornando-o ideal para manipular a física e garantir um comportamento consistente.*/
    private void FixedUpdate(){
        
        // MovePosition: Resulta em um movimento suave e sem problemas de colisão.
        //rb2D.position: É a posição do Player
        //movement: É o movimento do Player
        //Time.fixedDeltaTime: O tempo que se passou desde o último FixedUpdate. Garante que o movimento seja suave e consistente, independentemente da taxa de quadros.
        rb2D.MovePosition(rb2D.position + movement * player.entity.speed * Time.fixedDeltaTime);

    }

    /* KALLEDRA - Rodrigo - 06/10/2024 - Colisão do Player com o Inimigo */
    void OnTriggerStay2D(Collider2D collider){
        if (collider.transform.tag == "Enemy"){
            player.entity.target = collider.transform.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if (collider.transform.tag == "Enemy"){
            player.entity.target = null;
        }
    }

    /* KALLEDRA - Rodrigo - 06/10/2024 - Ataque do Player */
    void Attack(){

        if(player.entity.target == null){
            return;
        } 

        Monster monster = player.entity.target.GetComponent<Monster>();

        if(monster.entity.dead){
            player.entity.target = null;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.entity.target.transform.position);

        if(distance <= player.entity.attackDistance){
            int dmg      = player.manager.CalculateDamage(player.entity, player.entity.damage);
            int enemyDef = player.manager.CalculateDefense(monster.entity, monster.entity.defense);
            int result   = dmg - enemyDef;

            if(result < 0){
                result = 0;
            }

            monster.entity.currentHealth -= result;
            monster.entity.target = this.gameObject;
        }
    }
}