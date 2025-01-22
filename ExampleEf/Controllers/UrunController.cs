using ExampleEf.Data;
using ExampleEf.DTO;
using ExampleEf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExampleEf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UrunController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUrun()
        {
            var urunler = await _context.Urunler.ToListAsync();
            return Ok(urunler);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUrunById(int id)
        {
            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
                return NotFound();

            return Ok(urun);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUrun(UrunEklemeDto model)
        {
            if (model == null)
                return BadRequest();

            Urun urun = new Urun()
            {
                Adi = model.Adi,
                Fiyat = model.Fiyat,
                Stok = model.Stok,
                KategoriId = model.KategoriId
            };

            await _context.Urunler.AddAsync(urun); // Nesneyi izleme durumuna ekler
            var saveResult = await _context.SaveChangesAsync(); // Değişiklikleri veritabanına uygular

            return Ok(urun);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUrun(int id, UrunEklemeDto model)
        {
            if (model == null)
                return BadRequest();

            var urun = await _context.Urunler.FindAsync(id);

            if (urun == null)
                return NotFound();

            urun.Adi = model.Adi;
            urun.Fiyat = model.Fiyat;
            urun.Stok = model.Stok;
            urun.KategoriId = model.KategoriId;

            var saveResult = await _context.SaveChangesAsync();
            return Ok(urun);
        }


        //Tüm ürünlerin adlarını ve fiyatlarını listele. Fakat, yalnızca fiyatı 1000 TL'den büyük olan ürünleri göster. Sonuçları fiyatlarına göre azalan sırayla sıralayın.
        [HttpGet("AdFiyatListele")]
        public async Task<IActionResult> GetList()
        {
            var urunler = await _context.Urunler
                .Where(u => u.Fiyat > 1000)
                .OrderByDescending(u => u.Fiyat)
                .Select(u => new { u.Adi, u.Fiyat })
                .ToListAsync();
            return Ok(urunler);
        }


        //Bir kullanıcı, siparişlerinin toplam fiyatını görmek istiyor. Sipariş detaylarında her bir ürün için miktar ve fiyat bilgisi var. Bu bilgileri kullanarak, her bir siparişin toplam fiyatını hesaplayın ve sonuçları döndürün.
        [HttpGet("SiparisTopFiyat")]
        public async Task<IActionResult> GetSiparisToplamFiyat()
        {
            var siparislerToplamFiyat = await _context.Siparisler
                .Select(u => new
                {
                    SiparisId = u.Id,
                    ToplamFiyat = u.SiparisDetays.Sum(u => u.Adet * u.ToplamFiyat)
                }).ToListAsync();

            return Ok(siparislerToplamFiyat);
        }




        //Tüm ürünlerin adını ve stok miktarını listeleyin.
        [HttpGet("UrunleriListele")]
        public async Task<IActionResult> GetUrunleriListele()
        {
            var urunleriListele = await _context.Urunler
                .Select(u => new
                {
                    Adi = u.Adi,
                    Stok = u.Stok
                }).ToListAsync();
            return Ok(urunleriListele);
        }

        //Stok miktarı 50'den az olan ürünleri listeleyin.
        [HttpGet("Stok50DenAzListele")]
        public async Task<IActionResult> GetStokListele()
        {
            var stokListele = await _context.Urunler
                .Select(u => new
                {
                    Adi = u.Adi,
                    Stok = u.Stok
                }).Where(u => u.Stok < 50).ToListAsync();
            return Ok(stokListele);
        }

        //ürünlerin sadece adlarını ve stok miktarlarını değil, aynı zamanda fiyatlarını da listeleyelim. Ama yalnızca fiyatı 500 TL'den yüksek olan ürünleri listeleyeceğiz.
        [HttpGet("Fiyati500DenYükleskleriListele")]
        public async Task<IActionResult> GetListele()
        {
            var urunleriListele = await _context.Urunler
                .Where(u => u.Fiyat > 500 && u.Stok < 50)
                .Select(u => new
                {
                    Adi = u.Adi,
                    Stok = u.Stok,
                    Fiyat = u.Fiyat
                }).ToListAsync();
            return Ok(urunleriListele);
        }



        //soru1
        //Veritabanındaki "Urunler" tablosundan stok miktarı 50'den büyük olan ürünleri listele.
        [HttpGet("Stok50DenBüyükseListele")]
        public async Task<IActionResult> GetStok50DenBuyukseListele()
        {
            var stokListele = await _context.Urunler
                .Where(u => u.Stok > 50).ToListAsync();
            return Ok(stokListele);
        }

        //soru2
        //"Urunler" tablosundan, fiyatı artan sırayla ürünleri listele. Ancak, fiyatlar eşitse, ürün adını alfabetik sıraya göre listele.
        [HttpGet("FiyatArtanSıraUrunleriListele")]
        public async Task<IActionResult> GetFiyatArtanSiraUrunListele()
        {
            var fiyatArtanSiraUrunListele = await _context.Urunler
                .OrderBy(u => u.Fiyat)
                .ThenBy(u => u.Adi)
                .ToListAsync();
            return Ok(fiyatArtanSiraUrunListele);
        }

        //soru3
        //"Urunler" tablosundaki toplam ürün sayısını öğren.
        [HttpGet("ToplamUrunSayisi")]
        public async Task<IActionResult> GetToplamUrunSayisi()
        {
            var toplamUrunSayisi = await _context.Urunler
                .CountAsync();
            return Ok(toplamUrunSayisi);
        }

        //soru4
        //"Urunler" tablosundaki en pahalı ve en ucuz ürünü listele.
        [HttpGet("MaxUrunMinUrun")]
        public async Task<IActionResult> GetMaxUrunMinUrun()
        {
            var maxUrun = await _context.Urunler
                .MaxAsync(u => u.Fiyat);
            var minUrun = await _context.Urunler
                .MinAsync(u => u.Fiyat);

            return Ok($"Max: {maxUrun} - Min: {minUrun}");
        }

        //soru5
        //"Urunler" tablosunda, fiyatı 1000'den fazla olan herhangi bir ürün olup olmadığını kontrol et
        [HttpGet("fiyati1000DenBuyukUrunVarMi")]
        public async Task<IActionResult> GetFiyati1000DenBuyukUrunVarMi()
        {
            var fiyati1000denBuyukUrunVarMi = await _context.Urunler
                .AnyAsync(u => u.Fiyat > 1000);
            return Ok(fiyati1000denBuyukUrunVarMi);
        }

        //soru6
        //"Urunler" tablosundaki tüm ürünlerin fiyatlarının toplamını ve ortalamasını hesapla.
        [HttpGet("toplamVeFiyatOrtalamaHesaplama")]
        public async Task<IActionResult> GetToplamVeFiyatOrtalamaHesaplama()
        {
            var toplamHesaplama = await _context.Urunler
                .SumAsync(u => u.Fiyat);

            var ortalamaHesaplama = await _context.Urunler
                .AverageAsync(u => u.Fiyat);

            return Ok($"Toplam hesaplama: {toplamHesaplama} - Ortalama hesaplama: {ortalamaHesaplama}");
        }

        //soru7
        //"Urunler" tablosunda, fiyatı 1000'den fazla olan ürünlerin sayısını hesapla.
        [HttpGet("Fiyati1000DenFazlaUrunlerinSayisi")]
        public async Task<IActionResult> GetFiyat1000DenFazlaUrunlerinSayisi()
        {
            var fiyat1000DenFazlaUrunlerinSayisi = await _context.Urunler
                .Where(u => u.Fiyat > 1000).CountAsync();
            return Ok(fiyat1000DenFazlaUrunlerinSayisi);
        }

        //soru8
        //"Urunler" tablosundaki en pahalı ürünü bul ve fiyatını döndür. -1.YÖNTEM
        [HttpGet("enPahalıUrunuBul")]
        public async Task<IActionResult> GetEnPahalıUrunuBul()
        {
            var enPahalıUrunuBul = await _context.Urunler
                .MaxAsync(u => u.Fiyat);
            return Ok(enPahalıUrunuBul);
        }

        //soru8
        //"Urunler" tablosundaki en pahalı ürünü bul ve fiyatını döndür. -2.YÖNTEM 
        [HttpGet("enPahalıUrun")]
        public async Task<IActionResult> GetEnPahaliUrun()
        {
            var enPahalıUrun = await _context.Urunler
                .OrderByDescending(u => u.Fiyat)
                .FirstOrDefaultAsync();
            return Ok(enPahalıUrun);
        }

        //soru9
        //"Urunler" tablosundaki ürünlerin toplam fiyatını ve ortalama fiyatını hesapla. 
        [HttpGet("toplamVeOrtFiyat")]
        public async Task<IActionResult> GetToplamVeOrtFiyat()
        {
            var toplamHesapla = await _context.Urunler
                .SumAsync(u => u.Fiyat);

            var ortHesapla = await _context.Urunler
                .AverageAsync(u => u.Fiyat);
            return Ok($"Toplam: {toplamHesapla} - Ortalama: {ortHesapla}");
        }

        //soru10
        //"Urunler" tablosunda, fiyatı 500'den büyük ve stok miktarı 100'den az olan ürünlerin toplam fiyatını hesapla.
        [HttpGet("fiyati500DenBuyukVeStok100DenAzOlanlariListele")]
        public async Task<IActionResult> GetFiyat500DenBuyukVeStok100DenAzOlanlariListele()
        {
            var fiyati500DenBuyukStok100DenAz = await _context.Urunler
                .Where(u => u.Fiyat > 500 && u.Stok < 100)
                .SumAsync(u => u.Fiyat);
            return Ok(fiyati500DenBuyukStok100DenAz);
        }

        //soru11 -- zor
        //"Urunler" tablosunda, benzersiz kategoriler içinde stok miktarı 10'dan az olan ürünlerin sayısını bul.
        [HttpGet("benzersizUrunler")]
        public async Task<IActionResult> GetBenzersizUrunler()
        {
            var benzersizUrunler = await _context.Urunler
                .Where(u => u.Stok < 10)
                .Select(u => new { u.KategoriId, u.Adi })
                .Distinct()
                .CountAsync();
            return Ok(benzersizUrunler);
        }

        //soru12 --zor
        //Fiyatı 2000’den büyük olan ürünlerden, her benzersiz kategori için ürün isimlerini listeleyerek kaç farklı ürün olduğunu hesaplayın.
        [HttpGet("benzersizUrunSayisi")]
        public async Task<IActionResult> GetBenzersizUrunSayisi()
        {
            var benzersizUrunSayisi = await _context.Urunler
                .Where(u => u.Fiyat > 2000)
                .Select(u => new { u.KategoriId, u.Adi })
                .Distinct()
                .CountAsync();
            return Ok(benzersizUrunSayisi);
        }

        //soru13 --- zor
        //Her kategorideki ürünlerin toplam stoklarını hesaplayın ve sonucu kategori adına göre sıralayın
        [HttpGet("urunlerinToplamStoklari")]
        public async Task<IActionResult> GetUrunlerinToplamStoklari()
        {
            var urunlerinToplamStoklari = await _context.Urunler
                .GroupBy(u => u.KategoriId)
                .Select(u => new
                {
                    KategoriId = u.Key,
                    ToplamStok = u.Sum(u=>u.Stok)
                })
                .OrderBy(u=>u.KategoriId)
                .ToListAsync();
            return Ok(urunlerinToplamStoklari);
        }

        //soru14
        //Her kategorideki ürünlerin toplam fiyatlarını hesaplayın ve sonucu kategori adına göre alfabetik olarak sıralayın. Sonuçta, kategori adı ve toplam fiyat gösterilsin.
        [HttpGet("UrunlerinToplamFiyatlariniSirala")]
        public async Task<IActionResult> GetUrunlerinToplamFiyatlariniSirala()
        {
            var urunlerinToplamFiyatlariniSirala = await _context.Urunler
                .GroupBy(u => u.KategoriId)
                .Select(u => new
                {
                    Adi = u.Key,
                    ToplamFiyat = u.Sum(u => u.Fiyat)
                })
                .OrderBy(u => u.Adi).ToListAsync();
            return Ok(urunlerinToplamFiyatlariniSirala);
        }





    }
}
