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
		public static void SerializeToXmlFile(this Validade obj)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Validade));
			using (StreamWriter streamWriter = new StreamWriter(Directory.GetCurrentDirectory() + $@"\DADOS\VALIDADE\TEMPS\{obj.ID}-{obj.NOME.Trim()}-{obj.DATA.ToString("ddMMyyyHHmmss")}.xml",false,Encoding.UTF8))
			{
				serializer.Serialize(streamWriter, obj);
			}
		}
	}
}
