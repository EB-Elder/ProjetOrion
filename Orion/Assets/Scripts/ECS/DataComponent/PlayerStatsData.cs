using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerStatsData : IComponentData
{
    public float movementSpeed;
    public int Health;
    public bool hit;

}
