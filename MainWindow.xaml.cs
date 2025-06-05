using System.Windows;
using System.Windows.Controls;

namespace ConfigurationManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ConfigDialog _configDialog;

    public MainWindow()
    {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        // 初始化Factory下拉框
        cmbFactory.Items.Clear();
        cmbFactory.Items.Add("A");
        cmbFactory.Items.Add("B");
        cmbFactory.Items.Add("ALL");
        cmbFactory.SelectedIndex = 0;

        // 初始化DB下拉框
        cmbDB.Items.Clear();
        cmbDB.Items.Add("MESDB");
        cmbDB.Items.Add("GUYUDB");
        cmbDB.Items.Add("NULL");
        cmbDB.SelectedIndex = 0;
    }

    private void BtnConfig_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _configDialog = new ConfigDialog();
            string selectedFactory = cmbFactory.SelectedItem?.ToString() ?? "A";
            string selectedDB = cmbDB.SelectedItem?.ToString() ?? "NULL";
            
            _configDialog.SetDBSelection(selectedDB); // 先设置DB，因为它会影响控件状态
            _configDialog.SetFactorySelection(selectedFactory);
            
            if (_configDialog.ShowDialog() == true)
            {
                MessageBox.Show("配置已保存", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"打开配置窗口错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CmbFactory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Factory选择变更的处理逻辑
    }
}