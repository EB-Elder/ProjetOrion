using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MovementData : IComponentData
{
    public float movementSpeed;
    public float rotationSpeed;
    public float3 startPosition;

    public bool goingUp;
    public bool goingDown;

    public KeyCode up;
}
