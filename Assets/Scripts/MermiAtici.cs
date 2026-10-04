using UnityEngine;

// Bonus (Mermi): uçaksavar hedefi koniye aldığında, normalize yön vektörüyle ilerleyen
// bir küre üretir. Fizik yok, yalnızca vektör.
public class MermiAtici : MonoBehaviour
{
    [SerializeField] private Radar radar;
    [SerializeField] private Transform hedef;
    [SerializeField] private float aralik = 0.6f;
    [SerializeField] private float mermiHizi = 60f;

    private float sonAtis = -999f;

    private void Update()
    {
        if (radar == null || hedef == null || !radar.Goruyor) return;
        if (Time.time - sonAtis < aralik) return;
        sonAtis = Time.time;

        Vector3 baslangic = transform.position + transform.forward * 3f + Vector3.up * 1f;
        Vector3 yon = (hedef.position - baslangic).normalized;

        var mermi = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mermi.name = "Mermi";
        mermi.transform.localScale = Vector3.one * 0.8f;
        mermi.transform.position = baslangic;
        Destroy(mermi.GetComponent<Collider>());
        mermi.AddComponent<MermiHareket>().Ayarla(yon, mermiHizi);
        Destroy(mermi, 4f);
    }
}

public class MermiHareket : MonoBehaviour
{
    private Vector3 yon;
    private float hiz;

    public void Ayarla(Vector3 yon, float hiz) { this.yon = yon; this.hiz = hiz; }

    private void Update()
    {
        transform.position += yon * hiz * Time.deltaTime;
    }
}
