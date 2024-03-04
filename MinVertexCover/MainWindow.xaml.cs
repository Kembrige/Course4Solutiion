using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinVertexCover
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int elemCount = 1;
        bool isPoint = true;
        bool haveOneRoad = false;
        Grid PickedRoad;
        List<Point1> points = new List<Point1>();
      



        public MainWindow()
        {
            InitializeComponent();
        }

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isPoint)
            {
                var coord = e.GetPosition(sender as Canvas);
                var canvas = sender as Canvas;
                var grid = new Grid()
                {
                    Width = 50,
                    Height = 50,
                };
                var ell = new Ellipse()
                {
                    Fill = Brushes.Chocolate,
                    Stroke = Brushes.Chocolate,
                };
                var txtblock = new TextBlock()
                {
                    Text = this.elemCount++.ToString(),
                    Margin = new Thickness(20, 16, 0, 0)
                };
                grid.Children.Add(ell);
                grid.Children.Add(txtblock);
                Canvas.SetLeft(grid, coord.X);
                Canvas.SetTop(grid, coord.Y);
                grid.MouseDown += PointMouseDown;
                canvas.Children.Add(grid);
            }
        }

        private void PointMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isPoint)
            {
                if (haveOneRoad)
                {
                    var finishPoint = sender as Grid;
                    var startX = Canvas.GetLeft(PickedRoad);
                    var startY = Canvas.GetTop(PickedRoad);
                    var finishX = Canvas.GetLeft(finishPoint);
                    var finishY = Canvas.GetTop(finishPoint);

                    var line = new Line()
                    {
                        X1 = startX + 10,
                        Y1 = startY + 10,
                        X2 = finishX + 10,
                        Y2 = finishY + 10,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                    };

                    var finishPointnumber = Int32.Parse((finishPoint.Children[1] as TextBlock).Text);
                    var startPointnumber = Int32.Parse((PickedRoad.Children[1] as TextBlock).Text);

                    var existedFinishPoint = this.points.Where(x => x.number == finishPointnumber).FirstOrDefault();
                    var existedStartPoint = this.points.Where(x => x.number == startPointnumber).FirstOrDefault();

                    if (existedFinishPoint != null)
                    {
                        existedFinishPoint.connections.Add(startPointnumber);
                    }
                    else
                    {
                        var newPoint = new Point1(finishPointnumber);
                        newPoint.connections.Add(startPointnumber);
                        this.points.Add(newPoint);
                    }
                    if (existedStartPoint != null)
                    {
                        existedStartPoint.connections.Add(finishPointnumber);
                    }
                    else
                    {
                        var newPoint = new Point1(startPointnumber);
                        newPoint.connections.Add(finishPointnumber);
                        this.points.Add(newPoint);
                    }

                    CanvasPoints.Children.Add(line);
                    haveOneRoad = false;
                    List<Grid> points = new List<Grid>();
                    for (int i = 0; i < CanvasPoints.Children.Count; i++)
                    {
                        if (CanvasPoints.Children[i] is Grid)
                        {
                            points.Add(CanvasPoints.Children[i] as Grid);
                        }
                    }
                    foreach (var item in points)
                    {
                        CanvasPoints.Children.Remove(item);
                        CanvasPoints.Children.Add(item);
                    }
                }
                else
                {
                    PickedRoad = sender as Grid;
                    haveOneRoad = true;
                }
            }
        }

        private void Edge(object sender, RoutedEventArgs e)
        {
            isPoint = true;

        }

        private void Vertex(object sender, RoutedEventArgs e)
        {
            isPoint = false;

        }

      

        private void Clean(object sender, RoutedEventArgs e)
        {
            CanvasPoints.Children.Clear();
            elemCount = 1;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var str = MinVertexCover.Count.Main(points);
            MessageBox.Show(str);

        }

        private void Upload(object sender, RoutedEventArgs e)
        {
            elemCount = 1;
            var fd = new OpenFileDialog();
            fd.Filter = "*.xaml|*.xaml";
            if (fd.ShowDialog() == false)
            {
                MessageBox.Show("Выбран неверный файл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var file = fd.FileName;
            FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
            var savedCanvas = XamlReader.Load(fs) as CanvasStruct;
            fs.Close();
            CanvasPoints.Children.Clear();
            var ch = new List<UIElement>();
            foreach (UIElement child in savedCanvas.Canvas.Children)
            {
                ch.Add(child);
            };
            savedCanvas.Canvas.Children.Clear();
            foreach (var child in ch)
            {
                child.MouseDown += PointMouseDown;
                CanvasPoints.Children.Add(child);
            }
            foreach (var p in savedCanvas.Points)
            {
                elemCount++;
                this.points.Add(new MinVertexCover.Point1(p));
            }
            var asusdh = 0;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var fileName = string.Empty;
            var sd = new SaveFileDialog();
            sd.DefaultExt = "xaml";
            if (sd.ShowDialog() == true)
            {
                fileName = sd.FileName;
            }
            if (fileName == string.Empty)
                return;
            FileStream fs = File.Open(fileName, FileMode.Create);

            var ps = new ListOfPoint();
            foreach (var p in this.points)
            {
                var cn = p.connections.ToArray();
                var np = new PointXAML()
                {
                    Number = p.number,
                    Connections = cn,
                };
                ps.Add(np);
            }
            var CanvasStruct = new CanvasStruct()
            {
                Canvas = CanvasPoints,
                Points = ps,
            };

            XamlWriter.Save(CanvasStruct, fs);
            fs.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
        MessageBox.Show("Программа минимального вершинного покрытия графа. \n\n " +
        "\n\n\t Кнопка \"Добавить вершину\" позволяет расположить вершины на рабочей области" +
        "\n\n\t Кнопка \"Грань\" позволяет при нажатии сначала на родительскую вершину, затем на нужную вам создать между ними грань" +
        "\n\n\t Кнопка \"Решение\" выдаёт в новом окне решение в виде номеров вершин, составляющих минимальное вершинное покрытие данного графа"+
        "\n\n\t Кнопка \"Загрузить\"Открывает проводник, позволяет выбрать готовый граф в виде .xaml файла на диске "+
        "\n\n\t Кнопка \"Сохранить\"Сохраняет готовый граф на вашем ПК, при сохранении необходимо указать расширение .xaml после имени файла  ");
        }
    }
}
