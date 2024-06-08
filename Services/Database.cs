using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using EterPharma.Models;
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
		private System.Windows.Forms.ProgressBar _progressBar;
		public List<Produtos> Produtos;
		public List<User> Users;

		public  Database(System.Windows.Forms.ProgressBar progressBar, System.Windows.Forms.ToolStrip toolStrip)
		{
			_progressBar = progressBar;
			Init(toolStrip);
			
		}

		private async void Init(System.Windows.Forms.ToolStrip toolStrip)
		{
			await Task.Run(() => Produtos = ReadDb.ReadProdutos(_progressBar));
			await Task.Run(() => Users = ReadDb.ReadUsers(_progressBar));
			toolStrip.Invoke(new Action(() => { toolStrip.Enabled = true; }));
		}

		public bool WriteProdutos()
		{
			bool stats = false;
			stats = WriteDb.WriteProdutos(Produtos,_progressBar);
			_progressBar.Invoke(new Action(() => _progressBar.Value = 0));
			return stats;
		}
		public bool WriteUser()
		{
			bool stats = false;
			stats = WriteDb.WriteUser(Users, _progressBar);
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

		public static List<User> ReadUsers(System.Windows.Forms.ProgressBar progressBar)
		{
			List<User> list = new List<User>();
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

								});
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
	}
	public static class WriteDb
	{
		public static bool WriteProdutos(List<Produtos> produtos, System.Windows.Forms.ProgressBar progressBar)
		{

			try
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\DADOS\produtos.eter"))
				{
					File.Delete(Directory.GetCurrentDirectory() + @"\DADOS\produtos.eter");
				}
				using (var stream = File.Open(Directory.GetCurrentDirectory() + @"\DADOS\produtos.eter", FileMode.Create))
				{
					using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
					{
						writer.Write((Int32)produtos.Count);
						progressBar.Invoke(new Action(() => progressBar.Maximum = produtos.Count));
						for (int i = 0; i < produtos.Count; i++)
						{
							writer.Write((string)produtos[i].EAN);
							writer.Write((string)produtos[i].COD_PRODUTO);
							writer.Write((string)produtos[i].DESCRICAO_PRODUTO);
							writer.Write((bool)produtos[i].STATUS);
							writer.Write((string)produtos[i].LABORATORIO);
							writer.Write((string)produtos[i].GRUPO);
							progressBar.Invoke(new Action(() => progressBar.Increment(1)));

						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}", "WriteProdutos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				return false;
			}
			return false;

		}
		public static bool WriteUser(List<User> user, System.Windows.Forms.ProgressBar progressBar)
		{

			try
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\DADOS\user.eter"))
				{
					File.Delete(Directory.GetCurrentDirectory() + @"\DADOS\user.eter");
				}

				using (var stream = File.Open(Directory.GetCurrentDirectory() + @"\DADOS\user.eter", FileMode.Create, FileAccess.Write))
				{
					using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
					{
						writer.Write((Int32)user.Count);
						progressBar.Invoke(new Action(() => progressBar.Maximum = user.Count));
						for (int i = 0; i < user.Count; i++)
						{
							writer.Write((string)user[i].ID);
							writer.Write((string)user[i].Nome);
							writer.Write((int)user[i].Funcao);
							writer.Write((bool)user[i].Status);
							progressBar.Invoke(new Action(() => progressBar.Increment(1)));

						}
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show($"ERRO\n{ex.Message}", "WriteUser", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				progressBar.Invoke(new Action(() => progressBar.Value = 0));
				return false;
			}
			return false;

		}
	}
}
