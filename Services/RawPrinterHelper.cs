
using System;
using System.Drawing.Printing;
using System.Drawing;
using System.IO.Ports;
using System.Management;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


namespace EterPharma.Services
{
	public static class RawPrinterHelper
	{
		static SerialPort serialPort;

		public static void PrinterHelper(string text, string impressora)
		{
			serialPort = new SerialPort($"COM{PortFromName(impressora)}", 9600);
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
		static void CutPaper()
		{
			byte[] cutPaperCommand = new byte[] { 0x1B, 0x64, 0x02 };
			serialPort.Write(cutPaperCommand, 0, cutPaperCommand.Length);
			string cutPaperCommand1 = "\x1B" + "m";

			serialPort.Write(cutPaperCommand1);
		}
		static string PortFromName(string deviceDescription)
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
				// Query to find all printers
				string query = "SELECT * FROM Win32_Printer";

				using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
				using (ManagementObjectCollection printers = searcher.Get())
				{
					foreach (ManagementObject printer in printers)
					{
						string name = printer["Name"]?.ToString();
						string portName = printer["PortName"]?.ToString();

						if (!string.IsNullOrEmpty(portName) && portName.StartsWith("COM"))
						{
							dic.Add(name, portName);
							// Console.WriteLine($"Printer: {name}, Port: {portName}");
						}
					}
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(($"An error occurred: {ex.Message}"));
			}

			return dic;
		}

		public static void PrinterHelperIP(string printText, string printerIp)
		{
			int printerPort = 9100; // Porta padrão para impressoras de rede
			printText += "\x1B" + "m";
			try
			{
				// Conectar ao socket da impressora
				using (TcpClient client = new TcpClient(printerIp, printerPort))
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
	}
}
