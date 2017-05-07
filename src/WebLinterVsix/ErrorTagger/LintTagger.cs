using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Shell;
using Linters;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using EnvDTE;
using WebLinter;

namespace WebLinterVsix
{
    internal class LintTagger : ITagger<IErrorTag>
    {
        private ITextBuffer buffer;

        private string fileName;

        private CancellationTokenSource cancellationTokenSource;

        public LintTagger(ITextBuffer buffer, string fileName)
        {
            this.buffer = buffer;
            this.fileName = fileName;
            this.buffer.Changed += OnBufferChanged;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection snapshotSpans)
        {
            var list = new List<TagSpan<IErrorTag>>();

            List<LintingError> errors = TableDataSource.Instance.GetErrors(this.fileName);
            var snapshot = buffer.CurrentSnapshot;

            foreach (SnapshotSpan snapshotSpan in snapshotSpans)
            {
                foreach (LintingError error in errors)
                {
                    if (error.LineNumber > snapshotSpan.Snapshot.LineCount)
                    {
                        continue;
                    }

                    var lintTag = new LintTag(error.Message);

                    Span errorSpan = GetErrorSpan(snapshotSpan.Snapshot, error);

                    var tagSpan = new TagSpan<IErrorTag>(new SnapshotSpan(snapshotSpan.Snapshot, errorSpan), lintTag);

                    list.Add(tagSpan);
                }
            }

            return list;
        }

        private static Span GetErrorSpan(ITextSnapshot snapshot, LintingError error)
        {
            var line = snapshot.GetLineFromLineNumber(error.LineNumber);
            var endLine = snapshot.GetLineFromLineNumber(error.EndLineNumber);

            var start = line.Start.Position + error.ColumnNumber;
            var length = (endLine.Start.Position + error.EndColumnNumber) - start;

            return new Span(start, length);
        }

        public void InvokeTagsChanged() {
            var handler = TagsChanged;

            if (handler != null)
            {
                var snapshot = buffer.CurrentSnapshot;
                var span = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));

                handler(this, new SnapshotSpanEventArgs(span));
            }
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            cancellationTokenSource = new CancellationTokenSource();
            UpdateErrorsWithDelay(e.After, cancellationTokenSource.Token);
        }

        private void UpdateErrorsWithDelay(ITextSnapshot snapshot, CancellationToken token)
        {
            System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(500);

                if (token.IsCancellationRequested)
                {
                    return;
                }

            //TODO: call the linter

            InvokeTagsChanged();
            }, token);
        }
    }
}
