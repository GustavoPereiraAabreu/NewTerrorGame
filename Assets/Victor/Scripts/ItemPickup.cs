using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;
    [HideInInspector] public Outline outline;

    public bool isHeld;

    [Header("ID do Item")]
    public string itemID;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        outline = GetComponent<Outline>();
    }
}