using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    [SerializeField] private Entity entity;
    

    public virtual void Awake()
    {
        entity = GetComponent<Entity>();
        
    }

    public virtual void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();
    }
}
