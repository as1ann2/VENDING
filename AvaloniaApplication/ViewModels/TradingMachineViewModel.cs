using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AvaloniaApplication.Models;
using ReactiveUI;

namespace AvaloniaApplication.ViewModels;

public class TradingMachineViewModel : ReactiveObject
{
    public ObservableCollection<VendingMachine> Items { get; } = new();
    public ObservableCollection<VendingMachine> PagedItems { get; } = new();
    public int TotalCount => FilteredItems.Count();


    private string? _nameFilter;
    public string? NameFilter
    {
        get => _nameFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _nameFilter, value);
            CurrentPage = 1;
            LoadPage();
        }
    }

    public ObservableCollection<int> PageSizes { get; } = new() { 5, 10, 20 };

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            this.RaiseAndSetIfChanged(ref _pageSize, value);
            CurrentPage = 1;
            LoadPage();
        }
    }

    private int _currentPage = 1;
    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public string RecordsInfo =>
        $"Показано {PagedItems.Count} из {FilteredItems.Count()} записей";

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }

    public TradingMachineViewModel()
    {
        // DEMO DATA
        for (int i = 1; i <= 15; i++)
        {
            Items.Add(new VendingMachine
            {
                MachineId = i,
                SerialNumber = $"SN-{i:000}",
                InventoryNumber = $"INV-{i:000}",
                Model = "VM-100",
                Location = "Москва",
                Status = i % 2 == 0 ? "Активен" : "Отключен"
            });
        }

        EditCommand = ReactiveCommand.Create<VendingMachine>(m => { });
        DeleteCommand = ReactiveCommand.Create<VendingMachine>(m => { });

        PrevPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadPage();
            }
        });

        NextPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadPage();
            }
        });

        LoadPage();
    }

    private IEnumerable<VendingMachine> FilteredItems =>
        string.IsNullOrWhiteSpace(NameFilter)
            ? Items
            : Items.Where(x =>
                x.Model.Contains(NameFilter, StringComparison.OrdinalIgnoreCase) ||
                x.Location.Contains(NameFilter, StringComparison.OrdinalIgnoreCase));

    private int TotalPages =>
        (int)Math.Ceiling((double)FilteredItems.Count() / PageSize);

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
