namespace DegerlendirmeSoruları
{
    using System;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            int müşteriNumarası = 15000000;

            ÇalıştırmaMotoru.KomutÇalıştır("MuhasebeModulu", "MaaşYatır", new object[] { müşteriNumarası });

            ÇalıştırmaMotoru.KomutÇalıştır("MuhasebeModulu", "YıllıkÜcretTahsilEt", new object[] { müşteriNumarası });

            ÇalıştırmaMotoru.BekleyenİşlemleriGerçekleştir();
        }
    }

    public class ÇalıştırmaMotoru
    {
        private static List<(string, string, object[])> bekleyenİşlemler = new List<(string, string, object[])>();

        public static object[] KomutÇalıştır(string modülSınıfAdı, string methodAdı, object[] inputs)
        {
            // Certain commands are executed immediately, others are deferred
            if (methodAdı == "MaaşYatır" || methodAdı == "YıllıkÜcretTahsilEt")
            {
                return Gerçekleştir(modülSınıfAdı, methodAdı, inputs);
            }
            else
            {
                // Store the command for later execution
                bekleyenİşlemler.Add((modülSınıfAdı, methodAdı, inputs));
                Console.WriteLine($"Komut {methodAdı} beklemeye alındı.");
                return null;
            }
        }

        public static void BekleyenİşlemleriGerçekleştir()
        {
            Console.WriteLine("Bekleyen işlemler gerçekleştiriliyor...");

            foreach (var işlem in bekleyenİşlemler)
            {
                Gerçekleştir(işlem.Item1, işlem.Item2, işlem.Item3);
            }

            // Clear the list after execution
            bekleyenİşlemler.Clear();
        }

        private static object[] Gerçekleştir(string modülSınıfAdı, string methodAdı, object[] inputs)
        {
            Type type = Type.GetType($"DegerlendirmeSoruları.{modülSınıfAdı}");
            if (type == null)
            {
                Console.WriteLine($"Modül bulunamadı: {modülSınıfAdı}");
                return null;
            }

            object instance = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodAdı, BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                Console.WriteLine($"Yöntem bulunamadı: {methodAdı}");
                return null;
            }

            return new object[] { method.Invoke(instance, inputs) };
        }
    }

    public class MuhasebeModülü
    {
        private void MaaşYatır(int müşteriNumarası)
        {
            // gerekli işlemler gerçekleştirilir.
            Console.WriteLine(string.Format("{0} numaralı müşterinin maaşı yatırıldı.", müşteriNumarası));
        }

        private void YıllıkÜcretTahsilEt(int müşteriNumarası)
        {
            // gerekli işlemler gerçekleştirilir.
            Console.WriteLine("{0} numaralı müşteriden yıllık kart ücreti tahsil edildi.", müşteriNumarası);
        }

        private void OtomatikÖdemeleriGerçekleştir(int müşteriNumarası)
        {
            // gerekli işlemler gerçekleştirilir.
            Console.WriteLine("{0} numaralı müşterinin otomatik ödemeleri gerçekleştirildi.", müşteriNumarası);
        }
    }

    public class Veritabanıİşlemleri
    {
        private static List<İşlemKaydı> işlemListesi = new List<İşlemKaydı>();

        // Save a deferred transaction
        public static void İşlemKaydet(string modülSınıfAdı, string methodAdı, object[] inputs)
        {
            işlemListesi.Add(new İşlemKaydı
            {
                İşlemId = Guid.NewGuid(),
                ModülSınıfAdı = modülSınıfAdı,
                MethodAdı = methodAdı,
                Parametreler = inputs,
                Durum = "Beklemede",
                OluşturulmaZamanı = DateTime.Now
            });

            Console.WriteLine($"İşlem kaydedildi: {methodAdı}");
        }

        // Get all pending transactions
        public static List<İşlemKaydı> BekleyenİşlemleriGetir()
        {
            return işlemListesi.Where(işlem => işlem.Durum == "Beklemede").ToList();
        }

        // Mark a transaction as completed
        public static void İşlemTamamlandı(Guid işlemId)
        {
            var işlem = işlemListesi.FirstOrDefault(i => i.İşlemId == işlemId);
            if (işlem != null)
            {
                işlem.Durum = "Tamamlandı";
                Console.WriteLine($"İşlem tamamlandı: {işlem.MethodAdı}");
            }
        }
    }

    // Class to represent a transaction record
    public class İşlemKaydı
    {
        public Guid İşlemId { get; set; }
        public string ModülSınıfAdı { get; set; }
        public string MethodAdı { get; set; }
        public object[] Parametreler { get; set; }
        public string Durum { get; set; }
        public DateTime OluşturulmaZamanı { get; set; }
    }

}



