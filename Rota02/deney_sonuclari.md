# Rota 02 — Otomatik deney sonuçları

_Unity 6.3 LTS PlayMode testi, sabit adım = 1/60 sn (`Time.captureDeltaTime`). Ölçümler `Assets/Tests/PlayMode/Rota02Deneyleri.cs` ile üretildi._

## 0) Uçağın burnu +Z mi?
- Pervane yerel konumu: (0.00, 1.05, 3.21); dünya yönünde burun = (0.00, 0.31, 0.95); `Dot(transform.forward, burun)` = 0.95
- **Sonuç:** burun +Z (transform.forward) yönünde. FBX ayarları doğru.

## 1) Isınma — dönüşümler
- Uçak Rotation Y = 90: pervane **yerel** konum (0.00, 1.05, 3.21) → (0.00, 1.05, 3.21) (değişmedi); **dünya** konumu (0.00, 21.05, 3.21) → (3.21, 21.05, 0.00) (uçakla birlikte döndü).
- Balon Uçaksavar'ın çocuğu yapıldı, Uçaksavar +10 X kaydırıldı: Balon (40.00, 30.00, 60.00) → (50.00, 30.00, 60.00) (onunla gitti).
- Geri alındı (SetParent(null), Uçaksavar eski yerine): Balon (40.00, 30.00, 60.00), ebeveyn = yok (bağımsız).

## 2) HedefeGit — temel uçuş (hız 14 m/s, dönüş 60 °/s, varış 8 m)
- **Kalkış:** uçak pistte (-20.00, 0.00, 12.00)'den başlıyor (tekerlekler zeminde, y=0). Pistte hızlanıp t=3.37 sn'de yerden kesiliyor (y>0.3 m), t=5.45 sn'de 9 m'ye çıkıp kontrolü HedefeGit'e devrediyor; sonra balona gidiyor.
- İlk uzaklık ≈ 82.48 m (çıkarma + `.magnitude`).
- Varış mesafesi (8 m) altına inme zamanı: **7.08 sn**; en yakın uzaklık 7.83 m (varış eşiğinde duruyor, titremiyor/zıplamıyor).
- Zaman çizelgesi: t=2.00s: uzaklık 73.45 m; t=4.00s: uzaklık 49.93 m; t=6.00s: uzaklık 23.00 m; t=8.00s: uzaklık 7.83 m; t=10.00s: uzaklık 7.83 m; t=12.00s: uzaklık 7.83 m; t=14.00s: uzaklık 7.83 m; t=16.00s: uzaklık 7.83 m; t=18.00s: uzaklık 7.83 m; t=20.00s: uzaklık 7.83 m; 
- Uçak son yönü hedefe: `Dot(forward, yon)` = 1.00; uçuş boyunca en yüksek nokta y = 24.45 m

## 3) Radar — Goruyor ne zaman açılıp kapanıyor? (yarı açı 35°, menzil 120 m, Uçaksavar yaw 33°)
- 20 sn uçuş boyunca `Goruyor` **true** olduğu toplam süre: 16.83 sn; ilk true t=3.20, son true t=20.02. Yani başta (uçak koninin dışındayken) false, uçak koniye girince true oluyor — değer sabit değil.

