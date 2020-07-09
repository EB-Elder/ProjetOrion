﻿using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerStatsData : IComponentData
{
    public float movementSpeed;
    public float rotationSpeed;
    public int Health;
    public bool hit;

}
