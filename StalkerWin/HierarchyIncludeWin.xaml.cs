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
using StalkerHierarchyElement;
using System.IO;

namespace Stalker_Studio.StalkerWin
{
    /// <summary>
    /// Логика взаимодействия для HierarchyIncludeWin.xaml
    /// </summary>
    public partial class HierarchyIncludeWin : Window
    {
        public HierarchyIncludeWin(string MainPath)
        {
            InitializeComponent();
            FolderPath = MainPath;
        }

        private string FolderPath;

        public void CreateBlock(string nameBlock,string shortPathToFile,Point location,Size size)
        {
            StackPanel stack = new StackPanel();
            TextBlock lab = new TextBlock();
            lab.Foreground = new SolidColorBrush(Colors.Black);
            lab.VerticalAlignment = VerticalAlignment.Top;
            lab.HorizontalAlignment = HorizontalAlignment.Left;
            lab.Margin = new Thickness(location.X+1,location.Y - size.Height - location.Y,0,0);
            lab.Text = nameBlock;
            Rectangle rect = CreateRectangle(location.X, location.Y, size.Width, size.Height);
            stack.Children.Add(rect);
            stack.Children.Add(lab);
            /*
                stack.MouseMove += (sender, e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point p = new Point(e.GetPosition(this).X - (size.Width / 2), e.GetPosition(this).Y - (size.Height / 2));
                        stack.Margin = new Thickness(p.X, p.Y, p.X, p.Y);
                    }
                };
            */
            mPanel.Children.Add(stack);

        }

        private Size StandartSize = new Size(150, 60);
        private Point StandartPoint = new Point(0, 15);

        private Rectangle CreateRectangle(double x,double y,double x_size,double y_size)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Colors.White);
            rect.Width = x_size;
            rect.Height = y_size;
            rect.RadiusX = 5;
            rect.RadiusY = 5;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.Margin = new Thickness(x, y, 0,0);
            return rect;
            //mPanel.Children.Add(rect);
        }


        private Dictionary<Point, Size> AllBlocks = new Dictionary<Point, Size>();

        private void RecursiveLinker(string path)
        {

            if (!Directory.Exists(path))
                return;

            string[] fls = Directory.GetFiles(path);
            foreach (var v in fls)
            {
                if (File.ReadAllText(v).Contains("#include"))
                {


                    CreateBlock(MainWindow.ProgramData.GetLastSplash(v), null, StandartPoint, StandartSize);
                    StandartPoint = new Point(StandartPoint.X + (StandartSize.Width+10), -(StandartSize.Height));
                    if (AllBlocks.Where(x => x.Key != StandartPoint && x.Value != StandartSize).Count() <= 0)
                        AllBlocks.Add(StandartPoint, StandartSize);
                    string[] str = File.ReadAllLines(v, MainWindow.ProgramData.Encoding_LTX);
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i].Trim().StartsWith("#include"))
                        {
                            string _link = str[i].Trim().Replace("#include", "").Replace("\"", "");
                            //here is file or folder
                            if (_link.Contains("\\"))
                            {
                                //folder
                               // RecursiveLinker(path);
                            }
                            else
                            {
                                //file

                            }
                        }
                    }
                }
            }
        }

        private void grid_parent_Loaded(object sender, RoutedEventArgs e)
        {
            //load hierarchy
            RecursiveLinker(FolderPath);
        }
    }
}
