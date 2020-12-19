using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using System.Collections.Generic;
using Unity.Physics;

public class Boid_Spawner : MonoBehaviour
{
    public int boidsPerInterval;
    public int boidsToSpawn;
    public float interval;
    public float cohesionBias;
    public float separationBias;
    public float alignmentBias;
    public float targetBias;
    public float perceptionRadius;
    public float3 target;
    public UnityEngine.Material material;
    public UnityEngine.Material material2;
    public List<Entity> debugEntities;
    public Mesh mesh;
    public float maxSpeed;
    public float step;
    public int cellSize;
    public float fieldOfView;
    public int maxPercived;
    public Mesh colliderMesh;
    public float maxObstacleDistace;
    public float colliderRadius;

    private EntityManager entitymanager;
    private Entity entity;
    private float elapsedTime;
    private int totalSpawnedBoids;
    private EntityArchetype ea;
    private float3 currentPosition;
    private BlobAssetReference<BoidManagerBLOB> boidManagerReference;
    private BlobAssetReference<Unity.Physics.Collider> col;

    private void Start()
    {
        totalSpawnedBoids = 0;
        entitymanager = World.DefaultGameObjectInjectionWorld.EntityManager;
        currentPosition = this.transform.position;
        ea = entitymanager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(Boid_ComponentData)
            );
        debugEntities = new List<Entity>();

        BlobBuilder bb = new BlobBuilder(Unity.Collections.Allocator.Temp);
        boidManagerReference = new BlobAssetReference<BoidManagerBLOB>();
        ref BoidManagerBLOB bmb = ref bb.ConstructRoot<BoidManagerBLOB>();
        BlobBuilderArray<Boid_Manager> blobManagerArray = bb.Allocate(ref bmb.blobManagerArray, 9);
        blobManagerArray[0] = new Boid_Manager
        {
            cohesionBias = cohesionBias,
            separationBias = separationBias,
            alignmentBias = alignmentBias,
            targetBias = targetBias,
            perceptionRadius = perceptionRadius,
            step = step,
            cellSize = cellSize,
            fieldOfView = fieldOfView,
            maxPercived = maxPercived,
            maxObstacleDistance = maxObstacleDistace
        };
        boidManagerReference = bb.CreateBlobAssetReference<BoidManagerBLOB>(Unity.Collections.Allocator.Persistent);
        bb.Dispose();
        if (colliderMesh != null)
        {
            col = CreateSphereCollider(colliderMesh);
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= interval)
        {
            elapsedTime = 0;
            for (int i = 0; i <= boidsPerInterval; i++)
            {
                if (totalSpawnedBoids == boidsToSpawn)
                {
                    break;
                }
                Entity e = entitymanager.CreateEntity(ea);
                entitymanager.AddComponentData(e, new Translation
                {
                    Value = currentPosition
                });
                entitymanager.AddComponentData(e, new Boid_ComponentData
                {
                    velocity = math.normalize(UnityEngine.Random.insideUnitSphere) * maxSpeed,
                    speed = maxSpeed,
                    target = target,
                    boidManagerReference = boidManagerReference,
                    colliderCast = col
                });
                entitymanager.AddSharedComponentData(e, new RenderMesh
                {
                    mesh = mesh,
                    material = material,
                    castShadows = UnityEngine.Rendering.ShadowCastingMode.On
                });
                totalSpawnedBoids++;
            }
        }
    }

    private BlobAssetReference<Unity.Physics.Collider> CreateSphereCollider(UnityEngine.Mesh mesh)
    {
        Bounds bounds = mesh.bounds;
        return Unity.Physics.SphereCollider.Create(new SphereGeometry
        {
            Center = bounds.center,
            Radius = colliderRadius
        });
    }

    private void OnDestroy()
    {
        boidManagerReference.Dispose();
    }
}
