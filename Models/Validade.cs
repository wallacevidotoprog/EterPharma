using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using EterPharma.Ex;
using EterPharma.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Models
{
	[Serializable]
	public class Validade 
	{
        public Validade()
        {
				
        }
        public void Init(string id,string nome,DateTime date)
        {
			DADOS = new ValidadeDados();	
			DADOS.ID = id;
			DADOS.NOME = nome;
			DADOS.DATA = date;

			PRODUTOS = new eList<ValidadeProdutos>();			

			CATEGORIA = new eList<ValidadeCategoria>();
			CATEGORIA.Add(new ValidadeCategoria { ID = 0, NOME = "S/ CATEGORIA" },false);

			InitEvents();
			WriteFileAsync();
		}
		public void InitEvents()
		{
			PRODUTOS.ItemEdit += ValidadeEvents;
			CATEGORIA.ItemEdit += ValidadeEvents;
		}

		private void ValidadeEvents(object sender, EventArgs e) => WriteFileAsync();

		public async void WriteFileAsync()=> await RWXML.SerializeToXmlFileAsync(this);


		public ValidadeDados DADOS { get; set; }
		public eList<ValidadeProdutos> PRODUTOS { get; set; }
		public eList<ValidadeCategoria> CATEGORIA { get; set; }
	}
	[Serializable]
	public class ValidadeDados : INotifyPropertyChanged
	{
		string _ID;
		string _NOME;
		DateTime _DATA;

		public string ID
		{
			get => _ID; set
			{
				if (value != _ID)
				{
					_ID = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public string NOME
		{
			get => _NOME; set
			{
				if (value != _NOME)
				{
					_NOME = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public DateTime DATA
		{
			get => _DATA; set
			{
				if (value != _DATA)
				{
					_DATA = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
	[Serializable]
	public class ValidadeProdutos : INotifyPropertyChanged
	{
		int _ID;
		string _EAN;
		string _COD_PRODUTO;
		int _CATEGORIA;
		string _DESCRICAO_PRODUTO;
		int _QTD;
		DateTime _DATA;



		public int ID
		{
			get => _ID; set
			{
				if (value != _ID)
				{
					_ID = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public string EAN
		{
			get => _EAN; set
			{
				if (value != _EAN)
				{
					_EAN = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public string COD_PRODUTO
		{
			get => _COD_PRODUTO; set
			{
				if (value != _COD_PRODUTO)
				{
					_COD_PRODUTO = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public string DESCRICAO_PRODUTO
		{
			get => _DESCRICAO_PRODUTO; set
			{
				if (value != _DESCRICAO_PRODUTO)
				{
					_DESCRICAO_PRODUTO = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public int QTD
		{
			get => _QTD; set
			{
				if (value != _QTD)
				{
					_QTD = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public DateTime DATA
		{
			get => _DATA; set
			{
				if (value != _DATA)
				{
					_DATA = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public int CATEGORIA
		{
			get => _CATEGORIA; set
			{
				if (value != _CATEGORIA)
				{
					_CATEGORIA = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
	[Serializable]
	public class ValidadeCategoria : INotifyPropertyChanged
	{
		int _ID;
		string _NOME;

		public int ID
		{
			get => _ID; set
			{
				if (value != _ID)
				{
					_ID = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}
		public string NOME
		{
			get => _NOME; set
			{
				if (value != _NOME)
				{
					_NOME = value;
					PropertyChanged?.Invoke(this, null);
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
