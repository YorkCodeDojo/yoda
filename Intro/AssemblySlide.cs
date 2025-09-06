using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Intro;

public class AssemblySlide(string title, string[] textToDisplay, int fontSize, int instructionPointer) : Control, ISlide
{
    public DisplayResult Display(bool reset)
    {
        if (reset)
        {
            return DisplayResult.MoreToDisplay;
        }
        return DisplayResult.Completed;
    }
    
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        RenderTitle(context);
        
        for (var i= 0;i  <textToDisplay.Length; i++)
        {
            var parts = textToDisplay[i].Split(':');
            
            var addressText = new FormattedText(
                parts[0], 
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, 
                new Typeface("Segoe UI"), 
                fontSize, 
                Brushes.CornflowerBlue);

            var origin = new Point(100,  150 + ( i * 150));
            context.DrawText(addressText, origin);

            if (parts.Length > 1)
            {
                var contentsText = new FormattedText(
                    " : " + parts[1],
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    fontSize,
                    Brushes.White);

                origin = new Point(280, 150 + (i * 150));
                context.DrawText(contentsText, origin);

                if (instructionPointer == int.Parse(parts[0].Replace("0x", ""), NumberStyles.HexNumber))
                {
                    var ipText = new FormattedText(
                        "<-- Instruction Pointer",
                        CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"),
                        fontSize,
                        Brushes.IndianRed);

                    origin = new Point(600, 150 + (i * 150));
                    context.DrawText(ipText, origin);
                }
            }
        }
    }

    private void RenderTitle(DrawingContext context)
    {
        var formattedText = new FormattedText(
            title, 
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight, 
            new Typeface("Segoe UI"), 
            85, 
            Brushes.White);

        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        var origin = new Point(center.X - formattedText.Width / 2, 10);

        context.DrawText(formattedText, origin);
    }
}