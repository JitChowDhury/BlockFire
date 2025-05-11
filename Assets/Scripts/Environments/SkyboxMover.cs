using UnityEngine;

public class SkyboxMover : MonoBehaviour
{

    public float rotationSpeed = 1f; // Adjust the speed of rotation
   


    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }

}