using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private DraggableObject firstObject;
    private DraggableObject secondObject;
    public float forceMagnitude = 1;

    private List<DraggableObject> activeObjects = new List<DraggableObject>();
    private bool isProcessing = false;

    public Vector3 firstObjectPositionOffset = new Vector3(-1.5f, 0f, 0f); 
    public Vector3 secondObjectPositionOffset = new Vector3(1.5f, 0f, 0f);
    public Vector3 ejectPositionOffset = new Vector3(0f, 3f, 0f); 

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<DraggableObject>();

        if (obj == null || activeObjects.Contains(obj)) return;

        if (isProcessing || (firstObject != null && secondObject != null))
        {
            EjectExtraObject(obj); 
            return;
        }

        activeObjects.Add(obj); 

        if (firstObject == null)
        {
            firstObject = obj;
            obj.LockMovement();
            PositionObject(obj, transform.position + firstObjectPositionOffset);
        }
        else if (secondObject == null)
        {
            secondObject = obj;
            obj.LockMovement();
            PositionObject(obj, transform.position + secondObjectPositionOffset);

            StartCoroutine(ProcessObjects()); 
        }
    }

    private void PositionObject(DraggableObject obj, Vector3 position)
    {   
        Debug.Log($"Placing {obj.name} at {position}. Current Platform Position: {transform.position}");
        obj.transform.position = position; 
    }

    private void EjectExtraObject(DraggableObject extraObject)
    {
        Vector3 ejectPosition = transform.position + ejectPositionOffset;
        extraObject.transform.position = ejectPosition; 
        Rigidbody rb = extraObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Impulse); 
        }
        Debug.Log($"Ejecting {extraObject.name} to position: {ejectPosition}");
    }

    private IEnumerator ProcessObjects()
    {
        isProcessing = true; 
        yield return new WaitForSeconds(0.5f);

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
       Vector3 moveOffset = new Vector3(0, 3f, 0); 

       Vector3 firstTarget = firstObject.transform.position + moveOffset;
       Vector3 secondTarget = secondObject.transform.position + moveOffset;

       float elapsed = 0f;

       while (elapsed < duration)
       {
           elapsed += Time.deltaTime;
           float t = elapsed / duration;

           if (firstObject != null)
               firstObject.transform.position = Vector3.Lerp(firstObject.transform.position, firstTarget, t);

           if (secondObject != null)
               secondObject.transform.position = Vector3.Lerp(secondObject.transform.position, secondTarget, t);
       
           yield return null; 
       }

       UIManager.Instance.AddScore(10);
       if (firstObject != null)
       {
           firstObject.DestroyObject(); 
       }

       if (secondObject != null)
       {
           secondObject.DestroyObject(); 
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
                firstObject.UnlockMovement(); 
                rb.AddForce(-ejectDirection * forceMagnitude, ForceMode.Impulse);
            }
        }

        if (secondObject != null)
        {
            Rigidbody rb = secondObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                secondObject.UnlockMovement();
                rb.AddForce(ejectDirection * forceMagnitude, ForceMode.Impulse); 
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
