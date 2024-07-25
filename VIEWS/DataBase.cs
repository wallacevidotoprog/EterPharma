using EterPharma.Ex;
using EterPharma.Models;
using EterPharma.Properties;
using EterPharma.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EterPharma.VIEWS
{
	public partial class DataBase : Form
	{
		private IniFile ini;
		private RawPrinterHelper rawPrinter;
		private Dictionary<string, string> portComImpressora;
		public ProgressBar progressBar_status { get; set; }
		private List<Produtos> tempProdutos;
		private bool edit = false;
		private int editIDINDEX = -1;
		public DataBase()
		{
			InitializeComponent();
			//if (InputBox.Show("Qual a senha:", "SENHA =D", true) != "32195018")
			//{
			//	this.Close();
			//}
		}
		private async void DataBase_Load(object sender, EventArgs e)
		{

			comboBox_tipo.SelectedIndex = 0;
			comboBox_funcao.DataSource = Enum.GetValues(typeof(Funcao)).Cast<Funcao>().ToList();
			await Task.Run(() => DataProdutosGrid());

			for (int i = 0; i < MainWindow.database.EnderecoSJRPs.Count; i++)
			{
				listBox_bairro.Items.Add(MainWindow.database.EnderecoSJRPs[i].BAIRRO);
			}
			InitImp();
		}

		#region PROD
		private void pictureBox3_Click(object sender, EventArgs e)
		{
			this.Close();
		}


		private void DataProdutosGrid()
		{
			if (MainWindow.database?.Produtos != null)
			{
				dataGridView_dados.Invoke(new Action(() => { dataGridView_dados.DataSource = MainWindow.database.Produtos; }));
			}
			if (MainWindow.database?.Users != null)
			{
				dataGridView_user.Invoke(new Action(() => { dataGridView_user.DataSource = MainWindow.database.Users.ToList(); }));
			}
		}

		private async void pictureBox_import_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "Excel (*.xlsx)|*.xlsx";
				openFileDialog.FilterIndex = 2;
				openFileDialog.RestoreDirectory = true;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					textBox_contador.Text = $"AGUARDE, LENDO ARQUIVO ...";
					await Task.Run(() =>
					{
						tempProdutos = RWXLSX.ReadAllProdutos(openFileDialog.FileName, progressBar_status);

					});
					textBox_contador.Text = $"TOTAL DE LINHAS LIDAS [{tempProdutos.Count} ]";

				}
			}
		}

		private void pictureBox_atualiza_Click(object sender, EventArgs e)
		{
			MainWindow.database.Produtos = tempProdutos;
			dataGridView_dados.DataSource = MainWindow.database.Produtos.ToList();
			MainWindow.database.WriteProdutosBinary();
		}

		private void pictureBox_busca_Click(object sender, EventArgs e)
		{

			switch (comboBox_tipo.SelectedIndex)
			{
				case 0:
					dataGridView_dados.DataSource = MainWindow.database.Produtos.Where(x => x.EAN == textBox_codigo.Text.Trim().Replace(" ", null)).ToList();
					break;
				case 1:
					dataGridView_dados.DataSource = MainWindow.database.Produtos.Where(x => x.COD_PRODUTO == textBox_codigo.Text.Trim().Replace(" ", null)).ToList();
					break;
				case 2:
					dataGridView_dados.DataSource = MainWindow.database.Produtos.Where(x => x.DESCRICAO_PRODUTO.ToUpper().Contains(textBox_codigo.Text)).ToList();
					break;
				case 3:
					dataGridView_dados.DataSource = MainWindow.database.Produtos.Where(x => x.LABORATORIO.ToUpper().Contains(textBox_codigo.Text)).ToList();
					break;
				case 4:
					dataGridView_dados.DataSource = MainWindow.database.Produtos.Where(x => x.GRUPO.ToUpper().Contains(textBox_codigo.Text)).ToList();
					break;
				default:
					return;
					break;
			}
			if (((List<Produtos>)dataGridView_dados?.DataSource)?.Count() > 0)
			{
				textBox_codigo.Clear();
			}

		}

		private void textBox_codigo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				pictureBox_busca_Click(null, null);
			}
		}

		private void pictureBox_limpar_Click(object sender, EventArgs e)
		{
			textBox_codigo.Clear();
			dataGridView_dados.DataSource = MainWindow.database.Produtos;
		}
		#endregion

		#region USER



		private void pictureBox2_Click(object sender, EventArgs e)
		{
			if (MainWindow.database.Users == null)
			{
				MainWindow.database.Users = new eList<User>();
			}

			if (edit && editIDINDEX != -1)
			{
				MainWindow.database.Users[editIDINDEX].ID = textBox_id.Text;
				MainWindow.database.Users[editIDINDEX].Nome = textBox_nome.Text;
				MainWindow.database.Users[editIDINDEX].Funcao = (Funcao)comboBox_funcao.SelectedIndex;
				MainWindow.database.UserEvents(null, null);
				dataGridView_user.Rows[editIDINDEX].Cells[0].Value = textBox_id.Text;
				dataGridView_user.Rows[editIDINDEX].Cells[1].Value = textBox_nome.Text;
				dataGridView_user.Rows[editIDINDEX].Cells[2].Value = (Funcao)comboBox_funcao.SelectedIndex;
				dataGridView_user.DataSource = MainWindow.database.Users.ToList();
				dataGridView_user.CurrentCell = dataGridView_user.Rows[editIDINDEX].Cells[0];
				pictureBox4_Click(null, null);
			}
			else
			{

				if (textBox_nome.Text != string.Empty && textBox_id.Text != string.Empty)
				{
					if (!MainWindow.database.UserExite(textBox_id.Text))
					{
						MainWindow.database.Users.Add(new User
						{
							ID = textBox_id.Text,
							Nome = textBox_nome.Text,
							Funcao = (Funcao)comboBox_funcao.SelectedIndex,
							Status = true

						});

						dataGridView_user.DataSource = MainWindow.database.Users.ToList();
						pictureBox4_Click(null, null);
					}
					else
					{
						MessageBox.Show("ID já em uso.", "ALERTA", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
				else
				{
					MessageBox.Show("Preencha todos os campos.", "ALERTA", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}

		}


		private void pictureBox4_Click(object sender, EventArgs e)
		{
			textBox_id.Clear();
			textBox_nome.Clear();
			comboBox_funcao.SelectedIndex = 0;
			groupBox_modeedit.Visible = edit = false;
			editIDINDEX = -1;
		}


		#endregion

		#region ENDS		

		private void dataGridView_user_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex != -1)
			{
				groupBox_modeedit.Visible = edit = true;
				editIDINDEX = e.RowIndex;
				textBox_id.Text = MainWindow.database.Users[e.RowIndex].ID;
				textBox_nome.Text = MainWindow.database.Users[e.RowIndex].Nome;
				comboBox_funcao.SelectedIndex = (int)MainWindow.database.Users[e.RowIndex].Funcao;
			}

		}

		private void pictureBox5_Click(object sender, EventArgs e)
		{
			if (edit && editIDINDEX != -1)
			{
				dataGridView_user.Rows[editIDINDEX].Cells[3].Value = MainWindow.database.Users[editIDINDEX].Status = !MainWindow.database.Users[editIDINDEX].Status;
				MainWindow.database.UserEvents(null, null);
				dataGridView_user.DataSource = MainWindow.database.Users.ToList();
				dataGridView_user.CurrentCell = dataGridView_user.Rows[editIDINDEX].Cells[0];
				pictureBox4_Click(null, null);
			}
		}

		private async void pictureBox6_Click(object sender, EventArgs e)
		{
			if (edit && editIDINDEX != -1)
			{
				MainWindow.database.Users.RemoveAt(editIDINDEX);
				pictureBox4_Click(null, null);
				dataGridView_user.DataSource = MainWindow.database.Users.ToList();

			}
		}

		private void listBox_bairro_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox_bairro.SelectedIndex != -1)
			{
				listBox_log.Items.Clear();
				int selectedIndex = listBox_bairro.SelectedIndex;
				for (int i = 0; i < MainWindow.database.EnderecoSJRPs[selectedIndex].LOGADOURO.Count; i++)
				{
					listBox_log.Items.Add(MainWindow.database.EnderecoSJRPs[selectedIndex].LOGADOURO[i]);
				}
			}

		}

		private void button_buscarEnd_Click(object sender, EventArgs e)
		{
			List<string> t = MainWindow.database.GetZone(textBox_buscaEnd.Text);
			listBox_buca.Items.Clear();
			if (t.Count > 0)
			{

				for (int i = 0; i < t.Count; i++)
				{
					listBox_buca.Items.Add($"BAIRRO: {t[i]}");
				}
			}


		}

		private void listBox_buca_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox_buca.SelectedIndex != -1)
			{
				for (int i = 0; i < listBox_bairro.Items.Count; i++)
				{
					if (listBox_bairro.Items[i].ToString() == listBox_buca.Items[listBox_buca.SelectedIndex].ToString().Replace("BAIRRO: ", null))
					{
						listBox_bairro.SetSelected(i, true);
						break;
					}
				}
				for (int i = 0; i < listBox_log.Items.Count; i++)
				{
					if (listBox_log.Items[i].ToString().Contains(textBox_buscaEnd.Text.ToUpper()))
					{
						listBox_log.SetSelected(i, true);
					}
				}


			}
		}
		#endregion

		#region IMP
		void InitImp()
		{
			
			ini = new IniFile("config.ini");
			rawPrinter = new RawPrinterHelper(ini);
			portComImpressora = RawPrinterHelper.GetDeviceName();
			checkBox_imD_CheckedChanged(null, null);


			if (portComImpressora.Count > 0)
			{
				foreach (var item in portComImpressora)
				{
					comboBox_impF.Items.Add($"{item.Value} - {item.Key}");

				}
			}

			checkBox_imD.Checked = ini.Read("IMPRESSORAS", "IS_DYNAMIC")=="True"?true:false;

			textBox_impD.Text = ini.Read("IMPRESSORAS", "DYNAMIC");
			textBox_portCom.Text = ini.Read("IMPRESSORAS", "PORT_COM");
			textBox_impIP.Text = $"{ini.Read("IMPRESSORAS", "IP_IMP")}:{ini.Read("IMPRESSORAS", "IP_PORT")}" ;

		}
		private void checkBox_imD_CheckedChanged(object sender, EventArgs e)
		{
			SgroupBox_dinamico.Enabled = checkBox_imD.Checked;
			groupBox_fixo.Enabled = !checkBox_imD.Checked;
		}

		private void button_fPort_Click(object sender, EventArgs e)
		{
			if (portComImpressora.Count > 0)
			{
				string tempS = portComImpressora.ElementAt(comboBox_impF.SelectedIndex).Value;
				if (tempS.Contains("COM"))
				{
					textBox_portCom.Text = tempS.Substring(tempS.IndexOf("COM"), 4);
				}
				textBox_portCom.Clear();
			}
		}

		#endregion

		private void pictureBox_saveIMP_Click(object sender, EventArgs e)
		{
			if (textBox_impIP.Text.Trim().Split(':').Length == 2)
			{
				ini.Write("IMPRESSORAS", "IS_DYNAMIC", checkBox_imD.Checked.ToString());
				ini.Write("IMPRESSORAS", "DYNAMIC", textBox_impD.Text);
				ini.Write("IMPRESSORAS", "PORT_COM", textBox_portCom.Text);
				ini.Write("IMPRESSORAS", "IP_IMP", textBox_impIP.Text.Trim().Split(':')[0]);
				ini.Write("IMPRESSORAS", "IP_PORT", textBox_impIP.Text.Trim().Split(':')[1]);
				rawPrinter = new RawPrinterHelper(ini);
				MessageBox.Show("Dados Salvos.");
			}
			else
			{
				MessageBox.Show("Algo errado, verifique se digitou a porta da impressora");
			}
			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			rawPrinter.PrinterHelper(DevoImprimirCupomNaoFiscal());
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

		private void button2_Click(object sender, EventArgs e)
		{
			rawPrinter.PrinterHelperIP(DevoImprimirCupomNaoFiscal());
		}

		private void button3_Click(object sender, EventArgs e)
		{
			rawPrinter.AddLine("wallace vidoto de miranda");
			rawPrinter.AddLine("wallace vidoto de miranda",FormatText.Bolt);
			rawPrinter.AddLine("wallace vidoto de miranda",FormatText.Italic);
			rawPrinter.PrintDocument();
		}
	}
}
