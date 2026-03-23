<h1 align="center">🧩Qubix (Unity)</h1>

<p align="center">
  Bu proje, Unity ile geliştirilmiş, dinamik ızgara (grid) sistemine sahip, özelleştirilebilir bir 2D eşleştirme (Blast/Match) bulmaca oyunudur. Modüler yapısı ve esnek ayar menüsü sayesinde hem oyuncular hem de geliştiriciler için harika bir temel sunar.
</p>

<hr>

<h2>🚀 Özellikler</h2>
<ul>
  <li><strong>Dinamik Oyun Alanı:</strong> Izgara genişliği (N) ve yüksekliği (M) 2 ile 10 arasında; renk çeşidi ise 1 ile 6 arasında oyun içinden ayarlanabilir.</li>
  <li><strong>Gelişmiş Eşleştirme (BFS Algoritması):</strong> Tıklanan bloğun etrafındaki aynı renkli blokları anında tespit eder ve en az 2 blok yan yanaysa patlatma işlemini gerçekleştirir.</li>
  <li><strong>Otomatik Kamera ve Arka Plan:</strong> Oyun alanının (grid) boyutlarına göre kamera yakınlaştırması ve arka plan görseli otomatik olarak merkeze hizalanır.</li>
  <li><strong>Akıllı Tıkanıklık (Deadlock) Çözümü:</strong> Tahtada yapılabilecek hamle kalmadığında sistem bunu algılar, mevcut renkleri kullanarak en az bir hamle garantili yeni bir dizilim oluşturur.</li>
  <li><strong>Görsel Geri Bildirim:</strong> Biriken blok grubunun büyüklüğüne göre (CondA, CondB, CondC) blok ikonları dinamik olarak değişir.</li>
  <li><strong>Performans Odaklı:</strong> Blok yaratma ve yok etme işlemlerinde performansı artırmak için <strong>Object Pooling</strong> sistemi kullanılmıştır.</li>
</ul>

<h2>📂 Mimari ve Temel Scriptler</h2>
<ul>
  <li><code>BoardManager.cs</code>: Oyunun kalbidir. Grid'i oluşturur, BFS ile eşleşmeleri bulur ve blokları havuzdan (pool) çeker.</li>
  <li><code>Block.cs</code>: Bireysel blokların grid koordinatlarını ve görsel durumlarını tutar.</li>
  <li><code>DeadlockManager.cs</code>: Hamle kalıp kalmadığını kontrol eder ve tahtayı yeniden karıştırır.</li>
  <li><code>UI_Manager.cs</code>: Oyun içi HUD, ayarlar ve paneller arası geçişleri yönetir.</li>
</ul>

<h2>📸 Ekran Görüntüleri</h2>

<table align="center">
  <tr>
    <td align="center">
      <img src="https://github.com/senin-yukledigin-gorsel-1-urlsi" width="400" alt="Ana Menü">
      <br>
      <i>Ana Menü ve Ayarlar</i>
    </td>
    <td align="center">
      <img src="https://github.com/senin-yukledigin-gorsel-2-urlsi" width="400" alt="Oynanış">
      <br>
      <i>Oyun İçi Oynanış</i>
    </td>
  </tr>
</table>

<h2>🛠️ Kurulum</h2>
<ol>
  <li>Projeyi bilgisayarınıza klonlayın veya ZIP olarak indirin.</li>
  <li>Unity Hub üzerinden <code>Add Project</code> diyerek klasörü seçin (Unity 2D projesi olarak açılması tavsiye edilir).</li>
  <li><code>Scenes</code> klasöründen <strong>MainMenuScene</strong>'i açın ve oynamaya başlayın.</li>
</ol>
