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

namespace Algo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[] arr;
        int[] selectedArr;
        List<int[]> sortHistory;
        List<int[]> selectedHistory;
        DispatcherTimer timer;
        long comparisons;

        public MainWindow()
        {
            InitializeComponent();
        }


        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (timer != null) { timer.Stop(); }
            selectedHistory = new List<int[]>();
            sortHistory = new List<int[]>();
            isPaused = false;
            comparisons = 0;

            Random random = new Random();
            arr = new int[(int)numslider.Value];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i + 1;
            }
            for (int i = 0; i < arr.Length; i++)
            {
                int oldArrItem = arr[i];
                int switchIndex = random.Next(i, arr.Length);
                arr[i] = arr[switchIndex];
                arr[switchIndex] = oldArrItem;
            }

            switch (comboBox.SelectedIndex)
            {
                case 0:
                    MergeSort(0, arr.Length);
                    DrawHistory();
                    break;
                case 1:
                    InsertionSort();
                    DrawHistory();
                    break;
                case 2:
                    QuickSort(0, arr.Length);
                    DrawHistory();
                    break;
                case 3:
                    BubbleSort();
                    DrawHistory();
                    break;
                case 4:
                    SelectionSort();
                    DrawHistory();
                    break;
                default:
                    MessageBox.Show("You need to select an algorithm.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
            txtBlock.Text = comparisons.ToString();

        }

        void AddHistorySnap()
        {
            int[] historySnap = new int[arr.Length];
            arr.CopyTo(historySnap, 0);
            sortHistory.Add(historySnap);
            selectedHistory.Add(selectedArr);
        }

        void DrawLines(int[] arr, int[] selectedIndexes)
        {
            canv.Children.Clear();


            int howMany = (int)arr.Length;
            double size = canv.ActualWidth / howMany;

            for (int i = 0; i < howMany; i++)
            {
                Line liniika = new Line();
                liniika.Stroke = new SolidColorBrush(selectedIndexes.Contains(i) ? Colors.Red : Colors.Black);
                liniika.StrokeThickness = 2;
                liniika.X1 = size / 2 + i * size;
                liniika.X2 = size / 2 + i * size;
                liniika.Y2 = canv.ActualHeight - (canv.ActualHeight - 10) / howMany * arr[i];
                liniika.Y1 = canv.ActualHeight;
                canv.Children.Add(liniika);
            }
        }
        void DrawNumbers(int[] arr, int[] selectedHistory)
        {
            canv.Children.Clear();


            int howMany = (int)arr.Length;
            double size = canv.ActualWidth / howMany;

            for (int i = 0; i < howMany; i++)
            {
                Rectangle rect = new Rectangle();
                Canvas.SetLeft(rect, size * i);
                Canvas.SetBottom(rect, 0);
                rect.Width = size;
                rect.Height = (canv.ActualHeight - 5) / howMany * arr[i];
                if (selectedHistory.Contains(i) || (i < selectedHistory.Max() && i > selectedHistory.Min() ))
                {
                    rect.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    rect.Fill = new SolidColorBrush(Colors.Black);
                }
                canv.Children.Add(rect);
            }
        }
        void DrawHistory()
        {
            int counter = 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(speedSlider.Value);
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                DrawNumbers(sortHistory[counter], selectedHistory[counter]);
                counter++;
                if (counter < sortHistory.Count)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(speedSlider.Value);
                    timer.Start();
                }
            };
            timer.Start();
        }

        int[] MergeSort(int startI, int endI)
        {
            int length = endI - startI;
            if (length == 1)
            {
                return new int[] { arr[startI] };
            }
            int[] A = MergeSort(startI, startI + length / 2);
            int[] B = MergeSort(startI + length / 2, endI);
            int[] AB = new int[A.Length + B.Length];
            int iA = 0;
            int iB = 0;

            for (int i = 0; i < AB.Length; i++)
            {
                comparisons++;
                if (iB < B.Length && (iA == A.Length || B[iB] < A[iA]))
                {
                    AB[i] = B[iB];
                    arr[startI + i] = B[iB];
                    iB++;
                }
                else
                {
                    AB[i] = A[iA];
                    arr[startI + i] = A[iA];
                    iA++;
                }
                selectedArr = new int[] { startI+i};
                AddHistorySnap();
            }

            return AB;
        }

        void InsertionSort()
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int curr = i;
                while (curr - 1 >= 0 && arr[curr - 1] > arr[curr])
                {
                    comparisons++;
                    int oldIValue = arr[curr];
                    arr[curr] = arr[curr - 1];
                    arr[curr - 1] = oldIValue;
                    curr--;
                }
                comparisons++;
                selectedArr = new int[] { curr };
                AddHistorySnap();
            }
        }

        void QuickSort(int startI, int endI)
        {
            if (endI - startI < 1)
                return;

            int pI = endI - 1;

            int i = startI;
            int j = startI;

            while (j < endI - 1)
            {
                comparisons++;
                if (arr[j] <= arr[pI])
                {
                    int oldJ = arr[j];
                    arr[j] = arr[i];
                    arr[i] = oldJ;
                    i++;
                    selectedArr = new int[] { j,i };
                    AddHistorySnap();
                }
                j++;
            }

            int oldI = arr[i];
            arr[i] = arr[pI];
            arr[pI] = oldI;
            pI = i;

            selectedArr = new int[] { pI,i };
            AddHistorySnap();

            QuickSort(startI, pI);
            QuickSort(pI + 1, endI);
        }

        void BubbleSort()
        {
            for (int i = arr.Length; i >= 0; i--)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        int oldJ = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = oldJ;
                    }
                    comparisons++;
                }
                selectedArr = new int[] { i };
                AddHistorySnap();

            }
        }

        void SelectionSort()
        {
            for (int i = 0; i < arr.Length; i++)
            {
                int minI = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[minI] > arr[j])
                    {
                        minI = j;
                    }
                    comparisons++;
                }
                int oldI = arr[i];
                arr[i] = arr[minI];
                arr[minI] = oldI;
                AddHistorySnap();
                selectedArr = new int[] { i };

            }
        }

        void Button_Click_1(object sender, RoutedEventArgs e)
        {


        }

        bool isPaused = false;
        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                timer.Start();
                pauseButton.Content = "Pause";
                isPaused = false;
            }
            else
            {
                if (timer != null)
                {
                    pauseButton.Content = "Play";
                    timer.Stop();
                    isPaused = true;
                }
            }
        }
    }
}
