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
    public class PublicPromptViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<Prompt>? _listPrompts;
        private ObservableCollection<PromptCategory>? _listPromptCategories;
        #endregion

        #region Properties
        public ObservableCollection<PublicPromptItem>? PublicPromptItems { get; set; }
        public ObservableCollection<PromptCategoryItem>? ListCategoriesItem { get; set; }
        #endregion

        #region Commands
        public RelayCommand? FilterFavoriteCommand { get; set; }
        #endregion

        public PublicPromptViewModel()
        {
            InitPublicPromptItems();
            InitPromptCategoriesItem();
            SetupPublicPromptsList();
            SetupPromptCategoriesList();

            FilterFavoriteCommand = new RelayCommand(o => { MessageBox.Show("Filter favorite prompts", "Filter Favorite"); }, o => true);
        }

        private void SetupPromptCategoriesList()
        {
            _listPromptCategories = new ObservableCollection<PromptCategory>()
            {
                new PromptCategory(0, "All"),
                new PromptCategory(1, "Category 1"),
                new PromptCategory(2, "Category 2"),
                new PromptCategory(3, "Category 3"),
                new PromptCategory(4, "Category 4"),
                new PromptCategory(5, "Category 5"),
                new PromptCategory(6, "Category 6"),
                new PromptCategory(7, "Category 7"),
            };
        }

        private void InitPromptCategoriesItem()
        {
            ListCategoriesItem = new ObservableCollection<PromptCategoryItem>()
            {
                new PromptCategoryItem()
                {
                    Index = 0,
                    Name = "All",
                    Command = new RelayCommand(o => { MessageBox.Show("Show all prompts", "All Prompts"); }, o => true)
                },
                new PromptCategoryItem()
                {
                    Index = 1,
                    Name = "Category 1",
                    Command = new RelayCommand(o => { MessageBox.Show("Show category 1 prompts", "Category 1"); }, o => true)
                },
                new PromptCategoryItem()
                {
                    Index = 2,
                    Name = "Category 2",
                    Command = new RelayCommand(o => { MessageBox.Show("Show category 2 prompts", "Category 2"); }, o => true)
                },
                new PromptCategoryItem()
                {
                    Index = 3,
                    Name = "Category 3",
                    Command = new RelayCommand(o => { MessageBox.Show("Show category 3 prompts", "Category 3"); }, o => true)
                },
                new PromptCategoryItem()
                {
                    Index = 4,
                    Name = "Category 4",
                    Command = new RelayCommand(o => { MessageBox.Show("Show category 4 prompts", "Category 4"); }, o => true)
                },
                new PromptCategoryItem()
                {
                    Index = 5,
                    Name = "Category 5",
                    Command = new RelayCommand(o => { MessageBox.Show("Show category 5 prompts", "Category 5"); }, o => true)
                },
                new PromptCategoryItem()
                {
                    Index = 6,
                    Name = "Category 6",
                    Command = new RelayCommand(o => { MessageBox.Show("Show category 6 prompts", "Category 6"); }, o => true)
                },
                new PromptCategoryItem()
                {
                    Index = 7,
                    Name = "Category 7",
                    Command = new RelayCommand(o => { MessageBox.Show("Show category 7 prompts", "Category 7"); }, o => true)
                }
            };
        }

        private void SetupPublicPromptsList()
        {
            _listPrompts = new ObservableCollection<Prompt>()
            {
                new Prompt("Public Prompt", new PromptCategory(0, ""), "", "Brainstorm 1 story about [TOPIC] for [CUSTOMER]", "en")
            };
        }

        private void InitPublicPromptItems()
        {
            PublicPromptItems = new ObservableCollection<PublicPromptItem>()
            {
                new PublicPromptItem()
                {
                    Index = 0,
                    Title = "Public Prompt",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    AddFavoriteCommand = new RelayCommand(o => { MessageBox.Show("Add to favorite", "Add Favorite"); }, o => true),
                    ShowInfoCommand = new RelayCommand(o => { MessageBox.Show("Show prompt info", "Prompt Info"); }, o => true),
                    IsFavorite = false
                },
                new PublicPromptItem()
                {
                    Index = 1,
                    Title = "Public Prompt 1",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    AddFavoriteCommand = new RelayCommand(o => { MessageBox.Show("Add to favorite", "Add Favorite"); }, o => true),
                    ShowInfoCommand = new RelayCommand(o => { MessageBox.Show("Show prompt info", "Prompt Info"); }, o => true),
                    IsFavorite = false
                },
                new PublicPromptItem()
                {
                    Index = 2,
                    Title = "Public Prompt 2",
                    DetailCommand = new RelayCommand(o => { MessageBox.Show("Show prompt detail", "Prompt Detail"); }, o => true),
                    AddFavoriteCommand = new RelayCommand(o => { MessageBox.Show("Add to favorite", "Add Favorite"); }, o => true),
                    ShowInfoCommand = new RelayCommand(o => { MessageBox.Show("Show prompt info", "Prompt Info"); }, o => true),
                    IsFavorite = false
                }
            };
        }
    }
}
