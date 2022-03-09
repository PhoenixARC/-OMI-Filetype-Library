using System;
using System.Runtime.CompilerServices;

namespace LanguageWorker.model
{
	public class MessageEntry
	{
		public string Message
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

		public MessageEntry()
		{

			
		}

		public MessageEntry(string message)
		{

			
			Message = message;
		}

		[CompilerGenerated]
		private string string_0;
	}
}
