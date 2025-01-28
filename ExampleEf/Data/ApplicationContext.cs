using ExampleEf.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleEf.Data
{
	public class ApplicationContext:DbContext
	{
		public DbSet<Urun> Urunler { get; set; }
		public DbSet<Kategori> Kategoriler { get; set; }
		public DbSet<Siparis> Siparisler { get; set; }
		public DbSet<SiparisDetay> SiparisDetaylar { get; set; }


		// Yapıcı ekle
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Urun>()
				.HasIndex(x => x.Adi);
		}

	}
}
