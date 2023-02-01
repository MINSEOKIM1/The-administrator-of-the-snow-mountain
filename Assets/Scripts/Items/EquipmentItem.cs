using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : Item
{
    public int equipmentPosition; 
    /*
     * 0 : hat
     * 1 : top
     * 2 : bottom
     * 3 : weapon
     */
    
    public float atk;
    public float def;
    public float hp;
    public float mp;
    public float stance;

    public float[] upgrades;
}
