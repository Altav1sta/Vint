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
        public int dealer;
        public Suit? curSuit = null;
        public bool roundCompleted = false;
        public bool contractWasConfirmed = false;
        public bool canClearTableWithMouseClick = true;
        public int firstPlayer;
        public int bolvan;
        public int curPlayer;
        public int contractPerformer = 0;
        public int contractNominal = 0;
        public Suit? contractSuit = null;
        public int robber;
        public int part;
        public bool canChangeDB = false;

        public int myAcesCount;
        public int enemyAcesCount;
        public int myOnnersCount;
        public int enemyOnnersCount;

        public int MyOnners;
        public int MyAces;
        public int MyCrowns;
        public int MyBonus;
        public int MyLeve;

        public int EnemyOnners;
        public int EnemyAces;
        public int EnemyCrowns;
        public int EnemyBonus;
        public int EnemyLeve;

        public int MyTmpOnners = 0;
        public int EnemyTmpOnners = 0;

        bool isShown = false;
        public ResultTable rt = new ResultTable();
        Bot wBot; 
        Bot nBot;
        Bot eBot;
        Bot sBot;



        private void bindClickEvents(Deck k)
        {
            foreach (Canvas c in k.getSkins())
            {
                c.MouseLeftButtonDown +=
                    new System.Windows.Input.MouseButtonEventHandler(handCard_MouseLeftButtonDown);
            }
        }

        private bool cardIsProperToMove(Card card, Deck k)
        {
            if (card.suit == curSuit) return true;
            else
            {
                foreach (Card c in k)
                {
                    if (c.suit == curSuit) return false;
                }
                return true;
            }
        }

        private int crownsPoints(Deck deck)
        {
            int ans = 0;
            int aces = 0;
            Array suitArray = Enum.GetValues(Suit.Hearts.GetType());

            foreach (Suit s in suitArray)
            {
                int len = 0;
                foreach (Card c in deck)
                {
                    if (c.suit == s) len++;
                }
                if (len == 0) continue;

                Card[] arr = new Card[len];

                int i = 0;
                for (int j = 12; j >= 0; j--)
                {
                    foreach (Card c in deck)
                    {
                        if ((c.suit == s) && ((int)(c.nominal) == j))
                        {
                            arr[i] = c;
                            i++;
                            break;
                        }
                    }
                }

                if (arr[0].nominal == Nominal.Ace) aces++;
                if ((len < 3) || (arr[0].nominal != Nominal.Ace) || (arr[1].nominal != Nominal.King) || (arr[2].nominal != Nominal.Queen)) continue;

                if ((contractSuit != null) && (s == contractSuit) || (contractSuit == null))
                {
                    ans = 1000;
                    if (len == 3) continue;

                    for (int j = 4; j < arr.Length; j++)
                    {
                        if ((int)(arr[j - 1].nominal) - (int)(arr[j].nominal) == 1)
                            ans += 1000;
                        else break;
                    }
                }
                else
                {
                    ans = 500;
                    if (len == 3) continue;

                    for (int j = 4; j < arr.Length; j++)
                    {
                        if ((int)(arr[j - 1].nominal) - (int)(arr[j].nominal) == 1)
                            ans += 500;
                        else break;
                    }
                }
            }

            if (aces == 3)
            {
                if (contractSuit == null) ans += 1000;
                else ans += 500;
            }
            if (aces == 4)
            {
                if (contractSuit == null) ans += 2000;
                else ans += 1000;
            }

            return ans;
        }

        public Bot getBot(int side)
        {
            switch (side)
            {
                case 0:
                    return sBot;
                case 1:
                    return wBot;
                case 2:
                    return nBot;
                case 3:
                    return eBot;
                default:
                    return null;
            }
        }

        private Deck getHand(Canvas cnv)
        {
            for (int i = 0; i < 4; i++)
            {
                foreach (Card c in getHand(i))
                {
                    if (c.skin == cnv) return getHand(i);
                }
            }
            return mainDeck;
        }

        public Deck getHand(int side)
        {
            switch (side)
            {
                case 0:
                    return myHand;
                case 1:
                    return westHand;
                case 2:
                    return northHand;
                case 3:
                    return eastHand;
                default:
                    return mainDeck;
            }
        }

        public Label getLabel(int side)
        {
            switch (side)
            {
                case 0: return lblSouth;
                case 1: return lblWest;
                case 2: return lblNorth;
                case 3: return lblEast;
                default: return null;
            }
        }

        private bool possibleToUndo()
        {
            if (bolvan == 0) return false;
            int counter = cnvTable.Children.Count;
            switch (firstPlayer)
            {
                case 0:
                case 2:
                    if ((counter == 4) || (counter == 2) && (bolvan == 2)) return true;
                    return false;
                case 1:
                case 3:
                    if ((counter == 4) || (counter == 3) && (bolvan == 2)) return true;
                    return false;
                default:
                    return false;
            }
        }
        
        private void unbindClickEvents(Deck k)
        {
            foreach (Canvas c in k.getSkins())
            {
                c.MouseLeftButtonDown -=
                    new System.Windows.Input.MouseButtonEventHandler(handCard_MouseLeftButtonDown);

            }
        }
        
    }
}
