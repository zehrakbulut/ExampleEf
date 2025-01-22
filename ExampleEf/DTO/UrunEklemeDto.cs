using ExampleEf.Models;

namespace ExampleEf.DTO
{
	public class UrunEklemeDto
	{
		public string Adi { get; set; }
		public decimal Fiyat { get; set; }
		public int Stok { get; set; }
		public int KategoriId { get; set; }
	}
}
