using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    public float BaseAmount = 0.002f;
    public float BaseFrequency = 10f;
    public float Smooth = 10f;

    private Vector3 startPos;
    private float amount;
    private float frequency;

    void Start()
    {
        startPos = transform.localPosition;
        amount = BaseAmount;
        frequency = BaseFrequency;
    }

    void Update()
    {
        HandleSprint();
        if (IsMoving())
        {
            ApplyHeadBob();
        }
        else
        {
            ResetPosition();
        }
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            amount = BaseAmount * 1.5f;
            frequency = BaseFrequency * 1.5f;
        }
        else
        {
            amount = BaseAmount;
            frequency = BaseFrequency;
        }
    }

    private bool IsMoving()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude > 0;
    }

    private void ApplyHeadBob()
    {
        float time = Time.time * frequency;
        Vector3 bobOffset = new Vector3(
            Mathf.Cos(time * 0.5f) * amount * 1.6f,
            Mathf.Sin(time) * amount * 1.4f,
            0
        );

        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos + bobOffset, Smooth * Time.deltaTime);
    }

    private void ResetPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Smooth * Time.deltaTime);
    }
}
