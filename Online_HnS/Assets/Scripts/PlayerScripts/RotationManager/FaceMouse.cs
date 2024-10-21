using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMouse : MonoBehaviour
{
    void LateUpdate()
    {
        FaceMouse3D();
    }

    void FaceMouse3D()
    {
        // Get the mouse position in screen space
        Vector3 mousePos = Input.mousePosition;

        // Convert the mouse position to a world point in 3D space (on a plane in front of the camera)
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // Define a plane that faces the camera
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            // Get the point where the ray hits the plane
            Vector3 targetPoint = ray.GetPoint(distance);

            // Make the object look towards that point
            Vector3 direction = new Vector3(targetPoint.x - transform.position.x, 0, targetPoint.z - transform.position.z);

            // Rotate the object to face the target direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
