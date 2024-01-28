using UnityEngine;

public class TargetArrow : MonoBehaviour
{

    public Transform Target;
    public float HideDistance = 4f;
    private Transform parent;
    private Transform spriteTransform;
    private Vector3 initialArrowPos;
    private FixableObject targetObject;

    public void Setup(FixableObject target)
    {
        parent = transform.parent;
        spriteTransform = transform.GetChild(0);
        initialArrowPos = spriteTransform.localPosition;
        targetObject = target;
        Target = target.transform;
        spriteTransform.GetComponent<SpriteRenderer>().color = target.arrowColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null) return;
        if (!targetObject.IsCurrentlyActive()) // no point in pointing if there's no danger
        {
            Destroy(gameObject);
        }
        // TODO - could be improved if we call it directly from player probably?
        if (parent.localScale.x != transform.localScale.x)
        {
            transform.localScale = parent.localScale;
        }
        var dir = Target.position - transform.position;
        if (dir.magnitude < HideDistance)
        {
            SetChildrenActive(false);
        }
        else
        {
            SetChildrenActive(true);
            var angle = -Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // make the direction somewhat correct when left side
            if (angle < -100f)
            {
                spriteTransform.localPosition = new Vector3(initialArrowPos.x + 1.5f, -initialArrowPos.y, initialArrowPos.z);
            }
            else
            {
                spriteTransform.localPosition = initialArrowPos;
            }
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetChildrenActive(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }
}
