using UnityEngine;
namespace FPS.Weapon
{
    [CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Prototypes/CharacterStats", order = 1)]
    public class WEAPONSO : ScriptableObject
    {
        public float Damage;
        public float Range;
        public float FireRate;

        public bool isAutomatic;
    }
}
