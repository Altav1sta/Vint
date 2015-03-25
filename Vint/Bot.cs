using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Vint
{
    public class Bot
    {

        private MainWindow win;
        private ChooseContract cc;
        private Deck deck;
        private Suit? suitToMove = null;


        public Bot(Deck d, MainWindow mw, ChooseContract chCon)
        {
            win = mw;
            deck = d;
            cc = chCon;
        }




        // Вспомогательные методы


        private int suitLength(Suit s)
        {
            int n = 0;
            foreach (Card c in deck)
            {
                if (c.suit == s) n++;
            }
            return n;
        }

        private Suit? longestSuit()
        {
            int maxLength = 0;
            Suit? corSuit = null;
            Array suitArray = Enum.GetValues(Suit.Hearts.GetType());

            foreach (Suit s in suitArray)
            {
                if (suitLength(s) > maxLength)
                {
                    maxLength = suitLength(s);
                    corSuit = s;
                    continue;
                }
                // Если вдруг у нас две или более мастей одинаковой наибольшей длины - заявляем БК контракт
                if (suitLength(s) == maxLength)
                {
                    corSuit = null;
                }
            }
            return corSuit;
        }

        private string longestSuitSkin()
        {
            switch (longestSuit())
            {
                case Suit.Spades:
                    return "♠";
                case Suit.Clubs:
                    return "♣"; 
                case Suit.Diamonds:
                    return "♦";
                case Suit.Hearts:
                    return "♥";
                default:
                    return "БК";
            }
        }

        private int straightLength(Suit s, int nom)
        {
            Card[] arr = new Card[suitLength(s)];
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

            int pos = -1;
            for (int j = 0; j < arr.Length; j++)
            {
                if ((int)arr[j].nominal == nom) pos = j; 
            }
            if (pos == -1) return 0;

            int ans = 1;
            for (int j = pos + 1; j < arr.Length; j++)
            {
                if ((int)(arr[j - 1].nominal) - (int)(arr[j].nominal) == 1)
                    ans++;
                else return ans;
            }

            return ans;
        }

        private int topNominal(Suit s)
        {
            for (int i = 12; i >= 0; i--)
            {
                foreach (Card c in deck)
                {
                    if ((c.suit == win.curSuit) && ((int)(c.nominal) == i))
                        return i;
                }
            }
            return -1;
        }

        private void discard(Suit? s)
        {                
            int minNom = 100;
            Card minCard = null;
            
            if (s == null)
            {
                // Сначала попытаемся забрать взятку козырем
                if ((win.contractSuit != null) && (win.curSuit != null) && (suitLength((Suit)win.curSuit) == 0) && (suitLength((Suit)win.contractSuit) > 0))
                {
                    // Если сoюзник уже может забрать - не пытаемся забрать  
                    bool allyCanTake = false;                      
                    if ((win.getHand(deck.allySide()).Count == deck.Count - 1))
                    {
                        // Если он забирает козырем
                        if (win.tableCards[deck.allySide()].suit == win.contractSuit)
                        {
                            allyCanTake = true;
                            // Если открытый враг-болван, ходящий после нас может перебить - пропускаем
                            if ((win.bolvan != deck.side) && (win.bolvan != deck.allySide()) && win.getHand(win.bolvan).isFaced && (win.getHand(win.bolvan).Count == deck.Count) &&
                                ((win.curSuit == win.contractSuit) || (win.getBot(win.bolvan).suitLength((Suit)win.curSuit) == 0)) && 
                                (win.getBot(win.bolvan).topNominal((Suit)win.contractSuit) > (int)win.tableCards[deck.allySide()].nominal))
                            {
                                allyCanTake = false;
                            }

                            // Если на столе уже есть больший козырь - пропускаем
                            for (int k = 0; k < 4; k++)
                            {
                                if ((win.tableCards[k] != null) && (win.tableCards[k].suit == win.contractSuit) && ((int)(win.tableCards[k].nominal) > (int)win.tableCards[deck.allySide()].nominal))
                                {
                                    allyCanTake = false;
                                }
                            }
                        }
                        else
                        {
                            if (canLayAsHighCard((int)win.tableCards[deck.allySide()].nominal)) allyCanTake = true;
                        }
                    }

                    // Если нет
                    if (!allyCanTake)
                    {
                        for (int i = 0; i < 13; i++)
                        {
                            foreach (Card c in deck)
                            {
                                if (((int)c.nominal == i) && (c.suit == (Suit)win.contractSuit))
                                {
                                    // Если у открытого врага-болвана, ходящего после нас есть козырь старше и нет масти - пропускаем
                                    if ((win.bolvan != deck.side) && (win.bolvan != deck.allySide()) && win.getHand(win.bolvan).isFaced && (win.getHand(win.bolvan).Count == deck.Count) &&
                                        ((win.curSuit == win.contractSuit) || (win.getBot(win.bolvan).suitLength((Suit)win.curSuit) == 0)) && (win.getBot(win.bolvan).topNominal((Suit)win.contractSuit) > i))
                                    {
                                        continue;
                                    }

                                    // Если на столе уже есть больший козырь - пропускаем
                                    bool flag = false;
                                    for (int k = 0; i < 4; i++)
                                    {
                                        if ((win.tableCards[k] != null) && (win.tableCards[k].suit == win.contractSuit) && ((int)(win.tableCards[k].nominal) > i))
                                        {
                                            flag = true;
                                        }
                                    }
                                    if (flag) continue;
                                    
                                    win.handCard_MouseLeftButtonDown(c.skin, null);
                                    return;
                                }
                            }
                        }
                    }
                }

                // Если не получилось:

                // Смотрим, чтобы карта была наименьшего номинала и при этом не была козырем
                foreach (Card c in deck)
                {
                    if (((Suit?)c.suit != win.contractSuit) && ((int)(c.nominal) < minNom))
                    {
                        minCard = c;
                        minNom = (int)(c.nominal);
                    }
                }

                // Если остались только козыри - ходим ими
                if (minNom == 100)
                {
                    foreach (Card c in deck)
                    {
                        if ((int)(c.nominal) < minNom)
                        {
                            minCard = c;
                            minNom = (int)(c.nominal);
                        }
                    }
                }
            }
            else
            {
                if (suitLength((Suit)s) == 0)
                {
                    discard(null);
                    return;
                }

                foreach (Card c in deck)
                {
                    if ((c.suit == s) && ((int)(c.nominal) < minNom))
                    {
                        minNom = (int)(c.nominal);
                        minCard = c;
                    }
                }
            }
            
            // Если вдруг эта карта оказалась последней картой в масти, с которой мы должны заходить - отменяем масть
            if ((suitToMove != null) && (minCard.suit == suitToMove) && (suitLength(minCard.suit) == 1)) suitToMove = null;
            win.handCard_MouseLeftButtonDown(minCard.skin, null);
        }

        private bool canLayAsHighCard(int top)
        // Масть карты должна совпадать с мастью, которой ходил первый ирок в этом раунде!
        {
            for (int i = 0; i < 4; i++)
            {
                // Если мы заведомо не можем побить карту - false
                if ((win.tableCards[i] != null) && ((win.tableCards[i].suit == win.curSuit) && ((int)(win.tableCards[i].nominal) > top) ||
                    ((win.contractSuit != win.curSuit) && (win.tableCards[i].suit == win.contractSuit))))
                {
                    return false;
                }
            }

            // Если у нас на руках все-таки хай-карта, нужно проверить были ли сброшены карты, выше нашей
            for (int i = 12; i > top; i--)
            {
                bool flag = true;

                // Если у болвана-врага есть карта выше нашей - немедленный сброс
                if ((win.getHand(win.bolvan).Count == deck.Count) && (win.getHand(win.bolvan).isFaced) && (win.bolvan != deck.allySide()) && (win.bolvan != deck.side))
                {
                    foreach (Card c in win.getHand(win.bolvan))
                    {
                        if ((c.suit == win.curSuit) && ((int)(c.nominal) == i)) return false;
                    }

                    // Если есть козыри, которыми можно бить - скидываемся
                    if ((win.contractSuit != null) && (win.getBot(win.bolvan).suitLength((Suit)win.contractSuit) > 0) && (win.getBot(win.bolvan).suitLength((Suit)win.curSuit) == 0))
                    {
                        return false;
                    }
                    
                }

                // Учитываем вышедшие и свои карты, а также карты болвана, если он союзник
                // или карты союзника если мы на позциции болвана
                foreach (Card c in win.takenCards)
                {
                    if ((c.suit == win.curSuit) && ((int)(c.nominal) == i))
                    {
                        flag = false;
                        break;
                    }
                }
                foreach (Card c in win.lostCards)
                {
                    if ((c.suit == win.curSuit) && ((int)(c.nominal) == i))
                    {
                        flag = false;
                        break;
                    }
                }
                foreach (Card c in deck)
                {
                    if ((c.suit == win.curSuit) && ((int)(c.nominal) == i))
                    {
                        flag = false;
                        break;
                    }
                }
                if ((win.bolvan == deck.allySide()) || (win.bolvan == deck.side))
                {
                    foreach (Card c in win.getHand(deck.allySide()))
                    {
                        if ((c.suit == win.curSuit) && ((int)(c.nominal) == i))
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                // Если у врагов все еще могут быть карты выше - сброс
                if (flag && (((win.getHand((deck.side + 1) % 4).Count == deck.Count) && !(((deck.side + 1) % 4 == win.bolvan) && (win.getHand(win.bolvan).isFaced))) ||
                    ((win.getHand((deck.side + 3) % 4).Count == deck.Count) && !(((deck.side + 3) % 4 == win.bolvan) && (win.getHand(win.bolvan).isFaced))) ))
                {
                    return false;
                }
            }

            return true;
        }

        private int MiltonWorkValue()
        {
            int sum = 0;

            foreach (Card c in deck)
            {
                switch (c.nominal)
                {
                    case Nominal.Ace:
                        sum += 4;
                        break;
                    case Nominal.King:
                        sum += 3;
                        break;
                    case Nominal.Queen:
                        sum += 2;
                        break;
                    case Nominal.Jack:
                        sum += 1;
                        break;
                    default:
                        break;
                }
            }
            return sum;
        }

        private int MiltonWorkContract(int num)
        {
            switch (num)
            {
                case 21:
                case 22:
                    return 1;
                case 23:
                    return 2;
                case 24:
                case 25:
                    return 3;
                case 26:
                    return 4;
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                    return 5;
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                    return 6;
                case 37:
                case 38:
                case 39:
                case 40:
                    return 7;
                default:
                    return 0;
            }
        }

        private int supposedAllyValueAddition()
        {
            try
            {
                switch (Int32.Parse(cc.turnResult(deck.allySide(), 1).Substring(0, 1)))
                {
                    case 1:
                        return 10;
                    case 2:
                        return 11;
                    case 3:
                        return 12;
                    case 4:
                        return 13;
                    case 5:
                        return 14;
                    case 6:
                        return 16;
                    case 7:
                        return 18;
                    default:
                        return 0;
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Can't parse {0} as int32!\n\n" + e.ToString(), cc.turnResult(deck.allySide(), 1).Substring(0, 1));
                return 0;
            }
        }

        private void firstPlayerMove()
        {
            // Проверка, действительно ли в этой масти можно хоть что-то забрать
            // Если нет, то не будем с нее ходить
            if (suitToMove != null)
            {
                win.curSuit = suitToMove;
                if ((suitLength((Suit)suitToMove) == 0) || (!canLayAsHighCard(topNominal((Suit)suitToMove))))
                    suitToMove = null;
                win.curSuit = null;
            }

            // Если мы пока не знаем с какой масти ходить - ходим с той, 
            // где самая длинная "лесенка" при равных номиналах, начиная с туза
            if (suitToMove == null)
            {
                Array suitArray = Enum.GetValues(Suit.Hearts.GetType());
                for (int i = 12; i >= 0; i--)
                {
                    int len = 0;
                    foreach (Suit s in suitArray)
                    {
                        if ((suitLength(s) > 0) && (straightLength(s, i) > len))
                        {
                            win.curSuit = s;
                            if (!canLayAsHighCard(i))
                            {
                                win.curSuit = null;
                                continue;
                            }
                            len = straightLength(s, i);
                            suitToMove = s;
                        }
                    }
                    if (len > 0) break;
                }

                // Если с какой бы топ-карты мы не пошли - ее заберут, то лучше сброситься
                if (win.curSuit == null)
                {
                    // Если у союзника есть чем забрать, то лучше подмастить ему
                    if ((deck.allySide() == win.bolvan) || (deck.side == win.bolvan))
                    {
                        foreach (Suit s in suitArray)
                        {
                            win.curSuit = s;
                            if ((suitLength(s) > 0) && (win.getBot(deck.allySide()).suitLength(s) > 0) && canLayAsHighCard(win.getBot(deck.allySide()).topNominal(s)))
                            {
                                discard(s);
                                return;
                            }

                            // Ситуация, когда можно кинуть такую масть, которой нет у союзника, но зато он может забрать козырем
                            win.curSuit = win.contractSuit;
                            if ((suitLength(s) > 0) && (win.getBot(deck.allySide()).suitLength(s) == 0) && (win.contractSuit != null) && 
                                (win.getBot(deck.allySide()).suitLength((Suit)win.contractSuit) > 0) && canLayAsHighCard(win.getBot(deck.allySide()).topNominal((Suit)win.contractSuit)))
                            {
                                win.curSuit = s;
                                discard(s);
                                return;
                            }

                            win.curSuit = null;
                        }
                    }
                    
                    discard(null);
                    return;
                }
            }

            
            // Ходим старшей картой в выбранной масти
            for (int i = 12; i >= 0; i--)
            {
                foreach (Card c in deck)
                {
                    if ((c.suit == suitToMove) && ((int)(c.nominal) == i))
                    {
                        if (straightLength((Suit)suitToMove, i) == 1) suitToMove = null;
                        win.handCard_MouseLeftButtonDown(c.skin, null);
                        return;
                    }
                }
            }
        }




         
        // Основные методы




        public async void playerMove()
        {
            await (Task.Run(() => Thread.Sleep(500)));
            // Если ходим первыми - будем ходить старшими картами по самым длинным мастям
            // Пока не учитывается вариант, что у противника может оказаться карта старше
            if (win.firstPlayer == deck.side)
            {
                win.curSuit = null;
                firstPlayerMove();
            }
            //Если ходим не первым - отталкиваемся от карт противников
            else
            {
                // Смотрим есть ли у нас вообще подходящая масть
                if (topNominal((Suit)win.curSuit) > -1)
                {
                    // Ходим наименьшей картой, для которой нет угрозы старшей карты у врагов
                    // Если таковых нет - сброс
                    if (!canLayAsHighCard(topNominal((Suit)win.curSuit)))
                    {
                        discard(win.curSuit);
                    }
                    else
                    {
                        for (int i = 0; i < 13; i++)
                        {
                            foreach (Card c in deck)
                            {
                                if ((c.suit == (Suit)win.curSuit) && ((int)c.nominal == i) && canLayAsHighCard(i))
                                {
                                    // Если походивший союзник уже может забрать - скинемся в его пользу
                                    if ((win.getHand(deck.allySide()).Count == deck.Count - 1) && canLayAsHighCard((int)win.tableCards[deck.allySide()].nominal))
                                    {
                                        discard(c.suit);
                                        return;
                                    }


                                    if ((i == topNominal((Suit)win.curSuit)) && (straightLength((Suit)win.curSuit, topNominal((Suit)win.curSuit)) == 1))
                                        suitToMove = null;
                                    win.handCard_MouseLeftButtonDown(c.skin, null);
                                    return;
                                }
                            }
                        }
                    }
                }
                // Если нет масти - сбрасываем
                else
                {
                    discard(null);
                }
            }
        }

        public void ChooseContract()
        {
            // Если ходим до союзника или если он скинулся
            if ((cc.turnResult(deck.allySide(), 1) == "") || (cc.turnResult(deck.allySide(), 1) == "ПАС"))
            {
                // Если не дотягиваем и до 1 по шкале Милтона-Уорка
                if (MiltonWorkContract(MiltonWorkValue()) == 0)
                {
                    // Если уже заявлен контракт больший либо равный 1БК
                    if (win.contractNominal >= 1)
                    {
                        cc.getLabel(deck.side).Content += "\nПАС";
                    }
                    else
                    {
                        win.contractPerformer = deck.side;
                        win.contractNominal = 1;
                        win.contractSuit = null;
                        cc.getLabel(deck.side).Content += "\n1 БК";
                    }
                }
                else
                {
                    // Ходим если уже заявленный контракт меньше того, который мы собираемся заявлять 
                    if (win.contractNominal > MiltonWorkContract(MiltonWorkValue()))
                    {
                        cc.getLabel(deck.side).Content += "\nПАС";
                    }
                    else
                    {
                        if (win.contractNominal == MiltonWorkContract(MiltonWorkValue()))
                        {
                            if (win.contractSuit == null)
                            {
                                cc.getLabel(deck.side).Content += "\nПАС";
                            }
                            else 
                            {
                                if (longestSuit() == null)
                                {
                                    win.contractPerformer = deck.side;
                                    win.contractNominal = MiltonWorkContract(MiltonWorkValue());
                                    win.contractSuit = longestSuit();
                                    cc.getLabel(deck.side).Content += "\n" + MiltonWorkContract(MiltonWorkValue()).ToString() + " " + longestSuitSkin();
                                }
                                else
                                {
                                    if ((int)win.contractSuit >= (int)longestSuit())
                                    {
                                        cc.getLabel(deck.side).Content += "\nПАС";
                                    }
                                    else
                                    {
                                        win.contractPerformer = deck.side;
                                        win.contractNominal = MiltonWorkContract(MiltonWorkValue());
                                        win.contractSuit = longestSuit();
                                        cc.getLabel(deck.side).Content += "\n" + MiltonWorkContract(MiltonWorkValue()).ToString() + " " + longestSuitSkin();
                                    }
                                }
                            }
                        }
                        else
                        {
                            win.contractPerformer = deck.side;
                            win.contractNominal = MiltonWorkContract(MiltonWorkValue());
                            win.contractSuit = longestSuit();
                            cc.getLabel(deck.side).Content += "\n" + MiltonWorkContract(MiltonWorkValue()).ToString() + " " + longestSuitSkin();
                        }
                    }                    
                }
            }
            else
            {
                // При ходе после союзника в рассчет берется половина его заявки в очках в качестве добавки
                if (MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition()) == 0)
                {
                    cc.getLabel(deck.side).Content += "\nПАС";
                }
                else
                {
                    if (win.contractNominal > MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition()))
                    {
                        cc.getLabel(deck.side).Content += "\nПАС";
                    }
                    else
                    {
                        if (win.contractNominal == MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition()))
                        {
                            if (win.contractSuit == null)
                            {
                                cc.getLabel(deck.side).Content += "\nПАС";
                            }
                            else
                            {
                                if (longestSuit() == null)
                                {
                                    win.contractPerformer = deck.side;
                                    win.contractNominal = MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition());
                                    win.contractSuit = longestSuit();
                                    cc.getLabel(deck.side).Content += "\n" + MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition()).ToString() + " " + longestSuitSkin();
                                }
                                else
                                {
                                    if ((int)win.contractSuit >= (int)longestSuit())
                                    {
                                        cc.getLabel(deck.side).Content += "\nПАС";
                                    }
                                    else
                                    {
                                        win.contractPerformer = deck.side;
                                        win.contractNominal = MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition());
                                        win.contractSuit = longestSuit();
                                        cc.getLabel(deck.side).Content += "\n" + MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition()).ToString() + " " + longestSuitSkin();
                                    }
                                }
                            }
                        }
                        else
                        {
                            win.contractPerformer = deck.side;
                            win.contractNominal = MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition());
                            win.contractSuit = longestSuit();
                            cc.getLabel(deck.side).Content += "\n" + MiltonWorkContract(MiltonWorkValue() + supposedAllyValueAddition()).ToString() + " " + longestSuitSkin();
                        }
                    } 
                }
            }
        }
    }
}
