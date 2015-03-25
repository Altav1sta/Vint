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
    public partial class MainWindow : Window
    {
        private Deck mainDeck = new Deck(319, 268, 0, false, -1);
        private Deck myHand = new Deck(340, 460, 0, true, 0);
        private Deck westHand = new Deck(10, 300, 90, false, 1);
        private Deck northHand = new Deck(340, 34, 0, false, 2);
        private Deck eastHand = new Deck(583, 238, -90, false, 3);
        public Card[] tableCards = new Card[4];
        public Deck lostCards = new Deck(150, 350, 90, false, -1);
        public Deck takenCards = new Deck(470, 347, 0, false, -1);
        public Deck takenHighestCards = new Deck(470, 347, 0, false, -1);
        public Deck lostHighestCards = new Deck(470, 347, 0, false, -1);
        


        private void overturnDeck(Deck k)
        {
            clearDeckSkins(k);
            k.overturn();
            drawDeckSkins(k);
        }

        private void clearDeckSkins(Deck k)
        {
            foreach (Card card in k)
            {
                if (canvasMainArea.Children.Contains(card.skin))
                    canvasMainArea.Children.Remove(card.skin);
            }
        }

        private void drawDeckSkins(Deck k)
        {
            foreach (Canvas c in k.getSkins())
                canvasMainArea.Children.Add(c);
        }

        private void moveCard(Deck a, Deck b)
        {
            if (a.Count != 0)
            {
                Action act = () =>
                {
                    clearDeckSkins(b);
                    clearDeckSkins(a);

                    b.push(a.pop());
                    b.isSorted = false;
                    b.sort();

                    drawDeckSkins(a);
                    drawDeckSkins(b);
                };
                canvasMainArea.Dispatcher.Invoke(act);

            }
        }

        private void dealCards()
        {
            int counter = dealer + 1;
            int count = mainDeck.Count;
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(50);
                moveCard(mainDeck, getHand(counter % 4));
                counter++;
            }
        }

        private void clearTable()
        {
            // Находим карту, которая бьет остальные 3 и, соответственно, того, кто берет взятку
            bool isTaken = true;
            Nominal maxNom = Nominal.Two;
            Nominal? maxTrumpNom = null;
            for (int i = 0; i < 4; i++)
            {
                if ((tableCards[i].suit == contractSuit) && ((maxTrumpNom == null) || (tableCards[i].nominal > maxTrumpNom)))
                {
                    firstPlayer = i;
                    maxTrumpNom = tableCards[i].nominal;
                    if ((i == 0) || (i == 2)) isTaken = true;
                    else isTaken = false;
                }
                if ((maxTrumpNom == null) && (tableCards[i].suit == curSuit) && (tableCards[i].nominal >= maxNom))
                {
                    firstPlayer = i;
                    maxNom = tableCards[i].nominal;
                    if ((i == 0) || (i == 2)) isTaken = true;
                    else isTaken = false;
                }
            }

            // Этот фрагмент отвечает за то, чтобы в очки записывалась та карта, которой забрали взятку
            switch (tableCards[firstPlayer].nominal)
            {
                case Nominal.Ten:
                case Nominal.Jack:
                case Nominal.Queen:
                case Nominal.King:
                    if ((contractSuit != null) && (tableCards[firstPlayer].suit == contractSuit))
                    {
                        if (isTaken)
                        {
                            MyTmpOnners += 100 * contractNominal;
                            //myOnnersCount++;
                        }
                        else
                        {
                            EnemyTmpOnners += 100 * contractNominal;
                            //enemyOnnersCount++;
                        }
                    }
                    break;
                case Nominal.Ace:
                    if (contractSuit == null)
                    {
                        if (isTaken)
                        {
                            MyAces += 250 * contractNominal;
                            //myAcesCount++;
                        }
                        else
                        {
                            EnemyAces += 250 * contractNominal;
                            //enemyAcesCount++;
                        }
                    }
                    //else
                    {
                        if (isTaken) 
                        {
                            MyTmpOnners += 100 * contractNominal;
                            myAcesCount++;
                        }
                        else 
                        {
                            EnemyTmpOnners += 100 * contractNominal;
                            enemyAcesCount++;
                        }
                    }
                    break;
            }


            cnvTable.Children.Clear();

            // Карты со стола убираются в соответствующую стопку и запоминаются карты, которыми забрали
            switch (isTaken)
            {
                case true:
                    clearDeckSkins(takenCards);
                    takenHighestCards.push(tableCards[firstPlayer]);
                    for (int i = 0; i < 4; i++)
                    {
                        takenCards.push(tableCards[i]);
                        tableCards[i] = null;
                    }
                    drawDeckSkins(takenCards);
                    break;
                case false:
                    clearDeckSkins(lostCards);
                    lostHighestCards.push(tableCards[firstPlayer]);
                    for (int i = 0; i < 4; i++)
                    {
                        lostCards.push(tableCards[i]);
                        tableCards[i] = null;
                    }
                    drawDeckSkins(lostCards);
                    break;
            }
                
            lblTaken.Content = "Взятки: " + takenHighestCards.Count;
            lblLost.Content = "Лузы: " + lostHighestCards.Count;



            // Подсчет очков после раунда
            if (myHand.Count == 0)
            {    
                /*
                // Если в козырной игре у каждой стороны по два туза,
                // тузы записываются в онеры только стороне, взявшей леве
                if ((contractSuit != null) && (myAcesCount == 2) && (enemyAcesCount == 2))
                {
                    if (takenHighestCards.Count > 6) EnemyOnners -= 200 * contractNominal;
                    else MyOnners -= 200 * contractNominal;
                }

                // Если у стороны 3 онера и 2 туза, то она может засчитать
                // только 1 онер. Тузы силы не имеют
                if ((myOnnersCount == 3) && (myAcesCount == 2))
                {
                    if (contractSuit == null)
                    {
                        MyAces -= 500 * contractNominal;
                        MyOnners -= 200 * contractNominal;
                    }
                    else
                    {
                        MyOnners -= 400 * contractNominal;
                    }
                }*/

                // Онеры
                if (takenHighestCards.Count > lostHighestCards.Count)
                {
                    MyOnners += MyTmpOnners;
                    MyTmpOnners = 0;
                    EnemyTmpOnners = 0;
                }
                else
                {
                    EnemyOnners += EnemyTmpOnners;
                    MyTmpOnners = 0;
                    EnemyTmpOnners = 0;
                }

                // Леве
                MyLeve += takenHighestCards.Count * contractNominal * 10;
                EnemyLeve += lostHighestCards.Count * contractNominal * 10;

                // Коронки
                MyCrowns += crownsPoints(takenHighestCards);
                EnemyCrowns += crownsPoints(lostHighestCards);

                // Премии
                if (contractNominal == 6)
                {
                    if ((contractPerformer == 0) || (contractPerformer == 2))
                    {
                        MyBonus += 5000;
                        if (takenHighestCards.Count >= 6) MyBonus += 1000;
                        else EnemyBonus += 5000;
                    }
                    else
                    {
                        EnemyBonus += 5000;
                        if (lostHighestCards.Count >= 6) EnemyBonus += 1000;
                        else MyBonus += 5000;
                    }
                }
                if (contractNominal == 7)
                {
                    if ((contractPerformer == 0) || (contractPerformer == 2))
                    {
                        MyBonus += 10000;
                        if (takenHighestCards.Count == 7) MyBonus += 2000;
                        else EnemyBonus += 10000;
                    }
                    else
                    {
                        EnemyBonus += 10000;
                        if (lostHighestCards.Count == 7) EnemyBonus += 2000;
                        else MyBonus += 10000;
                    }
                }
                if (((contractPerformer == 0) || (contractPerformer == 2)) && (takenHighestCards.Count < contractNominal + 6))
                    EnemyBonus += (contractNominal + 6 - takenHighestCards.Count) * contractNominal * 1000;
                if (((contractPerformer == 1) || (contractPerformer == 3)) && (lostHighestCards.Count < contractNominal + 6))
                    MyBonus += (contractNominal + 6 - lostHighestCards.Count) * contractNominal * 1000;

            }
        }
    }
}
