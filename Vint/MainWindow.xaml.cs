using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {    
        
            InitializeComponent();

            
        }
        
        // --------- Event Handlers
       
        public async void newRound()
        {
            // Сначала обновляем все на начальные значения
            dealer = (dealer + 1) % 4;
            westHand.isFaced = false;
            northHand.isFaced = false;
            eastHand.isFaced = false;

            this.Title = "Винт";
            roundCompleted = false;
            contractWasConfirmed = false;
            miUndo.IsEnabled = false;
            miNew.IsEnabled = false;
            miHide.IsEnabled = true;
            getLabel(curPlayer).Foreground = Brushes.Black;
            getLabel(curPlayer).FontSize = 20;
            miHide.Header = "Показать карты";
            lblTaken.Visibility = Visibility.Hidden;
            lblLost.Visibility = Visibility.Hidden;

            for (int i = -1; i < 4; i++)
            {
                clearDeckSkins(getHand(i));
                getHand(i).clear();
            }

            cnvTable.Children.Clear();
            clearDeckSkins(takenCards);
            clearDeckSkins(lostCards);

            //tableCards = new Card[4];
            takenCards.clear();
            lostCards.clear();
            takenHighestCards.clear();
            lostHighestCards.clear();
            lblTaken.Content = "Взятки: 0";
            lblLost.Content = "Лузы: 0";

            myAcesCount = 0;
            enemyAcesCount = 0;
            myOnnersCount = 0;
            enemyOnnersCount = 0;

            // Раздача
            mainDeck.fill();
            mainDeck.reshuffle();

            drawDeckSkins(mainDeck);

            await (Task.Run(() => dealCards()));



            // Торги
            contractNominal = 0;
            contractSuit = null;
            curPlayer = dealer;

            ChooseContract c = new ChooseContract();
            c.Owner = this;

            wBot = new Bot(westHand, this, c);
            nBot = new Bot(northHand, this, c);
            eBot = new Bot(eastHand, this, c);
            sBot = new Bot(myHand, this, c);

            c.expSouth.Header = lblSouth.Content;

            c.ShowDialog();
            if (!c.contractWasConfirmed)
            {
                if (c.exitRequired) this.Close();
                else miNew_Click(null, new RoutedEventArgs());
                return;
            }

            contractWasConfirmed = true;
            bolvan = (contractPerformer + 2) % 4;
            if ((bolvan == 2) && !getHand(bolvan).isFaced) overturnDeck(getHand(bolvan));

            // Сообщаем о взятом контракте в заголовке окна
            if (contractSuit != null)
                this.Title += "   [Контракт: " + contractNominal.ToString() + (new Card(Nominal.Two, (Suit)contractSuit)).CardValueToString()[1] + ",  Выполняет: " + getLabel(contractPerformer).Content + "]";
            else
                this.Title += "   [Контракт: " + contractNominal.ToString() + "БК,  Выполняет: " + getLabel(contractPerformer).Content + "]";


            // Начинается розыгрыш
            curPlayer = (contractPerformer + 1) % 4;
            if ((curPlayer == 0) && (bolvan != 0) || (curPlayer == 2) && (bolvan == 2)) bindClickEvents(getHand(curPlayer));
            miNew.IsEnabled = true;


            firstPlayer = (contractPerformer + 1) % 4;
            getLabel(firstPlayer).Foreground = Brushes.Yellow;
            if (firstPlayer == 0) getLabel(firstPlayer).FontSize = 30;
            else getLabel(firstPlayer).FontSize = 36;
            
            if (!((curPlayer == 0) && (bolvan != 0) || (curPlayer == 2) && (bolvan == 2)))
                getBot(curPlayer).playerMove();
        }

        public void btnHide_Click(object sender, RoutedEventArgs e)
        {
            if (isShown)
                miHide.Header = "Показать карты";
            else
                miHide.Header = "Скрыть карты";

            isShown = !isShown;

            for (int i = 1; i < 4; i++)
            {
                if ((contractWasConfirmed && (!getHand(i).isFaced || (getHand(i).isFaced && ((i != 2) || (i != bolvan)) && ((getHand(contractPerformer).Count == 13) || (i != bolvan))))) || !contractWasConfirmed)
                {
                    overturnDeck(getHand(i));
                    if (curPlayer == i)
                    {
                        unbindClickEvents(getHand(i));
                        bindClickEvents(getHand(i));
                    }
                }
            }
            
        }

        public void handCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            Deck k = getHand(sender as Canvas);

            if (firstPlayer == k.side)
            {
                lblTaken.Visibility = Visibility.Visible;
                lblLost.Visibility = Visibility.Visible;

                curSuit = k.getCard(sender as Canvas).suit;
            }
            else
            {
                if (cardIsProperToMove(k.getCard(sender as Canvas), k) == false) return;
            }

            getLabel(curPlayer).Foreground = Brushes.Black;
            getLabel(curPlayer).FontSize = 20;
            miUndo.IsEnabled = false;

            clearDeckSkins(k);
            tableCards[k.side] = k.extract(sender as Canvas);
            drawDeckSkins(k);

            switch (k.side)
            {
                case 0:
                    tableCards[k.side].left = 89;
                    tableCards[k.side].top = 153;
                    break;
                case 1:
                    tableCards[k.side].left = 0;
                    tableCards[k.side].top = 89;
                    break;
                case 2:
                    tableCards[k.side].left = 89;
                    tableCards[k.side].top = 0;
                    break;
                case 3:
                    tableCards[k.side].left = 153;
                    tableCards[k.side].top = 89;
                    break;
                default:
                    tableCards[k.side].left = 0;
                    tableCards[k.side].top = 0;
                    break;
            }

            tableCards[k.side].skin = tableCards[k.side].getSkin();
            cnvTable.Children.Add(tableCards[k.side].skin);


            unbindClickEvents(k);

            curPlayer = (k.side + 1) % 4;
            
            if (possibleToUndo()) miUndo.IsEnabled = true;
            

            if (firstPlayer == curPlayer)
            {
                if (getHand(curPlayer).Count == 0) miHide.IsEnabled = false;
                if ((firstPlayer == 1) || (firstPlayer == 3) && (bolvan == 2)) canClearTableWithMouseClick = false;
                roundCompleted = true;                
            }
            else
            {
                if ((curPlayer == 0) && (bolvan != 0) || (curPlayer == 2) && (bolvan == 2)) bindClickEvents(getHand(curPlayer));

                getLabel(curPlayer).Foreground = Brushes.Yellow;
                if (curPlayer == 0) getLabel(curPlayer).FontSize = 30;
                else getLabel(curPlayer).FontSize = 36;

                if (!(((curPlayer == 0) && (bolvan != 0)) || ((curPlayer == 2) && (bolvan == 2))))
                    getBot(curPlayer).playerMove();
            }

        }

        public void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (!roundCompleted) return;

                    // В начале второго раунда карты болвана открываются для врагов
                    if ((bolvan != 0) && (!getHand(bolvan).isFaced)) overturnDeck(getHand(bolvan));

                    miUndo.IsEnabled = false;

                    clearTable();
                    roundCompleted = false;
                    if (myHand.Count == 0)
                    {
                        RoundResult rr = new RoundResult();
                        rr.Owner = this;
                                                
                        if ((MyLeve >= 500) || (EnemyLeve >= 500))
                        {
                            if (part == 1)
                            {
                                if (MyLeve > EnemyLeve) MyOnners += 1000;
                                else EnemyOnners += 1000;
                            }
                            else
                            {
                                if (MyLeve > EnemyLeve) MyOnners += 2000;
                                else EnemyOnners += 2000;
                            }

                            MyLeve = 0;
                            EnemyLeve = 0;
                            part++;
                        }


                        rr.txtOnnersMy.Text = MyOnners.ToString();
                        rr.txtAcesMy.Text = MyAces.ToString();
                        rr.txtCrownsMy.Text = MyCrowns.ToString();
                        rr.txtBonusMy.Text = MyBonus.ToString();
                        rr.txtLeveMy.Text = MyLeve.ToString();

                        rr.txtOnnersEnemy.Text = EnemyOnners.ToString();
                        rr.txtAcesEnemy.Text = EnemyAces.ToString();
                        rr.txtCrownsEnemy.Text = EnemyCrowns.ToString();
                        rr.txtBonusEnemy.Text = EnemyBonus.ToString();
                        rr.txtLeveEnemy.Text = EnemyLeve.ToString();


                        if (part < 3)
                        {
                            rr.ShowDialog();
                        }
                        else
                        {
                            rt.btnWinner.IsEnabled = false;

                            switch (robber)
                            {
                                case 1:
                                    rt.txtOnnersMy1.Text = MyOnners.ToString();
                                    rt.txtAcesMy1.Text = MyAces.ToString();
                                    rt.txtCrownsMy1.Text = MyCrowns.ToString();
                                    rt.txtBonusMy1.Text = MyBonus.ToString();
                                    rt.txtLeveMy1.Text = MyLeve.ToString();

                                    rt.txtOnnersEnemy1.Text = EnemyOnners.ToString();
                                    rt.txtAcesEnemy1.Text = EnemyAces.ToString();
                                    rt.txtCrownsEnemy1.Text = EnemyCrowns.ToString();
                                    rt.txtBonusEnemy1.Text = EnemyBonus.ToString();
                                    rt.txtLeveEnemy1.Text = EnemyLeve.ToString();

                                    break;

                                case 2:
                                    rt.txtOnnersMy2.Text = MyOnners.ToString();
                                    rt.txtAcesMy2.Text = MyAces.ToString();
                                    rt.txtCrownsMy2.Text = MyCrowns.ToString();
                                    rt.txtBonusMy2.Text = MyBonus.ToString();
                                    rt.txtLeveMy2.Text = MyLeve.ToString();

                                    rt.txtOnnersEnemy2.Text = EnemyOnners.ToString();
                                    rt.txtAcesEnemy2.Text = EnemyAces.ToString();
                                    rt.txtCrownsEnemy2.Text = EnemyCrowns.ToString();
                                    rt.txtBonusEnemy2.Text = EnemyBonus.ToString();
                                    rt.txtLeveEnemy2.Text = EnemyLeve.ToString();

                                    break;

                                case 3:
                                    rt.txtOnnersMy3.Text = MyOnners.ToString();
                                    rt.txtAcesMy3.Text = MyAces.ToString();
                                    rt.txtCrownsMy3.Text = MyCrowns.ToString();
                                    rt.txtBonusMy3.Text = MyBonus.ToString();
                                    rt.txtLeveMy3.Text = MyLeve.ToString();

                                    rt.txtOnnersEnemy3.Text = EnemyOnners.ToString();
                                    rt.txtAcesEnemy3.Text = EnemyAces.ToString();
                                    rt.txtCrownsEnemy3.Text = EnemyCrowns.ToString();
                                    rt.txtBonusEnemy3.Text = EnemyBonus.ToString();
                                    rt.txtLeveEnemy3.Text = EnemyLeve.ToString();

                                    canChangeDB = true;
                                    rt.btnWinner.IsEnabled = true;

                                break;
                            }

                            ResultTable rt2 = new ResultTable(rt);
                            rt2.Owner = this;
                            rt2.ShowDialog();


                            robber++;
                            part = 1;

                            MyOnners = 0;
                            MyAces = 0;
                            MyCrowns = 0;
                            MyBonus = 0;
                            MyLeve = 0;

                            EnemyOnners = 0;
                            EnemyAces = 0;
                            EnemyCrowns = 0;
                            EnemyBonus = 0;
                            EnemyLeve = 0;
                        }

                        if (robber == 4) return;
                        newRound();
                    }
                    else
                    {
                        getLabel(firstPlayer).Foreground = Brushes.Yellow;
                        if (firstPlayer == 0) getLabel(firstPlayer).FontSize = 30;
                        else getLabel(firstPlayer).FontSize = 36;

                        bindClickEvents(getHand(firstPlayer));
                        curPlayer = firstPlayer;
                        if (!(((curPlayer == 0) && (bolvan != 0)) || ((curPlayer == 2) && (bolvan == 2))))
                            getBot(curPlayer).playerMove();
                    }
                    break;

                case Key.Back:
                    if (possibleToUndo()) btnUndo_Click(null, new RoutedEventArgs());
                    break;

                case (Key.N):
                    miNew_Click(null, new RoutedEventArgs());
                    break;

                case (Key.Escape):
                    if (MessageBox.Show("Вы действительно хотите выйти из игры?",
                            "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        this.Close();
                    break;

                case (Key.Space):
                    if (miHide.IsEnabled) btnHide_Click(null, new RoutedEventArgs());
                    break;

                default:
                    return;

            }

        }

        public void btnUndo_Click(object sender, RoutedEventArgs e)
        // В этом методе рассмотрены все варианты отката хода игрока.
        // Все зависит от того, какая из сторон ходила первой и
        // управляет ли ходом болвана игрок
        {            
            roundCompleted = false;
            getLabel(curPlayer).Foreground = Brushes.Black;
            getLabel(curPlayer).FontSize = 20;

            if (firstPlayer == 0)
            {
                if (bolvan == 2)
                {
                    int counter = cnvTable.Children.Count;
                    for (int i = counter - 1; i >= counter - 2; i--)
                    {
                        clearDeckSkins(getHand(i));
                        cnvTable.Children.Remove(tableCards[i].skin);
                        getHand(i).push(tableCards[i]);
                        tableCards[i] = null;
                        getHand(i).isSorted = false;
                        getHand(i).sort();
                        drawDeckSkins(getHand(i));
                    }
                    unbindClickEvents(getHand(curPlayer));
                    curPlayer = counter - 2;
                }
                else
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        clearDeckSkins(getHand(i));
                        cnvTable.Children.Remove(tableCards[i].skin);
                        getHand(i).push(tableCards[i]);
                        tableCards[i] = null;
                        getHand(i).isSorted = false;
                        getHand(i).sort();
                        drawDeckSkins(getHand(i));
                    }
                    unbindClickEvents(getHand(curPlayer));
                    curPlayer = 0;
                }
            }

            if (firstPlayer == 1)
            {
                if ((bolvan == 2) && (cnvTable.Children.Count == 3))
                {
                    for (int i = 3; i >= 2; i--)
                    {
                        clearDeckSkins(getHand(i));
                        cnvTable.Children.Remove(tableCards[i].skin);
                        getHand(i).push(tableCards[i]);
                        tableCards[i] = null;
                        getHand(i).isSorted = false;
                        getHand(i).sort();
                        drawDeckSkins(getHand(i));
                    }
                    unbindClickEvents(myHand);
                    curPlayer = bolvan;
                }
                else
                {
                    clearDeckSkins(myHand);
                    cnvTable.Children.Remove(tableCards[0].skin);
                    myHand.push(tableCards[0]);
                    tableCards[0] = null;
                    myHand.isSorted = false;
                    myHand.sort();
                    drawDeckSkins(myHand);

                    curPlayer = 0;
                }
            }

            if (firstPlayer == 2)
            {
                int counter = cnvTable.Children.Count;
                for (int i = 5 - counter; i >= 4 - counter; i--)
                {
                    clearDeckSkins(getHand(i));
                    cnvTable.Children.Remove(tableCards[i].skin);
                    getHand(i).push(tableCards[i]);
                    tableCards[i] = null;
                    getHand(i).isSorted = false;
                    getHand(i).sort();
                    drawDeckSkins(getHand(i));
                }
                unbindClickEvents(getHand(curPlayer));
                curPlayer = 4 - counter;
            }

            if (firstPlayer == 3)
            {
                if (bolvan == 2)
                {
                    int counter = cnvTable.Children.Count;
                    for (int i = counter - 2; i >= counter - 2 - (counter % 2); i--)
                    {
                        clearDeckSkins(getHand(i));
                        cnvTable.Children.Remove(tableCards[i].skin);
                        getHand(i).push(tableCards[i]);
                        tableCards[i] = null;
                        getHand(i).isSorted = false;
                        getHand(i).sort();
                        drawDeckSkins(getHand(i));
                    }
                    unbindClickEvents(getHand(curPlayer));
                    curPlayer = counter - 2 - (counter % 2);
                }
                else
                {
                    for (int i = 2; i >= 0; i--)
                    {
                        clearDeckSkins(getHand(i));
                        cnvTable.Children.Remove(tableCards[i].skin);
                        getHand(i).push(tableCards[i]);
                        tableCards[i] = null;
                        getHand(i).isSorted = false;
                        getHand(i).sort();
                        drawDeckSkins(getHand(i));
                    }
                    unbindClickEvents(getHand(curPlayer));
                    curPlayer = 0;
                }
            }

            bindClickEvents(getHand(curPlayer));
            if (!(((curPlayer == 0) && (bolvan != 0)) || ((curPlayer == 2) && (bolvan == 2))))
                getBot(curPlayer).playerMove();
            if (!possibleToUndo()) miUndo.IsEnabled = false;

            getLabel(curPlayer).Foreground = Brushes.Yellow;
            if (curPlayer == 0) getLabel(curPlayer).FontSize = 30;
            else getLabel(curPlayer).FontSize = 36;
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Клик не должен обрабатываться в том случае, если последний ход за игроком,
            // т.к. клик по карте будет расцениваться как сигнал сбора карт            
            if (!canClearTableWithMouseClick)
            {
                canClearTableWithMouseClick = true;
                return;
            }

            MainWin_KeyDown(null, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter));
        }

        private void miManual_Click(object sender, RoutedEventArgs e)
        {
            Manual manual = new Manual();
            manual.Owner = this;
            manual.Show();
        }

        private void miNew_Click(object sender, RoutedEventArgs e)
        {
            EnterNickname en = new EnterNickname();
            en.Owner = this;
            en.ShowDialog();
            lblSouth.Content = en.txtBox.Text;

            rt = new ResultTable();
            rt.lblName1.Content = en.txtBox.Text + " и Бот 1";
            rt.lblName2.Content = en.txtBox.Text + " и Бот 2";
            rt.lblName3.Content = en.txtBox.Text + " и Бот 3";
            rt.Owner = this;

            robber = 1;
            part = 1;

            Random r = new Random();
            dealer = r.Next(0, 4);
                       
            newRound();
        }

        private void miControls_Click(object sender, RoutedEventArgs e)
        {
            ControlsWindow cw = new ControlsWindow();
            cw.Owner = this;
            cw.Show();
        }

        private void miStat_Click(object sender, RoutedEventArgs e)
        {
            Statistic s = new Statistic();
            s.Owner = this;
            s.Show();
        }

        private void miScore_Click(object sender, RoutedEventArgs e)
        {
            ResultTable r = new ResultTable(rt);
            r.Height = r.Height - 30;
            r.Owner = this;
            r.btnWinner.Visibility = System.Windows.Visibility.Hidden;
            r.Show();
        }
        
    }
}
