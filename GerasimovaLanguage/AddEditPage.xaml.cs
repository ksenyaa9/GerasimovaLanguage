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


namespace GerasimovaLanguage
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {

        private Client _currentClient = new Client();
        private int edit = 0;
        public AddEditPage(Client SelecradClient)
        {
            InitializeComponent();
            if (SelecradClient != null)
            {
                _currentClient = SelecradClient;
                edit = 1;
                IdPanel.Visibility = Visibility.Visible; // Показываем ID
            }
            else
            {
                // Добавление нового клиента
                _currentClient = new Client();
                _currentClient.RegistrationDate = DateTime.Today;
                IdPanel.Visibility = Visibility.Collapsed; // Скрываем ID

            }
            DataContext = _currentClient;
            SetGenderRadioButtons();
        }

        private void SetGenderRadioButtons()
        {
            if (_currentClient.Gender != null)
            {
                if (_currentClient.Gender.Name == "женский")
                {
                    RBtnF.IsChecked = true;
                }
                else if (_currentClient.Gender.Name == "мужской")
                {
                    RBtnM.IsChecked = true;
                }
            }
            else
            {
                // Устанавливаем женский пол по умолчанию для нового клиента
                if (edit == 0) // Проверяем, что это добавление нового клиента
                {
                    RBtnF.IsChecked = true;
                    _currentClient.Gender = GerasimovaLanguageEntities.GetContext().Gender.FirstOrDefault(g => g.Name == "женский");
                }
                else
                {
                    RBtnF.IsChecked = false;
                    RBtnM.IsChecked = false;
                }
            }
        }

        private void RBtnF_Checked(object sender, RoutedEventArgs e)
        {
            _currentClient.Gender = GerasimovaLanguageEntities.GetContext().Gender.FirstOrDefault(g => g.Name == "женский");
        }

        private void RBtnM_Checked(object sender, RoutedEventArgs e)
        {
            _currentClient.Gender = GerasimovaLanguageEntities.GetContext().Gender.FirstOrDefault(g => g.Name == "мужской");
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(_currentClient.LastName))
            {
                errors.AppendLine("Укажите фамилию");
            }
            else
            {
                if (_currentClient.LastName.Length > 50)
                    errors.AppendLine("Фамилия не может быть длиннее 50 символов");
                if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.LastName, @"^[a-zA-Zа-яА-ЯёЁ\s-]+$"))
                    errors.AppendLine("Фамилия может содержать только буквы, пробелы и дефисы");
            }

            // Проверка имени (аналогично, без else if)
            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
            {
                errors.AppendLine("Укажите имя");
            }
            if (!string.IsNullOrWhiteSpace(_currentClient.FirstName)) // независимая проверка
            {
                if (_currentClient.FirstName.Length > 50)
                    errors.AppendLine("Имя не может быть длиннее 50 символов");
                if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.FirstName, @"^[a-zA-Zа-яА-ЯёЁ\s-]+$"))
                    errors.AppendLine("Имя может содержать только буквы, пробелы и дефисы");
            }

            // Проверка отчества (аналогично)
            if (string.IsNullOrWhiteSpace(_currentClient.Patronymic))
            {
                errors.AppendLine("Укажите отчество");
            }
            if (!string.IsNullOrWhiteSpace(_currentClient.Patronymic))
            {
                if (_currentClient.Patronymic.Length > 50)
                    errors.AppendLine("Отчество не может быть длиннее 50 символов");
                if (!System.Text.RegularExpressions.Regex.IsMatch(_currentClient.Patronymic, @"^[a-zA-Zа-яА-ЯёЁ\s-]+$"))
                    errors.AppendLine("Отчество может содержать только буквы, пробелы и дефисы");
            }

            // Проверка email

            if (string.IsNullOrWhiteSpace(_currentClient.Email))
            {
                errors.AppendLine("Укажите email");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(
                _currentClient.Email,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                errors.AppendLine("Указан некорректный email.");
            }


            // Проверка телефона
            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
            {
                errors.AppendLine("Укажите телефон");
            }
            if (!string.IsNullOrWhiteSpace(_currentClient.Phone) && edit == 0)
            {
                string ph = _currentClient.Phone.Replace("(", "").Replace("-", "").Replace("+", "").Replace(")", "").Replace(" ", "");

                if (ph.Length < 9 || ph.Length > 13)
                {
                    errors.AppendLine("Номер телефона должен содержать от 9 до 13 цифр");
                }
                else if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) ||
                         (ph[1] == '3' && ph.Length != 12))
                {
                    errors.AppendLine("Укажите правильно телефон");
                }
            }

            // Проверка пола
            if (_currentClient.Gender == null)
            {
                errors.AppendLine("Укажите пол клиента");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Сохранение данных
            if (_currentClient.ID == 0)
            {
                GerasimovaLanguageEntities.GetContext().Client.Add(_currentClient);
            }

            try
            {
                GerasimovaLanguageEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }


        }

        public static string GetRelativePath(string basePath, string absolutePath)
        {
            Uri baseUri = new Uri(basePath);
            Uri absoluteUri = new Uri(absolutePath);
            Uri relativeUri = baseUri.MakeRelativeUri(absoluteUri);
            return relativeUri.ToString();
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.InitialDirectory = @"C:\Users\fellk\OneDrive\Рабочий стол\BebkoLaunguage\BebkoLaunguage\Клиенты";
            string clientsDirectory = myOpenFileDialog.InitialDirectory;

            if (myOpenFileDialog.ShowDialog() == true)
            {
                // Получаем полный путь к выбранному файлу
                string fullPath = myOpenFileDialog.FileName;

                // Получаем имя файла
                string fileName = System.IO.Path.GetFileName(fullPath);

                // Формируем путь в формате "Клиенты\имя_файла"
                string relativePath = "Клиенты\\" + fileName;

                // Сохраняем относительный путь
                _currentClient.PhotoPath = relativePath;

                // Устанавливаем изображение в интерфейсе
                PhotoPathImage.Source = new BitmapImage(new Uri(fullPath));
            }
        }
    }
}
