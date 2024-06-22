using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EterPharma.Ex
{
	public class InputBox : Form
	{
		private TextBox inputTextBox;
		private Button okButton;
		private Button cancelButton;

		public string InputText { get; private set; }

		public InputBox(string prompt, string title, bool pass)
		{

			Text = title;
			Width = 300;
			Height = 150;
			StartPosition = FormStartPosition.CenterParent;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			AcceptButton = okButton;
			CancelButton = cancelButton;


			Label promptLabel = new Label
			{
				Text = prompt,
				AutoSize = true,
				Location = new System.Drawing.Point(10, 10)

			};
			Controls.Add(promptLabel);


			inputTextBox = new TextBox
			{
				Location = new System.Drawing.Point(10, 40),
				Width = 260,
				CharacterCasing = CharacterCasing.Upper

			};
			if (pass)
			{
				inputTextBox.PasswordChar = '*';
			}

			inputTextBox.KeyDown += OkButton_KeyDown;
			Controls.Add(inputTextBox);


			okButton = new Button
			{
				Text = "OK",
				DialogResult = DialogResult.OK,
				Location = new System.Drawing.Point(110, 70)
			};
			okButton.Click += OkButton_Click;
			Controls.Add(okButton);


			cancelButton = new Button
			{
				Text = "Cancelar",
				DialogResult = DialogResult.Cancel,
				Location = new System.Drawing.Point(200, 70)
			};
			Controls.Add(cancelButton);
		}

		private void OkButton_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				InputText = inputTextBox.Text;
				DialogResult= DialogResult.OK;
			}
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			InputText = inputTextBox.Text;
		}

		public static string Show(string prompt, string title, bool pass = false)
		{
			using (InputBox inputBox = new InputBox(prompt, title, pass))
			{
				return inputBox.ShowDialog() == DialogResult.OK ? inputBox.InputText : null;
			}
		}
	}
}
