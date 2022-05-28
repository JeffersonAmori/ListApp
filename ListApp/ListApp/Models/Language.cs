using System.Collections.Generic;

namespace ListApp.Models
{
	public class Language
	{
		public Language(string name, string ci)
		{
			Name = name;
			CI = ci;
		}

		public string Name { get; }

		public string CI { get; }

		public readonly static IReadOnlyList<Language> KnownLanguages = new List<Language>()
			{
				{ new Language("English", "en") },
				{ new Language("Português", "pt") }
			};
	}
}
