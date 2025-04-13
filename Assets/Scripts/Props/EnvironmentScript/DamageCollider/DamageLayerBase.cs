using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLayerBase : MonoBehaviour
{
    [Header("Reference")]
    // 用于储存多个层级的 LayerMask
    [SerializeField] private LayerMask targetLayers;

    [Header("Setting")]
    // 用于储存伤害数据
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float duringTime = 0.5f;

    // 储存当前在触发器中的对象
    private HashSet<GameObject> objectsInTrigger = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // 使用 LayerMask 检测物体是否在目标层级中
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            // 如果物体属于目标层级并且尚未添加到集合中
            if (!objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Add(other.gameObject);
                StartCoroutine(ApplyDamageOverTime(other.gameObject));
                Debug.Log($"物体 {other.gameObject.name} 进入触发器并开始受到伤害");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 当物体离开触发器，将其从集合中移除
        if (objectsInTrigger.Contains(other.gameObject))
        {
            objectsInTrigger.Remove(other.gameObject);
            Debug.Log($"物体 {other.gameObject.name} 离开触发器，不再受到伤害");
        }
    }

    private IEnumerator ApplyDamageOverTime(GameObject obj)
    {
        // 当物体仍在触发器中时持续给予伤害
        while (objectsInTrigger.Contains(obj))
        {
            PlayerController playerController = obj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.CauseDamage((int)damageAmount); // 对玩家造成伤害
                Debug.Log("地形正在给与玩家伤害！");
            }

            // 获取EnemyAI组件并修改health属性
            EnemyAI enemyAI = obj.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.health -= damageAmount;
            }

            yield return new WaitForSeconds(duringTime); // 每秒造成一次伤害
        }
    }
}
