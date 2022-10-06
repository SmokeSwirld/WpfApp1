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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random = new();
        private const String FREE_SYM = "\x0DF4";
        private const String MINE_SYM = "\x2622";
        private const String FlAG_SYM = "\x2691";

       private DispatcherTimer Timer;
        private int time;
      

        public MainWindow()
        {
            InitializeComponent();
            Timer = new() { Interval = new TimeSpan(0, 0, 0, 1) };
            Timer.Tick += Timer_Tick;
            Title="00:00:00";
            
            
            for (int y = 0; y < App.FIELD_SIZE_Y; y++)
                for (int x = 0; x < App.FIELD_SIZE_X; x++)
                {
                    FieldLabel label = new()
                    {
                        X = x,
                        Y = y,
                        IsMine = random.Next(3) == 0
                    };
                    label.Content = FREE_SYM; //label.IsMine ? FREE_SYM : MINE_SYM;
                    
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    label.VerticalContentAlignment = VerticalAlignment.Center;

                    label.Background = Brushes.Bisque;
                    label.Margin = new Thickness(1);

                    label.MouseLeftButtonUp += LabelClick;
                    label.MouseRightButtonDown += RightLabelClick;

                    this.RegisterName($"label_{x}_{y}", label);

                    FIELD.Children.Add(label);
                }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            ++time;
            int h = time / 3600;
            int m = (time % 3600) / 60;
            int s = time  % 60;
            Title=h.ToString("00")+":"+m.ToString("00") +":"+s.ToString("00");
        }

        private bool IsWin()
        {
            foreach (var child in FIELD.Children)
            {
                if (child is FieldLabel label)
                {
                    if (!label.IsMine && (label.Content.Equals(FREE_SYM)
                                       || label.Content.Equals(FlAG_SYM)))
                    {
                        return false;
                    }
                }
            }
            Timer.Stop();
            return true;
        }


        void RightLabelClick(object sender, RoutedEventArgs e)
        {
            if (sender is FieldLabel label)
            {
                if (!label.Content.Equals(FlAG_SYM)
                 && !label.Content.Equals(FREE_SYM)) return;

                label.Content = label.Content.Equals(FlAG_SYM) ? FREE_SYM : FlAG_SYM;
            }
        }
        void LabelClick(object sender, RoutedEventArgs e)
        {
            time = 0;
            Timer.Start();
            if (sender is FieldLabel label)
            {
                String[] names =
                    {
                        $"label_{label.X-1}_{label.Y-1}",
                        $"label_{label.X  }_{label.Y-1}",
                        $"label_{label.X+1}_{label.Y-1}",
                        $"label_{label.X-1}_{label.Y}",
                        $"label_{label.X+1}_{label.Y}",
                        $"label_{label.X-1}_{label.Y+1}",
                        $"label_{label.X  }_{label.Y+1}",
                        $"label_{label.X+1}_{label.Y+1}",

                    };
                int mines = 0;
                bool bo = IsWin();
                foreach (String name in names)
 
                    if (this.FindName(name) is FieldLabel neighbour)
                    {
                        if (neighbour.IsMine) mines += 1;
                        
                        if (label.IsMine) 
                        {
                            label.Content = MINE_SYM;
                            if (MessageBoxResult.No ==
                                MessageBox.Show("Play again", "Game Over", 
                                MessageBoxButton.YesNo))
                            {
                                this.Close();                               
                            }
                            else
                            {
                                foreach (var child in FIELD.Children)
                                    if (child is FieldLabel cell)
                                    {
                                        cell.Content = FREE_SYM;
                                        cell.IsMine = random.Next(3) == 0;
                                        cell.Background = Brushes.Bisque;
                                    }
                            }
                            Timer.Stop();
                            return;
                        }
                        else if (bo == true) 
                        {
                            if (MessageBoxResult.No ==
                                MessageBox.Show("Play again", "You WIN!!!",
                                MessageBoxButton.YesNo))
                            {
                                this.Close();
                            }
                            else
                            {
                                foreach (var child in FIELD.Children)
                                    if (child is FieldLabel cell)
                                    {
                                        cell.Content = FREE_SYM;
                                        cell.IsMine = random.Next(3) == 0;
                                        cell.Background = Brushes.Bisque;
                                    }
                            }
                            return;
                        }
                        if (mines == 0) label.Background = Brushes.Chartreuse;
                        else if (mines == 1) label.Background = Brushes.Cornsilk;
                        else if (mines == 2) label.Background = Brushes.Chocolate;
                        else if (mines == 3) label.Background = Brushes.Crimson;
                        else if (mines == 4) label.Background = Brushes.Coral;
                        else if (mines == 5) label.Background = Brushes.DeepSkyBlue;
                        else if (mines == 6) label.Background = Brushes.DodgerBlue;
                        else if (mines == 7) label.Background = Brushes.Blue;
                        else if (mines == 8) label.Background = Brushes.FloralWhite;
                        label.Content = mines.ToString();                                            
                    }
            }
        }
        
        class FieldLabel : Label 
        {
            public int X { get; set; }
            public int Y { get; set; }
            public bool IsMine { get; set; }


        }
    }
}
