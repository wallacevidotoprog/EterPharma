
using System;
using System.Drawing.Printing;
using System.Drawing;
using System.IO.Ports;
using System.Management;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using EterPharma.Ex;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace EterPharma.Services
{
	public class RawPrinterHelper
	{
		private SerialPort serialPort;
		private string printerIp;
		private PrintDocument printDocument = null;
		private IniFile ini;
		private List<(FormatText,string)> linesToPrint =new List<(FormatText, string)>();
		public RawPrinterHelper(IniFile _ini)
		{
			ini = _ini;
			printDocument = new PrintDocument();
			printDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", 800, 0);
			printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
			


			if ((ini.Read("IMPRESSORAS", "IS_DYNAMIC") == "True" ? true : false))
			{
				serialPort = new SerialPort($"COM{PortFromName(ini.Read("IMPRESSORAS", "DYNAMIC"))}", 9600);
				printDocument.PrintPage += new PrintPageEventHandler(PrintPage);
			}
			else
			{
				serialPort = new SerialPort(ini.Read("IMPRESSORAS", "PORT_COM"), 9600);
				printDocument.PrintPage += new PrintPageEventHandler(PrintPage);
			}

		}
		public void PrinterHelper(string text)
		{

			if (serialPort == null)
			{
				System.Windows.Forms.MessageBox.Show("Impressora não encotrada.");
				return;
			}

			serialPort.Open();
			serialPort.WriteLine(text);
			CutPaper();
			serialPort.Close();
		}
		private void CutPaper()
		{
			byte[] cutPaperCommand = new byte[] { 0x1B, 0x64, 0x02 };
			serialPort.Write(cutPaperCommand, 0, cutPaperCommand.Length);
			string cutPaperCommand1 = "\x1B" + "m";

			serialPort.Write(cutPaperCommand1);
		}
		private static string PortFromName(string deviceDescription)
		{
			string query = "SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%'";

			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
			{
				foreach (ManagementObject obj in searcher.Get())
				{
					string caption = obj["Caption"] as string;
					if (caption != null && caption.Contains(deviceDescription))
					{
						int startIndex = caption.LastIndexOf("(COM");
						if (startIndex >= 0)
						{
							startIndex += 4;
							int endIndex = caption.IndexOf(")", startIndex);
							if (endIndex >= 0)
							{
								return caption.Substring(startIndex, endIndex - startIndex);
							}
						}
					}
				}
			}

			return null;
		}

		public static Dictionary<string, string> GetDeviceName()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			try
			{
				string query = "SELECT * FROM Win32_Printer";

				using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
				using (ManagementObjectCollection printers = searcher.Get())
				{
					foreach (ManagementObject printer in printers)
					{
						string name = printer["Name"]?.ToString();
						string portName = printer["PortName"]?.ToString();
						dic.Add(name.Replace(":",null), portName.Replace(":", null));
					}
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(($"An error occurred: {ex.Message}"));
			}

			return dic;
		}

		public static void PortCOM_tt()
		{
			List<string[]> stringss = new List<string[]>();
			// Cria uma busca para dispositivos conectados a portas COM
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SerialPort");

			// Executa a busca e obtém os resultados
			foreach (ManagementObject queryObj in searcher.Get())
			{
				stringss.Add(new string[] { queryObj["DeviceID"].ToString(), queryObj["Caption"].ToString(), queryObj["PNPDeviceID"].ToString(), queryObj["Status"].ToString() });
			}
		}

		public void PrinterHelperIP(string printText)
		{
			printText += "\x1B" + "m";
			try
			{
				// Conectar ao socket da impressora
				using (TcpClient client = new TcpClient(ini.Read("IMPRESSORAS", "IP_IMP"), Convert.ToInt32(ini.Read("IMPRESSORAS", "IP_PORT"))))
				using (NetworkStream stream = client.GetStream())
				{
					// Converter o texto em bytes
					byte[] data = Encoding.UTF8.GetBytes(printText);

					// Enviar os dados para a impressora
					stream.Write(data, 0, data.Length);

					Console.WriteLine("Texto enviado para a impressora.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao enviar texto para a impressora: {ex.Message}");
			}
		}



		#region PrintDocument 

		public void AddLine(string text, FormatText formatText = FormatText.Default) => linesToPrint.Add((formatText, text));
		
		private static void PrintPageIP(object sender, PrintPageEventArgs e)
		{
			string printText = "Texto a ser impresso\n\n\n"; // Adicione algumas linhas para garantir o corte
			Font printFont = new Font("Arial", 12); // Defina a fonte e o tamanho

			// Desenhe o texto na página de impressão
			e.Graphics.DrawString(printText, printFont, Brushes.Black, 10, 10);
		}
		private void SendCutPaperCommandIP()
		{
			byte[] cutPaperCommand = new byte[] { 0x1D, 0x56, 0x01 }; // Comando ESC/POS para corte total

			try
			{
				// Conectar ao socket da impressora
				using (TcpClient client = new TcpClient(printerIp, 9100))
				using (NetworkStream stream = client.GetStream())
				{
					// Enviar o comando de corte de papel
					stream.Write(cutPaperCommand, 0, cutPaperCommand.Length);

					Console.WriteLine("Comando de corte de papel enviado para a impressora.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao enviar comando de corte de papel para a impressora: {ex.Message}");
			}
		}
		//physic
		public void PrintDocument()=> Print();
		private void Print()
		{
			printDocument.PrinterSettings.PrinterName = ini.Read("IMPRESSORAS", "DYNAMIC");
			//printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF";
			printDocument.Print();
		}
		
		private void PrintPage(object sender, PrintPageEventArgs e)
		{
			Font fontDefault = new Font("Courier New", 8);
			Font fontBold = new Font("Courier New", 8, FontStyle.Bold);
			Font fontItalic = new Font("Courier New", 8, FontStyle.Italic);
			Font fontTitle = new Font("Courier New", 8, FontStyle.Bold);

			float leftMargin = e.MarginBounds.Left;
			float topMargin = e.MarginBounds.Top;
			float linhaAtual = topMargin;

			string TITLE = "REDE CENTRAL LOJA 15";

			e.Graphics.DrawString(TITLE, fontTitle, Brushes.Black, (e.Graphics.MeasureString(TITLE, fontTitle)).Width / 2, 0);
			linhaAtual += fontTitle.GetHeight();
			e.Graphics.DrawLine(Pens.Black, leftMargin, linhaAtual, leftMargin + 300, linhaAtual);

			for (int i = 0; i < linesToPrint.Count; i++)
			{
				switch (linesToPrint[i].Item1)
				{
					case FormatText.Default:
						e.Graphics.DrawString(linesToPrint[i].Item2, fontDefault, Brushes.Black, leftMargin, linhaAtual);
						linhaAtual += fontDefault.GetHeight();
						break;
					case FormatText.Bolt:
						e.Graphics.DrawString(linesToPrint[i].Item2, fontBold, Brushes.Black, leftMargin, linhaAtual);
						linhaAtual += fontBold.GetHeight();
						break;
					case FormatText.Italic:
						e.Graphics.DrawString(linesToPrint[i].Item2, fontItalic, Brushes.Black, leftMargin, linhaAtual);
						linhaAtual += fontItalic.GetHeight();
						break;
					default:
						break;
				}
			}
			e.Graphics.DrawLine(Pens.Black, leftMargin, linhaAtual, leftMargin + 300, linhaAtual);
			e.Graphics.DrawString(DateTime.Now.ToString("dddd dd/MMMM/yyyy - HH:mm"), fontItalic, Brushes.Black, leftMargin, linhaAtual);
			linhaAtual += fontItalic.GetHeight();


			//e.Graphics.DrawString(DevoImprimirCupomNaoFiscal(), fontDefault, Brushes.Black, leftMargin, topMargin, new StringFormat());

			//VIEWS.ViewPrint viewPrint = new VIEWS.ViewPrint();
			//viewPrint.printDocument = printDocument;
			//viewPrint.ShowDialog();
			return;


			// Imprime o texto centralizado
			//SizeF textSize = e.Graphics.MeasureString(text, font);
			//  e.Graphics.DrawString(text, font, Brushes.Black, ((e.PageBounds.Width - textSize.Width) / 2), 50);
			Graphics g = e.Graphics;
			//Font font = new Font("Courier New", 8);

			//float leftMargin = e.MarginBounds.Left;
			//float topMargin = e.MarginBounds.Top;
			//g.DrawString(DevoImprimirCupomNaoFiscal(), font, Brushes.Black, leftMargin, topMargin, new StringFormat());
			return;
			//Outra Forma
			string textoNegrito = "Texto em negrito";
			string textoItalico = "Texto em itálico";
			string textoTrocaFonte = "Texto com fonte diferente";

			// Configuração da fonte e posicionamento
			Font fonteNormal = new Font("Arial", 12);
			Font fonteNegrito = new Font("Arial", 12, FontStyle.Bold);
			Font fonteItalico = new Font("Arial", 12, FontStyle.Italic);
			Font fonteTroca = new Font("Times New Roman", 14); // Exemplo de troca de fonte

			float x = e.MarginBounds.Left;
			float y = e.MarginBounds.Top;
			//float linhaAtual = y;

			// Desenhar o texto na página
			e.Graphics.DrawString(textoNegrito, fonteNegrito, Brushes.Black, x, linhaAtual);
			linhaAtual += fonteNegrito.GetHeight();

			e.Graphics.DrawString(textoItalico, fonteItalico, Brushes.Black, x, linhaAtual);
			linhaAtual += fonteItalico.GetHeight();

			e.Graphics.DrawString(textoTrocaFonte, fonteTroca, Brushes.Black, x, linhaAtual);
			linhaAtual += fonteTroca.GetHeight();

			// Exemplo de como imprimir uma linha horizontal
			e.Graphics.DrawLine(Pens.Black, x, linhaAtual + 10, x + 300, linhaAtual + 10);
		}
		private void gPrintPage(object sender, PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;
			Font font = new Font("Arial", 12);

			float leftMargin = e.MarginBounds.Left;
			float topMargin = e.MarginBounds.Top;
			string text = "Este é um exemplo de impressão na impressora térmica MP-4200.";
			g.DrawString(text, font, Brushes.Black, leftMargin, topMargin, new StringFormat());

			// Exemplo de impressão de imagem
			Image img = Image.FromFile("caminho_para_imagem.jpg");
			g.DrawImage(img, new PointF(leftMargin, topMargin + 50));

			// Exemplo de impressão de código de barras (usando uma biblioteca de código de barras)
			// BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
			// Image barcodeImage = barcode.Encode(BarcodeLib.TYPE.CODE128, "1234567890");
			// g.DrawImage(barcodeImage, new PointF(leftMargin, topMargin + 150));
		}
		#endregion




		public static string DevoImprimirCupomNaoFiscal()
		{
			var texto = new String(' ', 50);
			texto += "------------------------------------------------";
			texto += "--------------------------------------------------\n";
			texto += "<ce><c>Empresa Teste\n";
			texto += "CNPJ: xxxxxxxxxxxxxx Inscrição Estadual: yyyyyyyyyy\n";
			texto += "Rua: aaaaaaaaaaaa, Número: 999   Bairro: bbbbbbb\n";
			texto += "Cidade: zzzzzzzzzzzz nn\n";
			texto += "--------------------------------------------------\n";
			texto += "DANFE NFC-e - Documento Auxiliar\n";
			texto += "da Nota Fiscal Eletrônica para Consumidor Final\n";
			texto += "Não permite aproveitamento de crédito de ICMS\n";
			texto += "--------------------------------------------------\n";
			texto += "Código Descrição do Item  Vlr.Unit. Qtde Vlr.Total\n";
			texto += "--------------------------------------------------\n";
			texto += "</c>" +
					 "<c>333333 ITEM 01        37,14 001Un     37,14</c>\n";
			texto += "<c>444444 ITEM 02         13,61 001Un    13,61</c>\n";
			texto += "--------------------------------------------------\n";
			texto += "QTD. TOTAL DE ITENS                              2\n";
			texto += "VALOR TOTAL R$                               50,75\n";
			texto += "\n";
			texto += "FORMA DE PAGAMENTO                      Valor Pago\n";
			texto += "</c><c>DINHEIRO                              50,75\n";
			texto += "</c><c>VALOR PAGO R$                         50,75\n";
			texto += "TROCO R$                                      0,00\n";
			texto += "</c><c>---------------------------------------</c>\n";
			texto += "Val Aprox Tributos R$ 16.29 (32.10%) Fonte: IBPT  \n";
			texto += "<c>-------------------------------------------</c>\n";
			texto += "<c><ce><ce>NFC-e nº 000001 Série 001\n";
			texto += "Emissão 03/12/2013 15:50:16</c></ce>\n";
			texto += "<ce><b></c><c><b>Via Consumidor</c></b>\n";
			texto += "</b></ce><c><Consulte pela Chave de Acesso em</c>\n";
			texto += "<c>https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx\n";
			texto += "\n";
			texto += "<c><b>CHAVE DE ACESSO</b></ce></c>\n";
			texto += "<c><ce>8877 2222 4444 1101 7777 6666</ce></c>\n";
			texto += "<c><ce>0000 8888 3333 6666 7788</ce></c>\n";
			texto += "\n\n";

			return texto;
		}
	}
}

