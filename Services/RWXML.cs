using EterPharma.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EterPharma.Services
{
	public static class RWXML
	{
		public static async Task<Validade> DeserializePessoaFromXmlAsync(string xml, bool ev=false)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Validade));
			Validade validade = new Validade();
			using (FileStream fs = new FileStream(xml, FileMode.Open, FileAccess.Read))
			{
				validade = (Validade)await Task.Run(() => serializer.Deserialize(fs));
				fs.Close();
			}
			if (ev)
			{
				validade?.InitEvents();
			}
			return validade;
		}
		public static async Task<string> SerializeToXmlFileAsync(Validade obj)
		{
			string fileName = $@"{Directory.GetCurrentDirectory()}\DADOS\VALIDADE\{obj.DADOS.ID}-{obj.DADOS.NOME.Replace(" ", null)}-{obj.DADOS.DATA.ToString("ddMMyyyHHmmss")}.xml";


			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			XmlSerializer serializer = new XmlSerializer(typeof(Validade));
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				serializer.Serialize(fs, obj);
				//await Task.Run(() => serializer.Serialize(fs, obj));
				fs.Close();
			}
			return fileName;
		}
	}
}
