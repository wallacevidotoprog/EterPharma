using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EterPharma.VIEWS
{
	public partial class ViewPrint : Form
	{
		public ViewPrint()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			richTextBox1.AppendText(DevoImprimirCupomNaoFiscal());
		}
		public static string DevoImprimirCupomNaoFiscal()
		{
			var texto = new String(' ', 50);
			texto += "------------------------------------------------";
			texto += "--------------------------------------------------\n";
			texto += "<ce><c>Empresa Teste\n";
			texto += "CNPJ: xxxxxxxxxxxxxx Inscrição Estadual: yyyyyyyyyy\n";
			texto += "Rua: aaaaaaaaaaaa, Número: 999   Bairro: bbbbbbb\n";
			texto += "Cidade: zzzzzzzzzzzz nn\n";
			texto += "--------------------------------------------------\n";
			texto += "DANFE NFC-e - Documento Auxiliar\n";
			texto += "da Nota Fiscal Eletrônica para Consumidor Final\n";
			texto += "Não permite aproveitamento de crédito de ICMS\n";
			texto += "--------------------------------------------------\n";
			texto += "Código Descrição do Item  Vlr.Unit. Qtde Vlr.Total\n";
			texto += "--------------------------------------------------\n";
			texto += "</c>" +
					 "<c>333333 ITEM 01        37,14 001Un     37,14</c>\n";
			texto += "<c>444444 ITEM 02         13,61 001Un    13,61</c>\n";
			texto += "--------------------------------------------------\n";
			texto += "QTD. TOTAL DE ITENS                              2\n";
			texto += "VALOR TOTAL R$                               50,75\n";
			texto += "\n";
			texto += "FORMA DE PAGAMENTO                      Valor Pago\n";
			texto += "</c><c>DINHEIRO                              50,75\n";
			texto += "</c><c>VALOR PAGO R$                         50,75\n";
			texto += "TROCO R$                                      0,00\n";
			texto += "</c><c>---------------------------------------</c>\n";
			texto += "Val Aprox Tributos R$ 16.29 (32.10%) Fonte: IBPT  \n";
			texto += "<c>-------------------------------------------</c>\n";
			texto += "<c><ce><ce>NFC-e nº 000001 Série 001\n";
			texto += "Emissão 03/12/2013 15:50:16</c></ce>\n";
			texto += "<ce><b></c><c><b>Via Consumidor</c></b>\n";
			texto += "</b></ce><c><Consulte pela Chave de Acesso em</c>\n";
			texto += "<c>https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx\n";
			texto += "\n";
			texto += "<c><b>CHAVE DE ACESSO</b></ce></c>\n";
			texto += "<c><ce>8877 2222 4444 1101 7777 6666</ce></c>\n";
			texto += "<c><ce>0000 8888 3333 6666 7788</ce></c>\n";
			texto += "\n\n";

			return texto;
		}

		private void ViewPrint_Load(object sender, EventArgs e)
		{

		}
	}
}
