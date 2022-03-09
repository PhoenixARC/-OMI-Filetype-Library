using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LanguageWorker.model
{
	public class LanguageEntry
	{
		public string Name
		{
			[CompilerGenerated]
			get
			{
				return string_0;
			}
			[CompilerGenerated]
			set
			{
				string_0 = value;
			}
		}

		public int Size
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

		public List<MessageEntry> Messages
		{
			[CompilerGenerated]
			get
			{
				return list_0;
			}
			[CompilerGenerated]
			set
			{
				list_0 = value;
			}
		}

		public LanguageEntry()
		{
			
		}

		public LanguageEntry(string name, int size)
		{
			
			Name = name;
			Size = size;
			Messages = new List<MessageEntry>();
		}

		[CompilerGenerated]
		private string string_0;

		[CompilerGenerated]
		private int int_0;

		[CompilerGenerated]
		private List<MessageEntry> list_0;
	}
}
