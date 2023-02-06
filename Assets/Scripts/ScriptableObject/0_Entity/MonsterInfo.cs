using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterInfo", menuName = "ScriptableObjects/MonsterInfo", order = 2)]
public class MonsterInfo : EntityInfo
{
    public float stateChangeInterval;
    
    /*
     * Player Detect Boundary
     */
    public Vector2 boxSize;
    public Vector2 boxOffset;
    
    /*
     * Boundary to decide which kind of attack
     */
    public Vector2[] attackDetectBoxes;
    
    /*
     * Attack Boundary
     */
    public Vector2[] attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets;

    /*
     * Attack Info
     */
    public float[] attackCoefficient;
    public float[] attackStunTime;
    public Vector2[] attackKnockback;

    public ItemInfo[] dropItems;
}
