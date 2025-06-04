using System.Windows;
using System.Windows.Controls;

namespace ConfigurationManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        cmbFactory.SelectedIndex = 0; // 默认选择第一项
    }

    private void CmbFactory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 空实现，保持事件处理器存在
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var configDialog = new ConfigDialog();
            configDialog.Owner = this;
            
            var selectedItem = cmbFactory.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                configDialog.SetFactorySelection(selectedItem.Content.ToString());
            }
            
            var result = configDialog.ShowDialog();
            if (result == true)
            {
                MessageBox.Show("配置已保存", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}