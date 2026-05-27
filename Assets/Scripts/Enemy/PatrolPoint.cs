using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [SerializeField] private float waitTime = 2f;

    public float WaitTime => waitTime;
}