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
		public static Validade DeserializePessoaFromXml(string xml)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Validade));
			using (StreamReader streamReader = new StreamReader(xml, Encoding.UTF8))
			{
				return (Validade)serializer.Deserialize(streamReader);
			}
		}
		public static string SerializeToXmlFile(this Validade obj, bool temp = true)
		{
			string fileName = $@"{Directory.GetCurrentDirectory()}\DADOS\VALIDADE\{(temp ? "TEMPS" : "FINALIZADA")}\{obj.ID}-{obj.NOME.Replace(" ", null)}-{obj.DATA.ToString("ddMMyyyHHmmss")}.xml";


			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}


			XmlSerializer serializer = new XmlSerializer(typeof(Validade));
			using (StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
			{
				serializer.Serialize(streamWriter, obj);
			}
			return fileName;
		}
	}
}
