using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Label.Elements;
using SkiaSharp;
using System;

namespace BinaryKits.Zpl.Viewer.ElementDrawers
{
    public class FieldBlockElementDrawer : ElementDrawerBase
    {
        ///<inheritdoc/>
        public override bool CanDraw(ZplElementBase element)
        {
            return element is ZplFieldBlock;
        }

        public override bool IsReverseDraw(ZplElementBase element)
        {
            if (element is ZplFieldBlock fieldBlock)
            {
                return fieldBlock.ReversePrint;
            }

            return false;
        }

        ///<inheritdoc/>
        public override void Draw(ZplElementBase element)
        {
            if (element is ZplFieldBlock fieldBlock)
            {
                float x = fieldBlock.PositionX;
                float y = fieldBlock.PositionY;
                var font = fieldBlock.Font;

                float fontSize = font.FontHeight > 0 ? font.FontHeight : font.FontWidth;
                var scaleX = 1.0f;
                if (font.FontWidth != 0 && font.FontWidth != fontSize)
                {
                    scaleX = (float)font.FontWidth / fontSize;
                }

                fontSize *= 0.95f;
                //bottomToTop == true means is a ^FT tag so is a ZplFieldTypeset
                if (fieldBlock.BottomToTop)
                {
                    switch (fieldBlock.Font.FieldOrientation)
                    {
                        case FieldOrientation.Rotated180:
                            y += (fieldBlock.MaxLineCount) * fontSize / 0.95f;
                            break;
                        case FieldOrientation.Normal:
                            y -= (fieldBlock.MaxLineCount) * fontSize / 0.95f;
                            break;
                    }
                }

                var typeface = SKTypeface.Default;
                if (font.FontName == "0")
                {
                    //typeface = SKTypeface.FromFile(@"swiss-721-black-bt.ttf");
                    typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
                }

                var textLines = fieldBlock.Text.Split(new[] { "\\&" }, StringSplitOptions.RemoveEmptyEntries);

                textLines = GetStringRows(typeface, fontSize, scaleX, textLines, fieldBlock.Width);
                int linecount = 0;
                foreach (var textLine in textLines)
                {
                    using var skPaint = new SKPaint();
                    skPaint.Color = SKColors.Black;
                    skPaint.Typeface = typeface;
                    skPaint.TextSize = fontSize;
                    skPaint.TextScaleX = scaleX;

                    var textBounds = new SKRect();
                    var textBoundBaseline = new SKRect();
                    skPaint.MeasureText(new string('A', fieldBlock.Text.Length), ref textBoundBaseline);
                    skPaint.MeasureText(textLine, ref textBounds);
                    float offset = 0;
                    switch (fieldBlock.TextJustification)
                    {
                        case TextJustification.Center:
                            var diff = fieldBlock.Width - textBounds.Width;
                            offset = diff / 2;
                            break;
                        case TextJustification.Right:
                            offset = fieldBlock.Width - textBounds.Width;
                            break;
                        case TextJustification.Left:
                        case TextJustification.Justified:
                        default:
                            break;
                    }
                    switch (fieldBlock.Font.FieldOrientation)
                    {
                        case FieldOrientation.Rotated90:
                            y += offset;
                            break;
                        case FieldOrientation.Rotated180:
                            x -= offset;
                            break;
                        case FieldOrientation.Rotated270:
                            y -= offset;
                            break;
                        case FieldOrientation.Normal:
                            x += offset;
                            break;
                    }

                    //if (fieldBlock.FieldTypeset != null)
                    //{
                    //    y -= textBounds.Height;
                    //}

                    using (new SKAutoCanvasRestore(this._skCanvas))
                    {
                        SKMatrix matrix = SKMatrix.Empty;

                        if (fieldBlock.FieldOrigin != null)
                        {
                            switch (fieldBlock.Font.FieldOrientation)
                            {
                                case FieldOrientation.Rotated90:
                                    matrix = SKMatrix.CreateRotationDegrees(90, x, y);
                                    y -= font.FontHeight - textBoundBaseline.Height;
                                    break;
                                case FieldOrientation.Rotated180:
                                    matrix = SKMatrix.CreateRotationDegrees(180, x, y);
                                    x -= textBounds.Width;
                                    y -= font.FontHeight - textBoundBaseline.Height;
                                    break;
                                case FieldOrientation.Rotated270:
                                    matrix = SKMatrix.CreateRotationDegrees(270, x, y);
                                    x -= textBounds.Width;
                                    y += textBoundBaseline.Height;
                                    break;
                                case FieldOrientation.Normal:
                                    y += textBoundBaseline.Height;
                                    break;
                            }
                        }
                        else
                        {
                            if (linecount < fieldBlock.MaxLineCount)
                            {
                                switch (fieldBlock.Font.FieldOrientation)
                                {
                                    case FieldOrientation.Rotated90:
                                        matrix = SKMatrix.CreateRotationDegrees(90, x, y);
                                        x += font.FontHeight;
                                        break;
                                    case FieldOrientation.Rotated180:
                                        matrix = SKMatrix.CreateRotationDegrees(180, x, y);
                                        y += font.FontHeight;
                                        break;
                                    case FieldOrientation.Rotated270:
                                        matrix = SKMatrix.CreateRotationDegrees(270, x, y);
                                        x -= font.FontHeight;
                                        break;
                                    case FieldOrientation.Normal:
                                        y += font.FontHeight;
                                        break;
                                }

                            }
                        }

                        if (matrix != SKMatrix.Empty)
                        {
                            this._skCanvas.SetMatrix(matrix);
                        }

                        this._skCanvas.DrawText(textLine, x, y, new SKFont(typeface, fontSize, scaleX, 0), skPaint);
                        x = fieldBlock.PositionX;
                        linecount++;
                    }
                }
            }
        }

        private string[] GetStringRows(SKTypeface sKTypeface, float TypeSize,float scaleX, string[] text, int width)
        {
            var skPaint = new SKPaint();
            skPaint.Typeface = sKTypeface;
            skPaint.TextSize = TypeSize;
            skPaint.TextScaleX = scaleX;
            string[] rows = new string[text.Length];
            int count = 0;
            string tempString = "";
            foreach (var textLine in text)
            {
                var subLines = System.Text.RegularExpressions.Regex.Split(textLine, @"(?<=[ ])");
                foreach (var subLine in subLines)
                {
                    var textBounds = new SKRect();
                    float LineWidth = skPaint.MeasureText(tempString + subLine, ref textBounds);
                    if (LineWidth < width)
                    {
                        tempString += subLine;
                    }
                    else
                    {
                        if (rows.Length-1 < count)
                        {
                            Array.Resize(ref rows, rows.Length+1);
                        }
                        rows[count] = tempString;
                        count++;
                        tempString = subLine;
                    }
                }
                if(tempString != "")
                {
                    if (rows.Length - 1 < count)
                    {
                        Array.Resize(ref rows, rows.Length + 1);
                    }
                    rows[count] = tempString;
                    count++;
                    tempString = "";
                }
            }

            return rows;
        }

    }
}
