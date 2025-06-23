using Dapper;
using Microsoft.AspNetCore.Mvc;
using PcAnketProject.CerenUI.Models;
using System.Data.SqlClient;

namespace PcAnketProject.CerenUI.Controllers
{
    public class PersonalController : Controller
    {
        private readonly IConfiguration _configuration;

        public PersonalController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<ActionResult> Add(string search)
        {
            var cs = _configuration.GetConnectionString("PauDefaultConnection");
            using var connection = new SqlConnection(cs);

            List<PersonelAramaViewModel> liste = new List<PersonelAramaViewModel>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string sql = @"
            SELECT 
                f.nufus_id,
                f.tcNo,
                f.ad,
                f.soyad,
                f.unvanGorevId,
                f.unvanGorev,
                f.birim_id,
                ff.id AS AnaBirimId,
                ff.adTr AS AnaBirimAd
            FROM Genel.dt_nufus dn
            CROSS APPLY PERSONEL.fnt_nufusIdyeGoreKadroBirim(dn.id) f
            OUTER APPLY PERSONEL.fnt_birimIdyeAnaBirimAd(f.birim_id, 2, 0) ff
            WHERE
                f.tcNo LIKE '%' + @search + '%' OR
                f.ad LIKE '%' + @search + '%' OR
                f.soyad LIKE '%' + @search + '%' OR
                ff.adTr LIKE '%' + @search + '%'
        ";

                var result = await connection.QueryAsync<PersonelAramaViewModel>(sql, new { search });
                liste = result.ToList();
            }

            ViewBag.Search = search;
            return View(liste);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertToSystem(DtPersonelInsertViewModel model)
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(cs);

            // NufusId sistemde var mı kontrol et
            var mevcut = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM dt_personel WHERE NufusId = @NufusId",
                new { model.NufusId });

            if (mevcut > 0)
            {
                // Sadece güncelle (Tc ve Durum sabit kalsın)
                string updateSql = @"
            UPDATE dt_personel
            SET Ad = @Ad,
                Soyad = @Soyad,
                UnvanId = @UnvanId,
                Unvan = @Unvan,
                BirimId = @BirimId,
                Birim = @Birim
            WHERE NufusId = @NufusId";

                await connection.ExecuteAsync(updateSql, model);
                TempData["Success"] = "Kayıt güncellendi.";
            }
            else
            {
                string insertSql = @"
            INSERT INTO dt_personel
               (NufusId, Tc, Ad, Soyad, UnvanId, Unvan, BirimId, Birim, Durum)
            VALUES
               (@NufusId, @Tc, @Ad, @Soyad, @UnvanId, @Unvan, @BirimId, @Birim, @Durum)";

                await connection.ExecuteAsync(insertSql, model);
                TempData["Success"] = "Kayıt başarıyla eklendi.";
            }

            return RedirectToAction("Add");
        }




    }
}
