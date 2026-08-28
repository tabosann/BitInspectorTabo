using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Effects;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BitInspectorTabo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel MPVM { get; } = App.AppParam.MPVM;

        public MainPage()
        {
            InitializeComponent();

            // NOTE: 過去のアイテムを参照しっぱなしの場合は切り離す.
            if (MPVM.SelectedMenu != null)
            {
                MPVM.SelectedMenu = null;
            }
        }

        private void GridRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // 画面をデフォルトの状態に切り替える.
            MPVM.FrameContent = FrameContent;
            MPVM.SelectedMenu = DefaultSelect;
        }

        private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
        {
            NavigationViewMain.PaneDisplayMode = NavigationViewMain.PaneDisplayMode switch
            {
                NavigationViewPaneDisplayMode.LeftCompact => NavigationViewPaneDisplayMode.Left,
                _ => NavigationViewPaneDisplayMode.LeftCompact
            };
        }
    }

    public partial class MainPageViewModel : ObservableObject
    {
        public Frame? FrameContent { get; set; } = null;

        [ObservableProperty]
        public partial object? SelectedMenu { get; set; }
        partial void OnSelectedMenuChanged(object? value)
        {
            var item = value as NavigationViewItem;
            if(item == null || FrameContent == null)
            {
                return;
            }

            switch(item.Tag)
            {
                case "Inspector":
                    FrameContent.Navigate(typeof(InspectorPage));
                    break;
                case "Help":
                    FrameContent.Navigate(typeof(HelpPage));
                    break;
                case "Settings":
                    FrameContent.Navigate(typeof(SettingsPage));
                    break;
                default:
                    FrameContent.Navigate(typeof(ErrorPage));
                    break;
            }
        }
    }
}
