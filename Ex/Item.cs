using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EterPharma.Ex
{
	public class Item<T> where T : INotifyPropertyChanged
	{
		private T _value;

		public Item(T valor)
		{
			_value = valor;
			_value.PropertyChanged += ValuePropertyChanged;
		}

		public T Valor
		{
			get => _value;
			set
			{
				if (!Equals(_value, value))
				{
					_value.PropertyChanged -= ValuePropertyChanged;
					_value = value;
					_value.PropertyChanged += ValuePropertyChanged;
					OnChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler ValueChanged;
		private void ValuePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnChanged(EventArgs.Empty);
		}
		protected virtual void OnChanged(EventArgs e)
		{
			ValueChanged?.Invoke(this, e);
		}
	}
}
