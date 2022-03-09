using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LanguageWorker.model
{
	public class LanguagesContainer
	{
		public int[] Indexes
		{
			[CompilerGenerated]
			get
			{
				return int_0;
			}
			[CompilerGenerated]
			set
			{
				int_0 = value;
			}
		}

		public Dictionary<string, LanguageEntry> Languages
		{
			[CompilerGenerated]
			get
			{
				return dictionary_0;
			}
			[CompilerGenerated]
			set
			{
				dictionary_0 = value;
			}
		}

		public LanguagesContainer()
		{

			
		}

		public LanguagesContainer(int[] indexes, Dictionary<string, LanguageEntry> languages)
		{

			
			Indexes = indexes;
			Languages = languages;
		}

		[CompilerGenerated]
		private int[] int_0;

		[CompilerGenerated]
		private Dictionary<string, LanguageEntry> dictionary_0;
	}
}
