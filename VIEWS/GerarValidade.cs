using DocumentFormat.OpenXml.Spreadsheet;
using EterPharma.Ex;
using EterPharma.Models;
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
	public partial class GerarValidade : Form
	{
		bool edit;

		List<ValidadeFiles> validadeFiles;
		List<ValidadeProdutos> validadeProdutos;
		List<ValidadeCategoria> validadeCategorias;


		public GerarValidade()
		{
			InitializeComponent();
		}
		private async void GerarValidade_Load(object sender, EventArgs e)
		{
			comboBox_user.Invoke(new Action(() => CBListUser()));
			groupBox_ne.Size = new System.Drawing.Size(566, 88);
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
				comboBox_user.SelectedIndex=0;
			}
		}

		private void pictureBox_novaV_Click(object sender, EventArgs e)
		{
			groupBox_ne.Size = new System.Drawing.Size(566, 315);
			groupBox_insert.Visible = true;
		}

		private void pictureBox_addCategoria_Click(object sender, EventArgs e)
		{
			string result = InputBox.Show("Por favor, insira a categoria:", "Categoria");

			if (result != string.Empty)
			{
				if (validadeCategorias == null)
				{
					validadeCategorias = new List<ValidadeCategoria>();
				}

				validadeCategorias.Add(new ValidadeCategoria { ID=validadeCategorias.Count+1 , NOME= result });
				CBListCategoria();
			}
		}

		private void pictureBox_addItem_Click(object sender, EventArgs e)
		{

		}
	}
}
