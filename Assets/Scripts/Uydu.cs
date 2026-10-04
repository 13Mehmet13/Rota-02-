using UnityEngine;

// Bonus (Uydu): uçağın çocuğu olan yörünge pivotunu döndürür.
// Uydu küresi pivotun çocuğu olduğu için uçakla birlikte gider ve uçağın etrafında döner:
// yerel uzayın bedava getirisi.
public class Uydu : MonoBehaviour
{
    [SerializeField] private float donusHizi = 180f; // derece / saniye

    private void Update()
    {
        transform.Rotate(Vector3.up, donusHizi * Time.deltaTime, Space.Self);
    }
}
