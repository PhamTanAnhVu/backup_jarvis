using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Jarvis_Windows.Sources.MVVM.Views.PromptLibrary
{
    public class MyPromptViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<Prompt>? _listPromptItems;
        #endregion

        #region Properties
        public ObservableCollection<MyPromptItem>? MyPromptItems { get; set; }
        #endregion

        public MyPromptViewModel()
        {
            InitMyPromptItems();
            SetupMyPromptsList();
        }

        private void SetupMyPromptsList()
        {
            //Mockup data
            //Real: read from local database or API
            _listPromptItems = new ObservableCollection<Prompt>()
            {
                new Prompt("Brainstorm", new PromptCategory(0, ""), "", "Brainstorm 1 story about [TOPIC] for [CUSTOMER]", "en")
            };
        }

        private void InitMyPromptItems()
        {
            MyPromptItems = new ObservableCollection<MyPromptItem>()
            {
                new MyPromptItem()
                {
                    Index = 0,
                    Title = "Brainstorm",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 1, 
                    Title = "Brainstorm 1",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 2,
                    Title = "Brainstorm 2",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 3,
                    Title = "Brainstorm 3",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 4,
                    Title = "Brainstorm 4",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 5,
                    Title = "Brainstorm 5",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 6,
                    Title = "Brainstorm 6",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 7,
                    Title = "Brainstorm 7",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 8,
                    Title = "Brainstorm 8",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 9,
                    Title = "Brainstorm 9",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 10,
                    Title = "Brainstorm 10",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 11,
                    Title = "Brainstorm 11",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                },
                new MyPromptItem()
                {
                    Index = 12,
                    Title = "Brainstorm 12",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    EditCommand = new RelayCommand(o => { MessageBox.Show("Show prompt edit", "Edit Prompt"); }, o => true),
                    DeleteCommand = new RelayCommand(o => { MessageBox.Show("Show prompt delete warning", "Delete Prompt"); }, o => true)
                }
            };
        }
    }
}