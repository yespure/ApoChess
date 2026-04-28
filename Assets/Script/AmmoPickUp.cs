using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AmmoManager.Instance.AddAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}