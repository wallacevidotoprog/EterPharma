using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Models
{
	public class Validade
	{
		public int ID { get; set; }
		public string NOME { get; set; }
		public DateTime DATA { get; set; }

		public List<ValidadeProdutos> PRODUTOS { get; set; }
		public List<ValidadeCategoria> CATEGORIA { get; set; }
	}
	public class ValidadeFiles
	{
		public int ID { get; set; }
		public string NOME { get; set; }
		public DateTime DATA { get; set; }
	}
	public class ValidadeProdutos
	{
		public int ID { get; set; }
		public string EAN { get; set; }
		public string COD_PRODUTO { get; set; }
		public string DESCRICAO_PRODUTO { get; set; }
		public int QTD { get;set; }
		public DateTime DATA { get; set; }
		public int CATEGORIA { get; set; }
	}
	public class ValidadeCategoria
	{
		public int ID { get; set; }
		public string NOME { get; set; }
	}
}
