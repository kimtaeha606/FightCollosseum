using UnityEngine;

public class AttackAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private Gladius gladius;

    private void Awake()
    {
        if (gladius == null)
        {
            gladius = GetComponentInChildren<Gladius>(true);
        }
    }

    // Bind this function in the Animation Event on Armature|Action.
    public void OnAttackEnd()
    {
        if (gladius == null)
        {
            gladius = GetComponentInChildren<Gladius>(true);
        }

        gladius?.OnAttackEnd();
    }
}
