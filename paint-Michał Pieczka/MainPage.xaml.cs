using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace paint_Michał_Pieczka
{
    public sealed partial class MainPage : Page
    {
        SpeechSynthesizer speechSynthesizer;
        private enum typLini
        {
            prosta, dowolna
        }

        Stack<UIElement> listaUndo = new Stack<UIElement>();
        private typLini obecnyTyp = typLini.dowolna;

        SolidColorBrush pisak = new SolidColorBrush(Windows.UI.Colors.Black);
        
        Point pktStartowy;
        Line tmpLinia;
        bool czyRysujemy = false;

        public MainPage()
        {
            this.InitializeComponent();
            speechSynthesizer = new SpeechSynthesizer();
        }

        private void poleRysowania_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point pktAkt = e.GetCurrentPoint(poleRysowania).Position;
            switch (obecnyTyp)
            {
                case typLini.prosta:
                    rysujLinieProsta(pktAkt);
                    break;
                case typLini.dowolna:
                    rysujLinieDowolna(pktAkt);
                    break;

                default:
                    break;
            }
        }
        private void poleRysowania_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            czyRysujemy = true;
            pktStartowy = e.GetCurrentPoint(poleRysowania).Position;
        }
        private void poleRysowania_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            czyRysujemy = false;
        }

        private void spKolory_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Rectangle rect = (Rectangle)e.OriginalSource;
            pisak = rect.Fill as SolidColorBrush;
        }
        private void rysujLinieDowolna(Point pktAkt)
        {

            if (czyRysujemy)
            {
                Line dowolna = new Line()
                {
                    Stroke = pisak,
                    X1 = pktStartowy.X,
                    Y1 = pktStartowy.Y,
                    X2 = pktAkt.X,
                    Y2 = pktAkt.Y,

                    StrokeThickness = slider.Value,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                };
                poleRysowania.Children.Add(dowolna);
                pktStartowy = pktAkt;

                listaUndo.Push(dowolna);
            }
        }
        private void rysujLinieProsta(Point pktAkt)
        {//zmienna dod jako pole klasy, usuwamy i rysujemy nową kreskę
            if (czyRysujemy)
            {
                Line prosta = new Line()
                {
                    Stroke = pisak,
                    X1 = pktStartowy.X,
                    Y1 = pktStartowy.Y,
                    X2 = pktAkt.X,
                    Y2 = pktAkt.Y,

                    StrokeThickness = slider.Value,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                };

                poleRysowania.Children.Add(prosta);

                if (tmpLinia != null)
                {
                    poleRysowania.Children.Remove(tmpLinia);
                }
                tmpLinia = prosta;

                listaUndo.Push(prosta);
            }
        }
        private void dowolna_Checked(object sender, RoutedEventArgs e)
        {
            obecnyTyp = typLini.dowolna;
        }

        private void prosta_Checked(object sender, RoutedEventArgs e)
        {
            obecnyTyp = typLini.prosta;
        }

        private void koniec_Click(object sender, RoutedEventArgs e)
        {
            Talk("Exit");
            
            Application.Current.Exit();
        }
        private async void Talk(string message)
        {
            var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(message);
            media.SetSource(stream, stream.ContentType);
            media.Play();
        }
        private void cofnij_Click(object sender, RoutedEventArgs e)
        {
            if (listaUndo.Count > 0)
            {
                Shape undo = (Shape)listaUndo.Pop();
                poleRysowania.Children.Remove(undo);
            }
        }

        private void x_Click(object sender, RoutedEventArgs e)
        {
            Talk("Are you sure, you want to leave?");
            string current = exitBoxMesege.Visibility.ToString();
            if (current == "Collapsed")
            {
                exitBoxMesege.Visibility = Visibility.Visible;
            }
        }

        private void worc_Click(object sender, RoutedEventArgs e)
        {
            Talk("I'm glad you are stay");
            string current = exitBoxMesege.Visibility.ToString();
            if (current == "Visible")
            {
                exitBoxMesege.Visibility = Visibility.Collapsed;
            }
        }
    }
}
