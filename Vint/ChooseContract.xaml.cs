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
    /// Логика взаимодействия для ChooseContract.xaml
    /// </summary>
    public partial class ChooseContract : Window
    {
        private Button[,] btnArr = new Button[7, 5];
        public bool contractWasConfirmed = false;
        public bool exitRequired = false;


        public ChooseContract()
        {
            InitializeComponent();
        }
        
        private string getContent(int n, int s)
        {
            string nominal = String.Format("{0}", n+1);
            string suit;
            switch (s)
            {
                case 0:
                    suit = "♠";
                    break;
                case 1:
                    suit = "♣";
                    break;
                case 2:
                    suit = "♦";
                    break;
                case 3:
                    suit = "♥";
                    break;
                default:
                    suit = "БК";
                    break;
            }
            return nominal + " " + suit;
        }
                
        public string turnResult(int side, int num)
        // num означает номер позиции с конца
        {
            string text;

            switch (side)
            {
                case 0:
                    text = lblExpSouth.Content as string;
                    break;
                case 1:
                    text = lblExpWest.Content as string;
                    break;
                case 2:
                    text = lblExpNorth.Content as string;
                    break;
                case 3:
                    text = lblExpEast.Content as string;
                    break;
                default:
                    text = "";
                    break;
            }
            if (text == "") return "";
                        
            string ans;
            if (num == 2)
            {
                ans = text.Remove(text.LastIndexOf("\n"));
            }
            else
            {
                ans = text;
            }

            if (ans == "") return "";
            else return ans.Substring(ans.LastIndexOf("\n") + 1);
        }

        public Label getLabel(int side)
        {
            switch (side)
            {
                case 0:
                    return lblExpSouth;
                case 1:
                    return lblExpWest;
                case 2:
                    return lblExpNorth;
                case 3:
                    return lblExpEast;
                default:
                    return null;
            }
        }

        private void trading()
        {
            while (true)
            {
                if ((turnResult(0, 2) == "ПАС") && (turnResult(0, 1) == "ПАС") && (turnResult(1, 2) == "ПАС") && (turnResult(1, 1) == "ПАС") &&
                      (turnResult(2, 2) == "ПАС") && (turnResult(2, 1) == "ПАС") && (turnResult(3, 2) == "ПАС") && (turnResult(3, 1) == "ПАС"))
                {
                    contractWasConfirmed = true;

                    if ((Owner as MainWindow).contractSuit != null)
                        MessageBox.Show("Назначен контракт: " + (Owner as MainWindow).contractNominal + (new Card(Nominal.Two, (Suit)((Owner as MainWindow).contractSuit))).CardValueToString()[1]);
                    else
                        MessageBox.Show("Назначен контракт: " + (Owner as MainWindow).contractNominal + "БК");
                    
                    Close();
                    return;
                }

                if ((Owner as MainWindow).curPlayer == 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            btnArr[i, j].IsEnabled = true;

                            if (i + 1 < (Owner as MainWindow).contractNominal)
                            {
                                if (btnArr[i, j].Foreground == Brushes.Red) btnArr[i, j].Foreground = Brushes.Pink;
                                btnArr[i, j].IsEnabled = false;
                            }
                            else
                            {
                                if ((i + 1 == (Owner as MainWindow).contractNominal) && (((Owner as MainWindow).contractSuit == null) || ((int)((Owner as MainWindow).contractSuit) >= j)))
                                {
                                    if (btnArr[i, j].Foreground == Brushes.Red) btnArr[i, j].Foreground = Brushes.Pink;
                                    btnArr[i, j].IsEnabled = false;
                                }
                                else
                                    btnArr[i, j].IsEnabled = true;
                            }
                        }
                    }
                    return;
                }
                else
                {
                    (Owner as MainWindow).getBot((Owner as MainWindow).curPlayer).ChooseContract();
                    (Owner as MainWindow).curPlayer = ((Owner as MainWindow).curPlayer + 1) % 4;
                    //trading();
                }
            }
        }


        // EventHandlers


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Заполняем окно кнопками выбора контракта
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    btnArr[i, j] = new Button()
                    {
                        Width = 60,
                        Height = 30,
                        Margin = new Thickness(j * 60, i * 30, 0, 0),
                        Content = getContent(i, j),
                        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center
                    };
                    if ((j == 2) || (j == 3)) btnArr[i, j].Foreground = Brushes.Red;
                    btnArr[i, j].Click += new System.Windows.RoutedEventHandler(Button_Click);

                    mainArea.Children.Add(btnArr[i, j]);
                }
            }
            Button btnFold = new Button()
                {
                    Width = 300,
                    Height = 30,
                    Margin = new Thickness(0, 210, 0, 0),
                    Content = "ПАС",
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center

                };
            btnFold.Click += new System.Windows.RoutedEventHandler(Button_Click);
            mainArea.Children.Add(btnFold);

            trading();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lblExpSouth.Content += "\n" + (sender as Button).Content;

            if ((sender as Button).Content.ToString() != "ПАС")
            {
                (Owner as MainWindow).contractPerformer = 0;
                (Owner as MainWindow).contractNominal = Int32.Parse((sender as Button).Content.ToString().Substring(0, 1));
                string tmp = (sender as Button).Content.ToString().Substring(2);
                switch (tmp)
                {
                    case "♠":
                        (Owner as MainWindow).contractSuit = Suit.Spades;
                        break;
                    case "♣":
                        (Owner as MainWindow).contractSuit = Suit.Clubs;
                        break;
                    case "♦":
                        (Owner as MainWindow).contractSuit = Suit.Diamonds;
                        break;
                    case "♥":
                        (Owner as MainWindow).contractSuit = Suit.Hearts;
                        break;
                    default:
                        (Owner as MainWindow).contractSuit = null;
                        break;
                }
            }

            (Owner as MainWindow).curPlayer = ((Owner as MainWindow).curPlayer + 1) % 4;
            trading();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (contractWasConfirmed) return;

            switch (MessageBox.Show("Вы не закончили торги.\nНажмите на клавишу \"Да\", чтобы выйти из игры, \"Нет\" - для того, чтобы начать новую игру. \n\nЗавершить игру?",
                "Внимание!", MessageBoxButton.YesNoCancel))
            {
                case MessageBoxResult.No: 
                    e.Cancel = false;
                    break;
                case MessageBoxResult.Yes:
                    e.Cancel = false;
                    exitRequired = true;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }
    }
}
