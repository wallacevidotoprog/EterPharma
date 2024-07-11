using DocumentFormat.OpenXml.ExtendedProperties;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ClosedXML.Excel.XLPredefinedFormat;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EterPharma.VIEWS
{
	public partial class Manipulados : Form
	{
		private ManipulacaoModel manipulados;
		public Manipulados()
		{
			InitializeComponent();
		}

		private void CleanAll(object sender, EventArgs e)
		{
			dateTimePicker_data.Value = System.DateTime.Now;
			textBox_atn.Clear();
			textBox_cpf.Clear();
			textBox_rg.Clear();
			textBox_nomeC.Clear();
			textBox5_tel.Clear();
			textBox_log.Clear();
			textBox_num.Clear();
			textBox_bairro.Clear();
			textBox_obsEnd.Clear();
			dataGridView_medicamentos.Rows.Clear();
			textBox_obsGeral.Clear();
			comboBox_situacao.SelectedIndex = -1;
			comboBox_pag.SelectedIndex = -1;
			comboBox_modo.SelectedIndex = -1;
			textBox_valorT.Text = "0,00";
		}

		private void Manipulados_Load(object sender, EventArgs e)
		{
			CleanAll(null, null);
			comboBox_user.Invoke(new Action(() => comboBox_user.CBListUser()));






		}

		private void pictureBox3_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void pictureBox_imprimir_Click(object sender, EventArgs e)
		{
			List<string> list = new List<string>();

			for (int i = 0; i < dataGridView_medicamentos.Rows.Count; i++)
			{
				if (dataGridView_medicamentos.Rows[i].Cells[0].Value != null)
				{

					list.Add(dataGridView_medicamentos.Rows[i].Cells[0].Value.ToString());
				}
			}

			
			MainWindow.database.RegisterManipulacao(new ManipulacaoModel
			{
				ID = Guid.NewGuid().ToString().Replace("-", null).ToUpper(),
				DADOSATENDIMENTO = new DadosAtemdimento
				{
					ATEN_LOJA = MainWindow.database.Users[Extensions.ReturnIndexUser(comboBox_user.SelectedValue.ToString())].ID,
					DATA = dateTimePicker_data.Value,
					ATEN_MANI = textBox_atn.Text
				},
				DADOSCLIENTE = new DadosCliente
				{
					CPF = textBox_cpf.Text,
					RG = textBox_rg.Text,
					NOME = textBox_nomeC.Text,
					TELEFONE = textBox5_tel.Text,
					ENDERECO = new Endereco
					{
						LOGRADOURO = textBox_log.Text,
						NUMERO = textBox_num.Text,
						BAIRRO = textBox_bairro.Text,
						OBS = textBox_obsEnd.Text
					}
				},

				MEDICAMENTO = list,
				OBSGERAL = textBox_obsGeral.Text,
				SITUCAO = comboBox_situacao.SelectedIndex,
				FORMAPAGAMENTO = comboBox_pag.SelectedIndex,
				MODOENTREGA = comboBox_modo.SelectedIndex
			});

			//eList<ManipulacaoModel> t1 = new eList<ManipulacaoModel>(); t1.Add(manipulados);
			//eList<DadosCliente> t2 = new eList<DadosCliente>(); t2.Add((DadosCliente)manipulados.DADOSCLIENTE);

			//WriteDb.WriteManipulado(t1, null);
			//WriteDb.WriteCliente(t2, null);

		}

		private void button2_Click(object sender, EventArgs e)
		{
			textBox_atn.Text = "JANAIRA";
			textBox_cpf.Text = "857483948537";
			textBox_rg.Text = "466294852";
			textBox_nomeC.Text = "WALLACE VIDOTO DE MIRANDA";
			textBox5_tel.Text = "17991983774";
			textBox_log.Text = "ARMANDO RODRIGUES";
			textBox_num.Text = "93";
			textBox_bairro.Text = "ESTANCIA BELA VISTA";
			textBox_obsEnd.Text = "CHACARA";
			dataGridView_medicamentos.Rows.Clear();
			for (int i = 0; i < 5; i++)
			{
				dataGridView_medicamentos.Rows.Add($"REMEDIO 0{i}");
			}
			textBox_obsGeral.Text = "o cliente é louco";
			comboBox_situacao.SelectedIndex = 0;
			comboBox_pag.SelectedIndex = 0;
			comboBox_modo.SelectedIndex = 0;
			textBox_valorT.Text = "90,00";
		}
	}
}
