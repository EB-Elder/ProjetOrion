using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayerStatsData : IComponentData
{
    public float movementSpeed;
    public int Health;
    public bool hit;

}
