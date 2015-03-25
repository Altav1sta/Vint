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
using System.Data;
using System.Data.OleDb;

namespace Vint
{
    /// <summary>
    /// Логика взаимодействия для Statistic.xaml
    /// </summary>
    public partial class Statistic : Window
    {
        public Statistic()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OleDbConnection con = new OleDbConnection(string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\..\..\sources\statistic.mdb", Environment.CurrentDirectory));
            con.Open();

            OleDbDataAdapter da = new OleDbDataAdapter("SELECT nick,wins,games,winrate,averageScore FROM players", con);
            DataSet ds = new DataSet();
            da.Fill(ds, "players");

            dg1.ItemsSource = ds.Tables["players"].DefaultView;
            dg1.IsReadOnly = true;
            con.Close();
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        }
                
    }
}
