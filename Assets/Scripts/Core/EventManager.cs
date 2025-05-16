using UnityEngine;
using UnityEngine.Events;
namespace FPS.Core
{
    public static class EventManager
    {
        public static event UnityAction<Enemy, float> OnEnemyDamage;
        public static event UnityAction<float> OnPlayerDamage;


        public static void RaiseOnEnemyDamage(Enemy enemy, float damage) => OnEnemyDamage?.Invoke(enemy, damage);
        public static void RaiseOnPlayerDamage(float damageAmount) => OnPlayerDamage?.Invoke(damageAmount);
    }
}
