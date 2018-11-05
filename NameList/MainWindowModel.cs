using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;
using Microsoft.Win32;

namespace NameList
{
	public class MainWindowModel : NotifyPropertyChangedDispatcherBase
	{
		public MainWindowModel()
		{
			m_rng = new Random();
			m_selectedNames = new List<NameViewModel>();
			m_allNames = new ObservableCollection<NameViewModel>();
			AllNames = new ReadOnlyObservableCollection<NameViewModel>(m_allNames);
			ListWidth = 100;
		}

		public ReadOnlyObservableCollection<NameViewModel> AllNames { get; }

		public string CurrentListFile
		{
			get
			{
				VerifyAccess();
				return m_currentListFile;
			}
			private set
			{
				SetPropertyField(value, ref m_currentListFile);
			}
		}

		public NameViewModel CurrentName
		{
			get
			{
				VerifyAccess();
				return m_currentName;
			}
			set
			{
				SetPropertyField(value, ref m_currentName);
			}
		}

		public double ListWidth
		{
			get
			{
				VerifyAccess();
				return m_listWidth;
			}
			set
			{
				SetPropertyField(value, ref m_listWidth);
			}
		}

		public int MaleCount
		{
			get
			{
				VerifyAccess();
				return m_maleCount;
			}
			set
			{
				SetPropertyField(value, ref m_maleCount);
			}
		}

		public int FemaleCount
		{
			get
			{
				VerifyAccess();
				return m_femaleCount;
			}
			set
			{
				SetPropertyField(value, ref m_femaleCount);
			}
		}

		public void SaveAsNewList()
		{
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = "*.*";
			var result = saveFileDialog.ShowDialog();
			if (result == true)
			{
				CurrentListFile = saveFileDialog.FileName;
				SaveList();
			}
		}

		public void OpenList()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = "*.*";
			var result = openFileDialog.ShowDialog();
			if (result == true)
			{
				var fileName = openFileDialog.FileName;
				var lines = File.ReadAllLines(fileName);
				var names = new List<NameViewModel>();
				int maleCount = 0;
				int femaleCount = 0;
				foreach (var line in lines)
				{
					var name = NameViewModel.Create(line);
					if (name != null)
					{
						names.Add(name);
						if (name.Gender == Gender.Female)
							femaleCount++;
						else
							maleCount++;
					}
				}

				MaleCount = maleCount;
				FemaleCount = femaleCount;
				var sortedNames = names.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
				m_allNames.Clear();
				foreach (var name in sortedNames)
					m_allNames.Add(name);
				CurrentListFile = fileName;
			}
		}

		public void SelectRandomName(Gender gender)
		{
			var count = m_allNames.Count(x => !x.IsUsed && (gender == Gender.Either || x.Gender == gender));
			if (count == 0)
			{
				CurrentName = null;
			}
			else
			{
				var nameIndex = m_rng.Next(0, count);
				CurrentName = m_allNames
					.Where(x => !x.IsUsed && (gender == Gender.Either || x.Gender == gender))
					.Skip(nameIndex)
					.First();
				CurrentName.IsUsed = true;
				Clipboard.SetText(CurrentName.FullName);
				SaveList();
			}
		}

		public void SetSelectedToMale()
		{
			foreach (var name in m_selectedNames)
				name.Gender = Gender.Male;
			SaveList();
		}

		public void SetSelectedToFemale()
		{
			foreach (var name in m_selectedNames)
				name.Gender = Gender.Female;
			SaveList();
		}

		public void SetSelectedNotInUse()
		{
			foreach (var name in m_selectedNames)
				name.IsUsed = false;
			SaveList();
		}

		public void SetSelectedInUse()
		{
			foreach (var name in m_selectedNames)
				name.IsUsed = true;
			SaveList();
		}

		public void SetSelectedNames(IEnumerable<NameViewModel> names)
		{
			m_selectedNames = names.ToList();
		}

		private void SaveList()
		{
			File.WriteAllLines(m_currentListFile, m_allNames.Select(x => x.ToLine()));
		}

		readonly ObservableCollection<NameViewModel> m_allNames;
		readonly Random m_rng;

		string m_currentListFile;
		NameViewModel m_currentName;
		double m_listWidth;
		int m_maleCount;
		int m_femaleCount;
		IReadOnlyCollection<NameViewModel> m_selectedNames;
	}
}
