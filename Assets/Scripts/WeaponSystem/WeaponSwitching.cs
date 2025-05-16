using UnityEngine;
namespace FPS.Weapon
{
    public class WeaponSwitching : MonoBehaviour
    {
        public int selectedWeapon = 1;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SelectWeapon();
        }

        // Update is called once per frame
        void Update()
        {

            int previousSelectedWeapon = selectedWeapon;
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedWeapon >= transform.childCount - 1)
                    selectedWeapon = 0;
                else
                    selectedWeapon++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedWeapon <= 0)
                    selectedWeapon = transform.childCount - 1;
                else
                    selectedWeapon--;
            }
            if (previousSelectedWeapon != selectedWeapon)
            {
                SelectWeapon();
            }
        }

        void SelectWeapon()
        {
            int i = 0;
            foreach (Transform weapon in transform)
            {
                bool isSelected = (i == selectedWeapon);
                weapon.gameObject.SetActive(isSelected);

                if (isSelected)
                {
                    // Tell UIManager which weapon is active
                    Gun gun = weapon.GetComponent<Gun>();
                    FindFirstObjectByType<WeaponUIManager>().SetGun(gun);
                }
                i++;
            }
        }
    }
}
