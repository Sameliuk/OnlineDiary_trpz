using System.IO;
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

                // Шрифт для кирилиці
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                var titleFont = new Font(baseFont, 16, Font.BOLD);
                var bodyFont = new Font(baseFont, 12);

                // Заголовок
                document.Add(new Paragraph(title, titleFont));
                document.Add(new Paragraph("\n"));

                // Очистка HTML
                string cleanedHtml = CleanHtml(content);

                // Перетворюємо HTML на Stream
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

                document.Close();
                return ms.ToArray();
            }
        }

        private static string CleanHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";

            html = html.Replace("<br>", "<br />").Replace("<BR>", "<br />");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"</?div[^>]*>", "");

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
