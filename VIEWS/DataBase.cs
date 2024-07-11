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
		public ProgressBar progressBar_status { get; set; }
		List<Produtos> tempProdutos;
		bool edit = false;
		int editIDINDEX = -1;
		public DataBase()
		{
			InitializeComponent();
			if (InputBox.Show("Qual a senha:", "SENHA =D", true) != "32195018")
			{
				this.Close();
			}
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
	}
}
