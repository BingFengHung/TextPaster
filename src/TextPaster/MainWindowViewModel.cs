using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace TextPaster
{
    class OptionModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    internal class MainWindowViewModel : ViewModelBase
    {
        List<KeyboardMap> keyboardMaps;
        Sql sql;

        public MainWindowViewModel()
        {
            sql = new Sql();
            sql.IsTableExist("keyboard", out bool isExist);

            if (!isExist)
            {
                sql.CreateTable("keyboard");
                sql.Insert(new KeyboardMap { Key = "1" });
                sql.Insert(new KeyboardMap { Key = "2" });
                sql.Insert(new KeyboardMap { Key = "3" });
                sql.Insert(new KeyboardMap { Key = "4" });
                sql.Insert(new KeyboardMap { Key = "5" });
                sql.Insert(new KeyboardMap { Key = "6" });
                sql.Insert(new KeyboardMap { Key = "7" });
                sql.Insert(new KeyboardMap { Key = "8" });
                sql.Insert(new KeyboardMap { Key = "9" });
            }

            bool isOk = sql.Select(out keyboardMaps);

            if (isOk)
            {
                var list = keyboardMaps.Select(x => new OptionModel { Name = x.Key, Value = x.Key });
                Keys = new ObservableCollection<OptionModel>(list.ToList());
            }

            SelectedKey = Keys[0];
        }

        private ObservableCollection<OptionModel> keys;
        public ObservableCollection<OptionModel> Keys
        {
            get => keys;
            set
            {
                keys = value;
                NotifyPropertyChanged(nameof(Keys));
            }
        }

        private OptionModel selectedKey;
        public OptionModel SelectedKey
        {
            get => selectedKey;
            set
            {
                selectedKey = value;
                RefreshContent(value);
                NotifyPropertyChanged(nameof(SelectedKey));
            }
        }

        private string content;
        public string Content
        {
            get => content;
            set
            {
                content = value;
                NotifyPropertyChanged(nameof(Content));
            }
        }

        private void RefreshContent(OptionModel option)
        {
            var item = keyboardMaps.Find(x => x.Key == option.Value);
            if (item != null)
            {
                Content = item.Text;
            }
        }

        public ICommand SaveCommand => new RelayCommand(() =>
        {
            var item = keyboardMaps[Convert.ToInt32(SelectedKey.Value) - 1];
            if (item != null)
            {
                item.Text = Content;
                sql.Update(item);
            }
        });
    }

    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action execute;

        public RelayCommand(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }
}
