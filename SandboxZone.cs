using UnityEngine;

public class SandboxZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Toy"))  // Zabawki musz� mie� tag "Toy"
        {
            Debug.Log("Zabawka poprawnie wrzucona do piaskownicy!");
            Destroy(other.gameObject, 1f); // Mo�na te� doda� efekt nagrody
        }
    }
}
