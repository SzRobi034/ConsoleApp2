using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static Program;

class Program
{
   
    public class Kiado
    {
        public string nev { get; set; }
        public int kiadas_ev { get; set; }
        public string orszag { get; set; }
    }

    public class Konyv
    {
        public string cim { get; set; }
        public string[] szerzok { get; set; }
        public Kiado kiado { get; set; }
        public int oldalszam { get; set; }
        public string[] kategoriak { get; set; }
        public int ar { get; set; }
        public bool elkeszult { get; set; }
    }

    public class Konyvtar
    {
        public List<Konyv> konyvtar { get; set; }
    }

    static void Main(string[] args)
    {
        try
        {
            
            string json = File.ReadAllText("C: \Users\sziliir\source\repos\ConsoleApp2\ConsoleApp2\konyvek.json");
            var konyvtar = JsonSerializer.Deserialize<Konyvtar>(json);

            
            
            
            // Csak ifjúsági regények szűrése (kategoriak tömbben "Ifjúsági")




            var ifjusagi = konyvtar.konyvtar
                .Where(k => k.kategoriak != null &&
                           Array.Exists(k.kategoriak, kat => kat.ToLower().Contains("ifjúsági")))
                .OrderBy(k => k.ar)          // Ár szerint növekvő sorrend
                .ToList();

            
            
            // Csoportosítás oldalszám szerint (100 oldalanként)


            var csoportok = ifjusagi
                .GroupBy(k => (k.oldalszam / 100) * 100)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => $"{g.Key}-{g.Key + 99} oldal",
                    g => g.ToList()
                );

           
            
            // Megjelenítés



            Console.WriteLine("IFJÚSÁGI REGÉNYEK ÁR SZERINT RENDEZVE\n");
            Console.WriteLine(new string('=', 90));
            Console.WriteLine($"Összesen {ifjusagi.Count} ifjúsági regény találat\n");

            foreach (var csoport in csoportok)
            {
                Console.WriteLine($"\n{csoport.Key.ToUpper()}");
                Console.WriteLine(new string('-', 90));

                Console.WriteLine($"| {"Cím",-45} | {"Szerző(k)",-20} | {"Ár",-10} | {"Oldalszám",-10} |");
                Console.WriteLine(new string('-', 90));

                foreach (var konyv in csoport.Value)
                {
                    string szerzok = string.Join(", ", konyv.szerzok ?? new string[0]);
                    Console.WriteLine($"| {konyv.cim ?? "Nincs cím",-45} | " +
                                    $"{szerzok,-20} | " +
                                    $"{konyv.ar,8:N0} Ft | " +
                                    $"{konyv.oldalszam,8} |");
                }

                Console.WriteLine(new string('-', 90));
            }

            Console.WriteLine($"\nTalálatok kategóriánként:");
            foreach (var csoport in csoportok)
            {
                Console.WriteLine($"  {csoport.Key}: {csoport.Value.Count} db");
            }
        }
        catch (Exception ex)
        


        Console.WriteLine("\nNyomj Enter-t a kilépéshez...");
        Console.ReadLine();
    }
}
