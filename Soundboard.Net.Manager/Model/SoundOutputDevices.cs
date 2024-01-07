using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundboard.Net.Manager.Model
{
	public class SoundOutputDevices
	{
		public string Name { get;set; }
		public string ID { get;set; }

		public override string ToString()
		{
			return Name;
		}

	}
}
