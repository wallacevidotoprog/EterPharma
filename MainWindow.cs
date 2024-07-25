using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EterPharma.Ex;
using EterPharma.Models;
using EterPharma.Services;
using EterPharma.VIEWS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Data.OleDb;

namespace EterPharma
{
	public partial class MainWindow : Form
	{
		public static Database database;
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenForm(Form form)
		{
			try
			{
				if (this.panel_center.Controls.Count > 0)
					this.panel_center.Controls.RemoveAt(0);

				form.TopLevel = false;
				form.FormBorderStyle = FormBorderStyle.None;
				form.Dock = DockStyle.Fill;

				var progressBarProperty = form.GetType().GetProperty("progressBar_status", BindingFlags.Public | BindingFlags.Instance);

				if (progressBarProperty != null)
				{
					progressBarProperty.SetValue(form, progressBar_status);
				}

				form.FormClosed += new FormClosedEventHandler(this.ChildForm_FormClosed);
				this.panel_center.Controls.Clear();
				this.panel_center.Controls.Add(form);
				form?.Show();
				this.toolStrip1.Visible = false;
			}
			catch (Exception ex)
			{


			}

		}

		private void ChildForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.toolStrip1.Visible = true;
		}

		private async void MainWindow_Load(object sender, EventArgs e)
		{
			database = new Database(progressBar_status, toolStrip1);

			
		}
		private void gERARVALIDADEDOMÊSToolStripMenuItem_Click(object sender, EventArgs e) => OpenForm(new GerarValidade());

		private void toolStripButton_conf_Click(object sender, EventArgs e) => OpenForm(new DataBase());

		private void rELATÓRIOToolStripMenuItem_Click(object sender, EventArgs e) => OpenForm(new RelatorioValidade());


		private void fORMUToolStripMenuItem_Click(object sender, EventArgs e) => OpenForm(new Manipulados());

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			// RawPrinterHelper.PortCOM_tt();
			//MessageBox.Show(IMPRESSORAS.);
			//(new ViewPrint()).ShowDialog();

			string tt = "Bematech_COM7";
			string tt2 = "Bematech (COM7)";


			string xtt = tt.Substring(tt.IndexOf("COM"),4);
			string xtt2 = tt2.Substring(tt2.IndexOf("COM"),4);


		}

		
	}


}
