using System;
using System.Collections;
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
    public class Deck : Stack<Card>
    {

        public int left;
        public int top;
        public double angle;
        public int side;
        public bool isFaced;
        public bool isShifted = false;
        public bool isSorted = false;


        public Deck(int l, int t, double a, bool f, int s)
        {
            left = l;
            top = t;
            angle = a;
            isFaced = f;
            side = s;
        }

        public void fill()
        {
            if (Count != 0) return;
            Nominal n = Nominal.Two;
            Suit m = Suit.Spades;
            Array nData = Enum.GetValues(n.GetType());
            Array mData = Enum.GetValues(m.GetType());
            foreach (Nominal i in nData)
            {
                foreach (Suit j in mData)
                {
                    push(new Card(i, j));
                }
            }
        }

        public void push(Card c)
        {
            c.left = left + Count / 3;
            c.top = top - Count / 2;
            c.angle = angle;
            c.isFaced = isFaced;
            c.skin = c.getSkin();

            Push(c);

            updateHand();
        }

        public Card pop()
        {
            Card c = Pop();

            updateHand();

            return c;
        }

        public void clear()
        {
            Clear();
            isShifted = false;
            isSorted = false;
        }

        public void updateHand()
        {

            if ((side < 0) || (side >= 4)) return;
            int counter = 0;
            int mod;
            // Очень хитрые вычисления:
            // обновление положения карт на руке при изменении количества
            Math.DivRem(side, 2, out mod);
            int x = left - (33 * (Count - 1) + 62) / 2 * (1 - mod);
            int y = top - (33 * (Count - 1) + 62) / 2 * mod * (2 - side);
            foreach (Card card in this.Reverse<Card>())
            {
                card.left = x + counter * 33 * (1 - mod);
                card.top = y + counter * 33 * mod * (2 - side);
                card.skin = card.getSkin();
                counter++;
            }
        }

        public Card getCard(Canvas cnv)
        {
            foreach (Card c in this)
            {
                if (c.skin == cnv) return c;
            }
            return null;
        }
                
        public Canvas[] getSkins()
        {
            Canvas[] cnv = new Canvas[Count];
            
            Card[] tmp = new Card[Count];
            CopyTo(tmp, 0);
            for (int i = 0; i < Count; i++ )
            {
                cnv[i] = tmp[Count-i-1].skin;
            }
            return cnv;
        }

        public void reshuffle()
        {
            if (Count == 0) return;
            int count = Count;
            Card[] cards = new Card[count];
            Random r = new Random();
            for (int i = 0; i < count; i++)
            {
                int index = r.Next(count);
                while (cards[index] != null)
                {
                    if (index != count - 1)
                        index++;
                    else
                        index = 0;
                }
                cards[index] = pop();    
            }

            for (int i = 0; i < count; i++)
                push(cards[i]);
        }

        public void overturn()
        {
            // Сначала определим, что куда сдвигать
            if (isSorted == true)
            {
                if ((isFaced == true) && (isShifted == true)) shift(-1);
                if ((isFaced == false) && (isShifted == false)) shift(1);
            }
            
            isFaced = !isFaced;
            foreach (Card c in this)
            {
                c.isFaced = isFaced;
                c.skin = c.getSkin();
            }
        }

        public void sort()
        {
            if (isSorted == true) return;
            if (Count == 0) return;
            Card[] cards = new Card[Count];

            Nominal n = Nominal.Two;
            Suit m = Suit.Hearts;
            Array nData = Enum.GetValues(n.GetType());
            Array mData = Enum.GetValues(m.GetType());
            int counter = 0;
 
            foreach (Suit j in mData)
            {
                foreach (Nominal i in nData)
                {
                    foreach (Card c in this)
                    {
                        if ((c.nominal == i) && (c.suit == j))
                        {
                            cards[counter] = c;
                            counter++;
                            break;
                        }
                    }
                }
            }

            clear();
            
            for (int i = 0; i < cards.Length; i++)
                push(cards[cards.Length - i - 1]);


            // Сдвигает выделенные цепочки у открытых карт
            if (isFaced == true) shift(1);

            isSorted = true;
        }

        public void shift(int sign)
        {
            int flag = sign;
            Suit m = Suit.Hearts;
            foreach (Card c in this)
            {
                if (c.suit != m)
                {
                    m = c.suit;
                    flag = Math.Sign(sign) - flag;
                }
                switch (side)
                {
                    case 0:
                        c.top -= 5 * flag;
                        break;
                    case 1:
                        c.left += 5 * flag;
                        break;
                    case 2:
                        c.top += 5 * flag;
                        break;
                    case 3:
                        c.left -= 5 * flag;
                        break;
                }

                c.skin = c.getSkin();
            }

            isShifted = !isShifted;
        }
        
        public Card extract(Canvas s)
        {
            int ind = 0;
            foreach (Card c in this)
            {
                if (c.skin == s)
                {
                    Card[] cards = new Card[ind + 1];
                    for (int i = 0; i <= ind; i++)
                        cards[i] = pop();
                    for (int i = ind - 1; i >= 0; i--)
                        push(cards[i]);

                    // Сортированную руку придется пересортировать,
                    // чтобы не испортить сдвиг
                    if (isSorted == true)
                    {
                        isSorted = false;
                        sort();
                    }

                    cards[ind].isFaced = true;
                    cards[ind].skin = cards[ind].getSkin();
                    return cards[ind];
                }
                ind++;
            }

            Card ans = new Card(Nominal.Two, Suit.Spades);
            ans.angle = 0;
            ans.isFaced = false;
            return ans;
        }

        public int allySide()
        {
            switch (side)
            {
                case 0:
                    return 2;
                case 1:
                    return 3;
                case 2:
                    return 0;
                case 3:
                    return 1;
                default:
                    return -1;
            }
        }

    }
}
