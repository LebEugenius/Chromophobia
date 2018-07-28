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
    public int StartDisguiseColoredsCount = 1;
    public int StartPanicCapacity = 5;

    [Header("Settings")]
    public float MinSpeed = 1;
    public float MaxSpeed = 2;

    public float MinSize = 1;
    public float MaxSize = 2;

    public float MinDisquiseTime = 0.5f;
    public float MaxDisquiseTime = 2f;

    public float PanicPenalty = 0.5f;

    [Header("Rates")]
    public int NewColoredsRate = 2;
    public int AgroColoredsRate = 5;
    public int MovingColoredsRate = 5;
    public int DisquiseColoredsRate = 5;
    public int PanicCapacityBonusRate = 10;

    [Header("Bonuses")]
    public float PanicCapacityBonus = 0.1f;

    [Header("Levels")]
    public int TutorialLength = 5;
    public int MovingColoredLevel = 15;
    public int DisquiseColoredsLevel = 30;
}
