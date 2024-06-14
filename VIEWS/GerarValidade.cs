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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EterPharma.VIEWS
{
	public partial class GerarValidade : Form
	{
		bool edit;
		int editP;
		bool news;
		string editF;

		string tempXML;

		Validade validade;
		List<ValidadeFiles> validadeFiles;
		List<ValidadeProdutos> validadeProdutos;
		List<ValidadeCategoria> validadeCategorias;

		Dictionary<int, string> xmlTemps;
		Dictionary<int, string> xmlFinalizadas;



		Produtos produtosInput;

		public GerarValidade()
		{
			InitializeComponent();
		}
		private async void GerarValidade_Load(object sender, EventArgs e)
		{
			Task.Run(new Action(() => GetFilesXML()));
			comboBox_user.Invoke(new Action(() => CBListUser()));
			groupBox_ne.Size = new System.Drawing.Size(566, 88);
			pictureBox_novaV.Image = Properties.Resources.novo_arquivo;
		}
		private void GetFilesXML()
		{

			dataGridView_validadeFileTemp.Invoke(new Action(() =>
			{
				dataGridView_validadeFileTemp.Rows.Clear();
			}));
			xmlTemps = new Dictionary<int, string>();
			string[] fileEntriesTemps = Directory.GetFiles(Directory.GetCurrentDirectory() + $@"\DADOS\VALIDADE\TEMPS");
			for (int i = 0; i < fileEntriesTemps.Length; i++)
			{
				if (fileEntriesTemps[i].ToUpper().EndsWith("XML"))
				{
					xmlTemps.Add(i, fileEntriesTemps[i]);
					Validade tempV = RWXML.DeserializePessoaFromXml(fileEntriesTemps[i]);
					dataGridView_validadeFileTemp.Invoke(new Action(() =>
					{
						dataGridView_validadeFileTemp.Rows.Add(new string[]
					{
						i.ToString(),tempV.NOME,tempV.DATA.ToString()
					});
					}));
					



				}
			}
			dataGridView_validadeFile.Invoke(new Action(() =>
			{
				dataGridView_validadeFile.Rows.Clear();
			}));

			string[] fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory() + $@"\DADOS\VALIDADE\FINALIZADA");
			xmlFinalizadas = new Dictionary<int, string>();
			for (int i = 0; i < fileEntries.Length; i++)
			{
				if (fileEntries[i].ToUpper().EndsWith("XML"))
				{
					xmlFinalizadas.Add(i, fileEntries[i]);
					Validade tempV = RWXML.DeserializePessoaFromXml(fileEntries[i]);
					dataGridView_validadeFile.Invoke(new Action(() =>
					{
						dataGridView_validadeFile.Rows.Add(new string[]
					{
						i.ToString(),tempV.NOME,tempV.DATA.ToString()
					});
					}));

				}

			}
		}
		private void pictureBox3_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void pictureBox_novo_Click(object sender, EventArgs e)
		{
			groupBox_ne.Visible = true;
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
				if (!news && !edit)
				{

					validade = new Validade
					{
						ID = MainWindow.database.Users[comboBox_user.SelectedIndex].ID,
						NOME = MainWindow.database.Users[comboBox_user.SelectedIndex].Nome,
						DATA = dateTimePicker_data.Value
					};
					news = true;
					pictureBox_novaV.Image = Properties.Resources.arquivo;
					if (validadeCategorias == null)
					{
						validadeCategorias = new List<ValidadeCategoria>();
						validadeCategorias.Add(new ValidadeCategoria { ID = 0, NOME = "S/ CATEGORIA" });
						validade.CATEGORIA = validadeCategorias;
						CBListCategoria();
					}
					groupBox_ne.Size = new System.Drawing.Size(566, 315);
					groupBox_insert.Visible = true;

				}
				else
				{
					if (MessageBox.Show($"Deseja Cancelar esse documento ?", "Cancelar Documento", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
					{
						news = edit = false;
						validade = null;
						validadeFiles = null;
						validadeProdutos = null;
						validadeCategorias = null;
						produtosInput = null;
						groupBox_insert.Visible = false;
						GerarValidade_Load(null, null);
					}
				}
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
			if (validadeProdutos == null)
			{
				validadeProdutos = new List<ValidadeProdutos>();
				validade.PRODUTOS = validadeProdutos;
			}

			if (produtosInput != null && !edit)
			{
				validadeProdutos.Add(new ValidadeProdutos
				{
					ID = validadeProdutos.Count(),
					EAN = produtosInput.EAN,
					COD_PRODUTO = produtosInput.COD_PRODUTO,
					DESCRICAO_PRODUTO = produtosInput.DESCRICAO_PRODUTO,
					CATEGORIA = comboBox_categoria.SelectedIndex,
					DATA = dateTimePicker_data.Value,
					QTD = (int)numericUpDown_qtd.Value

				}); ;
				textBox_codigo.Clear();
				textBox_nproduto.Clear();
				numericUpDown_qtd.Value = 1;
			}
			else if (textBox_nproduto.ReadOnly == false)
			{
				validadeProdutos.Add(new ValidadeProdutos
				{
					ID = validadeProdutos.Count(),
					EAN = "NAN",
					COD_PRODUTO = textBox_codigo.Text,
					DESCRICAO_PRODUTO = textBox_nproduto.Text,
					CATEGORIA = comboBox_categoria.SelectedIndex,
					DATA = dateTimePicker_data.Value,
					QTD = (int)numericUpDown_qtd.Value

				});
				produtosInput = null;
				textBox_codigo.Clear();
				textBox_nproduto.Clear();
				textBox_nproduto.ReadOnly = true;
				numericUpDown_qtd.Value = 1;
			}
			else if (produtosInput != null && edit)
			{
				validadeProdutos[editP].EAN = textBox_nproduto.ReadOnly ? produtosInput.EAN : "NAN";
				validadeProdutos[editP].COD_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.COD_PRODUTO : textBox_codigo.Text;
				validadeProdutos[editP].DESCRICAO_PRODUTO = textBox_nproduto.ReadOnly ? produtosInput.DESCRICAO_PRODUTO : textBox_nproduto.Text;
				validadeProdutos[editP].QTD = (int)numericUpDown_qtd.Value;
				validadeProdutos[editP].DATA = dateTimePicker_data.Value;
				validadeProdutos[editP].CATEGORIA = comboBox_categoria.SelectedIndex;

				produtosInput = null;
				textBox_codigo.Clear();
				textBox_nproduto.Clear();
				textBox_nproduto.ReadOnly = true;
				numericUpDown_qtd.Value = 1;
				pictureBox_addItem.Image = Properties.Resources.adicionar_ficheiro;

			}

			RWXML.SerializeToXmlFile(validade);
			RefrashGrid();
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

		private void textBox_codigo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FProduto();
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

		private void textBox_codigo_Validated(object sender, EventArgs e)
		{
			FProduto();
		}

		private void eDITARToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (listView1.SelectedItems.Count > 0)
				{
					int selectedItem = editP = int.Parse(listView1.SelectedItems[0].SubItems[0].Text);
					edit = true;
					produtosInput = null;
					pictureBox_addItem.Image = Properties.Resources.atualizar_ficheiro;

					textBox_codigo.Text = validadeProdutos[selectedItem].EAN == "" ? validadeProdutos[selectedItem].COD_PRODUTO : validadeProdutos[selectedItem].EAN;
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

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}
	}
}
