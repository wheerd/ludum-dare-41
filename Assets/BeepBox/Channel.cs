using System.Collections.Generic;
using System;

namespace BeepBox
{
	[Serializable]
    public class Channel
    {
        public string type;
        public int octaveScrollBar;
        public Instrument[] instruments;
        public Pattern[] patterns;
        public int[] sequence;
    }
}