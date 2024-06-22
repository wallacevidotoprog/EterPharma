using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using EterPharma.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EterPharma.Ex
{
	[Serializable]
	public class eList<T> : List<T>
	{
		
		public event EventHandler ItemEdit;
		public void OnEndEdit() => ItemEdit?.Invoke(this, new EventArgs());
		public new void Add(T item, bool ev = true)
		{
			base.Add(item);
			if (ev)
			{
				OnEndEdit();
			}
		}
		public new void RemoveAt(int index)
		{
			base.RemoveAt(index);
			OnEndEdit();
		}
		


	}
}
