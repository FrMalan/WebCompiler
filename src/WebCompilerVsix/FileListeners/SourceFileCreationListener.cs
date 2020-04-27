using System;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace WebCompilerVsix.Listeners
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("LESS")]
    [ContentType("SCSS")]
    [ContentType("CoffeeScript")]
    [ContentType("Iced")]
    [ContentType("jsx")]
    [ContentType("javascript")]
    [ContentType(SassContentTypeDefinition.SassContentType)]
    [ContentType(HandlebarsContentTypeDefinition.HandleBarsContentType)]
    [ContentType(HBSContentTypeDefinition.HBSContentType)]
    [ContentType(StylusContentTypeDefinition.StylusContentType)]
    [ContentType(StylContentTypeDefinition.StylContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class SourceFileCreationListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        private ITextDocument _document;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _document))
            {
                _document.FileActionOccurred += DocumentSaved;
            }

            textView.Closed += TextviewClosed;
        }

        private static void TextviewClosed(object sender, EventArgs e)
        {
            var view = (IWpfTextView)sender;

            if (view != null)
                view.Closed -= TextviewClosed;
        }

        private static void DocumentSaved(object sender, TextDocumentFileActionEventArgs e)
        {
            if (WebCompilerPackage._dte == null || e.FileActionType != FileActionTypes.ContentSavedToDisk)
            {
                return;
            }

            // Check if filename is absolute because when debugging, script files are sometimes dynamically created.
            if (string.IsNullOrEmpty(e.FilePath) || !Path.IsPathRooted(e.FilePath))
                return;

            var item = WebCompilerPackage._dte.Solution.FindProjectItem(e.FilePath);

            if (item != null && item.ContainingProject != null)
            {
                string configFile = item.ContainingProject.GetConfigFile();

                //ErrorList.CleanErrors(e.FilePath);

                if (File.Exists(configFile))
                {
                    CompilerService.SourceFileChanged(configFile, e.FilePath);
                }
            }
        }
    }
}
