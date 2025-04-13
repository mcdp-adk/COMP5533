using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class MinigunFire : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private ParticleSystem CollisionFx;

    [Header("Setting")]
    [SerializeField] private float LifeTime = 1.5f;
    [SerializeField] private int damage = 10;

    [Header("Parm")]
    [SerializeField] private Vector3 RotationOffset = new Vector3(90, 0, 0);

    private ParticleSystem _mainPs;
    private readonly List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();

    private void Awake()
    {
        _mainPs = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        visionEffect(other);
        damageEffect(other);
    }

    private void visionEffect(GameObject other)
    {
        int collisionEventsCount = _mainPs.GetCollisionEvents(other, _collisionEvents);

        if (collisionEventsCount <= 0)
            return;

        var collisionFx = Instantiate(CollisionFx, _collisionEvents[0].intersection, Quaternion.identity);

        collisionFx.transform.LookAt(_collisionEvents[0].intersection + _collisionEvents[0].normal);
        collisionFx.transform.rotation *= Quaternion.Euler(RotationOffset);

        Destroy(collisionFx.gameObject, LifeTime);
    }

    private void damageEffect(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.CauseDamage((int)damage); // 对玩家造成伤害
        }

        // 获取EnemyAI组件并修改health属性
        EnemyAI enemyAI = other.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.health -= damage;
            //Debug.Log($"对带有标签 {tag} 的对象 {hitCollider.gameObject.name} 造成 {explosionHealthCare} 点伤害，剩余生命值：{enemyAI.health}");
        }
    }
}
