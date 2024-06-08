using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Models
{
	public class User
	{
        public string ID { get; set; }
        public string Nome { get; set; }
        public Funcao Funcao { get; set; }
        public bool Status { get; set; }
    }
    public enum Funcao
    {
        DEV,ADMIN,GERENTE,FARMACEUTICO, BALCONISTA, OPLOJA, OPCAIXA,ENTREGADOR
    }
}
