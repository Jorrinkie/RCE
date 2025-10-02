using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Objects to Check")]
    [SerializeField] private GameObject obj1;
    [SerializeField] private GameObject obj2;
    [SerializeField] private GameObject obj3;
    [SerializeField] private GameObject obj4;

    [Header("Teleport Target")]
    [SerializeField] private Transform teleportPoint;

    [Header("Player Controller")]
    [SerializeField] private MonoBehaviour playerController; // drag your PlayerController here

    [SerializeField] private string sceneName;

    //private bool hasTeleported = false;

    private void Update()
    {
        
        
            // Auto teleport if any object becomes inactive
            if ((obj1 != null && !obj1.activeInHierarchy) ||
                (obj2 != null && !obj2.activeInHierarchy) ||
                (obj3 != null && !obj3.activeInHierarchy) ||
                (obj4 != null && !obj4.activeInHierarchy))
            {
                Teleport();
            }
        

        // Manual teleport test
        if (Input.GetKeyDown(KeyCode.T))
        {
            Teleport();
        }
    }

    private void Teleport()
    {

        /*
        if (playerController != null)
        {
            playerController.enabled = false; // disable the controller before teleporting
        }

        if (teleportPoint != null)
        {
            transform.position = teleportPoint.position;
            transform.rotation = teleportPoint.rotation; // optional: match rotation

            // Reset X rotation to 0 while keeping Y and Z rotations
            Vector3 euler = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, euler.y, euler.z);

            //hasTeleported = true;
            Debug.Log("Teleported " + gameObject.name + " to " + teleportPoint.position);
        }

        if (playerController != null)
        {
            playerController.enabled = true; // disable the controller before teleporting
        }
        */


        SceneManager.LoadScene(sceneName);
    }



}

