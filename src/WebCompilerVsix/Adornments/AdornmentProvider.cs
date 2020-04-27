using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using WebCompiler;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace WebCompilerVsix
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("json")]
    [ContentType("javascript")]
    [ContentType("CSS")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class AdornmentProvider : IWpfTextViewCreationListener
    {
        private const string _propertyName = "ShowWatermark";
        private const double _initOpacity = 0.3D;
        private SettingsManager _settingsManager;

        private static bool _isVisible, _hasLoaded;

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        [Import]
        public SVsServiceProvider serviceProvider { get; set; }

        private void LoadSettings()
        {
            _hasLoaded = true;

            _settingsManager = new ShellSettingsManager(serviceProvider);
            var store = _settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

            LogoAdornment.VisibilityChanged += AdornmentVisibilityChanged;

            _isVisible = store.GetBoolean(Constants.CONFIG_FILENAME, _propertyName, true);
        }

        private void AdornmentVisibilityChanged(object sender, bool isVisible)
        {
            var wstore = _settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            _isVisible = isVisible;

            if (!wstore.CollectionExists(Constants.CONFIG_FILENAME))
                wstore.CreateCollection(Constants.CONFIG_FILENAME);

            wstore.SetBoolean(Constants.CONFIG_FILENAME, _propertyName, isVisible);
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            if (!_hasLoaded)
                LoadSettings();

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out var document))
            {
                var fileName = Path.GetFileName(document.FilePath).ToLowerInvariant();

                // Check if filename is absolute because when debugging, script files are sometimes dynamically created.
                if (string.IsNullOrEmpty(fileName) || !Path.IsPathRooted(document.FilePath))
                    return;

                CreateAdornments(document, textView);
            }
        }

        private void CreateAdornments(ITextDocument document, IWpfTextView textView)
        {
            var fileName = document.FilePath;

            if (Path.GetFileName(fileName) == Constants.CONFIG_FILENAME)
            {
                var highlighter = new LogoAdornment(textView, _isVisible, _initOpacity);
            }
            else if (WebCompilerPackage._dte != null && Path.IsPathRooted(fileName))
            {
                try
                {
                    var item = WebCompilerPackage._dte.Solution.FindProjectItem(fileName);

                    if (item?.ContainingProject == null)
                        return;

                    var configFile = item.ContainingProject.GetConfigFile();

                    if (string.IsNullOrEmpty(configFile))
                        return;

                    var extension = Path.GetExtension(fileName.Replace(".map", ""));
                    var normalizedFilePath = fileName.Replace(".map", "").Replace(".min" + extension, extension);

                    var configs = ConfigHandler.GetConfigs(configFile);

                    if (configs.Any(config => config.GetAbsoluteOutputFile().FullName.Equals(normalizedFilePath, StringComparison.OrdinalIgnoreCase)))
                    {
                        var generated = new GeneratedAdornment(textView, _isVisible, _initOpacity);
                        textView.Properties.AddProperty("generated", true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }
    }
}