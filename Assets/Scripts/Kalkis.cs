using UnityEngine;

// Uçak yerden havalanır: 1) pistte hızlanır, 2) burnunu kaldırıp tırmanır,
// 3) yeterli yüksekliğe çıkınca kontrolü HedefeGit'e devreder.
public class Kalkis : MonoBehaviour
{
    [SerializeField] private Transform hedef;
    [SerializeField] private float hizlanma = 4.7f;        // m/sn² (3 sn'de 14 m/sn)
    [SerializeField] private float kalkisHizi = 14f;       // HedefeGit.hiz ile aynı
    [SerializeField] private float tirmanisAcisi = 22f;    // derece
    [SerializeField] private float burunKaldirma = 14f;    // derece / saniye
    [SerializeField] private float devirYuksekligi = 9f;   // bu yüksekliğe çıkınca HedefeGit alır

    private enum Evre { Pist, Tirmanis }
    private Evre evre = Evre.Pist;
    private float hiz;
    private Vector3 yatayYon;
    private Behaviour hedefeGit;

    private void Awake()
    {
        hedefeGit = GetComponent("HedefeGit") as Behaviour;
        if (hedefeGit != null) hedefeGit.enabled = false;
    }

    private void Start()
    {
        yatayYon = hedef.position - transform.position;
        yatayYon.y = 0f;
        yatayYon.Normalize();
        transform.rotation = Quaternion.LookRotation(yatayYon);
    }

    private void Update()
    {
        hiz = Mathf.Min(hiz + hizlanma * Time.deltaTime, kalkisHizi);

        if (evre == Evre.Tirmanis)
        {
            float a = tirmanisAcisi * Mathf.Deg2Rad;
            Vector3 yon = new Vector3(yatayYon.x * Mathf.Cos(a), Mathf.Sin(a), yatayYon.z * Mathf.Cos(a));
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(yon), burunKaldirma * Time.deltaTime);
        }

        transform.position += transform.forward * hiz * Time.deltaTime;

        if (evre == Evre.Pist && hiz >= kalkisHizi) evre = Evre.Tirmanis;

        if (evre == Evre.Tirmanis && transform.position.y >= devirYuksekligi)
        {
            if (hedefeGit != null) hedefeGit.enabled = true;
            enabled = false;
        }
    }
}
