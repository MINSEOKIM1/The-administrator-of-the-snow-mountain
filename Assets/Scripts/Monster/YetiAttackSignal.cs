using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class YetiAttackSignal : MonoBehaviour
{
    public bool canAttack;
    public int consecutiveAttack;
    
    [SerializeField] private Monster _monster;
    [SerializeField] private CinemachineImpulseSource ctx;

    public float dashPower;

    public bool isRoll;

    public void CanAttack()
    {
        canAttack = true;
    }

    public void SetCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    public void ResetConsecutiveAttack()
    {
        consecutiveAttack = 0;
    }

    public void ConsecutiveAttackIncrement()
    {
        consecutiveAttack++;
    }

    public void Dash()
    {
        _monster.dashSpeed = dashPower * transform.localScale.x;
        Debug.Log(dashPower * transform.localPosition.x);
    }

    public void CameraImpulse()
    {
        ctx.GenerateImpulse();
    }

    public void ImpulseToGround()
    {
        if (_monster._target.GetComponent<PlayerBehavior>()._isGround)
        {
            var c = ((Yeti)_monster)._monsterInfo;
            var k = ((Yeti)_monster)._monsterInfo.attackKnockback[3];
            _monster._target.GetComponent<PlayerBehavior>().GetComponent<PlayerBehavior>().Hit(
                c.atk * c.attackCoefficient[3],
                (k), 
                c.attackStunTime[3],
                transform.parent.position);
        }
    }

    public void IcicleFall()
    { 
        ((Yeti)_monster).IcicleFall();
    }
    
    public void Roll()
    {
        isRoll = true;
    }
}
