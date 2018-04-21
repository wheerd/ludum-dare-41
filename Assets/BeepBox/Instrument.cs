using System;

namespace BeepBox
{
	[Serializable]
    public class Instrument
    {
        public string type;
        public int volume;
        public string wave;
        public string transition;
        public string filter;
        public string chorus;
        public string effect;
    }
}