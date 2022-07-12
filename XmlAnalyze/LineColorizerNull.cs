using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XmlAnalyze
{
    class LineColorizerNull : DocumentColorizingTransformer
    {
        List<int> lineNumber = new List<int>();

        public LineColorizerNull(List<int> lineNumber)
        {
            this.lineNumber = lineNumber;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            for (int i = 0; i < lineNumber.Count; i++)
            {
                if (!line.IsDeleted && line.LineNumber == lineNumber[i])
                {
                    ChangeLinePart(line.Offset, line.EndOffset, ApplyChanges);
                }
            }

        }

        void ApplyChanges(VisualLineElement element)
        {

            element.TextRunProperties.SetForegroundBrush(Brushes.White);
            element.TextRunProperties.SetBackgroundBrush(Brushes.DarkRed);
        }
    }
}
