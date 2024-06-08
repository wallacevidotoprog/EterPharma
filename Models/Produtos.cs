
using DocumentFormat.OpenXml.Spreadsheet;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EterPharma.Models
{
	public class Produtos
	{
        public string EAN { get; set; }
		public string COD_PRODUTO { get; set; }
		public string DESCRICAO_PRODUTO { get; set; }
		public bool STATUS { get; set; }
		public string LABORATORIO { get; set; }
		public string GRUPO { get; set; }
	}
}
