using UnityEngine;

public class SandboxZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Toy"))  // Zabawki musz¹ mieæ tag "Toy"
        {
            Debug.Log("Zabawka poprawnie wrzucona do piaskownicy!");
            Destroy(other.gameObject, 1f); // Mo¿na te¿ dodaæ efekt nagrody
        }
    }
}
