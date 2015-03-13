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
    public enum Suit
    {
        Spades, Clubs, Diamonds, Hearts
    }

    public enum Nominal
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Card
    {
        public readonly Suit suit;
        public readonly Nominal nominal;
        public int left;
        public int top;
        public double angle;
        public bool isFaced;
        public Canvas skin;
    

        public Card(Nominal n, Suit m)
        {
            suit = m;
            nominal = n;
        }

        public Canvas getSkin()
        {
            // Задаем пустую панель Canvas
            Canvas card = new Canvas() 
            { 
                Height = 87, 
                Width = 62,
                Margin = new Thickness(left, top, 0, 0)
            };

            // Прямоугольник представляет собой основу и для рубашки и для лицевой стороны
            Rectangle r = new Rectangle()
            {
                Height = 87,
                Width = 62,
                Stroke = Brushes.DarkGray,
                RadiusX = 6,
                RadiusY = 6
            };
            
            // На лицевой стороне ничего кроме значения карты
            if (isFaced)
            {
                LinearGradientBrush br = new LinearGradientBrush();
                br.StartPoint = new Point() { X = 0, Y = 0.2 };
                br.EndPoint = new Point() { X = 1, Y = 0.7 };
                br.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                br.GradientStops.Add(new GradientStop() { Offset = 1, Color = Colors.LightGray });
                br.GradientStops.Add(new GradientStop() { Color = Colors.White });

                r.Fill = br;
                card.Children.Add(r);
                
                Label lbl = new Label();
                lbl.FontSize = 17;
                if ( (int)suit < 2 )    lbl.Foreground = Brushes.Black;
                else                    lbl.Foreground = Brushes.Red;
                lbl.Content = CardValueToString();
                card.Children.Add(lbl);
            } 

            // Рубашка - готовый шаблон
            else
            {
                try
                {
                    string curDir = Environment.CurrentDirectory;
                    BitmapImage img =
                        new BitmapImage(new Uri(string.Format(@"{0}\..\..\images\Capture.png", curDir)));

                    r.Stroke = Brushes.SlateGray;
                    ImageBrush ib = new ImageBrush(img);
                    ib.TileMode = TileMode.None;
                    r.Fill = ib;
                    
                    card.Children.Add(r);                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            // Поворачивает карту при надобности
            card.LayoutTransform = new RotateTransform(angle);
            return card;
        }
        
        public string CardValueToString()
        {
            string tmp;
            if ((int)nominal < 9)
                tmp = ((int)nominal + 2).ToString();
            else
            {
                switch (nominal)
                {
                    case Nominal.Jack:
                        tmp = "J";
                        break;
                    case Nominal.Queen:
                        tmp = "Q";
                        break;
                    case Nominal.King:
                        tmp = "K";
                        break;
                    case Nominal.Ace:
                        tmp = "A";
                        break;
                    default:
                        tmp = "";
                        break;
                }
            }
            switch (suit)
            {
                case Suit.Spades:
                    return tmp + "♠";
                case Suit.Clubs:
                    return tmp + "♣"; 
                case Suit.Diamonds:
                    return tmp + "♦";
                case Suit.Hearts:
                    return tmp + "♥";
                default:
                    return "";
            }
        }
    }
}
