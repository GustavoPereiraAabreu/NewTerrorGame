using UnityEngine;

public class OutlineDetector : MonoBehaviour
{
    [SerializeField] private float distance = 3f;

    private Outline currentOutline;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            Outline outline = hit.collider.GetComponentInParent<Outline>();

            if (outline != currentOutline)
            {
                if (currentOutline != null)
                    currentOutline.enabled = false;

                currentOutline = outline;

                if (currentOutline != null)
                    currentOutline.enabled = true;
            }

            // Interação só acontece se o Raycast acertou algo
            if (Input.GetKeyDown(KeyCode.E))
            {
                TVInteraction tv = hit.collider.GetComponentInParent<TVInteraction>();

                if (tv != null)
                {
                    tv.EnterTV();
                }
            }
        }
        else
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
        }
    }
}