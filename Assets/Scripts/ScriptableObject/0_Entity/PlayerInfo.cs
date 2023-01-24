using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "ScriptableObjects/PlayerInfo", order = 3)]
public class PlayerInfo : EntityInfo
{
    /*
     * Attack Skill index-meaning
     * 0 : down bash        (normal attack 1)
     * 1 : upper bash       (normal attack 2)
     * 2 : turn smash       (normal attack 3)
     * 3 : dash slash
     * 4 : counter attack   (parrying)
     */
    public Vector2[] attackKnockback;
    public Vector2[] attackBoundary;
    public Vector2[] attackOffset;
    public float[] attackCoefficient;
    public float[] attackStunTime;
    public float[] attackMp;

    /*
     * Util Skill index-meaning
     * 0 : backStep
     * 1 : wall-climbing
     * 2 : dash
     */
    public float[] utilMp;
}