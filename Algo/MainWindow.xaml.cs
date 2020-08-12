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
        //bool for the pause button && extra functionallity
        bool isPaused;

        //current array of numbers (the one being shown)
        int[] arr;

        //currently highlighted indexes
        int[] selectedArr;

        //all sorting steps (arrays of numbers)
        List<int[]> sortHistory;

        //all highlighted indexes during the sorting steps
        List<int[]> selectedHistory;

        //timer that we'll use when drawing the array
        DispatcherTimer timer;

        //for custom arrays
        bool isPremade;

        //how many comparisons were needed for the sort in total
        long comparisons;

        //how many array accesses were needed for the sort
        long arrAccesses;

        public MainWindow()
        {
            InitializeComponent();
            arr = new int[(int)numslider.Value];
        }


        void Visualize_Click(object sender, RoutedEventArgs e)
        {
            //initialize everything
            if (timer != null) { timer.Stop(); }
            isPaused = false;
            comparisons = 0;
            arrAccesses = 0;
            selectedHistory = new List<int[]>();
            sortHistory = new List<int[]>();
            ResetPauseButtonText();

            //create a random starting array, if its not already premade
            if (!isPremade)
            {
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
            }
            isPremade = false;

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
                case 5:
                    HeapSort();
                    DrawHistory();
                    break;
                default:
                    MessageBox.Show("You need to select an algorithm.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }

            comparisonsTxt.Text = comparisons.ToString();
            arrayAccTxt.Text = arrAccesses.ToString();
            numsTxt.Text = numslider.Value.ToString();

        }

        bool isLeftButtonDown;
        private void canv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isLeftButtonDown = true;            
        }

        private void canv_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftButtonDown && isPaused && (e.GetPosition(canv).X > 0 && e.GetPosition(canv).X < canv.ActualWidth) && (e.GetPosition(canv).Y > 0 && e.GetPosition(canv).Y < canv.ActualHeight))
            {
                isPremade = true;
                Point a = Mouse.GetPosition(canv);
                arr[(int)Math.Ceiling(a.X / (canv.ActualWidth / arr.Length)) - 1] = arr.Length - (int)Math.Ceiling(a.Y / (canv.ActualHeight / arr.Length));
                DrawNumbers(arr, null);
            }
            else
            {
                isLeftButtonDown = false;
            }
        }

        private void window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isLeftButtonDown = false;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPremade)
            {
                MessageBox.Show("To visualize a premade array click \"Visualize\"", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (timer == null)
            {
                MessageBox.Show("There is no running preview.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (isPaused)
            {
                PlayPreview();
            }
            else
            {
                PausePreview();
            }
        }

        private void PausePreview()
        {
            if(timer != null)
            {
                timer.Stop();
            }
            pauseButton.Content = "Play";
            isPaused = true;
        }

        private void PlayPreview()
        {
            if (timer != null)
            {
                timer.Start();
            }
            pauseButton.Content = "Pause";
            isPaused = false;
        }   

        private void ResetPauseButtonText()
        {
            pauseButton.Content = "Pause";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (arr != null)
                DrawNumbers(arr, selectedArr);
        }

        void AddHistorySnap()
        {
            int[] historySnap = new int[arr.Length];
            arr.CopyTo(historySnap, 0);
            sortHistory.Add(historySnap);
            selectedHistory.Add(selectedArr);
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
                if (selectedHistory != null && selectedHistory.Contains(i))
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
                arr = sortHistory[counter];
                selectedArr = selectedHistory[counter];
                DrawNumbers(sortHistory[counter], selectedHistory[counter]);
                counter++;
                if (counter < sortHistory.Count)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(speedSlider.Value);
                    timer.Start();
                }
                else
                {
                    isPaused = true;
                    timer = null;
                    ResetPauseButtonText();
                }
            };
            timer.Start();
        }

        int[] MergeSort(int startI, int endI)
        {
            int length = endI - startI;
            if (length == 1)
            {
                arrAccesses++;
                return new int[] { arr[startI] };
            }
            int[] A = MergeSort(startI, startI + length / 2);
            int[] B = MergeSort(startI + length / 2, endI);
            int[] AB = new int[A.Length + B.Length];
            int iA = 0;
            int iB = 0;

            for (int i = 0; i < AB.Length; i++)
            {

                arrAccesses += 4;
                comparisons++;
                if (iB < B.Length && (iA == A.Length || B[iB] < A[iA]))
                {
                    comparisons++;
                    if (iA != A.Length)
                    {
                        comparisons++;
                    }
                    AB[i] = B[iB];
                    arr[startI + i] = B[iB];
                    iB++;
                }
                else
                {
                    if (iB < B.Length)
                    {
                        comparisons += 2;
                    }

                    AB[i] = A[iA];
                    arr[startI + i] = A[iA];
                    iA++;
                }
                selectedArr = new int[] { startI + i };
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
                    arrAccesses += 6;

                    comparisons += 2;
                    int oldIValue = arr[curr];
                    arr[curr] = arr[curr - 1];
                    arr[curr - 1] = oldIValue;
                    curr--;
                }

                comparisons++;
                if (curr - 1 >= 0)
                {
                    comparisons++;
                }

                selectedArr = new int[] { curr };
                AddHistorySnap();
            }
        }

        void QuickSort(int startI, int endI)
        {
            comparisons++;
            if (endI - startI < 1)
                return;

            int pI = endI - 1;

            int i = startI;
            int j = startI;

            while (j < endI - 1)
            {
                comparisons++;
                arrAccesses += 2;
                if (arr[j] <= arr[pI])
                {
                    comparisons++;
                    arrAccesses += 4;
                    int oldJ = arr[j];
                    arr[j] = arr[i];
                    arr[i] = oldJ;
                    i++;
                    selectedArr = new int[] { j, i };
                    AddHistorySnap();
                }
                j++;
            }
            comparisons++;

            int oldI = arr[i];
            arr[i] = arr[pI];
            arr[pI] = oldI;
            pI = i;
            arrAccesses += 4;

            selectedArr = new int[] { pI, i };
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
                        arrAccesses += 6;

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
                    arrAccesses += 2;
                    comparisons++;
                }
                int oldI = arr[i];
                arr[i] = arr[minI];
                arr[minI] = oldI;
                arrAccesses += 4;
                selectedArr = new int[] { i };
                AddHistorySnap();

            }
        }

        void HeapSort()
        {

            for (int i = arr.Length / 2 - 1; i >= 0; i--)
            {
                Heapify(i, arr.Length);
            }

            for (int i = arr.Length - 1; i >= 0; i--)
            {
                int oldI = arr[i];
                arr[i] = arr[0];
                arr[0] = oldI;
                arrAccesses += 4;
                Heapify(0, i);
            }

            selectedArr = new int[] { arr.Length - 1 };

            AddHistorySnap();

            void Heapify(int i, int topI)
            {
                int maxI = i;
                int leftChildI = i * 2 + 1;
                int rightChildI = i * 2 + 2;

                comparisons++;
                if (leftChildI < topI)
                {
                    arrAccesses += 2;
                    comparisons++;
                    if (arr[leftChildI] > arr[maxI])
                        maxI = leftChildI;
                }

                comparisons++;
                if (rightChildI < topI)
                {
                    arrAccesses += 2;
                    comparisons++;
                    if (arr[rightChildI] > arr[maxI])
                        maxI = rightChildI;
                }

                comparisons++;
                if (maxI != i)
                {
                    int oldI = arr[i];
                    arr[i] = arr[maxI];
                    arr[maxI] = oldI;

                    arrAccesses += 4;

                    selectedArr = new int[] { i };
                    AddHistorySnap();
                    Heapify(maxI, topI);
                }
            }
        }

        
    }
}
