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
using System.Globalization;
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
    public sealed partial class InspectorPage : Page
    {
        public InspectorPageViewModel IPVM { get; } = App.AppParam.IPVM;

        public InspectorPage()
        {
            InitializeComponent();
        }
    }

    public partial class InspectorPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Hex { get; set; }
        partial void OnHexChanged(string value)
        {
            Bin = value;

            if (!UInt64.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out UInt64 ul))
            {
                return;
            }
            var ui = (UInt32)(ul & 0xFFFFFFFF);

            Float = BitConverter.UInt32BitsToSingle(ui).ToString("R");
            Double = BitConverter.UInt64BitsToDouble(ul).ToString("R");
        }
        [ObservableProperty]
        public partial string Bin { get; set; }
        [ObservableProperty]
        public partial string Float { get; set; }
        [ObservableProperty]
        public partial string Double { get; set; }
    }
}
