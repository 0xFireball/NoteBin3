using System;

namespace NoteBin3.Types
{
    public class Notebook
    {
        public NotebookData Contents { get; set; }
        public Guid Identifier { get; set; }
    }
}