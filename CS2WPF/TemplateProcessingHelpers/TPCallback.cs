using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.Collections.Generic;
using System.Text;

namespace CS2WPF.TemplateProcessingHelpers
{
    internal class TPCallback : ITextTemplatingCallback
    {
        private List<TPError> errorMessages = new List<TPError>();

        public IReadOnlyCollection<TPError> ProcessingErrors => errorMessages.AsReadOnly();

        public string FileExtension
        {
            get;
            private set;
        }

        public Encoding OutputEncoding
        {
            get;
            private set;
        }

        void ITextTemplatingCallback.ErrorCallback(bool warning, string message, int line, int column)
        {
            if (!warning)
            {
                errorMessages.Add(new TPError
                {
                    Message = message,
                    LineNumber = line,
                    ColumnNumber = column
                });
            }
        }

        void ITextTemplatingCallback.SetFileExtension(string extension)
        {
            FileExtension = extension;
        }

        void ITextTemplatingCallback.SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            OutputEncoding = encoding;
        }
    }
}
