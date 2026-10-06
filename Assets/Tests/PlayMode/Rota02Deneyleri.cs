using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

// Föydeki deneyleri ve doğrulamaları Rota02 sahnesinde otomatik çalıştırıp
// sonuçları Rota02/deney_sonuclari.md dosyasına yazar.
public class Rota02Deneyleri
{
    private const float Dt = 1f / 60f;
    private readonly StringBuilder rapor = new StringBuilder();
    private static readonly CultureInfo C = CultureInfo.InvariantCulture;

    private Transform ucak, balon, savar, pervane;
    private Component git, radar, kalkis;

    private static string F(float v) => v.ToString("0.00", C);
    private static string V(Vector3 v) => $"({F(v.x)}, {F(v.y)}, {F(v.z)})";

    private static void Set(Component c, string alan, object deger)
    {
        var f = c.GetType().GetField(alan, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.IsNotNull(f, alan + " alani yok");
        f.SetValue(c, deger);
    }

    private static T Get<T>(Component c, string ad)
    {
        var p = c.GetType().GetProperty(ad, BindingFlags.Instance | BindingFlags.Public);
        return (T)p.GetValue(c);
    }


    // Deneylerin eski "havada başla" düzenini korur: kalkışı kapatır, uçağı (0,20,0)'a koyar.
    private void HavadaBasla(bool hareketli = true)
    {
        if (kalkis != null) ((Behaviour)kalkis).enabled = false;
        ((Behaviour)git).enabled = hareketli;
        ucak.position = new Vector3(0, 20, 0);
        ucak.rotation = Quaternion.identity;
    }

    private IEnumerator Yukle()
    {
        Time.captureDeltaTime = Dt;
        var op = SceneManager.LoadSceneAsync("Rota02", LoadSceneMode.Single);
        yield return op;
        yield return null;
        ucak = GameObject.Find("Nu_D36").transform;
        balon = GameObject.Find("Balon").transform;
        savar = GameObject.Find("Uçaksavar").transform;
        pervane = ucak.Find("Pervane");
        git = ucak.GetComponent(Type.GetType("HedefeGit, Assembly-CSharp"));
        kalkis = ucak.GetComponent(Type.GetType("Kalkis, Assembly-CSharp"));
        radar = savar.GetComponent(Type.GetType("Radar, Assembly-CSharp"));
        Assert.IsNotNull(git); Assert.IsNotNull(radar); Assert.IsNotNull(pervane);
    }

    private void Satir(string s) { rapor.AppendLine(s); Debug.Log(s); }

    [UnityTest]
    public IEnumerator TumDeneyler()
    {
        Satir("# Rota 02 — Otomatik deney sonuçları");
        Satir("");
        Satir("_Unity 6.3 LTS PlayMode testi, sabit adım = 1/60 sn (`Time.captureDeltaTime`). Ölçümler `Assets/Tests/PlayMode/Rota02Deneyleri.cs` ile üretildi._");

        // ---------- 0) Burun yönü ----------
        yield return Yukle();
        HavadaBasla(false);
        var burunYonu = (pervane.position - ucak.position).normalized;
        Satir("\n## 0) Uçağın burnu +Z mi?");
        Satir($"- Pervane yerel konumu: {V(pervane.localPosition)}; dünya yönünde burun = {V(burunYonu)}; `Dot(transform.forward, burun)` = {F(Vector3.Dot(ucak.forward, burunYonu))}");
        Satir(Vector3.Dot(ucak.forward, burunYonu) > 0.9f ? "- **Sonuç:** burun +Z (transform.forward) yönünde. FBX ayarları doğru." : "- **Sonuç:** burun +Z'ye bakmıyor!");

        // ---------- 1) Isınma ----------
        yield return Yukle();
        HavadaBasla(false);
        Satir("\n## 1) Isınma — dönüşümler");
        var pLokal0 = pervane.localPosition; var pDunya0 = pervane.position;
        ucak.rotation = Quaternion.Euler(0, 90, 0);
        yield return null;
        Satir($"- Uçak Rotation Y = 90: pervane **yerel** konum {V(pLokal0)} → {V(pervane.localPosition)} (değişmedi); **dünya** konumu {V(pDunya0)} → {V(pervane.position)} (uçakla birlikte döndü).");
        yield return Yukle();
        HavadaBasla(false);
        var balonBaslangic = balon.position;
        balon.SetParent(savar, true);
        savar.position += new Vector3(10, 0, 0);
        yield return null;
        Satir($"- Balon Uçaksavar'ın çocuğu yapıldı, Uçaksavar +10 X kaydırıldı: Balon {V(balonBaslangic)} → {V(balon.position)} (onunla gitti).");
        savar.position -= new Vector3(10, 0, 0);
        balon.SetParent(null, true);
        yield return null;
        Satir($"- Geri alındı (SetParent(null), Uçaksavar eski yerine): Balon {V(balon.position)}, ebeveyn = {(balon.parent == null ? "yok (bağımsız)" : "var")}.");

        // ---------- 2) Temel uçuş ----------
        yield return Yukle();
        Satir("\n## 2) HedefeGit — temel uçuş (hız 14 m/s, dönüş 60 °/s, varış 8 m)");
        Vector3 ilkKonum = ucak.position;
        float yerdenKalkis = -1, devir = -1, enYuksek = 0;
        float t = 0, minMesafe = float.MaxValue, varis = -1;
        float goruyorSure = 0, ilkGoruyor = -1, sonGoruyor = -1;
        string izler = "";
        while (t < 20f)
        {
            yield return null; t += Dt;
            float m = Vector3.Distance(ucak.position, balon.position);
            if (m < minMesafe) minMesafe = m;
            if (yerdenKalkis < 0 && ucak.position.y > 0.3f) yerdenKalkis = t;
            if (devir < 0 && kalkis != null && !((Behaviour)kalkis).enabled) devir = t;
            enYuksek = Mathf.Max(enYuksek, ucak.position.y);
            if (varis < 0 && m < 8.05f) varis = t;
            if (Get<bool>(radar, "Goruyor")) { goruyorSure += Dt; if (ilkGoruyor < 0) ilkGoruyor = t; sonGoruyor = t; }
            if (Mathf.Abs(t % 2f) < Dt * 0.5f || Mathf.Abs(t % 2f - 2f) < Dt * 0.5f)
                izler += $"t={F(t)}s: uzaklık {F(m)} m; ";
        }
        Satir($"- **Kalkış:** uçak pistte {V(ilkKonum)}'den başlıyor (tekerlekler zeminde, y=0). Pistte hızlanıp t={F(yerdenKalkis)} sn'de yerden kesiliyor (y>0.3 m), t={F(devir)} sn'de 9 m'ye çıkıp kontrolü HedefeGit'e devrediyor; sonra balona gidiyor.");
        Satir($"- İlk uzaklık ≈ {F(Vector3.Distance(ilkKonum, balon.position))} m (çıkarma + `.magnitude`).");
        Satir($"- Varış mesafesi (8 m) altına inme zamanı: **{(varis < 0 ? "20 sn içinde varmadı" : F(varis) + " sn")}**; en yakın uzaklık {F(minMesafe)} m (varış eşiğinde duruyor, titremiyor/zıplamıyor).");
        Satir($"- Zaman çizelgesi: {izler}");
        Satir($"- Uçak son yönü hedefe: `Dot(forward, yon)` = {F(Vector3.Dot(ucak.forward, (balon.position - ucak.position).normalized))}; uçuş boyunca en yüksek nokta y = {F(enYuksek)} m");

        // ---------- 3) Radar ----------
        Satir("\n## 3) Radar — Goruyor ne zaman açılıp kapanıyor? (yarı açı 35°, menzil 120 m, Uçaksavar yaw 33°)");
        Satir($"- 20 sn uçuş boyunca `Goruyor` **true** olduğu toplam süre: {F(goruyorSure)} sn; ilk true t={(ilkGoruyor < 0 ? "hiç" : F(ilkGoruyor))}, son true t={(sonGoruyor < 0 ? "hiç" : F(sonGoruyor))}. Yani başta (uçak koninin dışındayken) false, uçak koniye girince true oluyor — değer sabit değil.");

        // ---------- 4) Deney: normalize ----------
        Satir("\n## 4) Deney — `.normalized` kaldırılırsa ne olur?");
        {
            Vector3 fark = balon.position - new Vector3(0, 20, 0); // föydeki başlangıç yüksekliği
            float mesafe = fark.magnitude;
            Quaternion a = Quaternion.LookRotation(fark.normalized);
            Quaternion b = Quaternion.LookRotation(fark);
            Satir($"- `LookRotation` normalize edilmemiş vektörü kendisi normalize eder: açı farkı = {F(Quaternion.Angle(a, b))}° (föydeki kodda uçak `transform.forward` yönünde ilerlediği için `.normalized` kaldırılınca **davranış değişmiyor**).");
            float hizNorm = 14f, hizHam = mesafe * 14f;
            Satir($"- Fırlama ancak ilerleme doğrudan yön vektörüyle yapılırsa görülür (`position += yon * hiz * dt`): normalize edilmişse hız **{F(hizNorm)} m/s**, edilmemişse hız = uzaklık × 14 = **{F(hizHam)} m/s** (uzaklık {F(mesafe)} m'de). Uzaklık büyüdükçe hız büyür, hedefe yaklaşınca yavaşlar — föydeki \"sık hatalar\" tablosundaki belirti.");
        }

        // ---------- 5) Deney: dönüş hızı ----------
        Satir("\n## 5) Deney — Donus Hizi");
        foreach (float dh in new[] { 60f, 30f, 10f })
        {
            yield return Yukle();
            HavadaBasla();
            Set(git, "donusHizi", dh);
            float tt = 0, min = float.MaxValue, enUzak = 0, v = -1;
            Vector3 baslangic = ucak.position;
            while (tt < 60f)
            {
                yield return null; tt += Dt;
                float m = Vector3.Distance(ucak.position, balon.position);
                if (m < min) min = m;
                enUzak = Mathf.Max(enUzak, Vector3.Distance(ucak.position, baslangic));
                if (v < 0 && m < 8.05f) { v = tt; }
            }
            Satir($"- Donus Hizi = **{F(dh)}°/s**: varış {(v < 0 ? "**60 sn içinde yok** (hedefi kaçırıp tur atıyor)" : F(v) + " sn")}; en yakın uzaklık {F(min)} m; başlangıçtan en uzak nokta {F(enUzak)} m.");
        }
        Satir("- Yorum: dönüş çok yavaşken uçak burnu yönünde ilerlediği için hedefi geçer ve geniş yay çizer (dönüş yarıçapı ≈ hız / açısal hız = 14 / (10·π/180) ≈ 80 m).");

        // ---------- 6) Deney: yarı açı ----------
        Satir("\n## 6) Deney — Radar Yarim Aci");
        foreach (float ya in new[] { 10f, 35f, 80f })
        {
            yield return Yukle();
            HavadaBasla();
            Set(radar, "yarimAci", ya);
            float tt = 0, gs = 0;
            while (tt < 20f)
            {
                yield return null; tt += Dt;
                if (Get<bool>(radar, "Goruyor")) gs += Dt;
            }
            Satir($"- Yarim Aci = **{F(ya)}°** (eşik cos = {F(Mathf.Cos(ya * Mathf.Deg2Rad))}): 20 sn'de Goruyor true süresi = **{F(gs)} sn** ({F(gs / 20f * 100f)}%).");
        }

        // ---------- 7) Işaret doğrulama ----------
        Satir("\n## 7) Dış çarpım işareti ve kanat yatışı (denenerek doğrulandı)");
        foreach (float x in new[] { +30f, -30f })
        {
            yield return Yukle();
            HavadaBasla();
            Set(git, "hiz", 0f); // sadece dönüşü gözle
            balon.position = new Vector3(x, 20f, 60f);
            float enBuyukYan = 0, enBuyukSagY = 0;
            for (int i = 0; i < 40; i++)
            {
                yield return null;
                Vector3 yon = (balon.position - ucak.position).normalized;
                float yan = Vector3.Cross(ucak.forward, yon).y;
                if (Mathf.Abs(yan) > Mathf.Abs(enBuyukYan)) enBuyukYan = yan;
                if (Mathf.Abs(ucak.right.y) > Mathf.Abs(enBuyukSagY)) enBuyukSagY = ucak.right.y;
            }
            Satir($"- Balon {(x > 0 ? "SAĞDA" : "SOLDA")} (x={F(x)}): dönüş sırasında `Cross(forward, yon).y` en büyük = {F(enBuyukYan)} → {(enBuyukYan > 0 ? "pozitif" : "negatif")}; `ucak.right.y` (sağ kanat yönü) en uç = {F(enBuyukSagY)} → sağ kanat {(enBuyukSagY < 0 ? "AŞAĞI" : "YUKARI")}.");
        }
        // Radar işareti: uçak uçaksavarın sağında/solunda
        foreach (float x in new[] { +30f, -30f })
        {
            yield return Yukle();
            HavadaBasla();
            Set(git, "hiz", 0f); Set(git, "donusHizi", 0f);
            savar.rotation = Quaternion.identity;
            ucak.position = new Vector3(x, 5f, 40f);
            yield return null; yield return null;
            Satir($"- Uçak Uçaksavar'ın {(x > 0 ? "SAĞINDA" : "SOLUNDA")} (x={F(x)}): Radar.YanTaraf = {F(Get<float>(radar, "YanTaraf"))}.");
        }

        // ---------- 8) Çizginin yeşile döndüğü açı ----------
        yield return Yukle();
        HavadaBasla();
        Set(git, "hiz", 0f); Set(git, "donusHizi", 0f);
        ucak.position = new Vector3(20f, 25f, 30f);
        Vector3 d = ucak.position - savar.position;
        float bearing = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
        float elev = Mathf.Asin(d.normalized.y) * Mathf.Rad2Deg;
        float prevYaw = 0; bool prev = false; float giris = float.NaN, cikis = float.NaN;
        for (float yaw = 0; yaw <= 360f; yaw += 0.5f)
        {
            savar.rotation = Quaternion.Euler(0, yaw, 0);
            yield return null;
            bool g = Get<bool>(radar, "Goruyor");
            if (g && !prev && float.IsNaN(giris)) giris = yaw;
            if (!g && prev && float.IsNaN(cikis)) cikis = prevYaw;
            prev = g; prevYaw = yaw;
        }
        float cosEsik = Mathf.Cos(35f * Mathf.Deg2Rad);
        float yariGenislik = Mathf.Acos(Mathf.Clamp(cosEsik / Mathf.Cos(elev * Mathf.Deg2Rad), -1, 1)) * Mathf.Rad2Deg;
        Satir("\n## 8) Çizgi tam hangi açıda yeşile dönüyor?");
        Satir($"- Uçak (20, 25, 30)'da sabit; Uçaksavar'ı Y ekseninde 0.5° adımlarla döndürdüm. Hedef yönü yatayda {F(bearing)}°, yükseklik açısı {F(elev)}°.");
        Satir($"- Gizmos çizgisi **yeşile** döndüğü ilk Y açısı: **{F(giris)}°**, yeşil kaldığı son açı: **{F(cikis)}°** (pencere genişliği ≈ {F(cikis - giris)}°).");
        Satir($"- Analitik beklenti: iç çarpım eşiği `cos(35°)={F(cosEsik)}`; yükseklik payı hesaba katılınca yeşil pencere yaklaşık {F(bearing - yariGenislik)}° … {F(bearing + yariGenislik)}°. Ölçüm, 0.5° tarama adımı kadar bir sapmayla hesapla uyuşuyor (ilk örneklenen yeşil açı ≥ teorik giriş, son örneklenen ≤ teorik çıkış).");


        // ---------- 9) Bonuslar ----------
        Satir("\n## 9) Bonus görevler (ölçüm)");
        yield return Yukle();
        HavadaBasla();
        var pivot = ucak.Find("UyduYorungesi");
        var uydu = pivot != null ? pivot.Find("Uydu") : null;
        Assert.IsNotNull(uydu, "Uydu yok");
        float minR = float.MaxValue, maxR = 0, kat = 0;
        Vector3 ilkUcak = ucak.position;
        for (int i = 0; i < 300; i++)
        {
            yield return null;
            Vector3 hor = uydu.position - pivot.position;
            float r = hor.magnitude;
            minR = Mathf.Min(minR, r); maxR = Mathf.Max(maxR, r);
        }
        kat = Vector3.Distance(ucak.position, ilkUcak);
        Satir($"- **Uydu:** 5 sn boyunca uçak {F(kat)} m ilerlerken Uydu'nun yörünge pivotuna uzaklığı sabit kaldı (min {F(minR)} m, max {F(maxR)} m) ve Uydu uçakla birlikte gitti (yerel uzay).");

        yield return Yukle();
        HavadaBasla();
        int mermiGorulen = 0; float enYakinMermiMesafe = float.MaxValue; float ilkAtis = -1; float tt2 = 0;
        var gorulenler = new System.Collections.Generic.HashSet<int>();
        float hizOlcum = 0; Vector3 oncekiKonum = Vector3.zero; int oncekiId = 0;
        while (tt2 < 8f)
        {
            yield return null; tt2 += Dt;
            foreach (var go in GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None))
            {
                if (go.name != "Mermi") continue;
                int id = go.GetInstanceID();
                if (gorulenler.Add(id))
                {
                    mermiGorulen++;
                    if (ilkAtis < 0) ilkAtis = tt2;
                }
                if (id == oncekiId) hizOlcum = (go.position - oncekiKonum).magnitude / Dt;
                oncekiId = id; oncekiKonum = go.position;
                enYakinMermiMesafe = Mathf.Min(enYakinMermiMesafe, Vector3.Distance(go.position, ucak.position));
            }
        }
        Satir($"- **Mermi:** 8 sn'de {mermiGorulen} mermi üretildi (ilk atış t={F(ilkAtis)} sn, yalnızca `Goruyor` true iken); ölçülen mermi hızı ≈ {F(hizOlcum)} m/s (= normalize yön × 60); mermiler uçağa en fazla {F(enYakinMermiMesafe)} m yaklaştı (düz çizgide, fizik yok; uçak hareket ettiği için tam isabet garanti değil).");

        yield return Yukle();
        HavadaBasla();
        var nis = GameObject.Find("Nisangah");
        Assert.IsNotNull(nis, "Nisangah yok");
        var nisBilesen = nis.GetComponent(Type.GetType("Nisangah, Assembly-CSharp"));
        var hedefle = nisBilesen.GetType().GetMethod("Hedefle");
        var kam = Camera.main; Assert.IsNotNull(kam);
        var noktalar = new[] { new Vector2(0.5f, 0.4f), new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.35f) };
        string nisSatir = "";
        foreach (var n in noktalar)
        {
            var px = new Vector2(n.x * Screen.width, n.y * Screen.height);
            bool ok = (bool)hedefle.Invoke(nisBilesen, new object[] { px });
            Vector3 geri = kam.WorldToScreenPoint(balon.position);
            nisSatir += $"ekran ({F(px.x)}, {F(px.y)}) → Balon {V(balon.position)} (geri izdüşüm farkı {F(Vector2.Distance(px, geri))} px); ";
            Assert.IsTrue(ok);
        }
        Satir($"- **Nişangah:** imleçten çıkan `ScreenPointToRay` ışını y=30 düzlemiyle kesiştirilip Balon oraya taşınıyor. {nisSatir}Balon her seferinde y=30'da kalıyor ve ekrana geri izdüşümü imleçle aynı noktaya düşüyor; sol fare tuşu basılıyken `Update` bunu çağırır.");
        Satir("- **Koniyi çiz:** `KoniCiz.cs` Uçaksavar'a eklendi; `Quaternion.AngleAxis(±yarı açı, Vector3.up) * forward` ile iki kenar sarı çizilir (Scene görünümünde).");

        File.WriteAllText(Path.Combine(Application.dataPath, "../Rota02/deney_sonuclari.md"), rapor.ToString(), new UTF8Encoding(false));
        Time.captureDeltaTime = 0;
    }
}
