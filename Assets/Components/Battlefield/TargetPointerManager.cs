using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointerManager : MonoBehaviour
{
    [SerializeField]private GameObject Arrow;
    [SerializeField]private SpriteRenderer ArrowSprite;
    [SerializeField]private GameObject Line;
    [SerializeField]private LineRenderer LineRenderer;
    public float defaultWidth = 0.06f;
    public void SetPositinsAndColors(Vector3 startPos, Vector3 endPos, Color startColor, Color endColor)
    {
        LineRenderer.SetPosition(0, startPos);
        LineRenderer.SetPosition(1, endPos);
        LineRenderer.startColor = startColor;
        LineRenderer.endColor = endColor;
        
        Arrow.transform.position = endPos;
        Vector3 direction = endPos - startPos;
        direction = direction.normalized;
        float angle = Vector3.SignedAngle(Arrow.transform.up, direction, Vector3.forward);
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Arrow.transform.rotation = rotation;
        ArrowSprite.color = endColor;
        DeHighlightElement();
    }
    public void HighlightElement()
    {
        Arrow.transform.localScale = new Vector3(3, 3, 3);
        LineRenderer.startWidth = defaultWidth*3;
    }
    public void DeHighlightElement()
    {
        Arrow.transform.localScale = Vector3.one;
        LineRenderer.startWidth = defaultWidth;
    }
}
