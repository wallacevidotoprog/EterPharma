using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EterPharma.Services
{
	// using System.Xml.Serialization;
	// XmlSerializer serializer = new XmlSerializer(typeof(VALIDADE));
	// using (StringReader reader = new StringReader(xml))
	// {
	//    var test = (VALIDADE)serializer.Deserialize(reader);
	// }

	[XmlRoot(ElementName = "DADOSDOC")]
	public class DADOSDOC
	{

		[XmlElement(ElementName = "ID")]
		public object ID;

		[XmlElement(ElementName = "NOME")]
		public object NOME;

		[XmlElement(ElementName = "DATA")]
		public DateTime DATA;
	}

	[XmlRoot(ElementName = "PROD")]
	public class PROD
	{

		[XmlElement(ElementName = "ID")]
		public List<object> ID;

		[XmlElement(ElementName = "EAN")]
		public object EAN;

		[XmlElement(ElementName = "COD_PRODUTO")]
		public object CODPRODUTO;

		[XmlElement(ElementName = "DESCRICAO_PRODUTO")]
		public object DESCRICAOPRODUTO;

		[XmlElement(ElementName = "QTD")]
		public object QTD;

		[XmlElement(ElementName = "DATA")]
		public object DATA;

		[XmlElement(ElementName = "CATEGORIA")]
		public object CATEGORIA;
	}

	[XmlRoot(ElementName = "PRODUTOS")]
	public class PRODUTOS
	{

		[XmlElement(ElementName = "PROD")]
		public List<PROD> PROD;
	}

	[XmlRoot(ElementName = "CAT")]
	public class CAT
	{

		[XmlElement(ElementName = "ID")]
		public List<object> ID;

		[XmlElement(ElementName = "NOME")]
		public object NOME;
	}

	[XmlRoot(ElementName = "CATEGORIAS")]
	public class CATEGORIAS
	{

		[XmlElement(ElementName = "CAT")]
		public List<CAT> CAT;
	}

	[XmlRoot(ElementName = "VALIDADE")]
	public class VALIDADEXML
	{

		[XmlElement(ElementName = "DADOSDOC")]
		public DADOSDOC DADOSDOC;

		[XmlElement(ElementName = "PRODUTOS")]
		public PRODUTOS PRODUTOS;

		[XmlElement(ElementName = "CATEGORIAS")]
		public CATEGORIAS CATEGORIAS;
	}
}
