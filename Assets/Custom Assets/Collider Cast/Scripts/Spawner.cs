using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class Spawner : MonoBehaviour
{
    public Mesh colliderMesh;
    public ColliderType colliderType;
    public Mesh prefab;
    public UnityEngine.Material material;
    public float interval;

    private EntityArchetype ea;
    private EntityManager entitymanager;
    private Entity entity;
    private float elapsedTime;
    private bool spawned;
    private BlobAssetReference<Unity.Physics.Collider> col;
    private NativeList<DistanceHit> DistanceHits;
    private PhysicsWorld pWorld;
    private ColliderDistanceInput ColliderDistanceInput;
    // Start is called before the first frame update
    void Start()
    {
        if (prefab != null)
        {
            col = CreateCollider(prefab, colliderType);
        }
        DistanceHits = new NativeList<DistanceHit>(Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {
        RunQueries();
    }

    private void RunQueries()
    {
        DistanceHits.Clear();
        pWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
        unsafe
        {
            float3 origin = transform.position;
            ColliderDistanceInput = new ColliderDistanceInput
            {
                Collider = (Unity.Physics.Collider*)(col.GetUnsafePtr()),
                Transform = new RigidTransform(transform.rotation, origin),
                MaxDistance = 10.0f
            };

            new ColliderDistanceJob
            {
                Transform = ColliderDistanceInput.Transform,
                MaxDistance = ColliderDistanceInput.MaxDistance,
                Collider = ColliderDistanceInput.Collider,
                DistanceHits = DistanceHits,
                CollectAllHits = false,
                World = pWorld,
            }.Schedule().Complete();
        }
        Debug.Log("DistanceHits>> " + DistanceHits.Length);
    }

    void OnDestroy()
    {
        if (DistanceHits.IsCreated)
        {
            DistanceHits.Dispose();
        }
    }

    private BlobAssetReference<Unity.Physics.Collider> CreateCollider(UnityEngine.Mesh mesh, ColliderType type)
    {
        switch (type)
        {
            case ColliderType.Sphere:
                {
                    Bounds bounds = mesh.bounds;
                    return Unity.Physics.SphereCollider.Create(new SphereGeometry
                    {
                        Center = bounds.center,
                        Radius = math.cmax(bounds.extents)
                    });
                }
            default:
                throw new System.NotImplementedException();
        }
    }

    [BurstCompile]
    public unsafe struct ColliderDistanceJob : IJob
    {
        public RigidTransform Transform;
        public float MaxDistance;
        [NativeDisableUnsafePtrRestriction] public Unity.Physics.Collider* Collider;
        public NativeList<DistanceHit> DistanceHits;
        public bool CollectAllHits;
        [ReadOnly] public PhysicsWorld World;

        public void Execute()
        {

            var colliderDistanceInput = new ColliderDistanceInput
            {
                Collider = Collider,
                Transform = Transform,
                MaxDistance = MaxDistance
            };

            if (CollectAllHits)
            {
                World.CalculateDistance(colliderDistanceInput, ref DistanceHits);
            }
            else if (World.CalculateDistance(colliderDistanceInput, out DistanceHit hit))
            {
                DistanceHits.Add(hit);
            }
        }
    }
}
