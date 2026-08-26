using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BitInspectorTabo.Controls
{
    public sealed partial class NumberBoxHex : TextBox
    {
        public NumberBoxHex()
        {
            DefaultStyleKey = typeof(NumberBoxHex);
            KeyDown += KeyDownFunc;
            BeforeTextChanging += BeforeTextChangingFunc;
        }

        private void BeforeTextChangingFunc(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            string text = args.NewText;

            // 0x プレフィックスを許可.
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                text = text.Substring(2);
            }

            // 16進数以外の文字が含まれていたら拒否.
            foreach (char c in text) {
                if (!IsHexChar(c)) {
                    args.Cancel = true;
                    return;
                }
            }
        }

        private void KeyDownFunc(object sender, KeyRoutedEventArgs e)
        {
            // バックスペース、Delete、矢印キーなどは許可.
            if (e.Key is Windows.System.VirtualKey.Back
                or Windows.System.VirtualKey.Delete
                or Windows.System.VirtualKey.Left
                or Windows.System.VirtualKey.Right
                or Windows.System.VirtualKey.Tab) {
                return;
            }

            if (e.Key is Windows.System.VirtualKey.Enter) {
                ConfirmValue();
                return;
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

        public void ConfirmValue()
        {
            var value = Text.Trim();

            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                value = value.Substring(2);
            }
            if (value.Length > STR_LEN_HEX64) {
                // オーバーフローなので下位64bitを切り抜く.
                value = value.Substring(value.Length - STR_LEN_HEX64);
            }

            var len = value.Length;
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

            Text = value.PadLeft(len, '0');
        }
        const int STR_LEN_HEX64 = 16;
        const int STR_LEN_HEX32 = 8;
        const int STR_LEN_HEX16 = 4;
        const int STR_LEN_HEX8 = 2;
    }
}
