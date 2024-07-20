
using System;
using System.IO.Ports;
using System.Management;


namespace EterPharma.Services
{
	public static class RawPrinterHelper
	{
		static SerialPort serialPort;

		public static void PrinterHelper(string text)
		{
			serialPort = new SerialPort($"COM{PortFromName("MP-4200")}", 9600);
			serialPort.Open();
			serialPort.WriteLine(text);
			CutPaper();
			serialPort.Close();
		}
		public static void testc()
		{
			serialPort = new SerialPort("COM2", 9600);
			serialPort.Open();
			serialPort.WriteLine("APENAS UM TESTE");
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
	}
}
