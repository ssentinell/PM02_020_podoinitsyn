using Microsoft.Win32;
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
using System.IO;

namespace транспортная_задача
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, txtResults.Text);
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            // Очистка стоимостей
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    TextBox tb = (TextBox)this.FindName($"cost{i}{j}");
                    if (tb != null) tb.Text = "";
                }
            }

            // Очистка запасов
            for (int i = 0; i < 5; i++)
            {
                TextBox tb = (TextBox)this.FindName($"supply{i}");
                if (tb != null) tb.Text = "";
            }

            // Очистка потребностей
            for (int j = 0; j < 3; j++)
            {
                TextBox tb = (TextBox)this.FindName($"demand{j}");
                if (tb != null) tb.Text = "";
            }

            // Очистка результатов
            txtResults.Text = "";
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            int rows = 5;
            int cols = 3;

            double[,] costs = new double[rows, cols];
            double[] supply = new double[rows];
            double[] demand = new double[cols];
            double[,] result = new double[rows, cols];
            double totalCost = 0;

            // Считывание и валидация введенных данных
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    TextBox tb = (TextBox)this.FindName($"cost{i}{j}");
                    if (string.IsNullOrWhiteSpace(tb.Text) || !double.TryParse(tb.Text, out costs[i, j]))
                    {
                        txtResults.Text = $"Ошибка ввода в стоимости перевозки от {i + 1} к {j + 1}. Пожалуйста, введите корректное числовое значение.";
                        return;
                    }
                }
            }

            for (int i = 0; i < rows; i++)
            {
                TextBox tb = (TextBox)this.FindName($"supply{i}");
                if (string.IsNullOrWhiteSpace(tb.Text) || !double.TryParse(tb.Text, out supply[i]))
                {
                    txtResults.Text = $"Ошибка ввода в запасах для поставщика {i + 1}. Пожалуйста, введите корректное числовое значение.";
                    return;
                }
            }

            for (int j = 0; j < cols; j++)
            {
                TextBox tb = (TextBox)this.FindName($"demand{j}");
                if (string.IsNullOrWhiteSpace(tb.Text) || !double.TryParse(tb.Text, out demand[j]))
                {
                    txtResults.Text = $"Ошибка ввода в потребностях для потребителя {j + 1}. Пожалуйста, введите корректное числовое значение.";
                    return;
                }
            }

            // Реализация алгоритма северо-западного угла
            int si = 0, sj = 0;
            while (si < rows && sj < cols)
            {
                double amount = Math.Min(supply[si], demand[sj]);
                result[si, sj] = amount;
                totalCost += amount * costs[si, sj];
                supply[si] -= amount;
                demand[sj] -= amount;
                if (supply[si] == 0) si++;
                if (demand[sj] == 0) sj++;
            }

            // Вывод результатов
            txtResults.Text = "Результаты:\n----------------------------\n";
            txtResults.Text += "Опорный план северо-западным методом:\n";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    txtResults.Text += $"{result[i, j]}\t";
                }
                txtResults.Text += "\n";
            }
            txtResults.Text += "----------------------------\n";
            txtResults.Text += $"Общая стоимость: {totalCost}\n";
        }
    }
}
