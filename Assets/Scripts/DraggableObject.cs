using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    [SerializeField] public int match = 0;


    // Oyun alanı sınırları
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
    }

    private void OnMouseDown()
    {
        isDragging = true;
        lastMousePosition = Input.mousePosition; // İlk mouse pozisyonunu kaydet
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z));

            // Pozisyonu oyun alanı sınırlarına sıkıştır
            worldPosition.x = Mathf.Clamp(worldPosition.x, minBounds.x, maxBounds.x);
            worldPosition.y = Mathf.Clamp(worldPosition.y, minBounds.y, maxBounds.y);
            worldPosition.z = Mathf.Clamp(worldPosition.z, minBounds.z, maxBounds.z);

            transform.position = worldPosition; // Objeyi mouse konumuna taşır
            lastMousePosition = mousePosition;
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;

            // Fare hareketinden kaynaklanan kuvveti hesapla
            Vector3 force = new Vector3(mouseDelta.x, mouseDelta.y, 0f) * 0.1f; // Kuvvet ölçeği ayarlanabilir
            rb.AddForce(force, ForceMode.Impulse); // Kuvvet uygula
        }
    }

    public void LockMovement()
    {
        rb.isKinematic = true; // Objenin hareketini kilitler
    }

    public void UnlockMovement()
    {
        rb.isKinematic = false; // Objenin hareketini serbest bırakır
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse); // İvme ekler
    }

    private void FixedUpdate()
    {
        // Oyun alanı sınırlarını kontrol et ve objeyi içeride tut
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minBounds.z, maxBounds.z);
        transform.position = clampedPosition;
    }
}
