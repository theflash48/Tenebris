using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player/Player Stats", menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Serializable]
    public struct MovementStats
    {
        [Header("Movement")]
        public float maxSpeed;
        public float acceleration;
        public float deceleration;
        [Range(0f, 1f)] public float airControl;
        
        [Header("Crouch")]
        [Range(0.1f, 1f)] public float crouchSpeedMultiplier; //Multiplicador de maxSpeed para cuando va agachado
    }

    [Serializable]
    public struct JumpStats
    {
        [Header("Jump")]
        public float jumpHeight;
        public float coyoteTime; //Despues de saltar
        public float bufferTime; //Antes de tocar el suelo y se ejecute al aterrizar
    }

    [Serializable]
    public struct GravityStats
    {
        [Header("Gravity")]
        public float gravity; //Gravedad (m/s^2)
        public float massKg; //kg
        public float terminalSpeed; //Velocidad Maxima de Caida (m/s)
    }
    
    [Serializable]
    public struct CollisionStats
    {
        [Header("Collisions")]
        public LayerMask collisionMask; //Layers con la que el player colisiona
        public float skinWidth; //Margen para no pegarse al suelo/pared
        
        [Header("Ground")]
        [Range(0f, 89f)] public float maxSlopeAngle; //Angulo maximo para pendiente caminable
        public float groundProbeDistance; //Distancia del CapsuleCast para detectar suelo
        public float groundSnapDistance; //Distancia extra para snapear al suelo

        [Header("Step Up")]
        public float stepHeight; //Altura Maxima de Escalones que puede subir
        
        [Header("Step Down")]
        public float maxSnapDownSpeed; //Velocidad Maxima para bajar escalones (m/s)
        
        [Header("Stability")]
        public float stationarySnapDownDistance; //Snap Estatico
        public float maxDepenetrationDistance; //Maximo empuje para evitar teletransporte
        
        [Header("Solver")]
        public int maxBounces; //Maximo de rebotes por frame
        public int maxDepenetrationIterations;

        [Header("Push Rigidbodies")]
        public bool enableRigidbodyPush;
        public float pushMultiplier;
    }
    
    [Serializable]
    public struct CapsuleSize
    {
        public float height;
        public float radius;
        public Vector3 center;
    }
    
    // ---- VALORES BASE ----
    
    public MovementStats movement = new MovementStats
    {
        maxSpeed = 4.5f,
        acceleration = 25f,
        deceleration = 30f,
        airControl = 0.35f,
        crouchSpeedMultiplier = 0.6f
    };
    
    public JumpStats jump = new JumpStats
    {
        jumpHeight = 1.2f,
        coyoteTime = 0.12f,
        bufferTime = 0.12f
    };

    public GravityStats gravity = new GravityStats
    {
        gravity = 9.81f,
        massKg = 70f,
        terminalSpeed = 55f
    };

    public CollisionStats collision = new CollisionStats
    {
        collisionMask = ~0,
        skinWidth = 0.02f,
        maxSlopeAngle = 55f,
        groundProbeDistance = 0.25f,
        groundSnapDistance = 0.20f,
        stepHeight = 0.3f,
        maxSnapDownSpeed = 10f,
        stationarySnapDownDistance = 0.08f,
        maxDepenetrationDistance = 0.15f,
        maxBounces = 5,
        maxDepenetrationIterations = 4,
        enableRigidbodyPush = false,
        pushMultiplier = 1f
    };

    [Header("Capsule Sizes")]
    public CapsuleSize standing = new CapsuleSize
    {
        height = 1.8f,
        radius = 0.3f,
        center = new Vector3(0f, 0.9f, 0f)
    };

    public CapsuleSize crouching = new CapsuleSize
    {
        height = 1.2f,
        radius = 0.3f,
        center = new Vector3(0f, 0.6f, 0f)
    };
}
