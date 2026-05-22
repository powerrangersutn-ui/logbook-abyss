using UnityEngine;

public class VisualBanking : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody targetRigidbody;

    [Header("Banking")]
    [SerializeField] private float bankAmount = 25f;

    [SerializeField] private float bankSmoothness = 5f;

    private Quaternion initialLocalRotation;

    private void Awake()
    {
        initialLocalRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        Vector3 localVelocity =
            transform.parent.InverseTransformDirection(targetRigidbody.linearVelocity);

        float horizontal =
            Mathf.Clamp(localVelocity.x, -1f, 1f);

        float targetZ =
            -horizontal * bankAmount;

        Quaternion targetRotation =
            initialLocalRotation *
            Quaternion.Euler(0f, 0f, targetZ);

        transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                bankSmoothness * Time.deltaTime);
    }
}