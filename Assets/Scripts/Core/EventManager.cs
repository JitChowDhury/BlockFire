using UnityEngine;
using UnityEngine.Events;
namespace FPS.Core
{
    public static class EventManager
    {
        public static event UnityAction<float> OnEnemyDamage;


        public static void RaiseOnEnemyDamage(float damage) => OnEnemyDamage?.Invoke(damage);
    }
}
