using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EterPharma.VIEWS
{
	public partial class GerarValidade : Form
	{
		bool mode_edit;
		bool editProduto;
		int indexEditProduto;
		bool mode_new;
		string docFile;

		Validade validade;
		List<ValidadeProdutos> validadeProdutos;
		List<ValidadeCategoria> validadeCategorias;

		List<string> xmlTemps;
		List<string> xmlFinalizadas;



		Produtos produtosInput;

		public GerarValidade()
		{
			InitializeComponent();
		}

		#region MyFunc
		private void GetFilesXML()
		{

			dataGridView_validadeFileTemp.Invoke(new Action(() =>
			{
				dataGridView_validadeFileTemp.Rows.Clear();
			}));
			xmlTemps = new List<string>();
			string[] fileEntriesTemps = Directory.GetFiles(Directory.GetCurrentDirectory() + $@"\DADOS\VALIDADE\TEMPS");
			for (int i = 0; i < fileEntriesTemps.Length; i++)
			{
				if (fileEntriesTemps[i].ToUpper().EndsWith("XML"))
				{
					xmlTemps.Add(fileEntriesTemps[i]);
					Validade tempV = RWXML.DeserializePessoaFromXml(fileEntriesTemps[i]);
					dataGridView_validadeFileTemp.Invoke(new Action(() =>
					{
						dataGridView_validadeFileTemp.Rows.Add(new string[]
					{
						i.ToString(),tempV.DATA.ToString("ddMMyyHmmss"),tempV.NOME,tempV.DATA.ToString()
					}); ;
					}));

				}
			}
			dataGridView_validadeFile.Invoke(new Action(() =>
			{
				dataGridView_validadeFile.Rows.Clear();
			}));

			string[] fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory() + $@"\DADOS\VALIDADE\FINALIZADA");
			xmlFinalizadas = new List<string>();
			for (int i = 0; i < fileEntries.Length; i++)
			{
				if (fileEntries[i].ToUpper().EndsWith("XML"))
				{
					xmlFinalizadas.Add(fileEntries[i]);
					Validade tempV = RWXML.DeserializePessoaFromXml(fileEntries[i]);
					dataGridView_validadeFile.Invoke(new Action(() =>
					{
						dataGridView_validadeFile.Rows.Add(new string[]
					{
						i.ToString(),tempV.DATA.ToString("ddMMyyHmmss"),tempV.NOME,tempV.DATA.ToString()
					});
					}));

				}

			}
		}
		public void CBListUser()
		{
			Dictionary<string, string> users = new Dictionary<string, string>();

			for (int i = 0; i < MainWindow.database.Users.Count; i++)
			{
				if (MainWindow.database.Users[i].Status)
				{
					users.Add(
						MainWindow.database.Users[i].ID,
						$"{MainWindow.database.Users[i].ID} - {MainWindow.database.Users[i].Nome}");
				}

			}
			BindingSource bindingSource = new BindingSource
			{
				DataSource = users
			};
			comboBox_user.DataSource = bindingSource;
			comboBox_user.DisplayMember = "Value";
			comboBox_user.ValueMember = "Key";
		}
		public void CBListCategoria()
		{
			Dictionary<int, string> categoria = new Dictionary<int, string>();

			for (int i = 0; i < validadeCategorias.Count; i++)
			{
				ListViewGroup group = new ListViewGroup(validadeCategorias[i].NOME, HorizontalAlignment.Left);
				listView1.Groups.Add(group);

				categoria.Add(
						validadeCategorias[i].ID,
						$"{validadeCategorias[i].ID} - {validadeCategorias[i].NOME}");
			}
			BindingSource bindingSource = new BindingSource
			{
				DataSource = categoria
			};
			comboBox_categoria.DataSource = bindingSource;
			comboBox_categoria.DisplayMember = "Value";
			comboBox_categoria.ValueMember = "Key";


		}
		private void RefrashGrid()
		{
			listView1.Items.Clear();
			ListViewItem item = null;
			for (int i = 0; i < validadeCategorias.Count; i++)
			{
				ListViewGroup group = new ListViewGroup(validadeCategorias[i].NOME, HorizontalAlignment.Left);
				listView1.Groups.Add(group);

				List<ValidadeProdutos> tp = validadeProdutos.Where(x => x.CATEGORIA == validadeCategorias[i].ID).ToList();
				for (int x = 0; x < tp.Count; x++)
				{
					item = new ListViewItem(tp[x].ID.ToString());
					item.SubItems.Add(tp[x].EAN);
					item.SubItems.Add(tp[x].COD_PRODUTO);
					item.SubItems.Add(tp[x].DESCRICAO_PRODUTO);
					item.SubItems.Add(tp[x].QTD.ToString());
					item.SubItems.Add(tp[x].DATA.ToString());
					item.Group = group;
					listView1.Items.Add(item);
				}

			}
		}
		private bool FProduto()
		{
			bool tempBool = false;
			try
			{
				Produtos tempProdutos;
				produtosInput = null;
				if (textBox_codigo.Text.Trim().Length < 7)
				{
					tempProdutos = MainWindow.database.Produtos.Find(x => x.COD_PRODUTO.Contains(textBox_codigo.Text.Trim()));
				}
				else
				{
					tempProdutos = MainWindow.database.Produtos.Find(x => x.EAN.Contains(textBox_codigo.Text.Trim()));
				}

				if (tempProdutos != null)
				{
					produtosInput = tempProdutos;
					textBox_nproduto.Text = $"{produtosInput.DESCRICAO_PRODUTO}";
					textBox_nproduto.ReadOnly = tempBool = true;
				}
				else
				{
					MessageBox.Show("Cédigo não encontrado.\nDigite o nome do produto no campo a baixo do código.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					textBox_nproduto.ReadOnly = false;
				}

			}
			catch (Exception ex)
			{

				throw;
			}
			return tempBool;
		}
		private void OpenNewDoc(bool state)
		{
			groupBox_ne.Visible = state;
			comboBox_user.Enabled = state;
			dateTimePicker_dataD.Enabled = state;
		}
		private void NewDoc(bool state)
		{
			switch (state)
			{
				case true:
					pictureBox_novaV.Image = Properties.Resources.arquivo;
					groupBox_ne.Size = new Size(566, 315);
					groupBox_insert.Visible = true;
					comboBox_user.Enabled = false;
					dateTimePicker_dataD.Enabled = false;
					break;
				case false:
					pictureBox_novaV.Image = Properties.Resources.novo_arquivo;
					mode_new = mode_edit = false;
					docFile = null;
					validade = null;
					validadeProdutos = null;
					validadeCategorias = null;
					produtosInput = null;
					groupBox_insert.Visible = false;
					comboBox_user.Enabled = true;
					dateTimePicker_dataD.Enabled = true;
					groupBox_ne.Size = new Size(566, 88);
					listView1.Items.Clear();
					OpenNewDoc(false);
					break;
			}

		}
		private void ResetPropProduto()
		{
			produtosInput = null;
			textBox_codigo.Clear();
			textBox_nproduto.Clear();
			textBox_nproduto.ReadOnly = true;
			numericUpDown_qtd.Value = 1;
			pictureBox_addItem.Image = Properties.Resources.adicionar_ficheiro;
		}
		#endregion

		private async void GerarValidade_Load(object sender, EventArgs e)
		{
			Task.Run(new Action(() => GetFilesXML()));
			comboBox_user.Invoke(new Action(() => CBListUser()));
			groupBox_ne.Size = new System.Drawing.Size(566, 88);
			pictureBox_novaV.Image = Properties.Resources.novo_arquivo;
		}
		private void pictureBox3_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void pictureBox_novo_Click(object sender, EventArgs e)
		{
			if (mode_new || mode_edit)
			{
				NewDoc(false);
				OpenNewDoc(true);
			}
			else
			{
				OpenNewDoc(true);
			}
		}

		private void comboBox_user_Validated(object sender, EventArgs e)
		{
			if (!MainWindow.database.UserExite((string)comboBox_user.SelectedValue))
			{
				comboBox_user.SelectedIndex = 0;
			}
		}

		private void pictureBox_novaV_Click(object sender, EventArgs e)
		{
			try
			{
				if (!mode_new && !mode_edit)
				{
					mode_new = true;
					validade = new Validade
					{
						ID = MainWindow.database.Users[comboBox_user.SelectedIndex].ID,
						NOME = MainWindow.database.Users[comboBox_user.SelectedIndex].Nome,
						DATA = dateTimePicker_data.Value
					};
					if (validadeCategorias == null)
					{
						validadeCategorias = new List<ValidadeCategoria>();
						validadeCategorias.Add(new ValidadeCategoria { ID = 0, NOME = "S/ CATEGORIA" });
						validade.CATEGORIA = validadeCategorias;
						CBListCategoria();
					}
					docFile = RWXML.SerializeToXmlFile(validade);
					NewDoc(true);

				}
				else if (!mode_edit && mode_new)
				{
					if (MessageBox.Show($"Deseja excluir esse documento ?", "Excluir Documento", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
					{
						NewDoc(false);
					}
				}
				else if (mode_edit && !mode_new)
				{

					if (MessageBox.Show($"Deseja excluir esse documento ?", "Excluir Documento", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
					{
						NewDoc(false);
					}
				}
				else
				{
					if (MessageBox.Show($"Deseja Cancelar esse documento ?", "Cancelar Documento", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
					{
						NewDoc(false);
					}
				}
				GetFilesXML();
			}
			catch (Exception ex)
			{

				throw;
			}

		}

		private void pictureBox_addCategoria_Click(object sender, EventArgs e)
		{
			string result = InputBox.Show("Por favor, insira a categoria:", "Categoria");

			if (result != string.Empty)
			{

				validadeCategorias.Add(new ValidadeCategoria { ID = validadeCategorias.Count, NOME = result });
				CBListCategoria();
			}
		}

		private void pictureBox_addItem_Click(object sender, EventArgs e)
		{
			pictureBox_addItem.Focus();
			if (validadeProdutos == null)
			{
				validadeProdutos = new List<ValidadeProdutos>();
				validade.PRODUTOS = validadeProdutos;
			}

			if (editProduto)
			{
				validadeProdutos[indexEditProduto].EAN = textBox_nproduto.ReadOnly ? produtosInput.EAN : "NAN";
				validadeProdutos[indexEditProduto].COD_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.COD_PRODUTO : textBox_codigo.Text;
				validadeProdutos[indexEditProduto].DESCRICAO_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.DESCRICAO_PRODUTO : textBox_nproduto.Text;
				validadeProdutos[indexEditProduto].QTD = (int)numericUpDown_qtd.Value;
				validadeProdutos[indexEditProduto].DATA = dateTimePicker_data.Value;
				validadeProdutos[indexEditProduto].CATEGORIA = comboBox_categoria.SelectedIndex;
				ResetPropProduto();
			}
			else /*if (mode_new && mode_edit && !editProduto)*/
			{
				validadeProdutos.Add(new ValidadeProdutos
				{
					ID = validadeProdutos.Count(),
					EAN = textBox_nproduto.ReadOnly ? produtosInput.EAN : "NAN",
					COD_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.COD_PRODUTO : textBox_codigo.Text.ToUpper(),
					DESCRICAO_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.DESCRICAO_PRODUTO : textBox_nproduto.Text.ToUpper(),
					CATEGORIA = comboBox_categoria.SelectedIndex,
					DATA = dateTimePicker_data.Value,
					QTD = (int)numericUpDown_qtd.Value

				});
				ResetPropProduto();
			}
			Task.Run(() => RWXML.SerializeToXmlFile(validade));
			RefrashGrid();
		}

		private void textBox_codigo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FProduto();
			}
		}

		private void eDITARToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (listView1.SelectedItems.Count > 0)
				{
					int selectedItem = indexEditProduto = int.Parse(listView1.SelectedItems[0].SubItems[0].Text);
					editProduto = true;
					produtosInput = null;
					pictureBox_addItem.Image = Properties.Resources.atualizar_ficheiro;

					textBox_codigo.Text = validadeProdutos[selectedItem].EAN == "NAN" ? validadeProdutos[selectedItem].COD_PRODUTO : validadeProdutos[selectedItem].EAN;
					textBox_nproduto.Text = validadeProdutos[selectedItem].DESCRICAO_PRODUTO;
					numericUpDown_qtd.Value = validadeProdutos[selectedItem].QTD;
					dateTimePicker_data.Value = validadeProdutos[selectedItem].DATA;
					comboBox_categoria.SelectedIndex = validadeProdutos[selectedItem].CATEGORIA;
					FProduto();
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private void eXCLUIRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{

				if (listView1.SelectedItems.Count > 0)
				{
					int temp = int.Parse(listView1.SelectedItems[0]?.SubItems[0].Text);
					if (MessageBox.Show($"Deseja excluir esse item ?\n{listView1.SelectedItems[0]?.SubItems[3].Text}", "Excluir Item", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
					{

						if (temp >= 0)
						{
							validadeProdutos.RemoveAt(temp);

							for (int i = 0; i < validadeProdutos.Count; i++)
							{
								validadeProdutos[i].ID = i;
							}
							RefrashGrid();
						}
					}
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private async void dataGridView_validadeFile_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (mode_new || mode_edit)
			{
				DialogResult result = MessageBox.Show("Há um documento em aberto, deseja fecha-lo ?\nOs dados seram salvos em TEMP", "ALERTA", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				if (result == DialogResult.Cancel)
				{ return; }
				else if (result == DialogResult.OK)
				{
					await Task.Run(() => RWXML.SerializeToXmlFile(validade));
					GetFilesXML();
				}
			}
			try
			{
				validade = null;
				validadeCategorias = null;
				validadeProdutos = null;
				switch (((DataGridView)sender).Name)
				{
					case "dataGridView_validadeFile":

						validade = RWXML.DeserializePessoaFromXml(xmlFinalizadas[Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value)]);
						break;
					case "dataGridView_validadeFileTemp":
						validade = RWXML.DeserializePessoaFromXml(xmlTemps[Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value)]);
						break;
				}

				if (validade != null)
				{
					validadeProdutos = validade.PRODUTOS;
					validadeCategorias = validade.CATEGORIA;
					dateTimePicker_dataD.Value = validade.DATA;
					comboBox_user.SelectedIndex = Extensions.ReturnIndexUser(validade.ID);

					groupBox_ne.Size = new Size(566, 315);
					groupBox_ne.Visible = true;
					groupBox_insert.Visible = true;
					comboBox_user.Enabled = false;
					dateTimePicker_dataD.Enabled = false;
					mode_edit = true;
					pictureBox_novaV.Image = Properties.Resources.arquivo;
					CBListCategoria();
					RefrashGrid();
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		private void pictureBox_delCategoria_Click(object sender, EventArgs e)
		{
			if (comboBox_categoria.SelectedIndex == 0)
			{
				return;
			}

			if (MessageBox.Show("Deseja excluir essa categoria.", validadeCategorias[comboBox_categoria.SelectedIndex].NOME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				validadeCategorias.RemoveAt(comboBox_categoria.SelectedIndex);

				for (int i = 0; i < validadeProdutos.Count; i++)
				{
					if (validadeProdutos[i].CATEGORIA == comboBox_categoria.SelectedIndex)
					{
						validadeProdutos[i].CATEGORIA = 0;
					}
				}

				CBListCategoria();
				RefrashGrid();
			}
		}

		private async void pictureBox_salvar_Click(object sender, EventArgs e)
		{
			try
			{
				using (SaveFileDialog op = new SaveFileDialog())
				{
					await Task.Run(() => RWXML.SerializeToXmlFile(validade, false));
					GetFilesXML();

					op.FileName = $"{validade.NOME} ({validade.DATA.ToString("MMMM")}-{validade.DATA.Year}).xlsx";
					op.Filter = "Excel Files|*.xlsx";
					op.Title = "Save an Excel File";

					if (op.ShowDialog() == DialogResult.OK)
					{
						await Task.Run(() => RWXLSX.SalveValidade(validade, op.FileName));
					}
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private void pictureBox_busca_Click(object sender, EventArgs e)
		{
			pictureBox_busca.Focus();
			if (xmlFinalizadas != null)
			{
				dataGridView_validadeFile.Rows.Clear();
				for (int i = 0; i < xmlFinalizadas.Count; i++)
				{
					if (xmlFinalizadas[i].ToUpper().EndsWith("XML"))
					{
						Validade tempV = RWXML.DeserializePessoaFromXml(xmlFinalizadas[i]);
						if (tempV.DATA.Month == dateTimePicker_dataBusca.Value.Month && tempV.DATA.Year == dateTimePicker_dataBusca.Value.Year)
						{
							dataGridView_validadeFile.Rows.Add(new string[]
							{
							i.ToString(),tempV.DATA.ToString("ddMMyyHmmss"),tempV.NOME,tempV.DATA.ToString()
							});
						}


					}

				}
			}
			if (xmlTemps != null)
			{
				dataGridView_validadeFileTemp.Rows.Clear();
				for (int i = 0; i < xmlTemps.Count; i++)
				{
					if (xmlTemps[i].ToUpper().EndsWith("XML"))
					{
						Validade tempV = RWXML.DeserializePessoaFromXml(xmlTemps[i]);
						if (tempV.DATA.Month == dateTimePicker_dataBusca.Value.Month && tempV.DATA.Year == dateTimePicker_dataBusca.Value.Year)
						{
							dataGridView_validadeFileTemp.Rows.Add(new string[]
							{
							i.ToString(),tempV.DATA.ToString("ddMMyyHmmss"),tempV.NOME,tempV.DATA.ToString()
							});
						}


					}

				}
			}

		}
	}
}
