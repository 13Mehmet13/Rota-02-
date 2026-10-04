# Rota 02 — Uçağı hedefe yönelt

**BMU1421 · Dijital Oyun Tasarımı** — Hafta 02 laboratuvarı (Mehmet Altıok · 23080410319)

- **Unity sürümü:** 6.3 LTS (6000.3.25f1), Universal 3D (URP)
- **Açıklama:** Hafta 1'in çift kanatlı uçağı, normalize yön vektörüyle gözlem balonuna yönelir; uçaksavar iç çarpımla görüş konisi, dış çarpımla sağ/sol tespiti yapar ve vektörleri Gizmos ile çizer.

## İçerik

| Dosya | Açıklama |
|---|---|
| `Assets/Scenes/Rota02.unity` | Uçak `(0,20,0)`, Balon Ø6 `(40,30,60)`, Uçaksavar `(0,0,0)` + namlu |
| `Assets/Scripts/HedefeGit.cs` | Fark vektörü → `normalized` → `LookRotation` + `RotateTowards`; dış çarpımla kanat yatışı |
| `Assets/Scripts/Radar.cs` | İç çarpım + kosinüs eşiği (görüş konisi), dış çarpım (sağ/sol), `OnDrawGizmos` |
| `Rota02/MDA_Calisma_Kagidi.pdf` (`.md`) | MDA çalışma kâğıdı (2048) |
| `Rota02/scene_gizmos.png` | Gizmos çizgileri görünürken Scene ekran görüntüsü |
| `Rota02/deney_sonuclari.md` | Föydeki deney ve doğrulamaların otomatik ölçüm sonuçları |
| `Assets/Tests/PlayMode/Rota02Deneyleri.cs` | Bu ölçümleri üreten PlayMode testi |
| `Assets/Scripts/Uydu.cs`, `KoniCiz.cs`, `MermiAtici.cs` | Bonuslar: Uydu, Koniyi çiz, Mermi |

## Deneyler ve doğrulamalar (özet)

Föydeki deneyler `Rota02Deneyleri.cs` ile Unity'de otomatik çalıştırıldı (sabit 1/60 sn adım); tüm sayılar `Rota02/deney_sonuclari.md` içinde:

- **Hedefe gidiş:** uçak 72.8 m'den 4.63 sn'de varış eşiğine (8 m) iniyor ve titremeden duruyor.
- **Donus Hizi 10°/s:** 60 sn içinde varış yok, uçak 161 m'ye kadar uzaklaşıp tur atıyor (60°/s'de 4.63 sn).
- **Yarim Aci:** 10° → 20 sn'de hiç görmüyor; 35° → %87; 80° → %99 (iç çarpım eşiği `cos(açı)`).
- **İşaretler doğrulandı:** balon sağdayken `Cross(forward, yon).y > 0` ve sağ kanat aşağı; solda tersi. Uçak uçaksavarın sağındayken `YanTaraf > 0`.
- **Yeşile dönüş açısı:** uçak sabitken uçaksavarı döndürünce çizgi 29.5°…38.0° arasında yeşil (teorik 29.1°…38.3°).
- **`.normalized` deneyi:** föydeki kodda ilerleme `transform.forward` ile yapıldığı ve `LookRotation` zaten normalize ettiği için ifadeyi kaldırmak davranışı **değiştirmiyor** (açı farkı 0°). Fırlama, ilerleme `yon * hiz` ile yapılırsa görülür (hız = uzaklık × 14 ≈ 1019 m/s).

## Çalıştırma

Unity Hub ile klasörü 6.3 LTS'de aç → `Assets/Scenes/Rota02.unity` → Play. Scene görünümünde yeşil çizgi, uçaksavarın uçağı koniye aldığını; kırmızı çizgi almadığını gösterir.

> Not: `Library/` klasörü `.gitignore` ile dışarıda bırakıldı; ilk açılışta Unity yeniden üretir.
>
> Önceki hafta (Hangar 01): https://github.com/13Mehmet13/Hangar01
