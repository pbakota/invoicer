using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

using Svg.Skia;

namespace Invoicer.App.Extensions;

public static class SvgExtensions
{
    public static void Svg(this IContainer container, SKSvg svg, float scale = 1.0f)
    {
        container
            .AlignCenter()
            .AlignMiddle()
            .Scale(scale)
            .Width(svg.Picture!.CullRect.Width)
            .Height(svg.Picture.CullRect.Height)
            .Canvas((canvas, space) => canvas.DrawPicture(svg.Picture));
    }
}