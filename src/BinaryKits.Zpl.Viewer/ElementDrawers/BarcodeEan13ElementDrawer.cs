using BarcodeLib;
using BinaryKits.Zpl.Label.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryKits.Zpl.Viewer.ElementDrawers
{
    class BarcodeEan13ElementDrawer : BarcodeDrawerBase
    {
        ///<inheritdoc/>
        public override bool CanDraw(ZplElementBase element)
        {
            return element is ZplBarcodeEan13;
        }

        ///<inheritdoc/>
        public override void Draw(ZplElementBase element)
        {
            if (element is ZplBarcodeEan13 barcode)
            {
                float x = barcode.PositionX;
                float y = barcode.PositionY;

                if (barcode.FieldTypeset != null)
                {
                    y -= barcode.Height;
                }

                var barcodeElement = new Barcode
                {
                    IncludeLabel = barcode.PrintInterpretationLine,
                    //StandardizeLabel = true will cause label and barcode overlapping,Maybe it's a bug in BarcodeLib
                    StandardizeLabel = false,
                    Alignment = AlignmentPositions.CENTER,
                    DisableEAN13CountryException = true,
                    LabelPosition = barcode.PrintInterpretationLineAboveCode ? LabelPositions.TOPCENTER : LabelPositions.BOTTOMCENTER,
                    BarWidth = barcode.ModuleWidth,
                    BackColor = Color.Transparent,
                    Height = barcode.Height 
                    
                };

                using var image = barcodeElement.Encode(TYPE.EAN13, barcode.Content);
                this.DrawBarcode(this.GetImageData(image), barcode.Height, image.Width, barcode.FieldOrigin != null, x, y, barcode.FieldOrientation);
            }
        }
    }
}
