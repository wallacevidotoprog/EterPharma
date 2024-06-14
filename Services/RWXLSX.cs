using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using EterPharma.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EterPharma.Services
{
	public static class RWXLSX
	{
		public static List<Produtos> ReadAllProdutos(string filename, System.Windows.Forms.ProgressBar progressBar)
		{
			List<Produtos> list = null;
			try
			{
				progressBar.Invoke(new Action(() => progressBar.Style = ProgressBarStyle.Marquee ));
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

				MessageBox.Show($"Erro ao ler XLSX\n{ex.Message}","ERRO",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			finally
			{
				progressBar.Invoke(new Action(() => progressBar.Value = 0));
				progressBar.Invoke(new Action(() => progressBar.Style = ProgressBarStyle.Continuous));
			}

			return list;

		}
	}
}
