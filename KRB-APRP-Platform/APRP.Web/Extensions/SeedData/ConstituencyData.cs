using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace APRP.Web.Extensions.SeedData
{
    public static class ConstituencyData
    {
         
        public static void SeedConstituency(this ModelBuilder modelBuilder)
        {
            CultureInfo _cultures = new CultureInfo("en-US");
            ResourceManager rm = new ResourceManager("APRP.Web.Resources.Extensions.SeedData.ConstituencyData", Assembly.GetExecutingAssembly());
            WKTReader reader = new WKTReader();
            reader.DefaultSRID = 4326;//WGS 84 GCS
            //modelBuilder.Entity<Constituency>().HasData(

            ////new Constituency { ID = 1, Name = "MAKADARA", CountyId = 47, Code = "1", Geom = (Geometry)reader.Read(rm.GetString("MAKADARA", _cultures)) }
            ////, new Constituency { ID = 2, Name = "KAMUKUNJI", CountyId = 47, Code = "2", Geom = (Geometry)reader.Read(rm.GetString("KAMUKUNJI", _cultures)) }
            ////, new Constituency { ID = 3, Name = "STAREHE", CountyId = 47, Code = "3", Geom = (Geometry)reader.Read(rm.GetString("STAREHE", _cultures)) }
            ////, new Constituency { ID = 4, Name = "LANGATA", CountyId = 47, Code = "4", Geom = (Geometry)reader.Read(rm.GetString("LANGATA", _cultures)) }
            ////, new Constituency { ID = 5, Name = "DAGORETTI", CountyId = 47, Code = "5", Geom = (Geometry)reader.Read(rm.GetString("DAGORETTI", _cultures)) }
            ////, new Constituency { ID = 6, Name = "WESTLANDS", CountyId = 47, Code = "6", Geom = (Geometry)reader.Read(rm.GetString("WESTLANDS", _cultures)) }
            ////, new Constituency { ID = 7, Name = "KASARANI", CountyId = 47, Code = "7", Geom = (Geometry)reader.Read(rm.GetString("KASARANI", _cultures)) }
            ////, new Constituency { ID = 8, Name = "EMBAKASI", CountyId = 47, Code = "8", Geom = (Geometry)reader.Read(rm.GetString("EMBAKASI", _cultures)) }
            ////, new Constituency { ID = 9, Name = "CHANGAMWE", CountyId = 1, Code = "9", Geom = (Geometry)reader.Read(rm.GetString("CHANGAMWE", _cultures)) }
            ////, new Constituency { ID = 10, Name = "KISAUNI", CountyId = 1, Code = "10", Geom = (Geometry)reader.Read(rm.GetString("KISAUNI", _cultures)) }
            ////, new Constituency { ID = 11, Name = "LIKONI", CountyId = 1, Code = "11", Geom = (Geometry)reader.Read(rm.GetString("LIKONI", _cultures)) }
            ////, new Constituency { ID = 12, Name = "MVITA", CountyId = 1, Code = "12", Geom = (Geometry)reader.Read(rm.GetString("MVITA", _cultures)) }
            ////, new Constituency { ID = 13, Name = "MSAMBWENI", CountyId = 2, Code = "13", Geom = (Geometry)reader.Read(rm.GetString("MSAMBWENI", _cultures)) }
            ////, new Constituency { ID = 14, Name = "MATUGA", CountyId = 2, Code = "14", Geom = (Geometry)reader.Read(rm.GetString("MATUGA", _cultures)) }
            ////, new Constituency { ID = 15, Name = "KINANGO", CountyId = 2, Code = "15", Geom = (Geometry)reader.Read(rm.GetString("KINANGO", _cultures)) }
            ////, new Constituency { ID = 16, Name = "BAHARI", CountyId = 3, Code = "16", Geom = (Geometry)reader.Read(rm.GetString("BAHARI", _cultures)) }
            ////, new Constituency { ID = 17, Name = "KALOLENI", CountyId = 3, Code = "17", Geom = (Geometry)reader.Read(rm.GetString("KALOLENI", _cultures)) }
            ////, new Constituency { ID = 18, Name = "GANZE", CountyId = 3, Code = "18", Geom = (Geometry)reader.Read(rm.GetString("GANZE", _cultures)) }
            ////, new Constituency { ID = 19, Name = "MALINDI", CountyId = 3, Code = "19", Geom = (Geometry)reader.Read(rm.GetString("MALINDI", _cultures)) }
            ////, new Constituency { ID = 20, Name = "MAGARINI", CountyId = 3, Code = "20", Geom = (Geometry)reader.Read(rm.GetString("MAGARINI", _cultures)) }
            ////, new Constituency { ID = 21, Name = "GARSEN", CountyId = 4, Code = "21", Geom = (Geometry)reader.Read(rm.GetString("GARSEN", _cultures)) }
            ////, new Constituency { ID = 22, Name = "GALOLE", CountyId = 4, Code = "22", Geom = (Geometry)reader.Read(rm.GetString("GALOLE", _cultures)) }
            ////, new Constituency { ID = 23, Name = "BURA", CountyId = 4, Code = "23", Geom = (Geometry)reader.Read(rm.GetString("BURA", _cultures)) }
            ////, new Constituency { ID = 24, Name = "LAMU EAST", CountyId = 5, Code = "24", Geom = (Geometry)reader.Read(rm.GetString("LAMU EAST", _cultures)) }
            ////, new Constituency { ID = 25, Name = "LAMU WEST", CountyId = 5, Code = "25", Geom = (Geometry)reader.Read(rm.GetString("LAMU WEST", _cultures)) }
            ////, new Constituency { ID = 26, Name = "WUNDANYI", CountyId = 6, Code = "27", Geom = (Geometry)reader.Read(rm.GetString("WUNDANYI", _cultures)) }
            ////, new Constituency { ID = 27, Name = "DUJIS", CountyId = 7, Code = "30", Geom = (Geometry)reader.Read(rm.GetString("DUJIS", _cultures)) }
            ////, new Constituency { ID = 28, Name = "LAGDERA", CountyId = 7, Code = "31", Geom = (Geometry)reader.Read(rm.GetString("LAGDERA", _cultures)) }
            ////, new Constituency { ID = 29, Name = "FAFI", CountyId = 7, Code = "32", Geom = (Geometry)reader.Read(rm.GetString("FAFI", _cultures)) }
            ////, new Constituency { ID = 30, Name = "IJARA", CountyId = 7, Code = "33", Geom = (Geometry)reader.Read(rm.GetString("IJARA", _cultures)) }
            ////, new Constituency { ID = 31, Name = "WAJIR NORTH", CountyId = 8, Code = "34", Geom = (Geometry)reader.Read(rm.GetString("WAJIR NORTH", _cultures)) }
            ////, new Constituency { ID = 32, Name = "WAJIR WEST", CountyId = 8, Code = "35", Geom = (Geometry)reader.Read(rm.GetString("WAJIR WEST", _cultures)) }
            ////, new Constituency { ID = 33, Name = "WAJIR EAST", CountyId = 8, Code = "36", Geom = (Geometry)reader.Read(rm.GetString("WAJIR EAST", _cultures)) }
            ////, new Constituency { ID = 34, Name = "WAJIR SOUTH", CountyId = 8, Code = "37", Geom = (Geometry)reader.Read(rm.GetString("WAJIR SOUTH", _cultures)) }
            ////, new Constituency { ID = 35, Name = "MANDERA WEST", CountyId = 9, Code = "38", Geom = (Geometry)reader.Read(rm.GetString("MANDERA WEST", _cultures)) }
            ////, new Constituency { ID = 36, Name = "MANDERA CENTRAL", CountyId = 9, Code = "39", Geom = (Geometry)reader.Read(rm.GetString("MANDERA CENTRAL", _cultures)) }
            ////, new Constituency { ID = 37, Name = "MANDERA EAST", CountyId = 9, Code = "40", Geom = (Geometry)reader.Read(rm.GetString("MANDERA EAST", _cultures)) }
            ////, new Constituency { ID = 38, Name = "MOYALE", CountyId = 10, Code = "41", Geom = (Geometry)reader.Read(rm.GetString("MOYALE", _cultures)) }
            ////, new Constituency { ID = 39, Name = "NORTH HORR", CountyId = 10, Code = "42", Geom = (Geometry)reader.Read(rm.GetString("NORTH HORR", _cultures)) }
            ////, new Constituency { ID = 40, Name = "SAKU", CountyId = 10, Code = "43", Geom = (Geometry)reader.Read(rm.GetString("SAKU", _cultures)) }
            ////, new Constituency { ID = 41, Name = "LAISAMIS", CountyId = 10, Code = "44", Geom = (Geometry)reader.Read(rm.GetString("LAISAMIS", _cultures)) }
            ////, new Constituency { ID = 42, Name = "ISIOLO NORTH", CountyId = 11, Code = "45", Geom = (Geometry)reader.Read(rm.GetString("ISIOLO NORTH", _cultures)) }
            ////, new Constituency { ID = 43, Name = "ISIOLO SOUTH", CountyId = 11, Code = "46", Geom = (Geometry)reader.Read(rm.GetString("ISIOLO SOUTH", _cultures)) }
            ////, new Constituency { ID = 44, Name = "IGEMBE", CountyId = 12, Code = "47", Geom = (Geometry)reader.Read(rm.GetString("IGEMBE", _cultures)) }
            ////, new Constituency { ID = 45, Name = "NTONYIRI", CountyId = 12, Code = "48", Geom = (Geometry)reader.Read(rm.GetString("NTONYIRI", _cultures)) }
            ////, new Constituency { ID = 46, Name = "TIGANIA WEST", CountyId = 12, Code = "49", Geom = (Geometry)reader.Read(rm.GetString("TIGANIA WEST", _cultures)) }
            ////, new Constituency { ID = 47, Name = "TIGANIA EAST", CountyId = 12, Code = "50", Geom = (Geometry)reader.Read(rm.GetString("TIGANIA EAST", _cultures)) }
            ////, new Constituency { ID = 48, Name = "NORTH IMENTI", CountyId = 12, Code = "51", Geom = (Geometry)reader.Read(rm.GetString("NORTH IMENTI", _cultures)) }
            ////, new Constituency { ID = 49, Name = "CENTRAL IMENTI", CountyId = 12, Code = "52", Geom = (Geometry)reader.Read(rm.GetString("CENTRAL IMENTI", _cultures)) }
            ////, new Constituency { ID = 50, Name = "SOUTH IMENTI", CountyId = 12, Code = "53", Geom = (Geometry)reader.Read(rm.GetString("SOUTH IMENTI", _cultures)) }
            ////, new Constituency { ID = 51, Name = "NITHI", CountyId = 13, Code = "54", Geom = (Geometry)reader.Read(rm.GetString("NITHI", _cultures)) }
            ////, new Constituency { ID = 52, Name = "THARAKA", CountyId = 13, Code = "55", Geom = (Geometry)reader.Read(rm.GetString("THARAKA", _cultures)) }
            ////, new Constituency { ID = 53, Name = "GACHOKA", CountyId = 14, Code = "58", Geom = (Geometry)reader.Read(rm.GetString("GACHOKA", _cultures)) }
            ////, new Constituency { ID = 54, Name = "SIAKAGO", CountyId = 14, Code = "59", Geom = (Geometry)reader.Read(rm.GetString("SIAKAGO", _cultures)) }
            ////, new Constituency { ID = 55, Name = "MWINGI NORTH", CountyId = 15, Code = "60", Geom = (Geometry)reader.Read(rm.GetString("MWINGI NORTH", _cultures)) }
            ////, new Constituency { ID = 56, Name = "MWINGI SOUTH", CountyId = 15, Code = "61", Geom = (Geometry)reader.Read(rm.GetString("MWINGI SOUTH", _cultures)) }
            ////, new Constituency { ID = 57, Name = "KITUI WEST", CountyId = 15, Code = "62", Geom = (Geometry)reader.Read(rm.GetString("KITUI WEST", _cultures)) }
            ////, new Constituency { ID = 58, Name = "KITUI CENTRAL", CountyId = 15, Code = "63", Geom = (Geometry)reader.Read(rm.GetString("KITUI CENTRAL", _cultures)) }
            ////, new Constituency { ID = 59, Name = "MUTITO", CountyId = 15, Code = "64", Geom = (Geometry)reader.Read(rm.GetString("MUTITO", _cultures)) }
            ////, new Constituency { ID = 60, Name = "KITUI SOUTH", CountyId = 15, Code = "65", Geom = (Geometry)reader.Read(rm.GetString("KITUI SOUTH", _cultures)) }
            ////, new Constituency { ID = 61, Name = "MASINGA", CountyId = 16, Code = "66", Geom = (Geometry)reader.Read(rm.GetString("MASINGA", _cultures)) }
            ////, new Constituency { ID = 62, Name = "YATTA", CountyId = 16, Code = "67", Geom = (Geometry)reader.Read(rm.GetString("YATTA", _cultures)) }
            ////, new Constituency { ID = 63, Name = "KANGUNDO", CountyId = 16, Code = "68", Geom = (Geometry)reader.Read(rm.GetString("KANGUNDO", _cultures)) }
            ////, new Constituency { ID = 64, Name = "KATHIANI", CountyId = 16, Code = "69", Geom = (Geometry)reader.Read(rm.GetString("KATHIANI", _cultures)) }
            ////, new Constituency { ID = 65, Name = "MACHAKOS TOWN", CountyId = 17, Code = "70", Geom = (Geometry)reader.Read(rm.GetString("MACHAKOS TOWN", _cultures)) }
            ////, new Constituency { ID = 66, Name = "MWALA", CountyId = 16, Code = "71", Geom = (Geometry)reader.Read(rm.GetString("MWALA", _cultures)) }
            ////, new Constituency { ID = 67, Name = "MBOONI", CountyId = 17, Code = "72", Geom = (Geometry)reader.Read(rm.GetString("MBOONI", _cultures)) }
            ////, new Constituency { ID = 68, Name = "KILOME", CountyId = 17, Code = "73", Geom = (Geometry)reader.Read(rm.GetString("KILOME", _cultures)) }
            ////, new Constituency { ID = 69, Name = "KAITI", CountyId = 17, Code = "74", Geom = (Geometry)reader.Read(rm.GetString("KAITI", _cultures)) }
            ////, new Constituency { ID = 70, Name = "MAKUENI", CountyId = 17, Code = "75", Geom = (Geometry)reader.Read(rm.GetString("MAKUENI", _cultures)) }
            ////, new Constituency { ID = 71, Name = "KIBWEZI", CountyId = 17, Code = "76", Geom = (Geometry)reader.Read(rm.GetString("KIBWEZI", _cultures)) }
            ////, new Constituency { ID = 72, Name = "KINANGOP", CountyId = 18, Code = "77", Geom = (Geometry)reader.Read(rm.GetString("KINANGOP", _cultures)) }
            ////, new Constituency { ID = 73, Name = "KIPIPIRI", CountyId = 18, Code = "78", Geom = (Geometry)reader.Read(rm.GetString("KIPIPIRI", _cultures)) }
            ////, new Constituency { ID = 74, Name = "OL_KALOU", CountyId = 18, Code = "79", Geom = (Geometry)reader.Read(rm.GetString("OL_KALOU", _cultures)) }
            ////, new Constituency { ID = 75, Name = "NDARAGWA", CountyId = 18, Code = "80", Geom = (Geometry)reader.Read(rm.GetString("NDARAGWA", _cultures)) }
            ////, new Constituency { ID = 76, Name = "TETU", CountyId = 19, Code = "81", Geom = (Geometry)reader.Read(rm.GetString("TETU", _cultures)) }
            ////, new Constituency { ID = 77, Name = "KIENI", CountyId = 19, Code = "82", Geom = (Geometry)reader.Read(rm.GetString("KIENI", _cultures)) }
            ////, new Constituency { ID = 78, Name = "OTHAYA", CountyId = 19, Code = "84", Geom = (Geometry)reader.Read(rm.GetString("OTHAYA", _cultures)) }
            ////, new Constituency { ID = 79, Name = "MUKURWE-INI", CountyId = 19, Code = "85", Geom = (Geometry)reader.Read(rm.GetString("MUKURWE-INI", _cultures)) }
            ////, new Constituency { ID = 80, Name = "NYERI TOWN", CountyId = 19, Code = "86", Geom = (Geometry)reader.Read(rm.GetString("NYERI TOWN", _cultures)) }
            ////, new Constituency { ID = 81, Name = "MWEA", CountyId = 20, Code = "87", Geom = (Geometry)reader.Read(rm.GetString("MWEA", _cultures)) }
            ////, new Constituency { ID = 82, Name = "NDIA", CountyId = 20, Code = "89", Geom = (Geometry)reader.Read(rm.GetString("NDIA", _cultures)) }
            ////, new Constituency { ID = 83, Name = "KANGEMA", CountyId = 21, Code = "91", Geom = (Geometry)reader.Read(rm.GetString("KANGEMA", _cultures)) }
            ////, new Constituency { ID = 84, Name = "MATHIOYA", CountyId = 21, Code = "92", Geom = (Geometry)reader.Read(rm.GetString("MATHIOYA", _cultures)) }
            ////, new Constituency { ID = 85, Name = "KIHARU", CountyId = 21, Code = "93", Geom = (Geometry)reader.Read(rm.GetString("KIHARU", _cultures)) }
            ////, new Constituency { ID = 86, Name = "KIGUMO", CountyId = 21, Code = "94", Geom = (Geometry)reader.Read(rm.GetString("KIGUMO", _cultures)) }
            ////, new Constituency { ID = 87, Name = "MARAGWA", CountyId = 21, Code = "95", Geom = (Geometry)reader.Read(rm.GetString("MARAGWA", _cultures)) }
            ////, new Constituency { ID = 88, Name = "KANDARA", CountyId = 21, Code = "96", Geom = (Geometry)reader.Read(rm.GetString("KANDARA", _cultures)) }
            ////, new Constituency { ID = 89, Name = "GATANGA", CountyId = 21, Code = "97", Geom = (Geometry)reader.Read(rm.GetString("GATANGA", _cultures)) }
            ////, new Constituency { ID = 90, Name = "GATUNDU SOUTH", CountyId = 22, Code = "98", Geom = (Geometry)reader.Read(rm.GetString("GATUNDU SOUTH", _cultures)) }
            ////, new Constituency { ID = 91, Name = "GATUNDU NORTH", CountyId = 22, Code = "99", Geom = (Geometry)reader.Read(rm.GetString("GATUNDU NORTH", _cultures)) }
            ////, new Constituency { ID = 92, Name = "JUJA", CountyId = 22, Code = "100", Geom = (Geometry)reader.Read(rm.GetString("JUJA", _cultures)) }
            ////, new Constituency { ID = 93, Name = "GITHUNGURI", CountyId = 22, Code = "101", Geom = (Geometry)reader.Read(rm.GetString("GITHUNGURI", _cultures)) }
            ////, new Constituency { ID = 94, Name = "KIAMBAA", CountyId = 22, Code = "102", Geom = (Geometry)reader.Read(rm.GetString("KIAMBAA", _cultures)) }
            ////, new Constituency { ID = 95, Name = "KABETE", CountyId = 22, Code = "103", Geom = (Geometry)reader.Read(rm.GetString("KABETE", _cultures)) }
            ////, new Constituency { ID = 96, Name = "LIMURU", CountyId = 22, Code = "104", Geom = (Geometry)reader.Read(rm.GetString("LIMURU", _cultures)) }
            ////, new Constituency { ID = 97, Name = "LARI", CountyId = 22, Code = "105", Geom = (Geometry)reader.Read(rm.GetString("LARI", _cultures)) }
            ////, new Constituency { ID = 98, Name = "TURKANA NORTH", CountyId = 23, Code = "106", Geom = (Geometry)reader.Read(rm.GetString("TURKANA NORTH", _cultures)) }
            ////, new Constituency { ID = 99, Name = "TURKANA CENTRAL", CountyId = 23, Code = "107", Geom = (Geometry)reader.Read(rm.GetString("TURKANA CENTRAL", _cultures)) }
            ////, new Constituency { ID = 100, Name = "TURKANA SOUTH", CountyId = 23, Code = "108", Geom = (Geometry)reader.Read(rm.GetString("TURKANA SOUTH", _cultures)) }
            ////, new Constituency { ID = 101, Name = "KACHELIBA", CountyId = 24, Code = "109", Geom = (Geometry)reader.Read(rm.GetString("KACHELIBA", _cultures)) }
            ////, new Constituency { ID = 102, Name = "KAPENGURIA", CountyId = 24, Code = "110", Geom = (Geometry)reader.Read(rm.GetString("KAPENGURIA", _cultures)) }
            ////, new Constituency { ID = 103, Name = "SIGOR", CountyId = 24, Code = "111", Geom = (Geometry)reader.Read(rm.GetString("SIGOR", _cultures)) }
            ////, new Constituency { ID = 104, Name = "SAMBURU WEST", CountyId = 25, Code = "112", Geom = (Geometry)reader.Read(rm.GetString("SAMBURU WEST", _cultures)) }
            ////, new Constituency { ID = 105, Name = "SAMBURU EAST", CountyId = 25, Code = "113", Geom = (Geometry)reader.Read(rm.GetString("SAMBURU EAST", _cultures)) }
            ////, new Constituency { ID = 106, Name = "KWANZA", CountyId = 26, Code = "114", Geom = (Geometry)reader.Read(rm.GetString("KWANZA", _cultures)) }
            ////, new Constituency { ID = 107, Name = "SABOTI", CountyId = 26, Code = "115", Geom = (Geometry)reader.Read(rm.GetString("SABOTI", _cultures)) }
            ////, new Constituency { ID = 108, Name = "CHERANGANY", CountyId = 26, Code = "116", Geom = (Geometry)reader.Read(rm.GetString("CHERANGANY", _cultures)) }
            ////, new Constituency { ID = 109, Name = "ELDORET NORTH", CountyId = 27, Code = "117", Geom = (Geometry)reader.Read(rm.GetString("ELDORET NORTH", _cultures)) }
            ////, new Constituency { ID = 110, Name = "ELDORET EAST", CountyId = 27, Code = "118", Geom = (Geometry)reader.Read(rm.GetString("ELDORET EAST", _cultures)) }
            ////, new Constituency { ID = 111, Name = "ELDORET SOUTH", CountyId = 27, Code = "119", Geom = (Geometry)reader.Read(rm.GetString("ELDORET SOUTH", _cultures)) }
            ////, new Constituency { ID = 112, Name = "MARAKWET EAST", CountyId = 28, Code = "120", Geom = (Geometry)reader.Read(rm.GetString("MARAKWET EAST", _cultures)) }
            ////, new Constituency { ID = 113, Name = "MARAKWET WEST", CountyId = 28, Code = "121", Geom = (Geometry)reader.Read(rm.GetString("MARAKWET WEST", _cultures)) }
            ////, new Constituency { ID = 114, Name = "KEIYO NORTH", CountyId = 28, Code = "122", Geom = (Geometry)reader.Read(rm.GetString("KEIYO NORTH", _cultures)) }
            ////, new Constituency { ID = 115, Name = "KEIYO SOUTH", CountyId = 28, Code = "123", Geom = (Geometry)reader.Read(rm.GetString("KEIYO SOUTH", _cultures)) }
            ////, new Constituency { ID = 116, Name = "MOSOP", CountyId = 29, Code = "124", Geom = (Geometry)reader.Read(rm.GetString("MOSOP", _cultures)) }
            ////, new Constituency { ID = 117, Name = "ALDAI", CountyId = 29, Code = "125", Geom = (Geometry)reader.Read(rm.GetString("ALDAI", _cultures)) }
            ////, new Constituency { ID = 118, Name = "EMGWEN", CountyId = 29, Code = "126", Geom = (Geometry)reader.Read(rm.GetString("EMGWEN", _cultures)) }
            ////, new Constituency { ID = 119, Name = "TINDERET", CountyId = 29, Code = "127", Geom = (Geometry)reader.Read(rm.GetString("TINDERET", _cultures)) }
            ////, new Constituency { ID = 120, Name = "BARINGO EAST", CountyId = 30, Code = "128", Geom = (Geometry)reader.Read(rm.GetString("BARINGO EAST", _cultures)) }
            ////, new Constituency { ID = 121, Name = "BARINGO NORTH", CountyId = 30, Code = "129", Geom = (Geometry)reader.Read(rm.GetString("BARINGO NORTH", _cultures)) }
            ////, new Constituency { ID = 122, Name = "BARINGO CENTRAL", CountyId = 30, Code = "130", Geom = (Geometry)reader.Read(rm.GetString("BARINGO CENTRAL", _cultures)) }
            ////, new Constituency { ID = 123, Name = "MOGOTIO", CountyId = 30, Code = "131", Geom = (Geometry)reader.Read(rm.GetString("MOGOTIO", _cultures)) }
            ////, new Constituency { ID = 124, Name = "ELDAMA RAVINE", CountyId = 30, Code = "132", Geom = (Geometry)reader.Read(rm.GetString("ELDAMA RAVINE", _cultures)) }
            ////, new Constituency { ID = 125, Name = "LAIKIPIA WEST", CountyId = 31, Code = "133", Geom = (Geometry)reader.Read(rm.GetString("LAIKIPIA WEST", _cultures)) }
            ////, new Constituency { ID = 126, Name = "LAIKIPIA EAST", CountyId = 31, Code = "134", Geom = (Geometry)reader.Read(rm.GetString("LAIKIPIA EAST", _cultures)) }
            ////, new Constituency { ID = 127, Name = "NAIVASHA", CountyId = 32, Code = "135", Geom = (Geometry)reader.Read(rm.GetString("NAIVASHA", _cultures)) }
            ////, new Constituency { ID = 128, Name = "NAKURU TOWN", CountyId = 32, Code = "136", Geom = (Geometry)reader.Read(rm.GetString("NAKURU TOWN", _cultures)) }
            ////, new Constituency { ID = 129, Name = "KURESOI", CountyId = 32, Code = "137", Geom = (Geometry)reader.Read(rm.GetString("KURESOI", _cultures)) }
            ////, new Constituency { ID = 130, Name = "MOLO", CountyId = 32, Code = "138", Geom = (Geometry)reader.Read(rm.GetString("MOLO", _cultures)) }
            ////, new Constituency { ID = 131, Name = "RONGAI", CountyId = 32, Code = "139", Geom = (Geometry)reader.Read(rm.GetString("RONGAI", _cultures)) }
            ////, new Constituency { ID = 132, Name = "SUBUKIA", CountyId = 32, Code = "140", Geom = (Geometry)reader.Read(rm.GetString("SUBUKIA", _cultures)) }
            ////, new Constituency { ID = 133, Name = "KILGORIS", CountyId = 33, Code = "141", Geom = (Geometry)reader.Read(rm.GetString("KILGORIS", _cultures)) }
            ////, new Constituency { ID = 134, Name = "NAROK NORTH", CountyId = 33, Code = "142", Geom = (Geometry)reader.Read(rm.GetString("NAROK NORTH", _cultures)) }
            ////, new Constituency { ID = 135, Name = "NAROK SOUTH", CountyId = 33, Code = "143", Geom = (Geometry)reader.Read(rm.GetString("NAROK SOUTH", _cultures)) }
            ////, new Constituency { ID = 136, Name = "KAJIADO NORTH", CountyId = 34, Code = "144", Geom = (Geometry)reader.Read(rm.GetString("KAJIADO NORTH", _cultures)) }
            ////, new Constituency { ID = 137, Name = "KAJIADO CENTRAL", CountyId = 34, Code = "145", Geom = (Geometry)reader.Read(rm.GetString("KAJIADO CENTRAL", _cultures)) }
            ////, new Constituency { ID = 138, Name = "KAJIADO SOUTH", CountyId = 34, Code = "146", Geom = (Geometry)reader.Read(rm.GetString("KAJIADO SOUTH", _cultures)) }
            ////, new Constituency { ID = 139, Name = "BOMET", CountyId = 36, Code = "147", Geom = (Geometry)reader.Read(rm.GetString("BOMET", _cultures)) }
            ////, new Constituency { ID = 140, Name = "CHEPALUNGU", CountyId = 36, Code = "148", Geom = (Geometry)reader.Read(rm.GetString("CHEPALUNGU", _cultures)) }
            ////, new Constituency { ID = 141, Name = "SOTIK", CountyId = 36, Code = "149", Geom = (Geometry)reader.Read(rm.GetString("SOTIK", _cultures)) }
            ////, new Constituency { ID = 142, Name = "KONOIN", CountyId = 36, Code = "150", Geom = (Geometry)reader.Read(rm.GetString("KONOIN", _cultures)) }
            ////, new Constituency { ID = 143, Name = "BURET", CountyId = 35, Code = "151", Geom = (Geometry)reader.Read(rm.GetString("BURET", _cultures)) }
            ////, new Constituency { ID = 144, Name = "BELGUT", CountyId = 35, Code = "152", Geom = (Geometry)reader.Read(rm.GetString("BELGUT", _cultures)) }
            ////, new Constituency { ID = 145, Name = "AINAMOI", CountyId = 35, Code = "153", Geom = (Geometry)reader.Read(rm.GetString("AINAMOI", _cultures)) }
            ////, new Constituency { ID = 146, Name = "KIPKELION", CountyId = 35, Code = "154", Geom = (Geometry)reader.Read(rm.GetString("KIPKELION", _cultures)) }
            ////, new Constituency { ID = 147, Name = "MALAVA", CountyId = 37, Code = "155", Geom = (Geometry)reader.Read(rm.GetString("MALAVA", _cultures)) }
            ////, new Constituency { ID = 148, Name = "LUGARI", CountyId = 37, Code = "156", Geom = (Geometry)reader.Read(rm.GetString("LUGARI", _cultures)) }
            ////, new Constituency { ID = 149, Name = "MUMIAS", CountyId = 37, Code = "157", Geom = (Geometry)reader.Read(rm.GetString("MUMIAS", _cultures)) }
            ////, new Constituency { ID = 150, Name = "MATUNGU", CountyId = 37, Code = "158", Geom = (Geometry)reader.Read(rm.GetString("MATUNGU", _cultures)) }
            ////, new Constituency { ID = 151, Name = "LURAMBI", CountyId = 37, Code = "159", Geom = (Geometry)reader.Read(rm.GetString("LURAMBI", _cultures)) }
            ////, new Constituency { ID = 152, Name = "SHINYALU", CountyId = 37, Code = "160", Geom = (Geometry)reader.Read(rm.GetString("SHINYALU", _cultures)) }
            ////, new Constituency { ID = 153, Name = "IKOLOMANI", CountyId = 37, Code = "161", Geom = (Geometry)reader.Read(rm.GetString("IKOLOMANI", _cultures)) }
            ////, new Constituency { ID = 154, Name = "BUTERE", CountyId = 37, Code = "162", Geom = (Geometry)reader.Read(rm.GetString("BUTERE", _cultures)) }
            ////, new Constituency { ID = 155, Name = "KHWISERO", CountyId = 37, Code = "163", Geom = (Geometry)reader.Read(rm.GetString("KHWISERO", _cultures)) }
            ////, new Constituency { ID = 156, Name = "EMUHAYA", CountyId = 38, Code = "164", Geom = (Geometry)reader.Read(rm.GetString("EMUHAYA", _cultures)) }
            ////, new Constituency { ID = 157, Name = "SABATIA", CountyId = 38, Code = "165", Geom = (Geometry)reader.Read(rm.GetString("SABATIA", _cultures)) }
            ////, new Constituency { ID = 158, Name = "VIHIGA", CountyId = 38, Code = "166", Geom = (Geometry)reader.Read(rm.GetString("VIHIGA", _cultures)) }
            ////, new Constituency { ID = 159, Name = "HAMISI", CountyId = 38, Code = "167", Geom = (Geometry)reader.Read(rm.GetString("HAMISI", _cultures)) }
            ////, new Constituency { ID = 160, Name = "MT ELGON", CountyId = 39, Code = "168", Geom = (Geometry)reader.Read(rm.GetString("MT ELGON", _cultures)) }
            ////, new Constituency { ID = 161, Name = "KIMILILI", CountyId = 39, Code = "169", Geom = (Geometry)reader.Read(rm.GetString("KIMILILI", _cultures)) }
            ////, new Constituency { ID = 162, Name = "WEBUYE", CountyId = 39, Code = "170", Geom = (Geometry)reader.Read(rm.GetString("WEBUYE", _cultures)) }
            ////, new Constituency { ID = 163, Name = "SIRISIA", CountyId = 39, Code = "171", Geom = (Geometry)reader.Read(rm.GetString("SIRISIA", _cultures)) }
            ////, new Constituency { ID = 164, Name = "KANDUYI", CountyId = 39, Code = "172", Geom = (Geometry)reader.Read(rm.GetString("KANDUYI", _cultures)) }
            ////, new Constituency { ID = 165, Name = "BUMULA", CountyId = 39, Code = "173", Geom = (Geometry)reader.Read(rm.GetString("BUMULA", _cultures)) }
            ////, new Constituency { ID = 166, Name = "AMAGORO", CountyId = 40, Code = "174", Geom = (Geometry)reader.Read(rm.GetString("AMAGORO", _cultures)) }
            ////, new Constituency { ID = 167, Name = "NAMBALE", CountyId = 40, Code = "175", Geom = (Geometry)reader.Read(rm.GetString("NAMBALE", _cultures)) }
            ////, new Constituency { ID = 168, Name = "BUTULA", CountyId = 40, Code = "176", Geom = (Geometry)reader.Read(rm.GetString("BUTULA", _cultures)) }
            ////, new Constituency { ID = 169, Name = "FUNYULA", CountyId = 40, Code = "177", Geom = (Geometry)reader.Read(rm.GetString("FUNYULA", _cultures)) }
            ////, new Constituency { ID = 170, Name = "BUDALANGI", CountyId = 40, Code = "178", Geom = (Geometry)reader.Read(rm.GetString("BUDALANGI", _cultures)) }
            ////, new Constituency { ID = 171, Name = "UGENYA", CountyId = 41, Code = "179", Geom = (Geometry)reader.Read(rm.GetString("UGENYA", _cultures)) }
            ////, new Constituency { ID = 172, Name = "ALEGO", CountyId = 41, Code = "180", Geom = (Geometry)reader.Read(rm.GetString("ALEGO", _cultures)) }
            ////, new Constituency { ID = 173, Name = "GEM", CountyId = 41, Code = "181", Geom = (Geometry)reader.Read(rm.GetString("GEM", _cultures)) }
            ////, new Constituency { ID = 174, Name = "BONDO", CountyId = 41, Code = "182", Geom = (Geometry)reader.Read(rm.GetString("BONDO", _cultures)) }
            ////, new Constituency { ID = 175, Name = "RARIEDA", CountyId = 41, Code = "183", Geom = (Geometry)reader.Read(rm.GetString("RARIEDA", _cultures)) }
            ////, new Constituency { ID = 176, Name = "KISUMU TOWN EAST", CountyId = 42, Code = "184", Geom = (Geometry)reader.Read(rm.GetString("KISUMU TOWN EAST", _cultures)) }
            ////, new Constituency { ID = 177, Name = "KISUMU TOWN WEST", CountyId = 42, Code = "185", Geom = (Geometry)reader.Read(rm.GetString("KISUMU TOWN WEST", _cultures)) }
            ////, new Constituency { ID = 178, Name = "KISUMU RURAL", CountyId = 42, Code = "186", Geom = (Geometry)reader.Read(rm.GetString("KISUMU RURAL", _cultures)) }
            ////, new Constituency { ID = 179, Name = "NYANDO", CountyId = 42, Code = "187", Geom = (Geometry)reader.Read(rm.GetString("NYANDO", _cultures)) }
            ////, new Constituency { ID = 180, Name = "MUHORONI", CountyId = 42, Code = "188", Geom = (Geometry)reader.Read(rm.GetString("MUHORONI", _cultures)) }
            ////, new Constituency { ID = 181, Name = "NYAKACH", CountyId = 42, Code = "189", Geom = (Geometry)reader.Read(rm.GetString("NYAKACH", _cultures)) }
            ////, new Constituency { ID = 182, Name = "KASIPUL-KABONDO", CountyId = 43, Code = "190", Geom = (Geometry)reader.Read(rm.GetString("KASIPUL-KABONDO", _cultures)) }
            ////, new Constituency { ID = 183, Name = "KARACHUONYO", CountyId = 43, Code = "191", Geom = (Geometry)reader.Read(rm.GetString("KARACHUONYO", _cultures)) }
            ////, new Constituency { ID = 184, Name = "RANGWE", CountyId = 43, Code = "192", Geom = (Geometry)reader.Read(rm.GetString("RANGWE", _cultures)) }
            ////, new Constituency { ID = 185, Name = "NDHIWA", CountyId = 43, Code = "193", Geom = (Geometry)reader.Read(rm.GetString("NDHIWA", _cultures)) }
            ////, new Constituency { ID = 186, Name = "RONGO", CountyId = 44, Code = "194", Geom = (Geometry)reader.Read(rm.GetString("RONGO", _cultures)) }
            ////, new Constituency { ID = 187, Name = "MIGORI", CountyId = 44, Code = "195", Geom = (Geometry)reader.Read(rm.GetString("MIGORI", _cultures)) }
            ////, new Constituency { ID = 188, Name = "URIRI", CountyId = 44, Code = "196", Geom = (Geometry)reader.Read(rm.GetString("URIRI", _cultures)) }
            ////, new Constituency { ID = 189, Name = "NYATIKE", CountyId = 44, Code = "197", Geom = (Geometry)reader.Read(rm.GetString("NYATIKE", _cultures)) }
            ////, new Constituency { ID = 190, Name = "MBITA", CountyId = 43, Code = "198", Geom = (Geometry)reader.Read(rm.GetString("MBITA", _cultures)) }
            ////, new Constituency { ID = 191, Name = "GWASSI", CountyId = 43, Code = "199", Geom = (Geometry)reader.Read(rm.GetString("GWASSI", _cultures)) }
            ////, new Constituency { ID = 192, Name = "KURIA", CountyId = 44, Code = "200", Geom = (Geometry)reader.Read(rm.GetString("KURIA", _cultures)) }
            ////, new Constituency { ID = 193, Name = "BONCHARI", CountyId = 45, Code = "201", Geom = (Geometry)reader.Read(rm.GetString("BONCHARI", _cultures)) }
            ////, new Constituency { ID = 194, Name = "SOUTH MUGIRANGO", CountyId = 45, Code = "202", Geom = (Geometry)reader.Read(rm.GetString("SOUTH MUGIRANGO", _cultures)) }
            ////, new Constituency { ID = 195, Name = "BOMACHOGE", CountyId = 45, Code = "203", Geom = (Geometry)reader.Read(rm.GetString("BOMACHOGE", _cultures)) }
            ////, new Constituency { ID = 196, Name = "BOBASI", CountyId = 45, Code = "204", Geom = (Geometry)reader.Read(rm.GetString("BOBASI", _cultures)) }
            ////, new Constituency { ID = 197, Name = "NYARIBARI MASABA", CountyId = 45, Code = "205", Geom = (Geometry)reader.Read(rm.GetString("NYARIBARI MASABA", _cultures)) }
            ////, new Constituency { ID = 198, Name = "NYARIBARI CHACHE", CountyId = 45, Code = "206", Geom = (Geometry)reader.Read(rm.GetString("NYARIBARI CHACHE", _cultures)) }
            ////, new Constituency { ID = 199, Name = "KITUTU CHACHE", CountyId = 45, Code = "207", Geom = (Geometry)reader.Read(rm.GetString("KITUTU CHACHE", _cultures)) }
            ////, new Constituency { ID = 200, Name = "KITUTU MASABA", CountyId = 46, Code = "208", Geom = (Geometry)reader.Read(rm.GetString("KITUTU MASABA", _cultures)) }
            ////, new Constituency { ID = 201, Name = "WEST MUGIRANGO", CountyId = 46, Code = "209", Geom = (Geometry)reader.Read(rm.GetString("WEST MUGIRANGO", _cultures)) }
            ////, new Constituency { ID = 202, Name = "N.MUGIRANGO BORABU", CountyId = 46, Code = "210", Geom = (Geometry)reader.Read(rm.GetString("N.MUGIRANGO BORABU", _cultures)) }
            ////, new Constituency { ID = 203, Name = "MATHIRA", CountyId = 19, Code = "83", Geom = (Geometry)reader.Read(rm.GetString("MATHIRA", _cultures)) }
            ////, new Constituency { ID = 204, Name = "MATHIRA", CountyId = 20, Code = "83", Geom = (Geometry)reader.Read(rm.GetString("MATHIRA", _cultures)) }
            ////, new Constituency { ID = 205, Name = "RUNYENJES", CountyId = 20, Code = "57", Geom = (Geometry)reader.Read(rm.GetString("RUNYENJES", _cultures)) }
            ////, new Constituency { ID = 206, Name = "RUNYENJES", CountyId = 20, Code = "57", Geom = (Geometry)reader.Read(rm.GetString("RUNYENJES", _cultures)) }
            ////, new Constituency { ID = 207, Name = "RUNYENJES", CountyId = 14, Code = "57", Geom = (Geometry)reader.Read(rm.GetString("RUNYENJES", _cultures)) }
            ////, new Constituency { ID = 208, Name = "KERUGOYA/KUTUS", CountyId = 20, Code = "90", Geom = (Geometry)reader.Read(rm.GetString("KERUGOYA/KUTUS", _cultures)) }
            ////, new Constituency { ID = 209, Name = "KERUGOYA/KUTUS", CountyId = 20, Code = "90", Geom = (Geometry)reader.Read(rm.GetString("KERUGOYA/KUTUS", _cultures)) }
            ////, new Constituency { ID = 210, Name = "GICHUGU", CountyId = 20, Code = "88", Geom = (Geometry)reader.Read(rm.GetString("GICHUGU", _cultures)) }
            ////, new Constituency { ID = 211, Name = "MANYATTA", CountyId = 14, Code = "56", Geom = (Geometry)reader.Read(rm.GetString("MANYATTA", _cultures)) }
            ////, new Constituency { ID = 212, Name = "MWATATE", CountyId = 6, Code = "28", Geom = (Geometry)reader.Read(rm.GetString("MWATATE", _cultures)) }
            ////, new Constituency { ID = 213, Name = "MWATATE", CountyId = 6, Code = "28", Geom = (Geometry)reader.Read(rm.GetString("MWATATE", _cultures)) }
            ////, new Constituency { ID = 214, Name = "VOI", CountyId = 6, Code = "29", Geom = (Geometry)reader.Read(rm.GetString("VOI", _cultures)) }
            ////, new Constituency { ID = 215, Name = "TAVETA", CountyId = 6, Code = "26", Geom = (Geometry)reader.Read(rm.GetString("TAVETA", _cultures)) }
            ////, new Constituency { ID = 216, Name = "BUDALANGI", CountyId = 40, Code = "178", Geom = (Geometry)reader.Read(rm.GetString("BUDALANGI", _cultures)) }


            //);
        }
    }
}
