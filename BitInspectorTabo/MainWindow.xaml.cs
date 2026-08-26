using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BitInspectorTabo.Pages;
using Microsoft.UI;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BitInspectorTabo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            FrameMain.Navigate(typeof(MainPage));

            App.AppParam.MainWindow = this;
            App.AppParam.MainWindowGridRoot = GridRoot;
        }

        public void RefreshContent()
        {
            FrameMain.Navigate(typeof(MainPage));
        }

        private void GridRoot_ActualThemeChanged(FrameworkElement sender, object args)
        {
            bool darkTheme = sender.ActualTheme == ElementTheme.Dark;
            ApplyTheme(AppWindow, darkTheme);

            void ApplyTheme(AppWindow appWindow, bool darkTheme)
            {
                if (AppWindow != null)
                {
                    var foregroundColor = darkTheme ? Colors.White : Colors.Black;
                    appWindow.TitleBar.ButtonForegroundColor = foregroundColor;
                    appWindow.TitleBar.ButtonHoverForegroundColor = foregroundColor;

                    var backgroundHoverColor = darkTheme ? Color.FromArgb(24, 255, 255, 255) : Color.FromArgb(24, 0, 0, 0);
                    appWindow.TitleBar.ButtonHoverBackgroundColor = backgroundHoverColor;
                }
            }
        }
    }
}
