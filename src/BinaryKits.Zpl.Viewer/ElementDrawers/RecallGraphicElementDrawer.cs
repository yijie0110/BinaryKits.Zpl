﻿using BinaryKits.Zpl.Label.Elements;
using SkiaSharp;

namespace BinaryKits.Zpl.Viewer.ElementDrawers
{
    public class RecallGraphicElementDrawer : ElementDrawerBase
    {
        ///<inheritdoc/>
        public override bool CanDraw(ZplElementBase element)
        {
            return element is ZplRecallGraphic;
        }

        ///<inheritdoc/>
        public override void Draw(ZplElementBase element)
        {
            if (element is ZplRecallGraphic recallGraphic)
            {
                var imageData = this._printerStorage.GetFile(recallGraphic.StorageDevice, recallGraphic.ImageName);

                if (imageData.Length == 0)
                {
                    return;
                }

                var x = recallGraphic.PositionX;
                var y = recallGraphic.PositionY;

                this._skCanvas.DrawBitmap(SKBitmap.Decode(imageData), x, y);
            }
        }
    }
}
