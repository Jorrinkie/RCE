using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{

    public Transform InteractorSource;
    public float InteractionRange = 3f;
    void Start()
    {
            InteractorSource = Camera.main.transform;
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractionRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactableObj))
                {
                    interactableObj.Interact();
                }
            }
        }
    }
}
