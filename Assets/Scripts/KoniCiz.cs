using UnityEngine;

// Bonus (Koniyi çiz): Radar'ın görüş konisinin iki kenarını Gizmos ile çizer.
// Değerler Radar.cs'tekilerle aynı olmalı (yarı açı 35°, menzil 120 m).
public class KoniCiz : MonoBehaviour
{
    [SerializeField] private float yarimAci = 35f;
    [SerializeField] private float menzil = 120f;

    private void OnDrawGizmos()
    {
        Vector3 sol = Quaternion.AngleAxis(-yarimAci, Vector3.up) * transform.forward;
        Vector3 sag = Quaternion.AngleAxis(yarimAci, Vector3.up) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, sol * menzil);
        Gizmos.DrawRay(transform.position, sag * menzil);
    }
}