## 4) Deney — `.normalized` kaldırılırsa ne olur?
- `LookRotation` normalize edilmemiş vektörü kendisi normalize eder: açı farkı = 0.00° (föydeki kodda uçak `transform.forward` yönünde ilerlediği için `.normalized` kaldırılınca **davranış değişmiyor**).
- Fırlama ancak ilerleme doğrudan yön vektörüyle yapılırsa görülür (`position += yon * hiz * dt`): normalize edilmişse hız **14.00 m/s**, edilmemişse hız = uzaklık × 14 = **1019.22 m/s** (uzaklık 72.80 m'de). Uzaklık büyüdükçe hız büyür, hedefe yaklaşınca yavaşlar — föydeki "sık hatalar" tablosundaki belirti.

## 5) Deney — Donus Hizi
- Donus Hizi = **60.00°/s**: varış 4.67 sn; en yakın uzaklık 7.80 m; başlangıçtan en uzak nokta 65.01 m.
- Donus Hizi = **30.00°/s**: varış 4.73 sn; en yakın uzaklık 7.82 m; başlangıçtan en uzak nokta 65.02 m.
- Donus Hizi = **10.00°/s**: varış **60 sn içinde yok** (hedefi kaçırıp tur atıyor); en yakın uzaklık 11.28 m; başlangıçtan en uzak nokta 161.66 m.
- Yorum: dönüş çok yavaşken uçak burnu yönünde ilerlediği için hedefi geçer ve geniş yay çizer (dönüş yarıçapı ≈ hız / açısal hız = 14 / (10·π/180) ≈ 80 m).

## 6) Deney — Radar Yarim Aci
- Yarim Aci = **10.00°** (eşik cos = 0.98): 20 sn'de Goruyor true süresi = **0.00 sn** (0.00%).
- Yarim Aci = **35.00°** (eşik cos = 0.82): 20 sn'de Goruyor true süresi = **17.43 sn** (87.17%).
- Yarim Aci = **80.00°** (eşik cos = 0.17): 20 sn'de Goruyor true süresi = **19.72 sn** (98.58%).

## 7) Dış çarpım işareti ve kanat yatışı (denenerek doğrulandı)
- Balon SAĞDA (x=30.00): dönüş sırasında `Cross(forward, yon).y` en büyük = 0.43 → pozitif; `ucak.right.y` (sağ kanat yönü) en uç = -0.14 → sağ kanat AŞAĞI.
- Balon SOLDA (x=-30.00): dönüş sırasında `Cross(forward, yon).y` en büyük = -0.43 → negatif; `ucak.right.y` (sağ kanat yönü) en uç = 0.14 → sağ kanat YUKARI.
- Uçak Uçaksavar'ın SAĞINDA (x=30.00): Radar.YanTaraf = 0.60.
- Uçak Uçaksavar'ın SOLUNDA (x=-30.00): Radar.YanTaraf = -0.60.

## 8) Çizgi tam hangi açıda yeşile dönüyor?
- Uçak (20, 25, 30)'da sabit; Uçaksavar'ı Y ekseninde 0.5° adımlarla döndürdüm. Hedef yönü yatayda 33.69°, yükseklik açısı 34.74°.
- Gizmos çizgisi **yeşile** döndüğü ilk Y açısı: **29.50°**, yeşil kaldığı son açı: **38.00°** (pencere genişliği ≈ 8.50°).
- Analitik beklenti: iç çarpım eşiği `cos(35°)=0.82`; yükseklik payı hesaba katılınca yeşil pencere yaklaşık 29.11° … 38.27°. Ölçüm, 0.5° tarama adımı kadar bir sapmayla hesapla uyuşuyor (ilk örneklenen yeşil açı ≥ teorik giriş, son örneklenen ≤ teorik çıkış).

## 9) Bonus görevler (ölçüm)
- **Uydu:** 5 sn boyunca uçak 65.01 m ilerlerken Uydu'nun yörünge pivotuna uzaklığı sabit kaldı (min 2.86 m, max 2.86 m) ve Uydu uçakla birlikte gitti (yerel uzay).
- **Mermi:** 8 sn'de 9 mermi üretildi (ilk atış t=2.60 sn, yalnızca `Goruyor` true iken); ölçülen mermi hızı ≈ 60.00 m/s (= normalize yön × 60); mermiler uçağa en fazla 0.43 m yaklaştı (düz çizgide, fizik yok; uçak hareket ettiği için tam isabet garanti değil).
- **Nişangah:** imleçten çıkan `ScreenPointToRay` ışını y=30 düzlemiyle kesiştirilip Balon oraya taşınıyor. ekran (320.00, 192.00) → Balon (-34.96, 30.00, -34.94) (geri izdüşüm farkı 0.00 px); ekran (192.00, 144.00) → Balon (-41.36, 30.00, -35.95) (geri izdüşüm farkı 0.00 px); ekran (448.00, 168.00) → Balon (-31.89, 30.00, -39.94) (geri izdüşüm farkı 0.00 px); Balon her seferinde y=30'da kalıyor ve ekrana geri izdüşümü imleçle aynı noktaya düşüyor; sol fare tuşu basılıyken `Update` bunu çağırır.
- **Koniyi çiz:** `KoniCiz.cs` Uçaksavar'a eklendi; `Quaternion.AngleAxis(±yarı açı, Vector3.up) * forward` ile iki kenar sarı çizilir (Scene görünümünde).
