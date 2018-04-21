using System.Collections.Generic;
using System;

namespace BeepBox
{
	[Serializable]
    public class Pattern
    {
        public int instrument;
        public Note[] notes;
    }
}