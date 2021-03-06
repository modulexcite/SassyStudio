﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class XmlDocumentationValueProvider : ICompletionValueProvider
    {
        public IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get
            {
                yield return SassCompletionContextType.XmlDocumentationComment;
                yield return SassCompletionContextType.FileReferenceFileAttribute;
            }
        }

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            switch (type)
            {
                case SassCompletionContextType.XmlDocumentationComment:
                    yield return new KeywordCompletionValue("reference");
                    break;
                case SassCompletionContextType.FileReferenceFileAttribute:
                    yield return new KeywordCompletionValue("file") { CompletionText = "file=\"" };
                    break;
            }
        }
    }
}
