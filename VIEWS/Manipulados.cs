﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EterPharma.VIEWS
{
	public partial class Manipulados : Form
	{
		public Manipulados()
		{
			InitializeComponent();
		}

		private void Manipulados_Load(object sender, EventArgs e)
		{
			//this.Controls.Add(this.vScrollBar1);
		}

		private void pictureBox3_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
