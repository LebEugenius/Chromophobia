using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    [Header("Start values")]
    public int StartColoredsCount = 3;
    public int StartAgroColoredsCount = 1;
    public int StartMovingColoredsCount = 2;
    public int StartPanicCapacity = 5;


    [Header("Settings")]
    public float minSpeed;
    public float maxSpeed;

    public float minSize;
    public float maxSize;

    [Header("Rates")]
    public int NewColoredsRate = 2;
    public int AgroColoredsRate = 5;
    public int MovingColoredsRate = 5;
    public int PanicCapacityBonusRate = 10;

    [Header("Bonuses")]
    public float PanicCapacityBonus = 0.1f;

    [Header("Levels")]
    public int TutorialLength = 5;
    public int MovingColoredLevel = 15;
}
