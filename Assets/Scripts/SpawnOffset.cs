using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpawnOffset : ScriptableObject
{
    [Range(0f, 0.5f)] public float Top;
    [Range(0f, 0.5f)] public float Right;
    [Range(0f, 0.5f)] public float Down;
    [Range(0f, 0.5f)] public float Left;
}
