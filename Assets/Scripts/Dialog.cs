using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Dialog : ScriptableObject
{
    public int DialogLevel;
    [Multiline]
    public string DialogText;
}
