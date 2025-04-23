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

namespace GerasimovaLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        int itemsPerPage = 10; // Добавлена переменная для хранения выбранного значения
        int totalRecordsCount = 0; // Добавляем переменную для общего количества записей

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;

        public ClientPage()
        {
            InitializeComponent();
            ComboPage.SelectedIndex = 0; // Установка значения по умолчанию

            totalRecordsCount = GerasimovaLanguageEntities.GetContext().Client.Count();

            UpdateClient();

            ComboGender.SelectedIndex = 0;
            ComboType.SelectedIndex = 0;
        }

        private void UpdateClient()
        {
            var currentClient = GerasimovaLanguageEntities.GetContext().Client.ToList();


            if (ComboGender.SelectedIndex == 0) {
                currentClient = currentClient.ToList();
            }


            if (ComboGender.SelectedIndex == 1) {
                
                currentClient = currentClient.Where(p => p.Gender.Name == "женский").ToList();
            }
            if (ComboGender.SelectedIndex == 2)
            {

                currentClient = currentClient.Where(p => p.Gender.Name == "мужской").ToList();
            }

            if(ComboType.SelectedIndex == 0)
            {
                currentClient = currentClient.ToList();
            }

            if(ComboType.SelectedIndex == 1)
            {
                currentClient = currentClient.OrderBy(p => p.LastName).ToList();
            }

            if (ComboType.SelectedIndex == 2)
            {
                currentClient = currentClient.OrderByDescending(p => p.LastVisitDate ?? DateTime.MinValue).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentClient = currentClient.OrderByDescending(p => p.VisitCount).ToList();
            }



            // Приводим текст поиска к нижнему регистру один раз
            string searchText = TBoxSearch.Text.ToLower().Replace("+", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
            // Фильтруем список агентов по всем трем полям одновременно
            currentClient = currentClient
                .Where(p => p.LastName.ToLower().Contains(searchText) ||
                           p.FirstName.ToLower().Contains(searchText) ||
                           p.Patronymic.ToLower().Contains(searchText) ||
                           p.Phone.Replace("+", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Contains(searchText) ||
                           p.Email.ToLower().Contains(searchText))
                .ToList();


            ClientListView.ItemsSource = currentClient.ToList();
            TableList = currentClient;
            ChangePage(0, 0);

        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            // Рассчитываем количество страниц
            if (itemsPerPage == 0) // Режим "Все"
                CountPage = 1;
            else
                CountPage = CountRecords > 0 ? (int)Math.Ceiling((double)CountRecords / itemsPerPage) : 1;

            bool Ifupdate = true;
            int startIndex = 0, endIndex = 0;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage < CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    startIndex = CurrentPage * itemsPerPage;
                    endIndex = itemsPerPage == 0 ?
                        CountRecords :
                        Math.Min(startIndex + itemsPerPage, CountRecords);
                }
            }
            else
            {
                switch (direction)
                {
                    case 1 when CurrentPage > 0:
                        CurrentPage--;
                        break;

                    case 2 when CurrentPage < CountPage - 1:
                        CurrentPage++;
                        break;

                    default:
                        Ifupdate = false;
                        break;
                }

                startIndex = CurrentPage * itemsPerPage;
                endIndex = itemsPerPage == 0 ?
                    CountRecords :
                    Math.Min(startIndex + itemsPerPage, CountRecords);
            }

            if (Ifupdate)
            {
                // Заполняем данные для текущей страницы
                for (int i = startIndex; i < endIndex && i < TableList.Count; i++)
                    CurrentPageList.Add(TableList[i]);

                // Обновляем интерфейс
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                    PageListBox.Items.Add(i);

                if (CountPage > 0)
                    PageListBox.SelectedIndex = CurrentPage;

                // Обновляем информацию о записях
                TBCount.Text = CountRecords.ToString(); // Количество после фильтрации
                TBAllRecords.Text = $" из {totalRecordsCount}"; // Общее количество в базе

                ClientListView.ItemsSource = CurrentPageList;
                ClientListView.Items.Refresh();
            }
        }

        // Обработчики событий
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (TableList != null)
            {
                if (ComboPage.SelectedItem is ComboBoxItem selectedItem)
                {
                    var content = selectedItem.Content.ToString();
                    itemsPerPage = content == "Все" ? 0 : int.Parse(content);
                    CurrentPage = 0;
                    ChangePage(0, 0);
                }
            }
            
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, PageListBox.SelectedIndex);
        }

        // Остальные обработчики остаются без изменений
        private void LeftDirButton_Click(object sender, RoutedEventArgs e) => ChangePage(1, null);
        private void RigtDirButton_Click(object sender, RoutedEventArgs e) => ChangePage(2, null);

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;

            var currentClientServices = GerasimovaLanguageEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ClientID == currentClient.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("НЕвозможно выполнить удаление");
            else
            {


                if (MessageBox.Show("Вы точно хотите удалить?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        GerasimovaLanguageEntities.GetContext().Client.Remove(currentClient);
                        GerasimovaLanguageEntities.GetContext().SaveChanges();
                        ClientListView.ItemsSource = GerasimovaLanguageEntities.GetContext().Client.ToList();
                        UpdateClient();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }



        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClient();
        }

        private void ComboGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Client));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные при каждом появлении страницы
                GerasimovaLanguageEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                totalRecordsCount = GerasimovaLanguageEntities.GetContext().Client.Count();
                UpdateClient();
            }
        }
    }

}
