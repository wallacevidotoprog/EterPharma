using DocumentFormat.OpenXml.Vml.Spreadsheet;
using EterPharma.Ex;
using EterPharma.Models;
using EterPharma.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EterPharma.VIEWS
{
	public partial class RelatorioValidade : Form
	{
		private List<string> xmlFiles;
		private List<(Validade validade, bool status)> validadeList;
		private List<ValidadeProdutos> validadeProdutos;

		public RelatorioValidade()
		{
			InitializeComponent();
		}

		private async void GetFilesXML()
		{
			dataGridView_validadeFile.Invoke(new Action(() =>
			{
				dataGridView_validadeFile.Rows.Clear();
				validadeList.Clear();

			}));

			string[] fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory() + $@"\DADOS\VALIDADE\", "*.xml");
			xmlFiles = new List<string>();
			for (int i = 0; i < fileEntries.Length; i++)
			{
				xmlFiles.Add(fileEntries[i]);
				Validade tempV = await RWXML.DeserializePessoaFromXmlAsync(fileEntries[i]);
				validadeList.Add((tempV, true));
				dataGridView_validadeFile.Invoke(new Action(() =>
				{
					dataGridView_validadeFile.Rows.Add(new object[]
				{
						i.ToString(),tempV.DADOS.DATA.ToString("ddMMyyHmmss"),tempV.DADOS.NOME,tempV.DADOS.DATA.ToString(),true
				});
				}));
			}
			pictureBox_busca.Invoke(new Action(() => { pictureBox_busca_Click(null, null); }));
		}
		private void RelatorioValidade_Load(object sender, EventArgs e)
		{
			DateTime timenew = dateTimePicker_ate.Value;
			timenew = timenew.AddMonths(4);
			dateTimePicker_ate.Value = timenew;
			validadeList = new List<(Validade validade, bool status)>();
			Task.Run(new Action(() => GetFilesXML()));
		}

		private void pictureBox_sair_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private async void pictureBox_busca_Click(object sender, EventArgs e)
		{
			pictureBox_busca.Focus();
			if (xmlFiles != null)
			{
				dataGridView_validadeFile.Rows.Clear();
				validadeList.Clear();
				for (int i = 0; i < xmlFiles.Count; i++)
				{
					if (xmlFiles[i].ToUpper().EndsWith("XML"))
					{
						Validade tempV = await RWXML.DeserializePessoaFromXmlAsync(xmlFiles[i]);
						if (tempV.DADOS.DATA.Month == dateTimePicker_dataBusca.Value.Month && tempV.DADOS.DATA.Year == dateTimePicker_dataBusca.Value.Year)
						{
							validadeList.Add((tempV, true));
							dataGridView_validadeFile.Rows.Add(new object[]
							{
							i.ToString(),tempV.DADOS.DATA.ToString("ddMMyyHmmss"),tempV.DADOS.NOME,tempV.DADOS.DATA.ToString(),true
							});
						}
					}
				}
			}
		}

		private void dataGridView_validadeFile_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			bool temp = (bool)dataGridView_validadeFile.Rows[e.RowIndex].Cells[4].Value;
			dataGridView_validadeFile.Rows[e.RowIndex].Cells[4].Value = !temp;

			validadeList[e.RowIndex] = (validade: validadeList[e.RowIndex].validade, status: !temp);
		}

		private void pictureBox_import_Click(object sender, EventArgs e)
		{
			try
			{
				if (validadeProdutos == null)
				{
					validadeProdutos = new List<ValidadeProdutos>();
				}
				else
				{
					validadeProdutos.Clear();
				}

				ListViewItem item = null;
				listView_produtos.Items.Clear();

				foreach (var Value in validadeList)
				{
					if (Value.status == true)
					{
						ListViewGroup group = new ListViewGroup($" Funcionário: [{Value.validade.DADOS.ID} - {MainWindow.database.Users[Extensions.ReturnIndexUser(Value.validade.DADOS.ID)].Nome}]  |  Total de Itens: [{Value.validade.PRODUTOS.Count}]", HorizontalAlignment.Left);
						listView_produtos.Groups.Add(group);

						for (int j = 0; j < Value.validade.PRODUTOS.Count; j++)
						{
							item = new ListViewItem(Value.validade.PRODUTOS[j].ID.ToString());
							item.SubItems.Add(Value.validade.PRODUTOS[j].EAN);
							item.SubItems.Add(Value.validade.PRODUTOS[j].COD_PRODUTO);
							item.SubItems.Add(Value.validade.PRODUTOS[j].DESCRICAO_PRODUTO);
							item.SubItems.Add(Value.validade.PRODUTOS[j].QTD.ToString());
							item.SubItems.Add(Value.validade.PRODUTOS[j].DATA.ToString("dd/MM/yyyy"));
							item.Group = group;

							//if ((DateTime.Now.Month + numericUpDown_mesV.Value) >= Value.validade.PRODUTOS[j].DATA.Month)
							//{
							//	item.BackColor = Color.LightCoral;
							//}
							listView_produtos.Items.Add(item);

							validadeProdutos.Add(Value.validade.PRODUTOS[j]);

						}
					}
				}

				mesV_ValueChanged(null, null);
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.Message);
			}
		}

		private async void pictureBox_exportExcel_Click(object sender, EventArgs e)
		{
			try
			{
				if (validadeProdutos == null)
				{
					return;
				}
				using (SaveFileDialog op = new SaveFileDialog())
				{
					op.FileName = $"Listagem de validade de {dateTimePicker_dataBusca.Value.ToString("MMMM-yyyy")}.xlsx";
					op.Filter = "Excel Files|*.xlsx";
					op.Title = "Save an Excel File";

					if (op.ShowDialog() == DialogResult.OK)
					{
						MainWindow.database._progressBar.Style = ProgressBarStyle.Marquee;
						await Task.Run(() => RWXLSX.SalveValidade(validadeProdutos, op.FileName));
						MainWindow.database._progressBar.Style = ProgressBarStyle.Continuous;
					}
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private async void pictureBox_exV_Click(object sender, EventArgs e)
		{
			try
			{
				if (validadeProdutos == null)
				{
					return;
				}



				var temp = validadeProdutos.Where(x => x.DATA.Month <= (DateTime.Now.Month)).ToList();



				using (SaveFileDialog op = new SaveFileDialog())
				{
					op.FileName = $"Listagem de validade de {dateTimePicker_dataBusca.Value.ToString("MMMM-yyyy")}.xlsx";
					op.Filter = "Excel Files|*.xlsx";
					op.Title = "Save an Excel File";

					if (op.ShowDialog() == DialogResult.OK)
					{
						MainWindow.database._progressBar.Style = ProgressBarStyle.Marquee;
						await Task.Run(() => RWXLSX.SalveValidade(temp, op.FileName));
						MainWindow.database._progressBar.Style = ProgressBarStyle.Continuous;
					}
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private void mesV_ValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (validadeProdutos == null)
				{
					return;
				}

				for (int i = 0; i < validadeProdutos.Count; i++)
				{
					if (validadeProdutos[i].DATA.Month >= dateTimePicker_de.Value.Month &&
						validadeProdutos[i].DATA.Year >= dateTimePicker_de.Value.Year &&
						validadeProdutos[i].DATA.Month <= dateTimePicker_ate.Value.Month &&
						validadeProdutos[i].DATA.Year <= dateTimePicker_ate.Value.Year
						)
					{
						listView_produtos.Items[i].BackColor = Color.LightCoral;
					}
					//if ((DateTime.Now.Month + numericUpDown_mesV.Value) >= validadeProdutos[i].DATA.Month)
					//{
						
					//}
					else
					{
						listView_produtos.Items[i].BackColor = Color.White;
					}
				}
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.Message);
			}
		}
	}
}
