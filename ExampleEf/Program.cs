using ExampleEf.Data;
using ExampleEf.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>();

// Add DbContext to DI container
builder.Services.AddDbContext<ApplicationContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Veri Ekleme Ýþlemi için
await SeedDatabaseAsync(app);

app.Run();

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

    // Mevcut verilerin maksimum ID deðerlerini bul
    var maxUrunId = context.Urunler.Any() ? context.Urunler.Max(u => u.Id) : 0;
    var maxSiparisId = context.Siparisler.Any() ? context.Siparisler.Max(s => s.Id) : 0;

    // Ürünler ekle
    if (context.Urunler.Count() < 3000000)
    {
        var random = new Random();
        var yeniUrunSayisi = 3000000 - context.Urunler.Count();

        for (int i = 1; i <= yeniUrunSayisi; i++)
        {
            context.Urunler.Add(new Urun
            {
                Adi = $"Ürün {maxUrunId + i}",
                Fiyat = random.Next(10, 5000),
                Stok = random.Next(1, 100),
                KategoriId = random.Next(1, 11) // 1 ile 10 arasýnda kategori
            });

            // Performansý artýrmak için toplu ekleme
            if (i % 10000 == 0) // Her 10 bin üründe bir veritabanýna kaydet
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"{i} ürün eklendi.");
            }
        }
        await context.SaveChangesAsync();
        Console.WriteLine("Ürünler baþarýyla eklendi!");
    }

    // Sipariþler ekle
    if (context.Siparisler.Count() < 20000)
    {
        var yeniSiparisSayisi = 20000 - context.Siparisler.Count();

        for (int i = 1; i <= yeniSiparisSayisi; i++)
        {
            context.Siparisler.Add(new Siparis
            {
                MusteriAdi = $"Müþteri {maxSiparisId + i}",
                Tarih = DateTime.Now.AddDays(-i)
            });

            if (i % 1000 == 0) // Her 1000 sipariþte bir kaydet
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"{i} sipariþ eklendi.");
            }
        }
        await context.SaveChangesAsync();
        Console.WriteLine("Sipariþler baþarýyla eklendi!");
    }

    // Sipariþ Detaylarý ekle
    if (context.SiparisDetaylar.Any())
    {
        var random = new Random();
        var urunler = context.Urunler.ToList();
        var siparisler = context.Siparisler.ToList();

        foreach (var siparis in siparisler)
        {
            var detaySayisi = random.Next(1, 5); // Her sipariþ için 1-5 detay
            for (int j = 0; j < detaySayisi; j++)
            {
                var urun = urunler[random.Next(urunler.Count)];
                var miktar = random.Next(1, 10); // Her ürün için 1-10 adet miktar
                var toplamFiyat = urun.Fiyat * miktar;

                context.SiparisDetaylar.Add(new SiparisDetay
                {
                    SiparisId = siparis.Id,
                    UrunId = urun.Id,
                    Adet = miktar,
                    ToplamFiyat = toplamFiyat
                });
            }
        }
        await context.SaveChangesAsync();
        Console.WriteLine("Sipariþ detaylarý baþarýyla eklendi!");
    }
}


////------------------------------------------\\












