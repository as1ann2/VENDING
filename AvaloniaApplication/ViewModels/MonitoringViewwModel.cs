using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using AvaloniaApplication.Models;

namespace AvaloniaApplication.ViewModels
{
    public class MonitoringViewModel : ReactiveObject
    {
        // Коллекция всех торговых автоматов
        public ObservableCollection<VendingMachine> Items { get; } = new ObservableCollection<VendingMachine>();

        // Коллекция для отображения на текущей странице
        public ObservableCollection<VendingMachine> PagedItems { get; } = new ObservableCollection<VendingMachine>();

        // Всего найдено
        public int TotalCount => FilteredItems.Count();

        // Фильтр по имени
        private string? _nameFilter;
        public string? NameFilter
        {
            get => _nameFilter;
            set
            {
                this.RaiseAndSetIfChanged(ref _nameFilter, value);
                CurrentPage = 1; // Сбрасываем на первую страницу при изменении фильтра
                LoadPage();
            }
        }

        // Размер страницы
        public ObservableCollection<int> PageSizes { get; } = new ObservableCollection<int> { 5, 10, 20 };
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                this.RaiseAndSetIfChanged(ref _pageSize, value);
                CurrentPage = 1; // Сбрасываем на первую страницу
                LoadPage();
            }
        }

        // Текущая страница
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        // Информация о записи
        public string RecordsInfo => $"Показано {PagedItems.Count} из {FilteredItems.Count()} записей";

        // Команды для кнопок "Применить" и "Очистить"
        public ICommand ApplyFilterCommand { get; }
        public ICommand ClearFilterCommand { get; }

        // Конструктор ViewModel
        public MonitoringViewModel()
        {
            // Заполняем данные для примера
            for (int i = 1; i <= 15; i++)
            {
                Items.Add(new VendingMachine
                {
                    MachineId = i,
                    SerialNumber = $"SN-{i:000}",
                    Location = "Москва",
                    Model = "VM-100",
                    Status = i % 2 == 0 ? "Активен" : "Отключен"
                });
            }

            ApplyFilterCommand = ReactiveCommand.Create(LoadPage); // Применить фильтры
            ClearFilterCommand = ReactiveCommand.Create(() =>
            {
                NameFilter = string.Empty; // Очищаем фильтр
                LoadPage();
            });

            LoadPage(); // Инициализация отображения первой страницы
        }

        // Отфильтрованные элементы
        private IEnumerable<VendingMachine> FilteredItems =>
            string.IsNullOrWhiteSpace(NameFilter)
                ? Items
                : Items.Where(x =>
                    x.SerialNumber.Contains(NameFilter, StringComparison.OrdinalIgnoreCase) ||
                    x.Location.Contains(NameFilter, StringComparison.OrdinalIgnoreCase));

        // Количество страниц
        private int TotalPages => (int)Math.Ceiling((double)FilteredItems.Count() / PageSize);

        // Загрузка данных для текущей страницы
        private void LoadPage()
        {
            PagedItems.Clear();
            foreach (var item in FilteredItems
                         .Skip((CurrentPage - 1) * PageSize)
                         .Take(PageSize))
            {
                PagedItems.Add(item);
            }
            this.RaisePropertyChanged(nameof(RecordsInfo));
            this.RaisePropertyChanged(nameof(TotalCount));
        }
    }
}
