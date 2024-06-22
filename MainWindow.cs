using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EterPharma.Ex;
using EterPharma.Models;
using EterPharma.Services;
using EterPharma.VIEWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EterPharma
{
	public partial class MainWindow : Form
	{
		public static Database database;
		public MainWindow()
		{
			InitializeComponent();
		}

		private void toolStripButton_manipulacao_Click(object sender, EventArgs e)
		{
			Manipulados form = new Manipulados
			{
				TopLevel = false,
				FormBorderStyle = FormBorderStyle.None,
				Dock = DockStyle.Fill				
			};
			form.FormClosed += new FormClosedEventHandler(this.ChildForm_FormClosed);
			this.panel_center.Controls.Clear();
			this.panel_center.Controls.Add(form);
			form.Show();
			this.toolStrip1.Visible = false;
		}
		private void ChildForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.toolStrip1.Visible = true;
		}

		private async void MainWindow_Load(object sender, EventArgs e)
		{
			database = new Database(progressBar_status, toolStrip1);
		}
		private void gERARVALIDADEDOMÊSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GerarValidade form = new GerarValidade
			{
				TopLevel = false,
				FormBorderStyle = FormBorderStyle.None,
				Dock = DockStyle.Fill
			};
			form.FormClosed += new FormClosedEventHandler(this.ChildForm_FormClosed);
			this.panel_center.Controls.Clear();
			this.panel_center.Controls.Add(form);
			form.Show();
			this.toolStrip1.Visible = false;
		}

		private void toolStripButton_conf_Click(object sender, EventArgs e)
		{
			if (InputBox.Show("Qual a senha:","SENHA =D",true)=="32195018")
			{
				DataBase form = new DataBase
				{
					TopLevel = false,
					FormBorderStyle = FormBorderStyle.None,
					Dock = DockStyle.Fill,
					progressBar = progressBar_status
				};
				form.FormClosed += new FormClosedEventHandler(this.ChildForm_FormClosed);
				this.panel_center.Controls.Clear();
				this.panel_center.Controls.Add(form);
				form.Show();
				this.toolStrip1.Visible = false;
			}
		}
	}
}
