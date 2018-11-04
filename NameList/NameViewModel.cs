using System;
using System.Linq;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Windows;

namespace NameList
{
	public sealed class NameViewModel : NotifyPropertyChangedDispatcherBase
	{
		public static NameViewModel Create(string line)
		{
			var parts = line.Split(new [] { ',' }, StringSplitOptions.None);
			if (parts.Length < 3)
				return null;

			var isUsed = parts[0].Length != 0;
			var gender = parts[1] == "f" ? Gender.Female : Gender.Male;
			var nameParts = parts.Skip(2).Join(",").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var fullName = nameParts.Join(" ");
			var firstName = nameParts[0];
			var lastName = nameParts.Skip(1).Join(" ");
			return new NameViewModel(fullName, firstName, lastName, gender, isUsed);
		}

		public string FullName { get; }
		public string FirstName { get; }
		public string LastName { get; }

		public Gender Gender
		{
			get
			{
				VerifyAccess();
				return m_gender;
			}
			set
			{
				SetPropertyField(value, ref m_gender);
			}
		}

		public bool IsUsed
		{
			get
			{
				VerifyAccess();
				return m_isUsed;
			}
			set
			{
				SetPropertyField(value, ref m_isUsed);
			}
		}

		public string ToLine()
		{
			return $"{(m_isUsed ? "-" : "")},{(m_gender == Gender.Female ? "f" : "m")},{FullName}";
		}

		private NameViewModel(string fullName, string firstName, string lastName, Gender gender, bool isUsed)
		{
			FullName = fullName;
			FirstName = firstName;
			LastName = lastName;
			Gender = gender;
			m_isUsed = isUsed;
		}

		bool m_isUsed;
		Gender m_gender;
	}
}