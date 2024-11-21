using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private DraggableObject firstObject;
    private DraggableObject secondObject;
    public float forceMagnitude = 1;

    private List<DraggableObject> activeObjects = new List<DraggableObject>();
    private bool isProcessing = false; // Platform işleme yapıyorsa yeni tetiklemeleri engeller

    public Vector3 firstObjectPositionOffset = new Vector3(-1.5f, 0f, 0f); // İlk objenin pozisyonu
    public Vector3 secondObjectPositionOffset = new Vector3(1.5f, 0f, 0f); // İkinci objenin pozisyonu
    public Vector3 ejectPositionOffset = new Vector3(0f, 3f, 0f); // Fazla objenin ışınlanacağı uzak pozisyon

    private void OnTriggerEnter(Collider other)
    {
        // Eğer platform işleme yapıyorsa yeni nesneleri kabul etme
        if (isProcessing) return;

        // DraggableObject türünde mi kontrol et
        var draggable = other.GetComponent<DraggableObject>();
        if (draggable != null && !activeObjects.Contains(draggable))
        {
            activeObjects.Add(draggable); // Yeni nesneyi listeye ekle

            // İlk ve ikinci nesneleri belirle
            if (firstObject == null)
            {
                firstObject = draggable;
                draggable.LockMovement();
                PositionObject(draggable, transform.position + firstObjectPositionOffset); // İlk pozisyona yerleştir
            }
            else if (secondObject == null)
            {
                secondObject = draggable;
                draggable.LockMovement();
                PositionObject(draggable, transform.position + secondObjectPositionOffset); // İkinci pozisyona yerleştir

                // İki nesne tamamlandığında işlem başlat
                StartCoroutine(ProcessObjects());
            }
            else
            {
                // Üçüncü nesne platforma girmeye çalışırsa onu platformdan uzağa ışınla
                EjectExtraObject(draggable);
            }
        }
    }

    private void PositionObject(DraggableObject obj, Vector3 position)
    {
        obj.transform.position = position; // Nesneyi belirtilen pozisyona taşı
    }

    private void EjectExtraObject(DraggableObject extraObject)
    {
        Vector3 ejectPosition = transform.position + ejectPositionOffset; // Platformdan uzağa bir pozisyon belirle
        extraObject.transform.position = ejectPosition; // Nesneyi ışınla
        Rigidbody rb = extraObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Impulse); // Yukarıya doğru hafif kuvvet uygula
        }
    }

    private IEnumerator ProcessObjects()
    {
        if (isProcessing) yield break; // Platform zaten işleme yapıyorsa çık
        isProcessing = true;

        yield return new WaitForSeconds(0.5f); // İşlem öncesi gecikme

        if (firstObject != null && secondObject != null)
        {
            if (firstObject.match == secondObject.match)
            {
                yield return StartCoroutine(DestroyMatchedObjects());
            }
            else
            {
                yield return StartCoroutine(EjectObjects());
            }
        }

        ResetPlatform();
        isProcessing = false;
    }

    private IEnumerator DestroyMatchedObjects()
    {
       float duration = 1f;
    Vector3 moveOffset = new Vector3(0, 3f, 0); // Yukarı doğru hareket miktarı

    // Hedef pozisyonlar
    Vector3 firstTarget = firstObject.transform.position + moveOffset;
    Vector3 secondTarget = secondObject.transform.position + moveOffset;

    float elapsed = 0f;

    // Yukarı hareketin gerçekleştirildiği döngü
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        if (firstObject != null)
            firstObject.transform.position = Vector3.Lerp(firstObject.transform.position, firstTarget, t);

        if (secondObject != null)
            secondObject.transform.position = Vector3.Lerp(secondObject.transform.position, secondTarget, t);
    
        yield return null; // Bir sonraki kareye geç
    }
    UIManager.Instance.AddScore(10);
    // Nesneleri yok etme
    if (firstObject != null)
    {
        firstObject.DestroyObject(); // DraggableObject sınıfını kullanarak yok et
    }

    if (secondObject != null)
    {
        secondObject.DestroyObject(); // DraggableObject sınıfını kullanarak yok et
    }
    }

    private IEnumerator EjectObjects()
    {
        Vector3 ejectDirection = new Vector3(0, 0, 1);

        if (firstObject != null)
        {
            Rigidbody rb = firstObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Fiziksel hareketi etkinleştir
                rb.AddForce(ejectDirection * forceMagnitude, ForceMode.Impulse); // Kuvvet uygula
            }
        }

        if (secondObject != null)
        {
            Rigidbody rb = secondObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Fiziksel hareketi etkinleştir
                rb.AddForce(-ejectDirection * forceMagnitude, ForceMode.Impulse); // Zıt yöne kuvvet uygula
            }
        }

        yield return new WaitForSeconds(1f);
    }

    private void ResetPlatform()
    {
        firstObject = null;
        secondObject = null;
        activeObjects.Clear();
    }
}
