using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss1 : Enemy
{   
    [Header("-- Boss Attack Pool --")]
    public Attack baseAttack;   //the basic boss attack
    [SerializeField] private int defaultBasicAttackCost;
    public Attack summonColumns;    //the boss goes to the center of the arena and summons columns
    [SerializeField] private int defaultColumnsCost;
    public Attack throwCableAttack; //the boss grabs a cable and throws it at the player
    [SerializeField] private int defaultCableCost;
    public LayerMask whatIsCable;
    public Attack jumpOntoServer;   //boss jumps on to the server and hits it to spawn enemies
    [SerializeField] int defaultJumpServerCost;

    [Header("-- Boss Name --")]
    public TextMeshProUGUI bossNameText;
    public string bossName;

    private int attackTokens = 0;   //boss needs a certain number of tokens to do different attacks
    private List<Attack> attacks;   //list of all the bosses attacks


    private void Start(){
        bossNameText.text = bossName;

        attacks = new List<Attack>();
        
        //set the costs of the attacks
        baseAttack.AttackCost = defaultBasicAttackCost;
        attacks.Add(baseAttack);

        summonColumns.AttackCost = defaultColumnsCost;
        attacks.Add(summonColumns);

        throwCableAttack.AttackCost = defaultCableCost;
        attacks.Add(throwCableAttack);

        jumpOntoServer.AttackCost = defaultJumpServerCost;
        attacks.Add(jumpOntoServer);
        
        //The boss begins to accumulate tokens over time
        StartCoroutine(TokensOverTime());
    }

    //override of enemy class
    public override void ChooseAttack()
    {
        Attack attackToPerform = baseAttack;

        if(Physics2D.OverlapCircle(transform.position, 50f, whatIsCable) == null){
            attacks.Remove(throwCableAttack);   //cannot do this attack when there are no cables
        }

        //choose the cheapest attack
        foreach(Attack attack in attacks){
            //if the attack is cheaper and the boss can afford it
            if(attack.AttackCost < attackToPerform.AttackCost){
                attackToPerform = attack;
            }
        }

        //double the attack cost of the chosen attack
        attackToPerform.AttackCost += attackToPerform.AttackCost;

        attackTokens -= attackToPerform.AttackCost; //subtract the cost from the tokens

        StartCoroutine(Attack(attackToPerform));
    }

    //continually increment the number of tokens the boss has
    private IEnumerator TokensOverTime(){
        while(true){
            attackTokens++;

            yield return new WaitForSeconds(1f);
        }
    }
}
