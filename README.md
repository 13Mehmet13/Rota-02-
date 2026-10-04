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
| `Rota02/MDA_Calisma_Kagidi.md` | MDA çalışma kâğıdı (2048) |
| `Rota02/scene_gizmos.png` | Gizmos çizgileri görünürken Scene ekran görüntüsü |

## Çalıştırma

Unity Hub ile klasörü 6.3 LTS'de aç → `Assets/Scenes/Rota02.unity` → Play. Scene görünümünde yeşil çizgi, uçaksavarın uçağı koniye aldığını; kırmızı çizgi almadığını gösterir.

> Not: `Library/` klasörü `.gitignore` ile dışarıda bırakıldı; ilk açılışta Unity yeniden üretir.
>
> Önceki hafta (Hangar 01): https://github.com/13Mehmet13/Hangar01
