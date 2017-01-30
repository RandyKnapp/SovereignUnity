using System;

namespace Sovereign
{
	public static class Names
	{
		private static readonly Random rand = new Random();

		public static readonly string[] ChiefTitles = {
			"Chief", "Jarl", "Arl", "Earl"
		};

		public static readonly string[] MaleNames = {
			"Ake", "Arvid", "Asger", "Asmund", "Audun",
			"Balder", "Bard", "Birger", "Bjarke", "Bjarte", "Bjorn", "Brandt", "Brynjar",
			"Calder", "Canute", "Carr", "Colborn", "Colden", "Cyler", "Dagfinn", "Destin",
			"Eerikki", "Egil", "Einar", "Eindride", "Eirik", "Elof", "Endre", "Erland",
			"Felman", "Fiske", "Folke", "Frey", "Frijof", "Frode", "Gandalf", "Geir", "Gosta",
			"Gudbrand", "Gunnar", "Hagen", "Hakon", "Haldor", "Halvar", "Halvdan", "Havardr",
			"Hjalmar", "Ingvar", "Inghard", "Ivar",
			"Jari", "Jerrik", "Kare", "Kelby", "Kensley", "Kirk", "Kjell", "Knud", "Kustaa",
			"Lamont", "Latham", "Leif", "Loki", "Manning", "Odin", "Olaf", "Olin", "Osmund",
			"Rolph", "Rangvald", "Ragnar", "Raul", "Sigurd", "Soini", "Sten", "Stian", "Stigr", "Sveinn",
			"Tait", "Tarben", "Thor", "Thorvaldr", "Torbjorn", "Torvald", "Troels", "Trygve", "Tyr",
			"Uffe", "Ulf", "Unn", "Vali", "Vern", "Vidar"
		};

		public static readonly string[] FemaleNames = {
			"Alfhild", "Alvilda", "Ase", "Aslaug", "Asta", "Astrid",
			"Bergljot", "Bodil", "Borghild", "Brenna",
			"Dagmar", "Dagny", "Eerika", "Eira", "Embla", "Eydis", "Freya",
			"Gerd", "Gul", "Gunbog", "Gunhild", "Gunvor", "Gyda",
			"Hege", "Helga", "Hertha", "Inger", "Ingrid", "Inkeri", "Iona",
			"Jorunn", "Kari", "Lathertha", "Magnhild", "Nanna", "Oili", "Olga", "Oydis",
			"Ragna", "Ragnhild", "Runa",
			"Sassa", "Sigfrid", "Signy", "Sigrun", "Siri", "Siv", "Solveig", "Sylvi",
			"Thora", "Thyra", "Tone", "Tordis", "Torhild", "Tove", "Turid", "Tyra", "Ylva"
		};

		public static readonly string[] SpouseNames = { "wife", "husband" };

		public static string GetRandom(string[] names)
		{
			return names[rand.Next(0, names.Length)];
		}

		public static string GetSpouseType(Person person)
		{
			return SpouseNames[(int)person.Sex];
		}

		public static string GetRandomPersonName(Sex sex)
		{
			return sex == Sex.Male ? MaleNames[rand.Next(MaleNames.Length)] : FemaleNames[rand.Next(FemaleNames.Length)];
		}
	}
}