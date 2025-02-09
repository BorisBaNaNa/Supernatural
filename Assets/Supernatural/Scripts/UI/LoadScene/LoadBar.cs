using UnityEngine;

public class LoadBar : MonoBehaviour
{
    public Vector3 StartPos;
    public Vector3 EndPos = Vector3.zero;
    public float StartScaleX = 2f;
    public float EndScaleX = 18f;

    public void SetLoadState(float val)
    {
        transform.localScale = new Vector3(Mathf.Lerp(StartScaleX, EndScaleX, val), transform.localScale.y);
        transform.position = Vector3.Lerp(StartPos, EndPos, val);
    }
}
