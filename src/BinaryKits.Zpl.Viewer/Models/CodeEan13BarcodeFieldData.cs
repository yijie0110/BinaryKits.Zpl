using BinaryKits.Zpl.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryKits.Zpl.Viewer.Models
{
    class CodeEan13BarcodeFieldData : FieldDataBase
    {
        public FieldOrientation FieldOrientation { get; set; }
        public int Height { get; set; }
        public bool PrintInterpretationLine { get; set; }
        public bool PrintInterpretationLineAboveCode { get; set; }
        public string Mode { get; set; }
    }
}
