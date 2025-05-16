using TMPro;
using UnityEngine;

namespace FPS.Weapon
{
    public class WeaponUIManager : MonoBehaviour
    {
        public TextMeshProUGUI ammoText;

        private Gun currentGun;

        void Update()
        {
            if (currentGun != null)
            {
                ammoText.text = $"Ammo: {currentGun.GetCurrentAmmo()} / {currentGun.GetCurrentMag()}";
            }
        }

        public void SetGun(Gun gun)
        {
            currentGun = gun;
        }
    }
}
