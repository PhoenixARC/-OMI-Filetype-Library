using System;
using System.IO;
using LanguageWorker.model;

namespace LanguageWorker
{
	public class LanguageBuilder
	{
		public void Build(LanguagesContainer languages, string path)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.Create)))
			{
				Stream baseStream = binaryWriter.BaseStream;
				ArrSupport.WriteIntToStream(2, baseStream);
				int count = languages.Languages.Count;
				ArrSupport.WriteIntToStream(count, baseStream);
				baseStream.WriteByte(1);
				int num = languages.Indexes.Length;
				ArrSupport.WriteIntToStream(num, baseStream);
				for (int i = 0; i < num; i++)
				{
					ArrSupport.WriteIntToStream(languages.Indexes[i], baseStream);
				}
				byte[] array = method_0(languages);
				baseStream.Write(array, 0, array.Length);
			}
		}

		public void BuildJSON(LanguagesContainer languages, string path)
		{
			
			StreamWriter sw = new StreamWriter(path);
		}

		private byte[] method_0(LanguagesContainer languagesContainer_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			MemoryStream memoryStream2 = new MemoryStream();
			foreach (LanguageEntry languageEntry in languagesContainer_0.Languages.Values)
			{
				byte[] array = method_1(languageEntry);
				ArrSupport.WriteStringToStream(languageEntry.Name, memoryStream);
				ArrSupport.WriteIntToStream(array.Length, memoryStream);
				memoryStream2.Write(array, 0, array.Length);
			}
			MemoryStream memoryStream3 = new MemoryStream();
			byte[] array2 = memoryStream.ToArray();
			memoryStream3.Write(array2, 0, array2.Length);
			array2 = memoryStream2.ToArray();
			memoryStream3.Write(array2, 0, array2.Length);
			return memoryStream3.ToArray();
		}

		private byte[] method_1(LanguageEntry languageEntry_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(0);
			ArrSupport.WriteIntToStream(0x201, memoryStream);
			ArrSupport.WriteStringToStream(languageEntry_0.Name, memoryStream);
			ArrSupport.WriteIntToStream(languageEntry_0.Messages.Count, memoryStream);
			foreach (MessageEntry messageEntry in languageEntry_0.Messages)
			{
				ArrSupport.WriteStringToStream(messageEntry.Message, memoryStream);
			}
			return memoryStream.ToArray();
		}

		public LanguageBuilder()
		{

			ArrSupport = new global::ArraySupport();
			
		}

		private global::ArraySupport ArrSupport;
	}
}
