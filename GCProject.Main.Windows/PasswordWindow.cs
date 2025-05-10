using System.Windows;
using System.Windows.Markup;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public partial class PasswordWindow : Window, IComponentConnector
{
	private bool showPassword;

	public string PromptLable { get; set; } = Lang.PwdWindow_Input;


	public string InputValue { get; set; } = string.Empty;


	public PasswordWindow(string Prompt, string title = "", bool showPassword = false)
	{
		if (string.IsNullOrEmpty(title))
		{
			title = Lang.Password;
		}
		base.Title = title;
		PromptLable = Prompt;
		InitializeComponent();
		base.DataContext = this;
		this.showPassword = showPassword;
		if (this.showPassword)
		{
			tb_PwdInput.Visibility = Visibility.Visible;
			PwdInput.Visibility = Visibility.Collapsed;
		}
		else
		{
			tb_PwdInput.Visibility = Visibility.Collapsed;
			PwdInput.Visibility = Visibility.Visible;
		}
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}

	private void OKClick(object sender, RoutedEventArgs e)
	{
		InputValue = (showPassword ? tb_PwdInput.Text : PwdInput.Password);
		base.DialogResult = true;
	}
}
