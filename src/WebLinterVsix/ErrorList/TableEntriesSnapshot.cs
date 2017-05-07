﻿// Modifications Copyright Rich Newman 2017
using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableManager;
using WebLinter;

namespace WebLinterVsix
{
    class TableEntriesSnapshot : TableEntriesSnapshotBase
    {
        private string _projectName;
        private readonly List<LintingError> _errors = new List<LintingError>();

        internal TableEntriesSnapshot(string filePath, IEnumerable<LintingError> errors)
        {
            FilePath = filePath;
            _errors.AddRange(errors);
        }

        public List<LintingError> Errors {
            get { return _errors; }
        }

        public override int Count
        {
            get { return _errors.Count; }
        }

        public string FilePath { get; }

        public override int VersionNumber { get; } = 1;

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if ((index >= 0) && (index < _errors.Count))
            {
                if (columnName == StandardTableKeyNames.DocumentName)
                {
                    content = FilePath;
                }
                else if (columnName == StandardTableKeyNames.ErrorCategory)
                {
                    content = Vsix.Name;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = Vsix.Name;
                }
                else if (columnName == StandardTableKeyNames.Line)
                {
                    content = _errors[index].LineNumber;
                }
                else if (columnName == StandardTableKeyNames.Column)
                {
                    content = _errors[index].ColumnNumber;
                }
                else if (columnName == StandardTableKeyNames.Text)
                {
                    content = $"({_errors[index].Provider.Name}) {_errors[index].Message}";
                }
                else if (columnName == StandardTableKeyNames.ErrorSeverity)
                {
                    content = _errors[index].IsError ? __VSERRORCATEGORY.EC_ERROR : __VSERRORCATEGORY.EC_WARNING;
                }
                else if (columnName == StandardTableKeyNames.Priority)
                {
                    content = _errors[index].IsError ? vsTaskPriority.vsTaskPriorityHigh : vsTaskPriority.vsTaskPriorityMedium;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = ErrorSource.Other;
                }
                else if (columnName == StandardTableKeyNames.BuildTool)
                {
                    content = _errors[index].Provider.Name;
                }
                else if (columnName == StandardTableKeyNames.ErrorCode)
                {
                    content = _errors[index].ErrorCode;
                }
                else if (columnName == StandardTableKeyNames.ProjectName)
                {
                    if (string.IsNullOrEmpty(_projectName))
                    {
                        var _item = WebLinterPackage.Dte.Solution.FindProjectItem(_errors[index].FileName);

                        if (_item != null && _item.Properties != null && _item.ContainingProject != null)
                            _projectName = _item.ContainingProject.Name;
                    }

                    content = _projectName;
                }
                else if ((columnName == StandardTableKeyNames.ErrorCodeToolTip) || (columnName == StandardTableKeyNames.HelpLink))
                {
                    var error = _errors[index];
                    string url;
                    if (!string.IsNullOrEmpty(error.HelpLink))
                    {
                        url = error.HelpLink;
                    }
                    else
                    {
                        url = string.Format("http://www.bing.com/search?q={0} {1}", _errors[index].Provider.Name, _errors[index].ErrorCode);
                    }

                    content = Uri.EscapeUriString(url);
                }
            }

            return content != null;
        }
    }
}
