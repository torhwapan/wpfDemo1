using System.Windows;
using System.Windows.Controls;

namespace ConfigurationManager
{
    public partial class ConfigDialog : Window
    {
        private string _selectedFactory = "A";
        private bool _isFactoryNoteRequired;
        private bool _isInitialized = false;

        public ConfigDialog()
        {
            try
            {
                InitializeComponent();
                
                // 初始化控件状态
                InitializeControls();
                
                _isInitialized = true;
                ValidateInputs();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"初始化错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeControls()
        {
            // 初始化DB配置列表
            InitializeDbConfigs();

            // 设置单选按钮初始状态
            if (rbNeedDbNo != null)
            {
                rbNeedDbNo.IsChecked = true;
                rbNeedDbYes.IsChecked = false;
            }

            if (rbIsOnlineNo != null)
            {
                rbIsOnlineNo.IsChecked = true;
                rbIsOnlineYes.IsChecked = false;
            }

            // 设置面板可见性
            UpdatePanelVisibility();
        }

        private void UpdatePanelVisibility()
        {
            if (pnlNoDbReason != null && pnlDbConfig != null && pnlDbSelection != null)
            {
                // 根据是否关联DB配置设置面板可见性
                bool isNeedDb = rbNeedDbYes?.IsChecked == true;
                pnlNoDbReason.Visibility = !isNeedDb ? Visibility.Visible : Visibility.Collapsed;
                pnlDbConfig.Visibility = isNeedDb ? Visibility.Visible : Visibility.Collapsed;

                // 根据是否已上线设置DB选择面板可见性
                if (isNeedDb)
                {
                    pnlDbSelection.Visibility = rbIsOnlineYes?.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    pnlDbSelection.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void SetFactorySelection(string factory)
        {
            if (string.IsNullOrEmpty(factory)) return;
            
            _selectedFactory = factory;
            _isFactoryNoteRequired = factory != "ALL";
            
            if (txtFactoryNoteRequired != null)
            {
                txtFactoryNoteRequired.Visibility = _isFactoryNoteRequired ? Visibility.Visible : Visibility.Collapsed;
            }
            
            ValidateInputs();
        }

        private void InitializeDbConfigs()
        {
            if (cmbDbConfigs != null)
            {
                cmbDbConfigs.Items.Clear();
                cmbDbConfigs.Items.Add("配置1");
                cmbDbConfigs.Items.Add("配置2");
                cmbDbConfigs.Items.Add("配置3");
            }
        }

        private void RbNeedDb_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            
            try
            {
                UpdatePanelVisibility();
                ValidateInputs();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"切换状态错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RbIsOnline_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            
            try
            {
                UpdatePanelVisibility();
                ValidateInputs();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"切换状态错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtFactoryNote_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            ValidateInputs();
        }

        private void TxtNoDbReason_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            ValidateInputs();
        }

        private void CmbDbConfigs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            ValidateInputs();
        }

        private void ValidateInputs()
        {
            if (!_isInitialized || btnOk == null) return;

            try
            {
                bool isValid = true;

                // 验证Factory备注
                if (_isFactoryNoteRequired && string.IsNullOrWhiteSpace(txtFactoryNote?.Text))
                {
                    isValid = false;
                }

                // 验证DB配置
                if (rbNeedDbNo?.IsChecked == true && string.IsNullOrWhiteSpace(txtNoDbReason?.Text))
                {
                    isValid = false;
                }
                else if (rbNeedDbYes?.IsChecked == true && rbIsOnlineYes?.IsChecked == true && cmbDbConfigs?.SelectedItem == null)
                {
                    isValid = false;
                }

                btnOk.IsEnabled = isValid;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"验证错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"保存错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"取消错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 