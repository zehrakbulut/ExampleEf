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

// Veritaban�n� g�nceller
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    context.Database.Migrate(); // Migrationlar� uygula
}

//Veri Ekleme ��lemi i�in
await SeedDatabaseAsync(app);

app.Run();

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

    // Kategoriler ekle
    if (!context.Kategoriler.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            context.Kategoriler.Add(new Kategori { Adi = $"Kategori {i}" });
        }
        await context.SaveChangesAsync();
    }

    // �r�nler ekle
    if (!context.Urunler.Any())
    {
        var random = new Random();
        for (int i = 1; i <= 1500; i++)
        {
            context.Urunler.Add(new Urun
            {
                Adi = $"�r�n {i}",
                Fiyat = random.Next(10, 5000),
                Stok = random.Next(1, 100),
                KategoriId = random.Next(1, 11) // 1 ile 10 aras�nda bir kategori se�iliyor
            });
        }
        await context.SaveChangesAsync();
    }

    // Sipari�ler ekle
    if (!context.Siparisler.Any())
    {
        for (int i = 1; i <= 500; i++) // 500 sipari� olu�turuluyor
        {
            context.Siparisler.Add(new Siparis
            {
                MusteriAdi = $"M��teri {i}",
                Tarih = DateTime.Now.AddDays(-i) // Sipari� tarihleri ge�mi�te farkl� g�nler
            });
        }
        await context.SaveChangesAsync();
    }

    // Sipari� Detaylar� ekle
    if (!context.SiparisDetaylar.Any())
    {
        var random = new Random();
        var urunler = context.Urunler.ToList();
        var siparisler = context.Siparisler.ToList();

        foreach (var siparis in siparisler)
        {
            var detaySayisi = random.Next(1, 5); // Her sipari� i�in 1-5 detay
            for (int j = 0; j < detaySayisi; j++)
            {
                var urun = urunler[random.Next(urunler.Count)];
                var miktar = random.Next(1, 10); // Her �r�n i�in 1-10 adet miktar
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
    }

    Console.WriteLine("T�m veriler ba�ar�yla eklendi!");
}

////------------------------------------------\\
