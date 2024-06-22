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

		List<string> xmlFiles;



		Produtos produtosInput;

		public GerarValidade()
		{
			InitializeComponent();
		}

		#region MyFunc
		private async void GetFilesXML()
		{
			dataGridView_validadeFile.Invoke(new Action(() =>
			{
				dataGridView_validadeFile.Rows.Clear();
			}));
			string[] fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory() + $@"\DADOS\VALIDADE\", "*.xml");
			xmlFiles = new List<string>();
			for (int i = 0; i < fileEntries.Length; i++)
			{
				xmlFiles.Add(fileEntries[i]);
				Validade tempV = await RWXML.DeserializePessoaFromXmlAsync(fileEntries[i]);
				dataGridView_validadeFile.Invoke(new Action(() =>
				{
					dataGridView_validadeFile.Rows.Add(new string[]
				{
						i.ToString(),tempV.DADOS.DATA.ToString("ddMMyyHmmss"),tempV.DADOS.NOME,tempV.DADOS.DATA.ToString()
				});
				}));
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

			for (int i = 0; i < validade.CATEGORIA.Count; i++)
			{
				ListViewGroup group = new ListViewGroup(validade.CATEGORIA[i].NOME, HorizontalAlignment.Left);
				listView1.Groups.Add(group);

				categoria.Add(
						 validade.CATEGORIA[i].ID,
						$"{validade.CATEGORIA[i].ID} - {validade.CATEGORIA[i].NOME}");
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
			for (int i = 0; i < validade.CATEGORIA.Count; i++)
			{
				ListViewGroup group = new ListViewGroup(validade.CATEGORIA[i].NOME, HorizontalAlignment.Left);
				listView1.Groups.Add(group);

				List<ValidadeProdutos> tp = validade.PRODUTOS.Where(x => x.CATEGORIA == validade.CATEGORIA[i].ID).ToList();

				for (int x = 0; x < tp.Count; x++)
				{
					item = new ListViewItem(tp[x].ID.ToString());
					item.SubItems.Add(tp[x].EAN);
					item.SubItems.Add(tp[x].COD_PRODUTO);
					item.SubItems.Add(tp[x].DESCRICAO_PRODUTO);
					item.SubItems.Add(tp[x].QTD.ToString());
					item.SubItems.Add(tp[x].DATA.ToString("dd/MM/yyyy"));
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

					numericUpDown_qtd.Focus();
				}
				else
				{
					MessageBox.Show("Cédigo não encontrado.\nDigite o nome do produto no campo a baixo do código.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					textBox_nproduto.ReadOnly = false;
					textBox_nproduto.Focus();
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

		private async void pictureBox_novo_Click(object sender, EventArgs e)
		{
			if (validade != null)
			{
				if (MessageBox.Show($"Existe um arquivo aberto, deseja fecha-lo ?\n(As alterações serão salvas)", "ALERTA", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
				{
					return;
				}
				//				await Task.Run(() => RWXML.SerializeToXmlFile(validade));
			}
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

		private async void pictureBox_novaV_Click(object sender, EventArgs e)
		{
			try
			{
				pictureBox_novaV.Focus();
				if (!mode_new && !mode_edit)
				{
					mode_new = true;
					validade = new Validade();
					validade.Init
					(MainWindow.database.Users[Extensions.ReturnIndexUser(comboBox_user.SelectedValue.ToString())].ID, MainWindow.database.Users[Extensions.ReturnIndexUser(comboBox_user.SelectedValue.ToString())].Nome, dateTimePicker_dataD.Value
					);
					CBListCategoria();
					docFile = await RWXML.SerializeToXmlFileAsync(validade);
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

			if (result != "")
			{

				validade.CATEGORIA.Add(new ValidadeCategoria { ID = validade.CATEGORIA.Count, NOME = result });
				CBListCategoria();
			}
		}

		private void pictureBox_addItem_Click(object sender, EventArgs e)
		{
			pictureBox_addItem.Focus();

			if (produtosInput == null && textBox_nproduto.ReadOnly == true)
			{
				FProduto();
			}

			if (editProduto)
			{
				validade.PRODUTOS[indexEditProduto].EAN = textBox_nproduto.ReadOnly ? produtosInput.EAN : "NAN";
				validade.PRODUTOS[indexEditProduto].COD_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.COD_PRODUTO : textBox_codigo.Text;
				validade.PRODUTOS[indexEditProduto].DESCRICAO_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.DESCRICAO_PRODUTO : textBox_nproduto.Text;
				validade.PRODUTOS[indexEditProduto].QTD = (int)numericUpDown_qtd.Value;
				validade.PRODUTOS[indexEditProduto].DATA = dateTimePicker_data.Value;
				validade.PRODUTOS[indexEditProduto].CATEGORIA = comboBox_categoria.SelectedIndex;
				editProduto = false;
				validade.WriteFileAsync();
				ResetPropProduto();
			}
			else /*if (mode_new && mode_edit && !editProduto)*/
			{

				validade.PRODUTOS.Add(new ValidadeProdutos
				{
					ID = validade.PRODUTOS.Count(),
					EAN = textBox_nproduto.ReadOnly ? produtosInput.EAN : "NAN",
					COD_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.COD_PRODUTO : textBox_codigo.Text.ToUpper(),
					DESCRICAO_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.DESCRICAO_PRODUTO : textBox_nproduto.Text.ToUpper(),
					CATEGORIA = comboBox_categoria.SelectedIndex,
					DATA = dateTimePicker_data.Value,
					QTD = (int)numericUpDown_qtd.Value

				});

				ResetPropProduto();
			}
			//			Task.Run(() => RWXML.SerializeToXmlFile(validade));
			RefrashGrid();
			textBox_codigo.Focus();
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

					textBox_codigo.Text = validade.PRODUTOS[selectedItem].EAN == "NAN" ? validade.PRODUTOS[selectedItem].COD_PRODUTO : validade.PRODUTOS[selectedItem].EAN;
					textBox_nproduto.Text = validade.PRODUTOS[selectedItem].DESCRICAO_PRODUTO;
					numericUpDown_qtd.Value = validade.PRODUTOS[selectedItem].QTD;
					dateTimePicker_data.Value = validade.PRODUTOS[selectedItem].DATA;
					comboBox_categoria.SelectedIndex = validade.PRODUTOS[selectedItem].CATEGORIA;
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
							validade.PRODUTOS.RemoveAt(temp);

							for (int i = 0; i < validade.PRODUTOS.Count; i++)
							{
								validade.PRODUTOS[i].ID = i;
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
				DialogResult result = MessageBox.Show("Há um documento em aberto, deseja fecha-lo ?\nOs dados seram salvos.", "ALERTA", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				if (result == DialogResult.Cancel)
				{ return; }
				else if (result == DialogResult.OK)
				{
					int indexRow = e.RowIndex;
					await Task.Run(() => validade.WriteFileAsync());					
					((DataGridView)sender).Rows[indexRow].Selected = true;
				}
			}
			try
			{
				validade = null;
				validade = await RWXML.DeserializePessoaFromXmlAsync(xmlFiles[Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value)],true);
				docFile = xmlFiles[Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value)];


				if (validade != null)
				{
					dateTimePicker_dataD.Value = validade.DADOS.DATA;

					comboBox_user.SelectedIndex = Extensions.ReturnIndexUserCB(validade.DADOS.ID, comboBox_user);

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
				GetFilesXML();
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

			if (MessageBox.Show("Deseja excluir essa categoria.", validade.CATEGORIA[comboBox_categoria.SelectedIndex].NOME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				validade.CATEGORIA.RemoveAt(comboBox_categoria.SelectedIndex);

				for (int i = 0; i < validade.PRODUTOS.Count; i++)
				{
					if (validade.PRODUTOS[i].CATEGORIA == comboBox_categoria.SelectedIndex)
					{
						validade.PRODUTOS[i].CATEGORIA = 0;
					}
				}

				CBListCategoria();
				RefrashGrid();
			}
		}

		private async void pictureBox_salvar_Click(object sender, EventArgs e)
		{
			if (validade == null)
			{
				return;
			}
			//			await Task.Run(() => RWXML.SerializeToXmlFile(validade));
			GetFilesXML();
			NewDoc(false);


		}

		private async void pictureBox_busca_Click(object sender, EventArgs e)
		{
			pictureBox_busca.Focus();
			if (xmlFiles != null)
			{
				dataGridView_validadeFile.Rows.Clear();
				for (int i = 0; i < xmlFiles.Count; i++)
				{
					if (xmlFiles[i].ToUpper().EndsWith("XML"))
					{
						Validade tempV = await RWXML.DeserializePessoaFromXmlAsync(xmlFiles[i]);
						if (tempV.DADOS.DATA.Month == dateTimePicker_dataBusca.Value.Month && tempV.DADOS.DATA.Year == dateTimePicker_dataBusca.Value.Year)
						{
							dataGridView_validadeFile.Rows.Add(new string[]
							{
							i.ToString(),tempV.DADOS.DATA.ToString("ddMMyyHmmss"),tempV.DADOS.NOME,tempV.DADOS.DATA.ToString()
							});
						}


					}

				}
			}
		}

		private async void pictureBox_exportExcel_Click(object sender, EventArgs e)
		{
			try
			{
				if (validade == null)
				{
					return;
				}
				using (SaveFileDialog op = new SaveFileDialog())
				{
					op.FileName = $"{validade.DADOS.NOME} ({validade.DADOS.DATA.ToString("MMMM")}-{validade.DADOS.DATA.Year}).xlsx";
					op.Filter = "Excel Files|*.xlsx";
					op.Title = "Save an Excel File";

					if (op.ShowDialog() == DialogResult.OK)
					{
						pictureBox_salvar_Click(null, null);
						//						await Task.Run(() => RWXLSX.SalveValidade(validade, op.FileName));
					}
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}
	}
}
