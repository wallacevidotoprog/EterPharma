using DocumentFormat.OpenXml.ExtendedProperties;
using EterPharma.Ex;
using EterPharma.Models;
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

		private void ClenAll()
		{
			dateTimePicker_data.Value = DateTime.Now;
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
			ClenAll();
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

			manipulados = new ManipulacaoModel
			{
				ID = Guid.NewGuid(),
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
			};

			using (Aes aes = Aes.Create())
			{
				byte[] chave = aes.Key;
				byte[] iv = aes.IV;

			


				// Serializar e criptografar
				SerializarPessoa(manipulados, Directory.GetCurrentDirectory() + @"\DADOS\man.eter", chave, iv);

				// Desserializar e descriptografar
				//ManipulacaoModel pessoaDesserializada = DesserializarPessoa(Directory.GetCurrentDirectory() + @"\DADOS\man.eter", chave, iv);
				
			}

			return;
			using (var stream = File.Open(Directory.GetCurrentDirectory() + @"\DADOS\man.eter", FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false))
				{
					writer.Write(manipulados.ID.ToByteArray());

					writer.Write(manipulados.DADOSATENDIMENTO.ATEN_LOJA);
					writer.Write(manipulados.DADOSATENDIMENTO.DATA.ToBinary());
					writer.Write(manipulados.DADOSATENDIMENTO.ATEN_MANI);

					writer.Write(manipulados.DADOSCLIENTE.CPF);
					writer.Write(manipulados.DADOSCLIENTE.RG);
					writer.Write(manipulados.DADOSCLIENTE.NOME);
					writer.Write(manipulados.DADOSCLIENTE.TELEFONE);
					writer.Write(manipulados.DADOSCLIENTE.ENDERECO.LOGRADOURO);
					writer.Write(manipulados.DADOSCLIENTE.ENDERECO.NUMERO);
					writer.Write(manipulados.DADOSCLIENTE.ENDERECO.BAIRRO);
					writer.Write(manipulados.DADOSCLIENTE.ENDERECO.OBS);

					writer.Write(manipulados.MEDICAMENTO.Count);

					for (int i = 0; i < manipulados.MEDICAMENTO.Count; i++)
					{
						writer.Write(manipulados.MEDICAMENTO[i]);
					}

					writer.Write(manipulados.OBSGERAL);
					writer.Write(manipulados.SITUCAO);
					writer.Write(manipulados.FORMAPAGAMENTO);
					writer.Write(manipulados.MODOENTREGA);

					writer.Close();

				}
				stream.Close();
			}
		}

		public void SerializarPessoa(ManipulacaoModel manip, string caminhoArquivo, byte[] chave, byte[] iv)
		{
			using (MemoryStream ms = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(ms))
			{
				writer.Write(manipulados.ID.ToByteArray());

				writer.Write(manipulados.DADOSATENDIMENTO.ATEN_LOJA);
				writer.Write(manipulados.DADOSATENDIMENTO.DATA.ToBinary());
				writer.Write(manipulados.DADOSATENDIMENTO.ATEN_MANI);

				writer.Write(manipulados.DADOSCLIENTE.CPF);
				writer.Write(manipulados.DADOSCLIENTE.RG);
				writer.Write(manipulados.DADOSCLIENTE.NOME);
				writer.Write(manipulados.DADOSCLIENTE.TELEFONE);
				writer.Write(manipulados.DADOSCLIENTE.ENDERECO.LOGRADOURO);
				writer.Write(manipulados.DADOSCLIENTE.ENDERECO.NUMERO);
				writer.Write(manipulados.DADOSCLIENTE.ENDERECO.BAIRRO);
				writer.Write(manipulados.DADOSCLIENTE.ENDERECO.OBS);

				writer.Write(manipulados.MEDICAMENTO.Count);

				for (int i = 0; i < manipulados.MEDICAMENTO.Count; i++)
				{
					writer.Write(manipulados.MEDICAMENTO[i]);
				}

				writer.Write(manipulados.OBSGERAL);
				writer.Write(manipulados.SITUCAO);
				writer.Write(manipulados.FORMAPAGAMENTO);
				writer.Write(manipulados.MODOENTREGA);

				writer.Close();

				byte[] dados = ms.ToArray();
				byte[] dadosCriptografados =  CriptoBinary.Criptografar(dados);

				File.WriteAllBytes(caminhoArquivo, dadosCriptografados);
			}
		}
	}
}
