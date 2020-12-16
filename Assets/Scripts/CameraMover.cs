using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private bool isActive = false;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 currentRotation = new Vector3(0, 0, 0);

    public void OnTerrainGeneration(Vector3 startPoint, Vector3 endPoint)
    {
        this.startPoint = startPoint - new Vector3(5, 5, 5);
        this.endPoint = endPoint + new Vector3(5, 20, 5);
        Camera.main.transform.position = ((endPoint + startPoint) / 2);
        isActive = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        };

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            currentRotation.x += Input.GetAxis("Mouse X");
            currentRotation.y -= Input.GetAxis("Mouse Y");
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);

            if (isActive)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 0.05f);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    Camera.main.transform.position = Camera.main.transform.position - (Camera.main.transform.forward * 0.05f);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.right * 0.05f);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    Camera.main.transform.position = Camera.main.transform.position - (Camera.main.transform.right * 0.05f);
                }

                if (Input.GetKey(KeyCode.C))
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0.1f, 0);
                }
                else if (Input.GetKey(KeyCode.V))
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, -0.1f, 0);
                }

                Vector3 safeVector = Camera.main.transform.position;

                if (safeVector.x > endPoint.x)
                {
                    Camera.main.transform.position = new Vector3(endPoint.x, safeVector.y, safeVector.z);
                }
                else if (safeVector.x < startPoint.x)
                {
                    Camera.main.transform.position = new Vector3(startPoint.x, safeVector.y, safeVector.z);
                }

                if (safeVector.y > endPoint.y)
                {
                    Camera.main.transform.position = new Vector3(safeVector.x, endPoint.y, safeVector.z);
                }
                else if (safeVector.y < startPoint.y)
                {
                    Camera.main.transform.position = new Vector3(safeVector.x, startPoint.y, safeVector.z);
                }

                if (safeVector.z > endPoint.z)
                {
                    Camera.main.transform.position = new Vector3(safeVector.x, safeVector.y, endPoint.z);
                }
                else if (safeVector.z < startPoint.z)
                {
                    Camera.main.transform.position = new Vector3(safeVector.x, safeVector.y, startPoint.z);
                }
            }
        }       
    }
}
