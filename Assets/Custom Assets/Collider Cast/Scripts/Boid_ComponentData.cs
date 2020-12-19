using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct Boid_ComponentData : IComponentData
{
    public float speed;
    public float3 currentPosition;
    public float3 velocity;
    public float3 acceleration;
    public float3 target;
    public BlobAssetReference<BoidManagerBLOB> boidManagerReference;
    public float3 obstacleAvoidance;
    public BlobAssetReference<Collider> colliderCast;

    public float debug;
}
