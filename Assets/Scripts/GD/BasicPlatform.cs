using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BasicPlatform : MonoBehaviour
{
    private BasicObject firstObject; 
    public Vector3 firstObjectPositionOffset = new Vector3(0f, 0f, 0f); 
    public Vector3 ejectPositionOffset = new Vector3(5f, 0f, 0f);
    public float forceMagnitude = 10f; 

    private List<BasicObject> activeObjects = new List<BasicObject>();

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<BasicObject>();

        if (obj == null || activeObjects.Contains(obj)) return;

        activeObjects.Add(obj);

        if (firstObject == null)
        {
            firstObject = obj;
            obj.LockMovement();
            PositionObject(obj, transform.position + firstObjectPositionOffset);
        }
        else
        {
            EjectExtraObject(obj);
        }
    }

    private void PositionObject(BasicObject obj, Vector3 position)
    {   
        Debug.Log($"Placing {obj.name} at {position}. Current Platform Position: {transform.position}");
        obj.transform.position = position; 
    }

   private void EjectExtraObject(BasicObject extraObject)
{
    Vector3 ejectPosition = transform.position + ejectPositionOffset;
    Rigidbody rb = extraObject.GetComponent<Rigidbody>();

    if (rb != null)
    {
        rb.isKinematic = false;

        Vector3 forceDirection = new Vector3(ejectPositionOffset.x, 0f, 0f).normalized;
        rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
    }

    extraObject.movement = false;

    StartCoroutine(HandleEjectedObject(extraObject, 2f)); 
    Debug.Log($"Ejecting {extraObject.name} to position: {ejectPosition}");
    activeObjects.Remove(extraObject);
}

private IEnumerator HandleEjectedObject(BasicObject obj, float duration)
{
    float elapsedTime = 0f;
    Renderer objRenderer = obj.GetComponent<Renderer>();
    bool isVisible = true;

    while (elapsedTime < duration)
    {
        isVisible = !isVisible;

        if (objRenderer != null)
        {
            objRenderer.enabled = isVisible; 
        }

        elapsedTime += 0.2f; 
        yield return new WaitForSeconds(0.2f);
    }

    if (objRenderer != null)
    {
        objRenderer.enabled = true;
    }

    obj.movement = true;
}
public void ReleaseFirstObject() 
{
    Vector3 releaseOffset = new Vector3(4f, 0f, 0f); 
    if (firstObject != null)
    {
        

        Debug.Log($"Releasing {firstObject.name} from platform.");

        firstObject.UnlockMovement();

        firstObject.transform.position = transform.position + releaseOffset;
        EjectExtraObject(firstObject);

        activeObjects.Remove(firstObject);

        firstObject = null;
    }
}
}
