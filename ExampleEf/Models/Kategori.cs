namespace ExampleEf.Models
{
	public class Kategori
	{
        public int Id { get; set; }
        public string Adi { get; set; }
        public List<Urun> Uruns { get; set; }
    }
}
