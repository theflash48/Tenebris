using UnityEngine;

/// <summary>
/// Ajusta el tamaño del CapsuleCollider para simular el tamaño agachado o tamaño normal.
///
/// Mantiene los pies del player en el mismo sitio e evitar que se levante si hay techo. 
/// </summary>

[RequireComponent(typeof(CapsuleCollider))]
public class CapsuleResizer : MonoBehaviour
{
    [SerializeField] CapsuleCollider capsule;
    [SerializeField] PlayerStats stats;
    
    public bool IsCrouching { get; set; }

    void Reset()
    {
        capsule = GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// Cambia entre AGACHADO y NORMAL
    ///     - Si el jugador intenta levantarse y no hay espacio, no cambia
    ///     - Ajusta el Collider y actualiza:   IsCrouching
    /// </summary>
    public void SetCrouch(bool crouch)
    {
        if (crouch == IsCrouching) return;

        if (!crouch)
        {
            //Intenta levantarse. Si hay techo no lo hace
            if (!CanStandUp()) return;
        }
        
        ApplySize(crouch ? stats.crouching : stats.standing);
        IsCrouching = crouch;
    }
    
    /// <summary>
    /// Comprueba si podemos volver al tamaño NORMAL sin chocar con nada.
    ///     - Calcula donde estaria el collider "NORMAL"
    ///     - Lanza un Physics.OverlapCapsule con ese tamaño
    ///     - Si detecta colisiones, no cabe
    /// </summary>
    bool CanStandUp()
    {
        if (stats == null) return true;
        
        //Guardamos el "bottom" para no mover los pies
        Vector3 oldBottom = GetBottomWorld(capsule);
        
        //Simulamos nuevo tamaño
        var stand = stats.standing;
        
        Vector3 newBottomToCenter = Vector3.up * ((stand.height * 0.5f) - stand.radius);
        Vector3 newCenterWorld = oldBottom + newBottomToCenter;

        //Puntos extremos de la capsula
        Vector3 p1 = newCenterWorld + Vector3.up * ((stand.height * 0.5f) - stand.radius);
        Vector3 p2 = newCenterWorld - Vector3.up * ((stand.height * 0.5f) - stand.radius);
        float radius = stand.radius;
        
        //OverlapCapsule para ver si cabemos
        Collider[] hits = Physics.OverlapCapsule(
            p1, p2, radius,
            stats.collision.collisionMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hits.Length; i++)
        {
            //Si tocamos cualquier collider == no puede levantarse
            if (hits[i] != null && hits[i] != capsule) return false;
        }
        return true;
    }

    /// <summary>
    /// Aplica el PlayerStats.CapsuleSize al CapsuleCollider y compesa el transform para mantener el "bottom" constante
    /// </summary>
    void ApplySize(PlayerStats.CapsuleSize size)
    {
        if (capsule == null) return;

        //Mantener pies en el mismo sitio
        Vector3 oldBottom = GetBottomWorld(capsule);

        capsule.height = size.height;
        capsule.radius = size.radius;
        capsule.center = size.center;

        Vector3 newBottom = GetBottomWorld(capsule);
        transform.position += (oldBottom - newBottom);
    }
    
    /// <summary>
    /// Devuelve el punto mas bajo del CapsuleCollider en Y
    /// Sirve para anclar el collider a los pies al cambiar de tamaño
    /// </summary>
    static Vector3 GetBottomWorld(CapsuleCollider c)
    {
        //Asumimos dirección Y (lo normal)
        Vector3 centerW = c.transform.TransformPoint(c.center);
        float half = Mathf.Max(0f, (c.height * 0.5f) - c.radius);
        return centerW - c.transform.up * half;
    }
}
