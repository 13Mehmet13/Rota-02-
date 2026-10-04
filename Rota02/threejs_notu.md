# Bonus — Three.js'te yerel uzay (threejs.org/editor)

Föy: "Bir küp ekle, altına ikinci bir küp ekleyip ebeveyni döndür."

`threejs.org/editor` sayfasında `Ebeveyn` (mavi) küpünü ekleyip ikinci küpü (`Cocuk`, turuncu, ölçek 0.5) onun **çocuğu** yaptım (`editor.addObject(cocuk, ebeveyn)`), sonra ebeveynin Rotation Y değerini 90° yaptım. Ölçüm:

| | Çocuğun **yerel** konumu | Çocuğun **dünya** konumu |
|---|---|---|
| Ebeveyn dönmeden önce | (1.50, 0.80, 0.00) | (1.50, 1.30, 0.00) |
| Ebeveyn Y = 90° sonra | (1.50, 0.80, 0.00) — **değişmedi** | (0.00, 1.30, −1.50) — ebeveynle birlikte döndü |

Unity'deki uçak–pervane ve uçak–uydu ilişkisiyle aynı davranış: yerel konum sabit, dünya konumu ebeveyni izler. Kavram motora bağlı değil.
