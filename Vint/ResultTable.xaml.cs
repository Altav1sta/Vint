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
using System.Data.OleDb;
using System.Data;

namespace Vint
{
    /// <summary>
    /// Логика взаимодействия для ResultTable.xaml
    /// </summary>
    public partial class ResultTable : Window
    {

        private int myPoints = 0;
        private int bot1Points = 0;
        private int bot2Points = 0;
        private int bot3Points = 0;
        private string winner;
        private int num;

        public ResultTable()
        {
            InitializeComponent();
        }

        public ResultTable(ResultTable r)
        {
            InitializeComponent();

            lblName1.Content = r.lblName1.Content;
            lblName2.Content = r.lblName2.Content;
            lblName3.Content = r.lblName3.Content;

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

        private void changeStatistic()
        {
            OleDbConnection con = new OleDbConnection(string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\..\..\sources\statistic.mdb", Environment.CurrentDirectory));
            for (int i = 0; i < 4; i++)
            {
                int points = 0;
                string nickname;
                switch (i)
                {
                    case 0:
                        points = myPoints;
                        nickname = (Owner as MainWindow).lblSouth.Content.ToString();
                        break;
                    case 1:
                        points = bot1Points;
                        nickname = "Бот 1";
                        break;
                    case 2:
                        points = bot2Points;
                        nickname = "Бот 2";
                        break;
                    case 3:
                        points = bot3Points;
                        nickname = "Бот 3";
                        break;
                    default:
                        nickname = "?";
                        break;
                }


                OleDbCommand oc = new OleDbCommand("SELECT count(*) FROM players WHERE nick = '" + nickname + "'", con);
                con.Open();
                int count = Convert.ToInt16(oc.ExecuteScalar());
                con.Close();

                if (count > 0)
                {
                    con.Open();
                    string sqlStr;
                    if (i == num)
                        sqlStr = "UPDATE players SET winrate = (wins + 1) / (games + 1) * 100, wins = wins + 1, averageScore = (averageScore * games + " + points + ") / (games + 1), games = games + 1 WHERE nick = '" + nickname + "'";
                    else
                        sqlStr = "UPDATE players SET winrate = wins / (games + 1) * 100, averageScore = (averageScore * games + " + points + ") / (games + 1), games = games + 1 WHERE nick = '" + nickname + "'";
                    OleDbCommand cmd = new OleDbCommand(sqlStr, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    con.Open();
                    string sqlStr;
                    if (i == num)
                        sqlStr = "INSERT INTO players (nick, wins, games, winrate, averageScore) VALUES ('" + nickname + "', 1, 1, 100, " + points + ")";
                    else
                        sqlStr = "INSERT INTO players (nick, wins, games, winrate, averageScore) VALUES ('" + nickname + "', 0, 1, 0, " + points + ")";
                    OleDbCommand cmd = new OleDbCommand(sqlStr, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        private void btnWinner_Click(object sender, RoutedEventArgs e)
        {        

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Будем узнавать победителя и менять статистику, только если это 3 роббер и мы этого еще не сделали 

            if (!(Owner as MainWindow).canChangeDB) return;

            // Считаем очки
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


            // Выявляем победителя
            int max = myPoints;

            if (bot1Points > max)
            {
                max = bot1Points;
                num = 1;
            }
            if (bot2Points > max)
            {
                max = bot2Points;
                num = 2;
            }
            if (bot3Points > max)
            {
                max = bot3Points;
                num = 3;
            }

            // Имя победителя
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


            changeStatistic();
            (Owner as MainWindow).canChangeDB = false;
        }

    }
}
