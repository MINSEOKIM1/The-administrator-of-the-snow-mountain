using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerAttackSignal : MonoBehaviour
{
    public bool canAttack;
    public int consecutiveAttack;

    public void CanAttack()
    {
        canAttack = true;
    }

    public void SetCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }


}
