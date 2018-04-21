using System.Collections.Generic;
using System;

namespace BeepBox
{
	[Serializable]
	public class Note
	{
		public int[] pitches;
		public Point[] points;

		public int StartTick {
			get { return points [0].tick; }
		}

		public int EndTick {
			get { return points [1].tick; }
		}
	}

}
