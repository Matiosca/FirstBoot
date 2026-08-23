using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FirstBoot.Models;

namespace FirstBoot;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        UpdateSelectedApplicationSummary();
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var category = button.Tag.ToString();

        ApplicationsPanel.Children.Clear();
        ApplicationsPanel.Children.Add(new TextBlock
        {
            Text = $"{category?.ToUpperInvariant()} APPLICATIONS",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 15)
        });

        foreach (var application in ApplicationCatalog.All.Where(app => app.Category == category))
        {
            var applicationCheckBox = new CheckBox
            {
                Content = application.Name,
                Tag = application,
                IsChecked = application.IsSelected,
                Height = 40,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };

            applicationCheckBox.Click += ApplicationSelection_Click;
            ApplicationsPanel.Children.Add(applicationCheckBox);
        }
    }

    private void ApplicationSelection_Click(object sender, RoutedEventArgs e)
    {
        var checkBox = (CheckBox)sender;
        var application = (ApplicationModel)checkBox.Tag;
        application.IsSelected = checkBox.IsChecked == true;

        UpdateSelectedApplicationSummary();
        ShowApplicationInformation(application);
    }

    private void ShowApplicationInformation(ApplicationModel application)
    {
        InformationPanel.Children.Clear();
        InformationPanel.Children.Add(new TextBlock
        {
            Text = application.Name,
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12)
        });
        InformationPanel.Children.Add(new TextBlock
        {
            Text = application.Description,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 16)
        });

        if (!string.IsNullOrWhiteSpace(application.Publisher))
        {
            InformationPanel.Children.Add(new TextBlock { Text = "PUBLISHER", FontWeight = FontWeights.Bold });
            InformationPanel.Children.Add(new TextBlock { Text = application.Publisher, Margin = new Thickness(0, 0, 0, 12) });
        }

        InformationPanel.Children.Add(new TextBlock { Text = "WINGET PACKAGE ID", FontWeight = FontWeights.Bold });
        InformationPanel.Children.Add(new TextBlock
        {
            Text = application.WingetId,
            FontFamily = new FontFamily("Consolas"),
            TextWrapping = TextWrapping.Wrap
        });
    }

    private void UpdateSelectedApplicationSummary()
    {
        var selectedCount = ApplicationCatalog.All.Count(app => app.IsSelected);
        SelectedApplicationsText.Text = $"{selectedCount} application{(selectedCount == 1 ? string.Empty : "s")} selected";
        InstallSelectedButton.IsEnabled = selectedCount > 0 && _installationCancellation is null;
    }
}
