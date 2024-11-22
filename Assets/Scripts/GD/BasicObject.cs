using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObject : MonoBehaviour
{
     private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    public bool movement = true;
    [SerializeField] public int match = 0;

    [SerializeField] private Vector3 minBounds = new Vector3(-16.5f, 0.1f, -10.5f);
    [SerializeField] private Vector3 maxBounds = new Vector3(16.5f, 5f, 10.5f);
    [SerializeField] private float minDistanceBetweenObjects = 2f;
    private static List<Vector3> usedPositions = new List<Vector3>();


    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is required for realistic physics.");
        }
        Vector3 randomPosition;
        do
        {
            randomPosition = new Vector3(
                Random.Range(-9f, maxBounds.x -1),
                1f, 
                Random.Range(minBounds.z +1 , maxBounds.z -1)
            );
        }
        while (!IsPositionValid(randomPosition)); 

       
        transform.position = randomPosition;
        usedPositions.Add(randomPosition);
        ResetObjectPhysics();

    }
    
    private bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistanceBetweenObjects)
            {
                return false; 
            }
        }
        return true; 
    }

    private void ResetObjectPhysics()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;  
            rb.Sleep(); 
        }
    }

    private void OnMouseDown()
    {   
        if(movement){
        isDragging = true;
        lastMousePosition = Input.mousePosition;
         }
    }

    private void OnMouseDrag()
    {
        if (isDragging && movement)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z));

            worldPosition.x = Mathf.Clamp(worldPosition.x, minBounds.x, maxBounds.x);
            worldPosition.y = Mathf.Clamp(worldPosition.y, minBounds.y, maxBounds.y);
            worldPosition.z = Mathf.Clamp(worldPosition.z, minBounds.z, maxBounds.z);

            transform.position = worldPosition; 
            lastMousePosition = mousePosition;
        }
    }

    private void OnMouseUp()
    {
        if (isDragging && movement)
        {
            isDragging = false;

            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;
            Vector3 force = new Vector3(mouseDelta.x, mouseDelta.y, 0f) * 0.1f; 
            rb.AddForce(force, ForceMode.Impulse); 
        }
    }

    public void LockMovement()
    {
        movement = false;
        rb.isKinematic = true;

    }

    public void UnlockMovement()
    {   
        movement = true;
        rb.isKinematic = false;
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse); 
    }

    private void FixedUpdate()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minBounds.z, maxBounds.z);
        transform.position = clampedPosition;
    }

        public void DestroyObject()
    {
        Destroy(gameObject); 
    }
}
