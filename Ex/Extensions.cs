using EterPharma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Ex
{
	public static class Extensions
	{
		public static string GetNameCategory(this int id,List<ValidadeCategoria> validadeCategorias)
		{
			return validadeCategorias.Find(x => x.ID==id).NOME;
		}
	}
}
