using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Models
{
	public class User : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string id;
		string nome;
		Funcao funcao;
		bool status;


		public string ID
		{
			get => id; set
			{
				if (value != id)
				{
					id = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public string Nome
		{
			get => nome; set
			{
				if (value != nome)
				{
					nome = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public Funcao Funcao
		{
			get => funcao; set
			{
				if (value != funcao)
				{
					funcao = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public bool Status
		{
			get => status; set
			{
				if (value != status)
				{
					status = value;
					PropertyChanged?.Invoke(this,null);
				}
			}
		}
	}
	public enum Funcao
	{
		DEV, ADMIN, GERENTE, FARMACEUTICO, BALCONISTA, OPLOJA, OPCAIXA, ENTREGADOR
	}
}
