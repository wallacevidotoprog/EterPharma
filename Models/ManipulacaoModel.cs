using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Models
{
	[Serializable]
	public class ManipulacaoModel
	{
		public string ID { get; set; }
		public DadosAtemdimento DADOSATENDIMENTO { get; set; }
		public object DADOSCLIENTE { get; set; }
		public List<string> MEDICAMENTO { get; set; }
		public string OBSGERAL { get; set; }
        public int SITUCAO { get; set; }
		public int FORMAPAGAMENTO { get; set; }
		public int MODOENTREGA { get; set; }
	}

	[Serializable]
	public class DadosAtemdimento
	{
		public string ATEN_LOJA { get; set; }
		public DateTime DATA { get; set; }
		public string ATEN_MANI { get; set; }
	}
	[Serializable]
	public class DadosCliente
	{
		public string CPF { get; set; }
		public string RG { get; set; }
		public string NOME { get; set; }
		public string TELEFONE { get; set; }
		public object ENDERECO { get; set; }
	}
	[Serializable]
	public class Endereco
	{
		public string LOGRADOURO { get; set; }
		public string NUMERO { get; set; }
		public string BAIRRO { get; set; }
		public string OBS { get; set; }
	}

}
