using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BitInspectorTabo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPageViewModel SPVM { get; } = App.AppParam.SPVM;
        public SettingsPage()
        {
            InitializeComponent();
        }
    }

    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial int SelectedTheme { get; set; }
        partial void OnSelectedThemeChanged(int value)
        {
            if (App.AppParam.MainWindowGridRoot == null)
            {
                return;
            }

            switch(value)
            {
                case 0: // System
                    App.AppParam.MainWindowGridRoot.RequestedTheme = ElementTheme.Default;
                    break;
                case 1: // Light
                    App.AppParam.MainWindowGridRoot.RequestedTheme = ElementTheme.Light;
                    break;
                case 2: // Dark
                    App.AppParam.MainWindowGridRoot.RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                    break;
            }
        }

        [ObservableProperty]
        public partial int SelectedLanguage { get; set; }
        partial void OnSelectedLanguageChanged(int value)
        {
            var window = App.AppParam.MainWindow;
            if(window == null)
            {
                return;
            }

            switch(value)
            {
                case 0: // System
                    ApplicationLanguages.PrimaryLanguageOverride = string.Empty;
                    break;
                case 1: // English
                    ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                    break;
                case 2: // Japanese
                    ApplicationLanguages.PrimaryLanguageOverride = "ja-JP";
                    break;
                default:
                    break;
            }

            window.RefreshContent();
        }
    }
}
