using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace OnlineDiaryApp.Utilities
{
    public static class PdfGenerator
    {
        public static byte[] GenerateNotePdf(string title, string content)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 36, 36, 36, 36);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                var titleFont = new Font(baseFont, 16, Font.BOLD);
                var bodyFont = new Font(baseFont, 12);

                document.Add(new Paragraph(title, titleFont));
                document.Add(new Paragraph("\n"));

                if (string.IsNullOrWhiteSpace(content))
                {
                    document.Add(new Paragraph(" ", bodyFont));
                }
                else
                {
                    if (content.Contains("<") && content.Contains(">"))
                    {
                        string cleanedHtml = CleanHtml(content);

                        cleanedHtml = "<p>&nbsp;</p>" + cleanedHtml;

                        using (var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(cleanedHtml)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(
                                writer,
                                document,
                                htmlStream,
                                null,
                                Encoding.UTF8,
                                new UnicodeFontProvider(fontPath)
                            );
                        }
                    }
                    else
                    {
                        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        foreach (var line in lines)
                        {
                            document.Add(new Paragraph(line, bodyFont));
                        }
                    }
                }

                document.Close();
                return ms.ToArray();
            }
        }

        private static string CleanHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";

            html = html.Replace("<br>", "<br />").Replace("<BR>", "<br />");

            html = System.Text.RegularExpressions.Regex.Replace(html, @"<div[^>]*>", "<p>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"</div>", "</p>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<span[^>]*>\s*</span>", "");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<p>\s*</p>", "<p>&nbsp;</p>");

            return html;
        }

        private class UnicodeFontProvider : XMLWorkerFontProvider
        {
            private readonly string _fontPath;
            public UnicodeFontProvider(string fontPath) => _fontPath = fontPath;

            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color)
            {
                return base.GetFont(_fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
            }
        }
    }
}
