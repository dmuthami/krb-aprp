using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RoadData
    {

        public static void SeedRoad(this ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Road>().HasData(
            //new Road
            //{
            //    ID = 1L,
            //    RoadNumber = "A101",
            //    AuthorityId = 1,
            //    RoadName = "Namanga-Nairobi"
            //},
            //new Road
            //{
            //    ID = 2L,
            //    RoadNumber = "A109",
            //    AuthorityId = 1,
            //    RoadName = "Mombasa Malaba"
            //},
            //new Road
            //{
            //    ID = 3L,
            //    RoadNumber = "A110",
            //    AuthorityId = 1,
            //    RoadName = "Kericho Busia"
            //},
            //new Road
            //{
            //    ID = 4L,
            //    RoadNumber = "A111",
            //    AuthorityId = 1,
            //    RoadName = "Eldoret-Lokichar"
            //},
            //new Road
            //{
            //    ID = 5L,
            //    RoadNumber = "C781",
            //    AuthorityId = 3,
            //    RoadName = "Ekero-Ebuyalu"
            //},
            //new Road
            //{
            //    ID = 6L,
            //    RoadNumber = "C779",
            //    AuthorityId = 2,
            //    RoadName = "Mayoni-Kanduyi- Chwele"
            //},
            //new Road
            //{
            //    ID = 7L,
            //    RoadNumber = "C675",
            //    AuthorityId = 2,
            //    RoadName = "Yala-Musalaba-Chavakali- Chepsonoi"
            //},
            //new Road
            //{
            //    ID = 8L,
            //    RoadNumber = "C776",
            //    AuthorityId = 2,
            //    RoadName = "Ruambwa-Ugunja-Sabatia-Bukura- Sigalagala"
            //},
            //new Road
            //{
            //    ID = 9L,
            //    RoadNumber = "C623",
            //    AuthorityId = 2,
            //    RoadName = "Kiungani-Mitoni Mitatu-Kona Mbaya- Matunda"
            //},
            //new Road
            //{
            //    ID = 10L,
            //    RoadNumber = "C780",
            //    AuthorityId = 3,
            //    RoadName = "Chimoi-Kambi-Mwanza"
            //},
            //new Road
            //{
            //    ID = 11L,
            //    RoadNumber = "C786",
            //    AuthorityId = 2,
            //    RoadName = "Namirama - Sharambatsa"
            //}
            //, new Road { ID = 12L, RoadNumber = "C650", AuthorityId = 3, RoadName = "Mautuma-Kipkaren" }
            //, new Road { ID = 13L, RoadNumber = "C619", AuthorityId = 3, RoadName = "Endebes-Sikhendu-Turbo-Kaiboi- Cheptiret" }
            //, new Road { ID = 14L, RoadNumber = "C638", AuthorityId = 3, RoadName = "Chimoi-Kapkoros-Kapchumo- Musoriot" }
            //, new Road { ID = 15L, RoadNumber = "C777", AuthorityId = 3, RoadName = "Mungatsi-Buyofu-Bungoma-Namukoye- Kakamega" }
            //, new Road { ID = 16L, RoadNumber = "C784", AuthorityId = 3, RoadName = "Ibokolo - Nambacha" }
            //, new Road { ID = 17L, RoadNumber = "C791", AuthorityId = 3, RoadName = "Musanda-Ekero" }
            //, new Road { ID = 18L, RoadNumber = "C783", AuthorityId = 3, RoadName = "Eshangwe - Akatsa - Lutonyi" }
            //, new Road { ID = 19L, RoadNumber = "C790", AuthorityId = 3, RoadName = "Malanga - Mundeku" }
            //, new Road { ID = 20L, RoadNumber = "C778", AuthorityId = 3, RoadName = "Ruambwa-Nangina-Bumala-Butula- Ejinja" }
            //, new Road { ID = 21L, RoadNumber = "C782", AuthorityId = 3, RoadName = "Napara-Bumula-Nasyanda-Matungu" }
            //, new Road { ID = 22L, RoadNumber = "C678", AuthorityId = 3, RoadName = "Sigalagala - Kaimosi - Koitabut" }
            //, new Road { ID = 23L, RoadNumber = "C787", AuthorityId = 3, RoadName = "Sidindi - Mundeku" }
            //, new Road { ID = 24L, RoadNumber = "C635", AuthorityId = 3, RoadName = "Mfupi-Natiri-Matunda-Sirikwa-Moi-Ben- Chebilbai" }
            //, new Road { ID = 25L, RoadNumber = "C622", AuthorityId = 3, RoadName = "Kiminini-Turbo" }
            //, new Road { ID = 26L, RoadNumber = "C788", AuthorityId = 3, RoadName = "Lwandeti - Lugari - Nyange" }
            //, new Road { ID = 27L, RoadNumber = "C789", AuthorityId = 3, RoadName = "Harambee - Sang`alo" }
            //, new Road { ID = 28L, RoadNumber = "C785", AuthorityId = 3, RoadName = "Chebuyusi - Malava" }
            //, new Road { ID = 29L, RoadNumber = "C825", AuthorityId = 3, RoadName = "Butula-Nambale-Amukura" }
            //, new Road { ID = 30L, RoadNumber = "C826", AuthorityId = 3, RoadName = "Busia-Mayenje-Mundika-Nambale" }
            //, new Road { ID = 31L, RoadNumber = "C672", AuthorityId = 3, RoadName = "Mabinju Beach-Siaya-Luanda-Vihiga-Majengo-Serem- Kipsigak" }
            //, new Road { ID = 32L, RoadNumber = "C827", AuthorityId = 3, RoadName = "Magombe-Boro" }
            //, new Road { ID = 33L, RoadNumber = "C828", AuthorityId = 3, RoadName = "Budalangi-Sio Port-Nangina" }
            //, new Road { ID = 34L, RoadNumber = "C829", AuthorityId = 3, RoadName = "Lake Victoria - Hakati" }
            //, new Road { ID = 35L, RoadNumber = "C831", AuthorityId = 3, RoadName = "Angurai -Chemasir" }
            //, new Road { ID = 36L, RoadNumber = "C807", AuthorityId = 3, RoadName = "Mungatsi-Lwakhakha" }
            //, new Road { ID = 37L, RoadNumber = "C808", AuthorityId = 3, RoadName = "Machakusi-Amukura-Bumula-Limboka" }
            //, new Road { ID = 38L, RoadNumber = "C814", AuthorityId = 3, RoadName = "Anga'Ro - Malinda" }
            //, new Road { ID = 39L, RoadNumber = "C830", AuthorityId = 3, RoadName = "Nangina-Nambale" }
            //, new Road { ID = 40L, RoadNumber = "C680", AuthorityId = 3, RoadName = "Kambogi - Serem" }
            //, new Road { ID = 41L, RoadNumber = "C799", AuthorityId = 3, RoadName = "Riat - Rabuor" }
            //, new Road { ID = 42L, RoadNumber = "C800", AuthorityId = 3, RoadName = "Kombewa - Maseno" }
            //, new Road { ID = 43L, RoadNumber = "C798", AuthorityId = 2, RoadName = "Ekwanda - Luanda - Ebusiratsi - Bukuga" }
            //, new Road { ID = 44L, RoadNumber = "C813", AuthorityId = 2, RoadName = "Cheptonon - Chepkaka-Matisi" }
            //, new Road { ID = 45L, RoadNumber = "C809", AuthorityId = 2, RoadName = "Wapukha- Kamukuywa" }
            //, new Road { ID = 46L, RoadNumber = "C811", AuthorityId = 2, RoadName = "Bungoma -  Matulo" }
            //, new Road { ID = 47L, RoadNumber = "C816", AuthorityId = 2, RoadName = "Sango-Mayanja" }
            //, new Road { ID = 48L, RoadNumber = "C815", AuthorityId = 2, RoadName = "Korosiondet - Tulienge - Sirisia" }
            //, new Road { ID = 49L, RoadNumber = "C818", AuthorityId = 2, RoadName = "Wapukha-Sikusi" }
            //, new Road { ID = 50L, RoadNumber = "C810", AuthorityId = 2, RoadName = "Mwibali-Webuye" }
            //, new Road { ID = 51L, RoadNumber = "C817", AuthorityId = 2, RoadName = "Bumula - Mayanja" }
            //, new Road { ID = 52L, RoadNumber = "C812", AuthorityId = 2, RoadName = "Sikata - Kimilili" }
            //, new Road { ID = 53L, RoadNumber = "C627", AuthorityId = 2, RoadName = "Sikhendu - Bunambo -  Ndalu" }
            //);
        }
    }
}
