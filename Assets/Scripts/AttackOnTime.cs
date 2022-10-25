using UnityEngine;

public class AttackOnTime : MonoBehaviour
{
    private FollowerManager followerManager;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public float attackRate = 0.5f;
    private float nextAttackTime = 0f;

    private void Start()
    {
        followerManager = GetComponentInParent<FollowerManager>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack(attackDamage);
            nextAttackTime = Time.time + attackRate;
        }
    }

    public void Attack(int damage)
    {
        Collider[] hitEnemy = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemy)
        {
            //if (enemy.tag != "Follower")
            //if (followerManager.currentState == FollowerManager.State.Attack && followerManager.enemyTarget != null && enemy.name == followerManager.enemyTarget.name)
            if (followerManager.enemyTarget != null && enemy.name == followerManager.enemyTarget.name)
            {
                Debug.Log(this.name + " hit " + enemy.name);
                enemy.GetComponent<HitPointsManager>().RegisterHit(damage);

                if (followerManager.enemyTarget.GetComponent<HitPointsManager>().currentHitPoints <= 0)
                {
                    followerManager.ResolveKill();
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
