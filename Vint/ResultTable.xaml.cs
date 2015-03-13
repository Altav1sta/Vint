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
    /// Логика взаимодействия для ResultTable.xaml
    /// </summary>
    public partial class ResultTable : Window
    {
        public ResultTable()
        {
            InitializeComponent();
        }

        public ResultTable(ResultTable r)
        {
            InitializeComponent();

            if (r.btnWinner.IsEnabled) btnWinner.IsEnabled = true;
            else btnWinner.IsEnabled = false;

            txtOnnersMy1.Text = r.txtOnnersMy1.Text;
            txtAcesMy1.Text = r.txtAcesMy1.Text;
            txtCrownsMy1.Text = r.txtCrownsMy1.Text;
            txtBonusMy1.Text = r.txtBonusMy1.Text;
            txtLeveMy1.Text = r.txtLeveMy1.Text;

            txtOnnersEnemy1.Text = r.txtOnnersEnemy1.Text;
            txtAcesEnemy1.Text = r.txtAcesEnemy1.Text;
            txtCrownsEnemy1.Text = r.txtCrownsEnemy1.Text;
            txtBonusEnemy1.Text = r.txtBonusEnemy1.Text;
            txtLeveEnemy1.Text = r.txtLeveEnemy1.Text;


            txtOnnersMy2.Text = r.txtOnnersMy2.Text;
            txtAcesMy2.Text = r.txtAcesMy2.Text;
            txtCrownsMy2.Text = r.txtCrownsMy2.Text;
            txtBonusMy2.Text = r.txtBonusMy2.Text;
            txtLeveMy2.Text = r.txtLeveMy2.Text;

            txtOnnersEnemy2.Text = r.txtOnnersEnemy2.Text;
            txtAcesEnemy2.Text = r.txtAcesEnemy2.Text;
            txtCrownsEnemy2.Text = r.txtCrownsEnemy2.Text;
            txtBonusEnemy2.Text = r.txtBonusEnemy2.Text;
            txtLeveEnemy2.Text = r.txtLeveEnemy2.Text;

            txtOnnersMy3.Text = r.txtOnnersMy3.Text;
            txtAcesMy3.Text = r.txtAcesMy3.Text;
            txtCrownsMy3.Text = r.txtCrownsMy3.Text;
            txtBonusMy3.Text = r.txtBonusMy3.Text;
            txtLeveMy3.Text = r.txtLeveMy3.Text;

            txtOnnersEnemy3.Text = r.txtOnnersEnemy3.Text;
            txtAcesEnemy3.Text = r.txtAcesEnemy3.Text;
            txtCrownsEnemy3.Text = r.txtCrownsEnemy3.Text;
            txtBonusEnemy3.Text = r.txtBonusEnemy3.Text;
            txtLeveEnemy3.Text = r.txtLeveEnemy3.Text;
        }

        private void btnWinner_Click(object sender, RoutedEventArgs e)
        {
            int myPoints = 0;
            int bot1Points = 0;
            int bot2Points = 0;
            int bot3Points = 0;

            
            try
            {
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtOnnersMy1.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtAcesMy1.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtCrownsMy1.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtBonusMy1.Text);

                myPoints += Int32.Parse((Owner as MainWindow).rt.txtOnnersMy2.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtAcesMy2.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtCrownsMy2.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtBonusMy2.Text);

                myPoints += Int32.Parse((Owner as MainWindow).rt.txtOnnersMy3.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtAcesMy3.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtCrownsMy3.Text);
                myPoints += Int32.Parse((Owner as MainWindow).rt.txtBonusMy3.Text);



                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersMy1.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtAcesMy1.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsMy1.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtBonusMy1.Text);

                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersEnemy2.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtAcesEnemy2.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsEnemy2.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtBonusEnemy2.Text);

                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersEnemy3.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtAcesEnemy3.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsEnemy3.Text);
                bot1Points += Int32.Parse((Owner as MainWindow).rt.txtBonusEnemy3.Text);



                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersEnemy1.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtAcesEnemy1.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsEnemy1.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtBonusEnemy1.Text);

                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersMy2.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtAcesMy2.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsMy2.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtBonusMy2.Text);

                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersEnemy3.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtAcesEnemy3.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsEnemy3.Text);
                bot2Points += Int32.Parse((Owner as MainWindow).rt.txtBonusEnemy3.Text);



                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersEnemy1.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtAcesEnemy1.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsEnemy1.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtBonusEnemy1.Text);

                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersEnemy2.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtAcesEnemy2.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsEnemy2.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtBonusEnemy2.Text);

                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtOnnersMy3.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtAcesMy3.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtCrownsMy3.Text);
                bot3Points += Int32.Parse((Owner as MainWindow).rt.txtBonusMy3.Text);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Not a number!");
            }



            int max = myPoints;
            int num = 0;

            if (bot1Points > max)
            {
                max = bot1Points;
                num = 1;
            }
            if (bot1Points > max)
            {
                max = bot1Points;
                num = 2;
            }
            if (bot1Points > max)
            {
                max = bot1Points;
                num = 3;
            }

            string winner;
            switch (num)
            {
                case 0:
                    winner = (Owner as MainWindow).lblSouth.Content.ToString();
                    break;
                case 1:
                    winner = "Бот 1";
                    break;
                case 2:
                    winner = "Бот 2";
                    break;
                case 3:
                    winner = "Бот 3";
                    break;
                default:
                    winner = "-";
                    break;
            }

            WinnerAnnouncer wa = new WinnerAnnouncer();
            wa.Owner = this;
            wa.Title += winner;
            wa.lblNick.Content = (Owner as MainWindow).lblSouth.Content;
            wa.lbl0.Content = myPoints.ToString();
            wa.lbl1.Content = bot1Points.ToString();
            wa.lbl2.Content = bot2Points.ToString();
            wa.lbl3.Content = bot3Points.ToString();
            wa.ShowDialog();
        }

    }
}
