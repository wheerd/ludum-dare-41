using System.Collections.Generic;
using System;

namespace BeepBox
{
	[Serializable]
    public class BeepBoxFile
    {
		public string format;
		public int version;
		public string scale;
        public string key;
        public int introBars;
        public int loopBars;
        public int beatsPerBar;
        public int ticksPerBeat;
        public int beatsPerMinute;
        public int reverb;
        public Channel[] channels;
    }
}