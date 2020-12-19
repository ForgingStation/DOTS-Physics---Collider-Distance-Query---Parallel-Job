using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;

public class ColliderCast_System : SystemBase
{
    protected override void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }

    /*[BurstCompile]
    public struct ColliderCastJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] public Collider* collider;
        public quaternion orientation;
        public float3 start;
        public float3 end;
        public NativeList<ColliderCastHit> colliderCastHits;
        public bool collectAllHits;
        [ReadOnly] public PhysicsWorld world;

        public void Execute()
        {
            ColliderCastInput colliderCastInput = new ColliderCastInput
            {
                Collider = collider,
                Orientation = orientation,
                Start = start,
                End = end
            };

            if (collectAllHits)
            {
                world.CastCollider(colliderCastInput, ref colliderCastHits);
            }
            else if (world.CastCollider(colliderCastInput, out ColliderCastHit hit))
            {
                colliderCastHits.Add(hit);
            }
        }
    }*/
}

