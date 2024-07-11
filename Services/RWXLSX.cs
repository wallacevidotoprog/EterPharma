using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.ExtendedProperties;
using EterPharma.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EterPharma.Services
{
	public static class RWXLSX
	{
		public static List<Produtos> ReadAllProdutos(string filename, ProgressBar progressBar)
		{
			List<Produtos> list = null;
			try
			{
				progressBar.Invoke(new Action(() => progressBar.Style = ProgressBarStyle.Marquee));
				using (var workbook = new XLWorkbook(filename))
				{
					var worksheet = workbook.Worksheet(1);
					int rowCount = worksheet.LastRowUsed().RowNumber();
					progressBar.Invoke(new Action(() => progressBar.Style = ProgressBarStyle.Continuous));
					progressBar.Invoke(new Action(() => progressBar.Maximum = rowCount));
					list = new List<Produtos>();
					var xr = worksheet.Cells();
					for (int r = 1; r < rowCount; r++)
					{
						list.Add(new Produtos
						{
							EAN = worksheet.Cell(r + 1, 1).GetValue<string>(),
							COD_PRODUTO = worksheet.Cell(r + 1, 2).GetValue<string>(),
							DESCRICAO_PRODUTO = worksheet.Cell(r + 1, 3).GetValue<string>(),
							STATUS = worksheet.Cell(r + 1, 4).GetValue<string>().ToUpper() == "ATIVO" ? true : false,
							LABORATORIO = worksheet.Cell(r + 1, 5).GetValue<string>(),
							GRUPO = worksheet.Cell(r + 1, 6).GetValue<string>(),

						});
						progressBar.Invoke(new Action(() => progressBar.Increment(1)));
					}
				}

			}
			catch (Exception ex)
			{

				MessageBox.Show($"Erro ao ler XLSX\n{ex.Message}", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				progressBar.Invoke(new Action(() => progressBar.Value = 0));
				progressBar.Invoke(new Action(() => progressBar.Style = ProgressBarStyle.Continuous));
			}

			return list;

		}

		public static void SalveValidade(Validade validade, string salveFile, bool inCategory = false)
		{
			try
			{
				using (XLWorkbook workbook = new XLWorkbook())
				{


					var worksheet = workbook.Worksheets.Add(validade.DADOS.ID);
					worksheet.Cell("A1").Value = "CÓDIGO";
					worksheet.Cell("B1").Value = "DESCRIÇÃO DO PRODUTO";
					worksheet.Cell("C1").Value = "QUANTIDADE";
					worksheet.Cell("D1").Value = "VALIDADE";

					IXLRange title = worksheet.Range($"A1:D1");
					title.Style.Font.SetBold().Font.FontSize = 16;
					title.Style.Fill.SetBackgroundColor(XLColor.FromArgb(189, 189, 183));
					int line = 2;
					if (inCategory)
					{
						for (int i = 0; i < validade.CATEGORIA.Count; i++)
						{
							List<ValidadeProdutos> tp = validade.PRODUTOS.Where(x => x.CATEGORIA == validade.CATEGORIA[i].ID).ToList();

							if (tp.Count > 0)
							{
								worksheet.Cell($"A{line}").Value = validade.CATEGORIA[i].NOME;
								IXLRange range = worksheet.Range($"A{line}:D{line}");
								range.Merge().Style.Font.SetBold().Font.FontSize = 16;
								range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
								line++;
								for (int x = 0; x < tp.Count; x++)
								{
									worksheet.Cell($"A{line}").Value = tp[x].COD_PRODUTO;
									worksheet.Cell($"B{line}").Value = tp[x].DESCRICAO_PRODUTO;
									worksheet.Cell($"C{line}").Value = tp[x].QTD;
									worksheet.Cell($"D{line}").Value = tp[x].DATA.ToShortDateString();
									line++;
								}
							}
						}
					}
					else
					{
						for (int i = 0; i < validade.PRODUTOS.Count; i++)
						{
							worksheet.Cell($"A{line}").Value = validade.PRODUTOS[i].COD_PRODUTO;
							worksheet.Cell($"B{line}").Value = validade.PRODUTOS[i].DESCRICAO_PRODUTO;
							worksheet.Cell($"C{line}").Value = validade.PRODUTOS[i].QTD;
							worksheet.Cell($"D{line}").Value = validade.PRODUTOS[i].DATA.ToShortDateString();
							line++;
						}
					}
					line--;
					worksheet.Range($"A1:D{line}").Style.Border.TopBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.RightBorder = XLBorderStyleValues.Thin;

					worksheet.Columns().AdjustToContents();
					workbook.SaveAs(salveFile);
					MessageBox.Show("Planilha criada com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public static void SalveValidade(List<ValidadeProdutos> validade, string salveFile)
		{
			try
			{
				using (XLWorkbook workbook = new XLWorkbook())
				{


					var worksheet = workbook.Worksheets.Add(DateTime.Now.ToString("dd-MM-yyyy"));
					worksheet.Cell("A1").Value = "CÓDIGO";
					worksheet.Cell("B1").Value = "DESCRIÇÃO DO PRODUTO";
					worksheet.Cell("C1").Value = "QUANTIDADE";
					worksheet.Cell("D1").Value = "VALIDADE";

					IXLRange title = worksheet.Range($"A1:D1");
					title.Style.Font.SetBold().Font.FontSize = 16;
					title.Style.Fill.SetBackgroundColor(XLColor.FromArgb(189, 189, 183));
					int line = 2;

					for (int x = 0; x < validade.Count; x++)
					{
						worksheet.Cell($"A{line}").Value = validade[x].COD_PRODUTO;
						worksheet.Cell($"B{line}").Value = validade[x].DESCRICAO_PRODUTO;
						worksheet.Cell($"C{line}").Value = validade[x].QTD;
						worksheet.Cell($"D{line}").Value = validade[x].DATA.ToShortDateString();
						line++;
					}

					line--;
					worksheet.Range($"A1:D{line}").Style.Border.TopBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
					worksheet.Range($"A1:D{line}").Style.Border.RightBorder = XLBorderStyleValues.Thin;

					worksheet.Columns().AdjustToContents();
					workbook.SaveAs(salveFile);
					MessageBox.Show("Planilha criada com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		#region TEMP

		
		public static List<EnderecoSJRPDb> ReadStreet(string filename)
		{
			List<EnderecoSJRPDb> list = null;
			try
			{
				using (var workbook = new XLWorkbook(filename))
				{
					var worksheet = workbook.Worksheet(1);
					int rowCount = worksheet.LastRowUsed().RowNumber();
					list = new List<EnderecoSJRPDb>();

					string _bairro = string.Empty;
					int _indexBairro = -1;

					for (int r = 1; r < rowCount; r++)
					{
						string sTemp = worksheet.Cell(r, 1).GetValue<string>();

						if (sTemp != "" && sTemp != null)
						{

							if (IsAnyCharacterBold(worksheet.Cell(r, 1)))
							{

								_bairro = RemoveCharI(sTemp);
								list.Add(new EnderecoSJRPDb { BAIRRO = _bairro, LOGADOURO = new List<string>() });
								_indexBairro++;
							}
							else
							{

								if (sTemp.Contains("\n"))
								{
									string[] aSTEmp = sTemp.Split('\n').Where(x=> x!= "").ToArray();

									for (int i = 0; i < aSTEmp.Length; i++)
									{
										list[_indexBairro].LOGADOURO.Add(RemoveCharIeB(aSTEmp[i],$"line:{r}|Contains[] index:{i}"));
									}
								}
								else
								{
									list[_indexBairro].LOGADOURO.Add(RemoveCharIeB(sTemp, $"line:{r}|x"));
								}

							}
						}
					}
				}

				WriteDb.WriteEndereco(list);
			}
			catch (Exception ex)
			{
				throw;
				MessageBox.Show(ex + "");
				MessageBox.Show($"Erro ao ler XLSX\n{ex.Message}", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return list;

		}

		private static string RemoveCharIeB(string aSTEmp,string line ="null")
		{
			string atst = line;
			if (aSTEmp == "")
			{
				return "NULL";
			}

			if (aSTEmp.Contains("|"))
			{
				aSTEmp = aSTEmp.Split('|')[0];
			}
			try
			{
				var tt = aSTEmp?.Substring(0, aSTEmp.IndexOf(' '));
			}
			catch (Exception ex)
			{

				throw;
			}
			
			return aSTEmp?.Replace(aSTEmp?.Substring(0, aSTEmp.IndexOf(' ')), null).TrimStart();
		}

		private static string RemoveCharI(string sTemp)
		{
			if (sTemp == "")
			{
				return "NULL";
			}
			return sTemp?.Replace(sTemp.Substring(0, sTemp.IndexOf(' ')), null).Replace("|", null).TrimStart();

		}
		static bool IsAnyCharacterBold(IXLCell cell)
		{
			var richText = cell.GetRichText();

			foreach (var rt in richText)
			{
				if (rt.Bold)
				{
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}
