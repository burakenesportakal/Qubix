<img width="1546" height="871" alt="2" src="https://github.com/user-attachments/assets/5637e854-b0a7-4408-9f8c-007a05c29d78" /><h1 align="center">🧩Qubix (Unity)</h1>

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

<img width="1552" height="868" alt="1" src="https://github.com/user-attachments/assets/ef8284b8-4907-4a66-8f50-557e829776db" />
![<img width="1551" height="870" alt="3" src="https://github.com/user-attachments/assets/05cdee42-fba5-43f5-8778-adc7870e438e" />
<img width="1546" height="871" alt="2" src="https://github.com/user-attachments/assets/b0f799dc-45f9-4122-97bc-d6d7794b4dad" />
Uploading 2.PNG…]()<img width="1551" height="870" alt="3" src="https://github.com/user-attachments/assets/9f7cca68-aea9-4c66-ad74-4d264a567242" />
<img width="1552" height="868" alt="1" src="https://github.com/user-attachments/assets/27d3d7ec-b8f2-4f93-aff2-71d0c7649c79" />
<img width="1546" height="871" alt="2" src="https://github.com/user-attachments/assets/7d9dd786-81e3-4bbd-8eeb-8d630916cce6" />
<img width="1552" height="868" alt="1" src="https://github.com/user-attachments/assets/dd5940a3-33d9-4811-b07c-df3b817ea391" />
<img width="1551" height="870" alt="3" src="https://github.com/user-attachments/assets/7a4fbf4f-71f4-45dd-8714-3c227eae97b9" />



<h2>🛠️ Kurulum</h2>
<ol>
  <li>Projeyi bilgisayarınıza klonlayın veya ZIP olarak indirin.</li>
  <li>Unity Hub üzerinden <code>Add Project</code> diyerek klasörü seçin (Unity 2D projesi olarak açılması tavsiye edilir).</li>
  <li><code>Scenes</code> klasöründen <strong>MainMenuScene</strong>'i açın ve oynamaya başlayın.</li>
</ol>
