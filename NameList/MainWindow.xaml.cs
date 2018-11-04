using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NameList
{
	public partial class MainWindow : Window
	{
		public MainWindow(MainWindowModel model)
		{
			ViewModel = model;

			InitializeComponent();
		}

		public MainWindowModel ViewModel { get; }

		void NameList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.SetSelectedNames(((ListBox) sender).SelectedItems.Cast<NameViewModel>());
		}
	}
}
