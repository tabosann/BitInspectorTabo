using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
        public partial string Hex { get; set; } = string.Empty;
        partial void OnHexChanged(string value)
        {
            if (!UInt64.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out UInt64 ul))
            {
                return;
            }
            var ui = (UInt32)(ul & 0xFFFFFFFF);

            Bin = ul.ToString($"B{ value.Length * 4 }");
            Float = BitConverter.UInt32BitsToSingle(ui).ToString("R");
            Double = BitConverter.UInt64BitsToDouble(ul).ToString("R");
        }

        [ObservableProperty]
        public partial string Bin { get; set; } = string.Empty;
        partial void OnBinChanged(string value)
        {
            if (!UInt64.TryParse(value, NumberStyles.BinaryNumber, CultureInfo.InvariantCulture, out UInt64 ul))
            {
                return;
            }
            var ui = (UInt32)(ul & 0xFFFFFFFF);

            Hex = ul.ToString($"X{ value.Length / 4 }");
            Float = BitConverter.UInt32BitsToSingle(ui).ToString("R");
            Double = BitConverter.UInt64BitsToDouble(ul).ToString("R");
        }

        [ObservableProperty]
        public partial string Float { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string Double { get; set; } = string.Empty;

        public ICommand TextBoxHexEnterCommand { get; init; }
        public ICommand TextBoxHexBeforeTextChangingCommand { get; init; }
        public ICommand TextBoxBinEnterCommand { get; init; }
        public ICommand TextBoxBinBeforeTextChangingCommand { get; init; }

        public InspectorPageViewModel()
        {
            TextBoxHexEnterCommand = new RelayCommand<string?>(TextBoxHexEnterAction);
            TextBoxHexBeforeTextChangingCommand = new RelayCommand<TextBoxBeforeTextChangingEventArgs>(TextBoxHexBeforeTextChangingAction);

            TextBoxBinEnterCommand = new RelayCommand<string?>(TextBoxBinEnterAction);
            TextBoxBinBeforeTextChangingCommand = new RelayCommand<TextBoxBeforeTextChangingEventArgs>(TextBoxBinBeforeTextChangingAction);
        }

        void TextBoxHexEnterAction(string? text)
        {
            if (text == null)
            {
                return;
            }

            text = text.Trim();

            if (text.Length > STR_LEN_HEX64) {
                // オーバーフローなので下位64bitを切り抜く.
                text = text.Substring(text.Length - STR_LEN_HEX64);
            }

            var len = text.Length;
            if (len > STR_LEN_HEX32) {
                len = STR_LEN_HEX64;
            }
            else if (len > STR_LEN_HEX16) {
                len = STR_LEN_HEX32;
            }
            else if (len > STR_LEN_HEX8) {
                len = STR_LEN_HEX16;
            }
            else {
                len = STR_LEN_HEX8;
            }

            // 変更前後が同じ値でも変更通知が出されるようにする.
            Hex = string.Empty;
            Hex = text.PadLeft(len, '0');
        }

        void TextBoxHexBeforeTextChangingAction(TextBoxBeforeTextChangingEventArgs? e)
        {
            if (e == null)
            {
                return;
            }

            var text = e.NewText.Trim();

            // 0x プレフィックスを許可.
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                text = text.Substring(2);
            }

            // 16進数以外の文字が含まれていたら拒否.
            foreach (char c in text) {
                if (!IsHexChar(c)) {
                    e.Cancel = true;
                    return;
                }
            }
        }

        void TextBoxBinEnterAction(string? text)
        {
            if (text == null)
            {
                return;
            }

            text = text.Trim();

            if (text.Length > STR_LEN_BIN64) {
                // オーバーフローなので下位64bitを切り抜く.
                text = text.Substring(text.Length - STR_LEN_BIN64);
            }

            var len = text.Length;
            if (len > STR_LEN_BIN32) {
                len = STR_LEN_BIN64;
            }
            else if (len > STR_LEN_BIN16) {
                len = STR_LEN_BIN32;
            }
            else if (len > STR_LEN_BIN8) {
                len = STR_LEN_BIN16;
            }
            else {
                len = STR_LEN_BIN8;
            }

            // 変更前後が同じ値でも変更通知が出されるようにする.
            Bin = string.Empty;
            Bin = text.PadLeft(len, '0');
        }

        void TextBoxBinBeforeTextChangingAction(TextBoxBeforeTextChangingEventArgs? e)
        {
            if (e == null)
            {
                return;
            }

            var text = e.NewText.Trim();

            // 0x プレフィックスを許可.
            if (text.StartsWith("0b", StringComparison.OrdinalIgnoreCase)) {
                text = text.Substring(2);
            }

            // 16進数以外の文字が含まれていたら拒否.
            foreach (char c in text) {
                if (!IsBinChar(c)) {
                    e.Cancel = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 16進数を構成する文字であれば真
        /// </summary>
        /// <param name="c"></param>
        /// <returns>0~9,a~f,A~F いずれか、の文字であれば真</returns>
        private bool IsHexChar(char c)
            => (c >= '0' && c <= '9')
            || (c >= 'A' && c <= 'F')
            || (c >= 'a' && c <= 'f');

        /// <summary>
        /// 2進数を構成する文字であれば真
        /// </summary>
        /// <param name="c"></param>
        /// <returns>0,1 いずれか、の文字であれば真</returns>
        private bool IsBinChar(char c) => (c == '0' || c == '1');

        const int STR_LEN_HEX64 = 16;
        const int STR_LEN_HEX32 = 8;
        const int STR_LEN_HEX16 = 4;
        const int STR_LEN_HEX8 = 2;
        const int STR_LEN_BIN64 = 64;
        const int STR_LEN_BIN32 = 32;
        const int STR_LEN_BIN16 = 16;
        const int STR_LEN_BIN8 = 8;
    }

    /// <summary>
    /// TextBox の BeforeTextChanging イベントを MVVM で扱うための Behavior。
    /// 
    /// WinUI 3 の TextBox.BeforeTextChanging は、テキストが変更される直前に発火する特殊なイベントで、
    /// 通常のバインディングでは扱えないため、この Behavior を使って ViewModel の ICommand に委譲する。
    /// 
    /// この Behavior により、
    /// - 入力値の検証（数値のみ許可など）
    /// - 入力のキャンセル（e.Cancel = true）
    /// - フォーマット調整（全角→半角など）
    /// を ViewModel 側で実装できる。
    /// </summary>
    public class BeforeTextChangingEventBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// BeforeTextChanging 発火時に実行する ICommand。
        /// EventArgs（TextBoxBeforeTextChangingEventArgs）が CommandParameter として渡される。
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// ICommand を保持するための DependencyProperty。
        /// </summary>
        static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(BeforeTextChangingEventBehavior),
                null);

        /// <summary>
        /// Behavior が TextBox にアタッチされたときにイベントを購読する。
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.BeforeTextChanging += BeforeTextChanging;
        }

        /// <summary>
        /// Behavior がデタッチされたときにイベント購読を解除する。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.BeforeTextChanging -= BeforeTextChanging;
        }

        /// <summary>
        /// BeforeTextChanging イベント発火時に ICommand を実行する。
        /// CommandParameter には TextBoxBeforeTextChangingEventArgs が渡される。
        /// </summary>
        private void BeforeTextChanging(TextBox s, TextBoxBeforeTextChangingEventArgs e)
        {
            if (Command?.CanExecute(e) == true)
            {
                Command.Execute(e);
            }
        }
    }
}
