using UnityEngine;
using UnityEngine.InputSystem;

// Bonus (Nişangah): fare imlecinin dünyadaki yerini bulup hedefi (Balon) oraya taşır.
// Fare sol tuşu basılıyken, imleçten çıkan ışın yatay bir düzlemle (balon yüksekliği) kesiştirilir.
public class Nisangah : MonoBehaviour
{
    [SerializeField] private Transform hedef;
    [SerializeField] private Camera kamera;      // boşsa Camera.main
    [SerializeField] private float yukseklik = 30f;

    private void Update()
    {
        var fare = Mouse.current;
        if (fare == null || hedef == null || !fare.leftButton.isPressed) return;
        Hedefle(fare.position.ReadValue());
    }

    // Ekran noktasından dünyaya: ScreenPointToRay + düzlem kesişimi
    public bool Hedefle(Vector2 ekranNoktasi)
    {
        var k = kamera != null ? kamera : Camera.main;
        if (k == null || hedef == null) return false;

        Ray isin = k.ScreenPointToRay(ekranNoktasi);
        var duzlem = new Plane(Vector3.up, new Vector3(0f, yukseklik, 0f));
        if (!duzlem.Raycast(isin, out float uzaklik)) return false;

        hedef.position = isin.GetPoint(uzaklik);
        return true;
    }
}
