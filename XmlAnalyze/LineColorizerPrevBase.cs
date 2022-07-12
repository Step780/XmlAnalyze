﻿using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XmlAnalyze
{
    class LineColorizerPrevBase : DocumentColorizingTransformer
    {

        List<int> lineNumberr = new List<int>();

        public LineColorizerPrevBase(int lineNumber)
        {
            lineNumberr.Add(lineNumber);
        }

        protected override void ColorizeLine(DocumentLine line)
        {

            for (int i = 0; i < lineNumberr.Count; i++)
            {
                if (!line.IsDeleted && line.LineNumber == lineNumberr[i])
                {
                    ChangeLinePart(line.Offset, line.EndOffset, ApplyChanges);
                }
            }

        }

        void ApplyChanges(VisualLineElement element)
        {
            element.TextRunProperties.SetForegroundBrush(Brushes.Black);
            element.TextRunProperties.SetBackgroundBrush(Brushes.LightSeaGreen);

        }
    }
}
