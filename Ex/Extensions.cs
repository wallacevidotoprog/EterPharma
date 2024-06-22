using EterPharma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EterPharma.Ex
{
	public static class Extensions
	{
		public static string GetNameCategory(this int id,List<ValidadeCategoria> validadeCategorias)
		{
			return validadeCategorias.Find(x => x.ID==id).NOME;
		}

		public static int ReturnIndexUser(string id)
		{
			int retId = -1;
            for (int i = 0; i < MainWindow.database.Users.Count; i++)
            {
				if (MainWindow.database.Users[i].ID == id)
				{
					retId = i;
					break;
				}
            }
			return retId;
        }

		public static int ReturnIndexUserCB(string id, ComboBox cb)
		{

			BindingSource sb = (BindingSource)cb.DataSource;
			Dictionary<string, string> tempD = (Dictionary<string, string>)sb.DataSource;

			int index = 0;
			foreach (var kvp in tempD)
			{
				if (kvp.Key.Equals(id))
				{
					return index;
				}
				index++;
			}
			return -1;
		}
	}
}
