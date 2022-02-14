using BinaryKits.Zpl.Label.Elements;
using BinaryKits.Zpl.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryKits.Zpl.Viewer.CommandAnalyzers
{
    class CodeEan13BarcodeZplCommandAnalyzer : ZplCommandAnalyzerBase
    {
        public CodeEan13BarcodeZplCommandAnalyzer(VirtualPrinter virtualPrinter) : base("^BE", virtualPrinter)
        { }

        public override ZplElementBase Analyze(string zplCommand)
        {
            var zplDataParts = this.SplitCommand(zplCommand);

            var fieldOrientation = this.ConvertFieldOrientation(zplDataParts[0]);
            var height = this.VirtualPrinter.BarcodeInfo.Height;
            var printInterpretationLine = true;
            var printInterpretationLineAboveCode = false;
            var mode = "N";

            if (zplDataParts.Length > 1 && zplDataParts[1].Trim().Length > 0)
            {
                _ = int.TryParse(zplDataParts[1], out height);
            }
            if (zplDataParts.Length > 2)
            {
                printInterpretationLine = this.ConvertBoolean(zplDataParts[2]);
            }
            if (zplDataParts.Length > 3)
            {
                printInterpretationLineAboveCode = this.ConvertBoolean(zplDataParts[3]);
            }
            if (zplDataParts.Length > 4)
            {
                mode = zplDataParts[4];
            }

            //The field data are processing in the FieldDataZplCommandAnalyzer
            this.VirtualPrinter.SetNextElementFieldData(new CodeEan13BarcodeFieldData
            {
                FieldOrientation = fieldOrientation,
                Height = height,
                PrintInterpretationLine = printInterpretationLine,
                PrintInterpretationLineAboveCode = printInterpretationLineAboveCode,
                Mode = mode
            });

            return null;
        }
    }
}
