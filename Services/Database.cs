using DocumentFormat.OpenXml.ExtendedProperties;
using EterPharma.Ex;
using EterPharma.Models;
using EterPharma.VIEWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace EterPharma.Services
{
	public class Database
	{
		public System.Windows.Forms.ProgressBar _progressBar;
		public List<Produtos> Produtos;
		public eList<User> Users;

		public eList<DadosCliente> Clientes;
		public eList<ManipulacaoModel> Manipulados;

		public Database(System.Windows.Forms.ProgressBar progressBar, ToolStrip toolStrip)
		{
			_progressBar = progressBar;
			Init(toolStrip);
		}

		private async void Init(ToolStrip toolStrip)
		{
			await Task.Run(() => Produtos = ReadDb.ReadProdutos(_progressBar));
			await Task.Run(() => Users = ReadDb.ReadUsers(_progressBar));
			Users.ItemEdit += UserEvents;
			toolStrip.Invoke(new Action(() => { toolStrip.Enabled = true; }));
			await Task.Run(() => Clientes = ReadDb.ReadClientes(_progressBar));
			await Task.Run(() => Manipulados = ReadDb.ReadManipulado(_progressBar));

			
		}

		public void UserEvents(object sender, EventArgs e) => WriteUserBinary();

		public bool UserExite(string id)
		{
			for (int i = 0; i < Users.Count; i++)
			{
				if (Users[i].ID == id)
				{
					return true;
				}
			}
			return false;
		}

		public bool WriteProdutosBinary()
		{
			bool stats = false;
			stats = WriteDb.WriteProdutos(Produtos, _progressBar);
			_progressBar.Invoke(new Action(() => _progressBar.Value = 0));
			return stats;
		}

		public bool WriteUserBinary()
		{
			bool stats = false;
			stats = WriteDb.WriteUser(Users, _progressBar);
			_progressBar.Invoke(new Action(() => _progressBar.Value = 0));
			return stats;
		}

		public bool WriteManipuladosBinary()
		{
			bool stats = false;
			stats = WriteDb.WriteManipulado(Manipulados, _progressBar);
			_progressBar.Invoke(new Action(() => _progressBar.Value = 0));
			return stats;
		}
		public bool WriteClientesBinary()
		{
			bool stats = false;
			stats = WriteDb.WriteCliente(Clientes, _progressBar);
			_progressBar.Invoke(new Action(() => _progressBar.Value = 0));
			return stats;
		}


	}
	public static class ReadDb
	{
		public static List<Produtos> ReadProdutos(System.Windows.Forms.ProgressBar progressBar)
		{
			List<Produtos> list = new List<Produtos>();
			try
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\DADOS\produtos.eter"))
				{
					using (var stream = File.Open(Directory.GetCurrentDirectory() + @"\DADOS\produtos.eter", FileMode.Open))
					{
						using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
						{
							int lines = reader.ReadInt32();
							if (progressBar.InvokeRequired)
							{
								progressBar.Invoke(new Action(() => progressBar.Maximum = lines));
							}
							else
							{
								progressBar.Maximum = lines;
							}

							for (int i = 0; i < lines; i++)
							{
								list.Add(new Produtos
								{
									EAN = reader.ReadString(),
									COD_PRODUTO = reader.ReadString(),
									DESCRICAO_PRODUTO = reader.ReadString(),
									STATUS = reader.ReadBoolean(),
									LABORATORIO = reader.ReadString(),
									GRUPO = reader.ReadString()

								});
								if (progressBar.InvokeRequired)
								{
									progressBar.Invoke(new Action(() => progressBar.Increment(1)));
								}
								else
								{
									progressBar.Increment(1);
								}
							}
						}
					}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show($"ERRO\nArquivo não encontrado.", "ReadProdutos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}", "ReadProdutos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
			finally
			{
				progressBar.Invoke(new Action(() => progressBar.Value = 0));
			}

			return list;
		}

		public static eList<User> ReadUsers(System.Windows.Forms.ProgressBar progressBar)
		{
			eList<User> list = new eList<User>();
			try
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\DADOS\user.eter"))
				{
					using (var stream = File.Open(Directory.GetCurrentDirectory() + @"\DADOS\user.eter", FileMode.Open))
					{
						using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
						{
							int lines = reader.ReadInt32();
							progressBar.Invoke(new Action(() => progressBar.Maximum = lines));

							for (int i = 0; i < lines; i++)
							{
								list.Add(new User
								{
									ID = reader.ReadString(),
									Nome = reader.ReadString(),
									Funcao = (Funcao)reader.ReadInt32(),
									Status = reader.ReadBoolean()

								}, false);
								progressBar.Invoke(new Action(() => progressBar.Increment(1)));
							}
						}
					}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show($"ERRO\nArquivo não encontrado.", "ReadUsers", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}", "ReadUsers", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
			finally
			{
				progressBar.Invoke(new Action(() => progressBar.Value = 0));
			}
			return list;
		}

		public static eList<DadosCliente> ReadClientes(System.Windows.Forms.ProgressBar progressBar)
		{
			eList<DadosCliente> list = new eList<DadosCliente>();
			string fileName = (Directory.GetCurrentDirectory() + @"\DADOS\clientes.eter");
			try
			{
				if (File.Exists(fileName))
				{
					using (var stream = File.Open(fileName, FileMode.Open))
					{
						using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
						{
							int lines = reader.ReadInt32();
							progressBar.Invoke(new Action(() => progressBar.Maximum = lines));

							for (int i = 0; i < lines; i++)
							{
								var temp = new DadosCliente
								{
									CPF = reader.ReadString(),
									RG = reader.ReadString(),
									NOME = reader.ReadString(),
									TELEFONE = reader.ReadString(),
									ENDERECO = new List<Endereco>()
								};
								int linesEnd = reader.ReadInt32();
								for (int j = 0; j < linesEnd; j++)
								{
									temp.ENDERECO.Add(new Endereco
									{
										LOGRADOURO = reader.ReadString(),
										NUMERO = reader.ReadString(),
										BAIRRO = reader.ReadString(),
										OBS = reader.ReadString()
									});
								}


								list.Add(temp, false);
								progressBar.Invoke(new Action(() => progressBar.Increment(1)));
							}
						}
					}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show($"ERRO\nArquivo não encontrado.", "ReadClientes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}", "ReadClientes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
			finally
			{
				progressBar.Invoke(new Action(() => progressBar.Value = 0));
			}
			return list;
		}
		public static eList<ManipulacaoModel> ReadManipulado(System.Windows.Forms.ProgressBar progressBar)
		{
			eList<ManipulacaoModel> list = new eList<ManipulacaoModel>();
			string fileName = (Directory.GetCurrentDirectory() + @"\DADOS\manipulados.eter");
			try
			{
				if (File.Exists(fileName))
				{
					using (var stream = File.Open(fileName, FileMode.Open))
					{
						using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
						{
							int lines = reader.ReadInt32();
							progressBar.Invoke(new Action(() => progressBar.Maximum = lines));

							for (int i = 0; i < lines; i++)
							{
								var temp = new ManipulacaoModel
								{
									ID = reader.ReadString(),
									DADOSATENDIMENTO = new DadosAtemdimento
									{
										ATEN_LOJA = reader.ReadString(),
										DATA = new DateTime(reader.ReadInt64()),
										ATEN_MANI = reader.ReadString(),
									},
									DADOSCLIENTE = reader.ReadString(),
									MEDICAMENTO = new List<string>(),
									OBSGERAL = reader.ReadString(),
									SITUCAO = reader.ReadInt32(),
									FORMAPAGAMENTO = reader.ReadInt32(),
									MODOENTREGA = reader.ReadInt32(),
								};
								int linesEnd = reader.ReadInt32();
								for (int j = 0; j < linesEnd; j++)
								{
									temp.MEDICAMENTO.Add(reader.ReadString());
								}


								list.Add(temp, false);
								progressBar.Invoke(new Action(() => progressBar.Increment(1)));
							}
						}
					}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show($"ERRO\nArquivo não encontrado.", "ReadClientes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}", "ReadClientes", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
			finally
			{
				progressBar.Invoke(new Action(() => progressBar.Value = 0));
			}
			return list;
		}
	}
	public static class WriteDb
	{
		private static BACKUP _backup;
		public static bool WriteProdutos(List<Produtos> produtos, System.Windows.Forms.ProgressBar progressBar)
		{
			try
			{
				string fileName = (Directory.GetCurrentDirectory() + @"\DADOS\produtos.eter");

				_backup = new BACKUP(fileName);

				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				using (var stream = File.Open(fileName, FileMode.Create))
				{
					using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
					{
						writer.Write((Int32)produtos.Count);
						progressBar?.Invoke(new Action(() => progressBar.Maximum = produtos.Count));
						for (int i = 0; i < produtos.Count; i++)
						{
							writer.Write((string)produtos[i].EAN);
							writer.Write((string)produtos[i].COD_PRODUTO);
							writer.Write((string)produtos[i].DESCRICAO_PRODUTO);
							writer.Write((bool)produtos[i].STATUS);
							writer.Write((string)produtos[i].LABORATORIO);
							writer.Write((string)produtos[i].GRUPO);
							progressBar?.Invoke(new Action(() => progressBar.Increment(1)));

						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				_backup.RestoreBackup();
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}\nBACKUP Restaurado", "WriteProdutos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				progressBar?.Invoke(new Action(() => progressBar.Value = 0));
				return false;
			}
			return false;

		}

		public static bool WriteUser(eList<User> user, System.Windows.Forms.ProgressBar progressBar)
		{
			try
			{
				string fileName = (Directory.GetCurrentDirectory() + @"\DADOS\user.eter");
				_backup = new BACKUP(fileName);
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}

				using (var stream = File.Open(fileName, FileMode.Create, FileAccess.Write))
				{
					using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
					{
						writer.Write((Int32)user.Count);
						progressBar?.Invoke(new Action(() => progressBar.Maximum = user.Count));
						for (int i = 0; i < user.Count; i++)
						{
							writer.Write((string)user[i].ID);
							writer.Write((string)user[i].Nome);
							writer.Write((int)user[i].Funcao);
							writer.Write((bool)user[i].Status);
							progressBar?.Invoke(new Action(() => progressBar.Increment(1)));

						}
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				_backup.RestoreBackup();
				MessageBox.Show($"ERRO\n{ex.Message}", "WriteUser", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				progressBar?.Invoke(new Action(() => progressBar.Value = 0));
				return false;
			}
			return false;

		}

		public static bool WriteCliente(eList<DadosCliente> clientes, System.Windows.Forms.ProgressBar progressBar)
		{
			try
			{
				string fileName = (Directory.GetCurrentDirectory() + @"\DADOS\clientes.eter");

				_backup = new BACKUP(fileName);

				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				using (var stream = File.Open(fileName, FileMode.Create))
				{
					using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
					{
						writer.Write((Int32)clientes.Count);
						progressBar?.Invoke(new Action(() => progressBar.Maximum = clientes.Count));
						for (int i = 0; i < clientes.Count; i++)
						{
							writer.Write((string)clientes[i].CPF);
							writer.Write((string)clientes[i].RG);
							writer.Write((string)clientes[i].NOME);
							writer.Write((string)clientes[i].TELEFONE);

							writer.Write((Int32)clientes[i].ENDERECO.Count);
							for (int j = 0; j < clientes[i].ENDERECO.Count; j++)
							{
								writer.Write((string)clientes[i].ENDERECO[j].LOGRADOURO);
								writer.Write((string)clientes[i].ENDERECO[j].NUMERO);
								writer.Write((string)clientes[i].ENDERECO[j].BAIRRO);
								writer.Write((string)clientes[i].ENDERECO[j].OBS);

							}
							progressBar?.Invoke(new Action(() => progressBar.Increment(1)));

						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				_backup.RestoreBackup();
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}\nBACKUP Restaurado", "WriteCliente", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				progressBar?.Invoke(new Action(() => progressBar.Value = 0));
				return false;
			}
			return false;

			return false;
		}

		public static bool WriteManipulado(eList<ManipulacaoModel> manipulados, System.Windows.Forms.ProgressBar progressBar)
		{
			try
			{
				string fileName = (Directory.GetCurrentDirectory() + @"\DADOS\manipulados.eter");

				_backup = new BACKUP(fileName);

				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				using (var stream = File.Open(fileName, FileMode.Create))
				{
					using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
					{
						writer.Write((Int32)manipulados.Count);
						progressBar?.Invoke(new Action(() => progressBar.Maximum = manipulados.Count));


						for (int i = 0; i < manipulados.Count; i++)
						{
							writer.Write((string)manipulados[i].ID);

							writer.Write((string)manipulados[i].DADOSATENDIMENTO.ATEN_LOJA);
							writer.Write((long)manipulados[i].DADOSATENDIMENTO.DATA.Ticks);
							writer.Write((string)manipulados[i].DADOSATENDIMENTO.ATEN_MANI);

							writer.Write((string)((DadosCliente)manipulados[i].DADOSCLIENTE).CPF);

							writer.Write((string)manipulados[i].OBSGERAL);
							writer.Write((int)manipulados[i].SITUCAO);
							writer.Write((int)manipulados[i].FORMAPAGAMENTO);
							writer.Write((int)manipulados[i].MODOENTREGA);

							writer.Write((Int32)manipulados[i].MEDICAMENTO.Count);
							for (int j = 0; j < manipulados[i].MEDICAMENTO.Count; j++)
							{
								writer.Write((string)manipulados[i].MEDICAMENTO[j]);
							}

							progressBar?.Invoke(new Action(() => progressBar.Increment(1)));

						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				_backup.RestoreBackup();
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}\nBACKUP Restaurado", "WriteManipulado", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				progressBar?.Invoke(new Action(() => progressBar.Value = 0));
				return false;
			}
			return false;

			return false;
		}



	}

	public class BACKUP
	{
		private string fileName;
		private string data;
		public BACKUP(string _file)
		{
			fileName = _file;
			SetBackup();
		}
		private void SetBackup()
		{
			if (File.Exists(fileName))
			{
				data = File.ReadAllText(fileName);
			}
		}

		public void RestoreBackup()
		{
			if (data != null)
			{
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				File.WriteAllText(fileName, data);
			}

		}
	}
}
