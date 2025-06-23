using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using PcAnketProject.CerenUI.Models;

namespace PcAnketProject.WebUI.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IConfiguration _configuration;

        public InventoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(string search, List<int> selectedDonanimlar)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // Donanım listesi filtre (checkbox) için
            var allDonanimlar = await connection.QueryAsync<DonanimListViewModel>("SELECT Id, Ad FROM lu_donanim");
            ViewBag.AllDonanimlar = allDonanimlar.ToList();
            ViewBag.CurrentSearch = search;
            ViewBag.SelectedDonanimlar = selectedDonanimlar;

            var sql = @"
    SELECT 
        dp.NufusId,
        dp.Tc,
        dp.Ad, 
        dp.Soyad, 
        dp.Unvan, 
        dp.Birim,
        STUFF((
            SELECT ', ' + ld2.Ad
            FROM tx_personelDonanim AS tpd2
            JOIN lu_donanim AS ld2 ON ld2.Id = tpd2.DonanimId
            WHERE tpd2.Aktif = 1 AND tpd2.NufusId = dp.NufusId
            FOR XML PATH(''), TYPE
        ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Donanimlar
    FROM PcAnketTest.dbo.dt_personel AS dp
    WHERE (
        @Search IS NULL OR
        dp.Tc LIKE '%' + @Search + '%' OR
        dp.Ad LIKE '%' + @Search + '%' OR
        dp.Soyad LIKE '%' + @Search + '%' OR
        dp.Birim LIKE '%' + @Search + '%'
    )";

            var personeller = (await connection.QueryAsync<InventoryPersonelViewModel>(sql, new { Search = search })).ToList();

            // Donanım filtrelemesi
            if (selectedDonanimlar != null && selectedDonanimlar.Any())
            {
                personeller = personeller.Where(p =>
                {
                    var kisininDonanimAdlari = p.Donanimlar?
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(d => d.Trim())
                        .ToList() ?? new List<string>();

                    // Kullanıcının seçtiği donanım adları (checkboxlar)
                    var secilenDonanimAdlari = allDonanimlar
                        .Where(d => selectedDonanimlar.Contains(d.Id))
                        .Select(d => d.Ad.Trim())
                        .ToList();

                    // Kişinin sahip olduğu donanım adları ile seçilenler birebir eşleşmeli
                    return kisininDonanimAdlari.Count == secilenDonanimAdlari.Count &&
                           !secilenDonanimAdlari.Except(kisininDonanimAdlari).Any();
                }).ToList();
            }



            return View(personeller);
        }

        public async Task<IActionResult> Detail(int kullaniciID)
        {
            string cs = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(cs);

            var sql = @"  SELECT 
    dp.NufusId, dp.Tc, dp.Ad, dp.Soyad, dp.Unvan, dp.Birim,
    ld.Ad AS DonanimAd,
    ldd.Ad AS DonanimDurum,
    il.IslemNo,
    il.Aciklama,
    il.Tarih AS IslemTarihi,
    ku.Ad + ' ' + ku.Soyad AS YetkiliAdSoyad,
    tpd.Aktif
  FROM dt_personel dp
  JOIN tx_personelDonanim tpd ON dp.NufusId = tpd.NufusId
  JOIN lu_donanim ld ON ld.Id = tpd.DonanimId
  JOIN tx_islemLog il ON il.IslemNo = tpd.IslemNo
  LEFT JOIN dt_kullanici ku ON ku.ID = il.YetkiliId
  JOIN lu_donanimDurum ldd ON tpd.DonanimDurum = ldd.Id
  WHERE dp.NufusId = @NufusId
  ORDER BY il.Tarih DESC";

            var detayList = await connection.QueryAsync<InventoryDetailViewModel>(sql, new { NufusId = kullaniciID });
            // ——— kisi bilgileri boşsa bile personel tablosundan çek
            if (!detayList.Any())
            {
                var kisiSql = @"SELECT TOP 1 
                        dp.NufusId, dp.Tc, dp.Ad, dp.Soyad, dp.Unvan, dp.Birim
                    FROM dt_personel dp
                    WHERE dp.NufusId = @NufusId";
                var fallback = await connection.QueryFirstOrDefaultAsync<InventoryDetailViewModel>(kisiSql, new { NufusId = kullaniciID });
                ViewBag.Kisi = fallback;
            }
            else
            {
                ViewBag.Kisi = detayList.FirstOrDefault();
            }
            var aktifDonanimSql = @"
SELECT 
    ld.Ad AS DonanimAd,
    ldd.Ad AS DonanimDurum,
    tpd.IslemNo AS IslemNo
FROM tx_personelDonanim tpd
JOIN lu_donanim ld ON ld.Id = tpd.DonanimId
JOIN lu_donanimDurum ldd ON ldd.Id = tpd.DonanimDurum
WHERE tpd.Aktif = 1 AND tpd.NufusId = @NufusId";

            var aktifDonanimlar = (await connection.QueryAsync<ActiveDonanimViewModel>(aktifDonanimSql, new { NufusId = kullaniciID }))
                                  .ToList();

            ViewBag.AktifDonanimlar = aktifDonanimlar;


            // ——— donanım durumları (dropdown için)
            var durumList = await connection.QueryAsync<DonanimDurumViewModel>(
                "SELECT Id, Ad FROM lu_donanimDurum");
            ViewBag.DurumList = durumList.ToList();

            // ——— donanım durumları (dropdown için)
            // ——— donanım listesi (dropdown için)
            var donanimList = await connection.QueryAsync<DonanimListViewModel>(
                "SELECT Id, Ad FROM lu_donanim");
            ViewBag.DonanimList = donanimList.ToList(); // ← düzeltildi


            // ——— diğer atamalar
            //ViewBag.Kisi = detayList.FirstOrDefault();
            var grouped = detayList
              .GroupBy(x => x.IslemNo)
              .ToDictionary(g => g.Key, g => g.ToList());

            // Mevcut oturuma ait işlemNo
            string currentIslemNo = HttpContext.Session.GetString("CurrentIslemNo");

            if (!string.IsNullOrEmpty(currentIslemNo))
            {
                var aciklama = await connection.ExecuteScalarAsync<string>(
                    @"SELECT TOP 1 Aciklama FROM tx_islemLog 
          WHERE IslemNo = @IslemNo", new { IslemNo = currentIslemNo });

                ViewBag.CurrentAciklama = aciklama ?? "";
            }
            else
            {
                ViewBag.CurrentAciklama = "";
            }


            return View(grouped);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDonanim(int donanimId, int kullaniciId)
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(cs);

            // YetkiliId'yi Session'dan al
            int? yetkiliId = HttpContext.Session.GetInt32("KullaniciID");
            if (yetkiliId == null)
            {
                // Giriş yapılmamışsa tekrar girişe yönlendir
                return RedirectToAction("Giris", "Login");
            }

            // IslemNo üretimi
            string islemNo;
            if (HttpContext.Session.GetString("CurrentIslemNo") != null)
            {
                islemNo = HttpContext.Session.GetString("CurrentIslemNo");
            }
            else
            {
                islemNo = $"{kullaniciId}-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";
                HttpContext.Session.SetString("CurrentIslemNo", islemNo);
            }

            // tx_islemLog kaydı
            string insertIslemLogSql = @"
IF NOT EXISTS (SELECT 1 FROM tx_islemLog WHERE IslemNo = @IslemNo)
BEGIN
    INSERT INTO tx_islemLog (IslemNo, YetkiliId, Aciklama, Tarih, NufusId)
    VALUES (@IslemNo, @YetkiliId, @Aciklama, GETDATE(), @NufusId)
END";

            await connection.ExecuteAsync(insertIslemLogSql, new
            {
                IslemNo = islemNo,
                YetkiliId = yetkiliId.Value,
                Aciklama = "Donanım eklendi",
                NufusId = kullaniciId
            });

            // tx_personelDonanim kaydı
            string insertDonanimSql = @"
INSERT INTO tx_personelDonanim (NufusId, DonanimId, Tarih, IslemNo, DonanimDurum, Aktif)
VALUES (@NufusId, @DonanimId, GETDATE(), @IslemNo, 1, 1)";

            await connection.ExecuteAsync(insertDonanimSql, new
            {
                NufusId = kullaniciId,
                DonanimId = donanimId,
                IslemNo = islemNo
            });

            return RedirectToAction("Detail", new { kullaniciID = kullaniciId });
        }



[HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KaydetDurumlar(int kullaniciId, string aciklama, List<DonanimDurumUpdateViewModel> donanimlar)
    {
        var cs = _configuration.GetConnectionString("DefaultConnection");
        using var connection = new SqlConnection(cs);

        int? yetkiliId = HttpContext.Session.GetInt32("KullaniciID");
        if (yetkiliId == null)
            return RedirectToAction("Giris", "Login");

        string currentIslemNo = HttpContext.Session.GetString("CurrentIslemNo");
        string today = DateTime.Now.ToString("yyyyMMdd");
        string newIslemNo = $"{yetkiliId}-{today}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";

        bool sameSession = currentIslemNo != null && currentIslemNo.StartsWith($"{kullaniciId}-{today}");
        string finalIslemNo = sameSession ? currentIslemNo : newIslemNo;

        if (!sameSession)
            HttpContext.Session.SetString("CurrentIslemNo", finalIslemNo);

        // ————— TRANSACTION SCOPE başlıyor
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                // ——— İşlem logu insert veya update
                string islemLogSql = sameSession
                    ? @"UPDATE tx_islemLog SET Aciklama = @Aciklama WHERE IslemNo = @IslemNo"
                    : @"INSERT INTO tx_islemLog (IslemNo, YetkiliId, Aciklama, Tarih, NufusId)
                   VALUES (@IslemNo, @YetkiliId, @Aciklama, GETDATE(), @NufusId)";

                await connection.ExecuteAsync(islemLogSql, new
                {
                    IslemNo = finalIslemNo,
                    YetkiliId = yetkiliId,
                    Aciklama = aciklama,
                    NufusId = kullaniciId
                });

                foreach (var item in donanimlar)
                {
                    var donanimId = await connection.ExecuteScalarAsync<int?>(
                        "SELECT Id FROM lu_donanim WHERE Ad = @Ad", new { Ad = item.DonanimAd });

                    if (donanimId == null)
                        continue;

                    var currentDurumId = await connection.ExecuteScalarAsync<int>(
                        @"SELECT TOP 1 DonanimDurum FROM tx_personelDonanim 
                      WHERE NufusId = @NufusId AND DonanimId = @DonanimId AND Aktif = 1 
                      ORDER BY Tarih DESC",
                        new { NufusId = kullaniciId, DonanimId = donanimId });

                    if (currentDurumId == item.YeniDurumId)
                        continue;

                    // ——— Mevcut kaydı pasifleştir
                    await connection.ExecuteAsync(
                        @"UPDATE tx_personelDonanim 
                      SET Aktif = 0 
                      WHERE NufusId = @NufusId AND DonanimId = @DonanimId AND Aktif = 1",
                        new { NufusId = kullaniciId, DonanimId = donanimId });

                    if (item.YeniDurumId == 1)
                    {
                        // ——— Yeni durum 1 ise: aktif insert
                        await connection.ExecuteAsync(
                            @"INSERT INTO tx_personelDonanim 
                          (NufusId, DonanimId, Tarih, IslemNo, DonanimDurum, Aktif)
                          VALUES (@NufusId, @DonanimId, GETDATE(), @IslemNo, @YeniDurumId, 1)",
                            new
                            {
                                NufusId = kullaniciId,
                                DonanimId = donanimId,
                                IslemNo = finalIslemNo,
                                item.YeniDurumId
                            });
                    }
                    else
                    {
                        // ——— Yeni durum ≠ 1 ise: pasif insert
                        await connection.ExecuteAsync(
                            @"INSERT INTO tx_personelDonanim 
                          (NufusId, DonanimId, Tarih, IslemNo, DonanimDurum, Aktif)
                          VALUES (@NufusId, @DonanimId, GETDATE(), @IslemNo, @YeniDurumId, 0)",
                            new
                            {
                                NufusId = kullaniciId,
                                DonanimId = donanimId,
                                IslemNo = finalIslemNo,
                                item.YeniDurumId
                            });
                    }
                }

                scope.Complete(); // ✅ Her şey başarılı → commit
            }
            catch
            {
                // ❌ hata varsa rollback (otomatik olur)
                throw;
            }
        }

        return RedirectToAction("Detail", new { kullaniciID = kullaniciId });
    }


        public async Task<IActionResult> DefinitionTable()
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var donanimlar = await conn.QueryAsync<DonanimListViewModel>("SELECT Id, Ad FROM lu_donanim");
            var durumlar = await conn.QueryAsync<DonanimDurumViewModel>("SELECT Id, Ad FROM lu_donanimDurum");

            ViewBag.Donanimlar = donanimlar.ToList();
            ViewBag.Durumlar = durumlar.ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddLuDonanim(string ad)
        {
            ad = ad?.Trim();

            if (string.IsNullOrWhiteSpace(ad))
            {
                TempData["Error"] = "Ad alanı boş olamaz.";
                return RedirectToAction("DefinitionTable");
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var existing = await conn.QueryFirstOrDefaultAsync<string>(
                "SELECT Ad FROM lu_donanim WHERE LOWER(LTRIM(RTRIM(Ad))) = LOWER(LTRIM(RTRIM(@Ad)))", new { Ad = ad });

            if (existing != null)
            {
                TempData["Error"] = "Aynı adlı bir donanım zaten mevcut.";
                return RedirectToAction("DefinitionTable");
            }

            await conn.ExecuteAsync("INSERT INTO lu_donanim (Ad) VALUES (@Ad)", new { Ad = ad });
            TempData["Success"] = "Donanım başarıyla eklendi.";
            return RedirectToAction("DefinitionTable");
        }


        [HttpPost]
        public async Task<IActionResult> AddLuDurum(string ad)
        {
            ad = ad?.Trim();

            if (string.IsNullOrWhiteSpace(ad))
            {
                TempData["Error"] = "Ad alanı boş olamaz.";
                return RedirectToAction("DefinitionTable");
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var existing = await conn.QueryFirstOrDefaultAsync<string>(
                "SELECT Ad FROM lu_donanimDurum WHERE LOWER(LTRIM(RTRIM(Ad))) = LOWER(LTRIM(RTRIM(@Ad)))", new { Ad = ad });

            if (existing != null)
            {
                TempData["Error"] = "Aynı adlı bir durum zaten mevcut.";
                return RedirectToAction("DefinitionTable");
            }

            await conn.ExecuteAsync("INSERT INTO lu_donanimDurum (Ad) VALUES (@Ad)", new { Ad = ad });
            TempData["Success"] = "Durum başarıyla eklendi.";
            return RedirectToAction("DefinitionTable");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateLuDonanim(int id, string ad)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.ExecuteAsync("UPDATE lu_donanim SET Ad = @Ad WHERE Id = @Id", new { Id = id, Ad = ad });
            return RedirectToAction("DefinitionTable");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLuDurum(int id, string ad)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.ExecuteAsync("UPDATE lu_donanimDurum SET Ad = @Ad WHERE Id = @Id", new { Id = id, Ad = ad });
            return RedirectToAction("DefinitionTable");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string search, List<int> selectedDonanimlar)
        {
            return View();



        }


    }
}
