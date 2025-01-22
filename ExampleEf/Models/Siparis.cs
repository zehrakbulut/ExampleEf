namespace ExampleEf.Models
{
	public class Siparis
	{
        public int Id { get; set; }
        public string MusteriAdi { get; set; }
        public DateTime Tarih { get; set; }
        public List<SiparisDetay> SiparisDetays { get; set; }
    }
}
