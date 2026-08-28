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

            m_updatingHex = true;

            if (!m_updatingBin)
            {
                Bin = ul.ToString($"B{ value.Length * 4 }");
            }
            if (!m_updatingFloat)
            {
                Float = BitConverter.UInt32BitsToSingle(ui).ToString("R");
            }
            if (!m_updatingDouble)
            {
                Double = BitConverter.UInt64BitsToDouble(ul).ToString("R");
            }
            m_updatingHex = false;
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

            m_updatingBin = true;

            if (!m_updatingHex)
            {
                Hex = ul.ToString($"X{ value.Length / 4 }");
            }
            if (!m_updatingFloat)
            {
                Float = BitConverter.UInt32BitsToSingle(ui).ToString("R");
            }
            if (!m_updatingDouble)
            {
                Double = BitConverter.UInt64BitsToDouble(ul).ToString("R");
            }
            m_updatingBin = false;
        }

        [ObservableProperty]
        public partial string Float { get; set; } = string.Empty;
        partial void OnFloatChanged(string value)
        {
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
            {
                return;
            }
            var ui = BitConverter.SingleToUInt32Bits(f);
            var ul = (UInt64)ui;

            m_updatingFloat = true;

            if (!m_updatingHex)
            {
                Hex = ul.ToString("X8");
            }
            if (!m_updatingBin)
            {
                Bin = ul.ToString("B32");
            }
            if (!m_updatingDouble)
            {
                Double = BitConverter.UInt64BitsToDouble(ul).ToString("R");
            }
            m_updatingFloat = false;
        }

        [ObservableProperty]
        public partial string Double { get; set; } = string.Empty;
        partial void OnDoubleChanged(string value)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
            {
                return;
            }
            var ul = BitConverter.DoubleToUInt64Bits(d);
            var ui = (UInt32)(ul & 0xFFFFFFFF);

            m_updatingDouble = true;

            if (!m_updatingHex)
            {
                Hex = ul.ToString("X16");
            }
            if (!m_updatingBin)
            {
                Bin = ul.ToString("B64");
            }
            if (!m_updatingFloat)
            {
                Float = BitConverter.UInt32BitsToSingle(ui).ToString("R");
            }
            m_updatingDouble = false;
        }

        public ICommand TextBoxHexBeforeTextChangingCommand { get; init; }
        public ICommand TextBoxBinBeforeTextChangingCommand { get; init; }
        public ICommand TextBoxFloatBeforeTextChangingCommand { get; init; }
        public ICommand TextBoxDoubleBeforeTextChangingCommand { get; init; }

        public InspectorPageViewModel()
        {
            TextBoxHexBeforeTextChangingCommand = new RelayCommand<TextBoxBeforeTextChangingEventArgs>(TextBoxHexBeforeTextChangingAction);
            TextBoxBinBeforeTextChangingCommand = new RelayCommand<TextBoxBeforeTextChangingEventArgs>(TextBoxBinBeforeTextChangingAction);
            TextBoxFloatBeforeTextChangingCommand = new RelayCommand<TextBoxBeforeTextChangingEventArgs>(TextBoxFloatBeforeTextChangingAction);
            TextBoxDoubleBeforeTextChangingCommand = new RelayCommand<TextBoxBeforeTextChangingEventArgs>(TextBoxDoubleBeforeTextChangingAction);
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
            if (text.Length > STR_LEN_HEX64)
            {
                e.Cancel = true;
                return;
            }

            // 16進数以外の文字が含まれていたら拒否.
            foreach (char c in text) {
                if (!IsHexChar(c)) {
                    e.Cancel = true;
                    return;
                }
            }
        }

        void TextBoxBinBeforeTextChangingAction(TextBoxBeforeTextChangingEventArgs? e)
        {
            if (e == null)
            {
                return;
            }

            var text = e.NewText.Trim();

            // 0x プレフィックスを許可.
            if (text.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(2);
            }
            if (text.Length > STR_LEN_BIN64)
            {
                e.Cancel = true;
                return;
            }

            // 16進数以外の文字が含まれていたら拒否.
            foreach (char c in text)
            {
                if (!IsBinChar(c))
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        void TextBoxFloatBeforeTextChangingAction(TextBoxBeforeTextChangingEventArgs? e)
        {
            if (e == null)
            {
                return;
            }
            if (!IsFloatText(e.NewText.Trim()))
            {
                e.Cancel = true;
                return;
            }
        }

        void TextBoxDoubleBeforeTextChangingAction(TextBoxBeforeTextChangingEventArgs? e)
        {
            if (e == null)
            {
                return;
            }
            if (!IsFloatText(e.NewText.Trim()))
            {
                e.Cancel = true;
                return;
            }
        }

        static bool IsFloatText(string text)
        {
            var numE = 0;
            var numDot = 0;
            var numMinus = 0;
            var numPlus = 0;
            foreach (char c in text)
            {
                if (IsNanOrInfChar(c))
                {
                    // 非数表記で使うのでスルー.
                    // nan, infinity の表記を許可するため.
                    continue;
                }

                if (c == '.')
                {
                    if (++numDot > 1)
                    {
                        // 小数点が２つもあるので許可しない.
                        return false;
                    }
                }
                else if (c == '-')
                {
                    if (++numMinus > 2)
                    {
                        // マイナス記号が３つもあるので許可しない.
                        // マイナス記号は最大２つのはず. 例えば -2E-7
                        return false;
                    }
                }
                else if (c == '+')
                {
                    if (++numPlus > 2)
                    {
                        // プラス記号が３つもあるので許可しない.
                        // 次の書き方は許可している. +2E+7
                        return false;
                    }
                }
                else if (c == 'e' || c == 'E')
                {
                    if (++numE > 1)
                    {
                        // 指数記号が２つもあるので許可しない.
                        return false;
                    }
                }
                else if (!IsNumberChar(c))
                {
                    return false;
                }
            }

            bool IsNumberChar(char c)
                => ('0' <= c && c <= '9');

            bool IsNanOrInfChar(char c)
                => (c == 'i' || c == 'I')
                || (c == 'n' || c == 'N')
                || (c == 'f' || c == 'F')
                || (c == 't' || c == 'T')
                || (c == 'y' || c == 'Y')
                || (c == 'a' || c == 'A');

            return true;
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

        bool m_updatingHex = false;
        bool m_updatingBin = false;
        bool m_updatingFloat = false;
        bool m_updatingDouble = false;
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
