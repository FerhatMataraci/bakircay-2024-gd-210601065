using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    public bool movement = true;
    [SerializeField] public int match = 0;

    [SerializeField] private Vector3 minBounds = new Vector3(-16.5f, 0.1f, -10.5f);
    [SerializeField] private Vector3 maxBounds = new Vector3(16.5f, 5f, 10.5f);


    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is required for realistic physics.");
        }
        LevelManager.Instance.RegisterObject(this);
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
        LevelManager.Instance.RemoveObject(this); 
        Destroy(gameObject); 
    }
}
