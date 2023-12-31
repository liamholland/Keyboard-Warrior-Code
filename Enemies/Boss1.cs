using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss1 : Enemy
{   
    [Header("-- Boss Attack Pool --")]
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
    private List<Attack> attacks = new List<Attack>();   //list of all the bosses attacks
    private List<Attack> affordableAttacks = new List<Attack>();    //temp list used to store the affordable attacks

    //delay object
    private static WaitForSeconds oneSecondDelay = new WaitForSeconds(1f);

    private void Start(){
        bossNameText.text = bossName;

        //set the costs of the attacks
        //base attack is not really part of the attack pool
        mainAttack.AttackCost = 0;

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
        Attack attackToPerform = mainAttack;

        if(Physics2D.OverlapCircle(transform.position, 50f, whatIsCable) == null){
            attacks.Remove(throwCableAttack);   //cannot do this attack when there are no cables
        }

        affordableAttacks.Clear();

        foreach(Attack a in attacks){
            if(a.AttackCost <= attackTokens){
                affordableAttacks.Add(a);
            }
        }

        if(affordableAttacks.Count > 0){
            //pick a random affordable attack
            attackToPerform = affordableAttacks[Random.Range(0, affordableAttacks.Count)];
        }

        attackTokens -= attackToPerform.AttackCost; //subtract the cost from the tokens

        StartCoroutine(Attack(attackToPerform));
    }

    //continually increment the number of tokens the boss has
    private IEnumerator TokensOverTime(){
        while(true){
            attackTokens++;

            yield return oneSecondDelay;
        }
    }
}
