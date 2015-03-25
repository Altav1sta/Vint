using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vint
{
    /// <summary>
    /// Логика взаимодействия для EnterNickname.xaml
    /// </summary>
    public partial class EnterNickname : Window
    {
        bool nickWasConfirmed = false;

        public EnterNickname()
        {
            InitializeComponent();
            
            
            txtBox.Focus();
            
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {           
            if (e.Key != Key.Enter)
            {
                if (txtBox.Foreground != Brushes.Black)
                {
                    txtBox.Foreground = Brushes.Black;
                    txtBox.FontStyle = FontStyles.Normal;
                    txtBox.Clear();
                }
                return;
            }
            nickWasConfirmed = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!nickWasConfirmed) txtBox.Text = "Player";
            else
            {
                // Убираем пробелы в начале и конце
                string tmp = txtBox.Text;
                txtBox.Text = removeSpaces(tmp);
            }
        }

        private string removeSpaces(string text)
        {
            string s1;
            
            if ((text.Length > 1) && (text[0].Equals(" ")))
            {
                s1 = text.Substring(1);
            }
            else s1 = text;

            string s2;

            if ((s1.Length > 1) && (s1[s1.Length - 1].Equals(" ")))
            {
                s2 = s1.Substring(0, s1.Length - 1);
            }
            else s2 = s1;

            if (!s2.Equals(text)) return removeSpaces(s2);
            else return s2;
        }

    }
}
