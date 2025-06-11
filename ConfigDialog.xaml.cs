using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using ConfigurationManager.Services;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Threading;
using System;

namespace ConfigurationManager
{
    public partial class ConfigDialog : Window, INotifyPropertyChanged
    {
        private readonly DbConfigService _dbConfigService;
        private List<DbConfig> _allDbConfigs;
        private string _selectedFactory = "A";
        private string _selectedDB = "MESDB";
        private bool _isFactoryNoteRequired;
        private bool _isDbConfigRequired;
        private bool _isNoDbReasonRequired;
        private bool _isOnlineStatusRequired;
        private bool _isDbSelectionRequired;
        private bool _isInitialized = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsFactoryNoteRequired
        {
            get => _isFactoryNoteRequired;
            private set
            {
                if (_isFactoryNoteRequired != value)
                {
                    _isFactoryNoteRequired = value;
                    OnPropertyChanged(nameof(IsFactoryNoteRequired));
                }
            }
        }

        public bool IsDbConfigRequired
        {
            get => _isDbConfigRequired;
            private set
            {
                if (_isDbConfigRequired != value)
                {
                    _isDbConfigRequired = value;
                    OnPropertyChanged(nameof(IsDbConfigRequired));
                    UpdateRequiredFields();
                }
            }
        }

        public bool IsNoDbReasonRequired
        {
            get => _isNoDbReasonRequired;
            private set
            {
                if (_isNoDbReasonRequired != value)
                {
                    _isNoDbReasonRequired = value;
                    OnPropertyChanged(nameof(IsNoDbReasonRequired));
                }
            }
        }

        public bool IsOnlineStatusRequired
        {
            get => _isOnlineStatusRequired;
            private set
            {
                if (_isOnlineStatusRequired != value)
                {
                    _isOnlineStatusRequired = value;
                    OnPropertyChanged(nameof(IsOnlineStatusRequired));
                }
            }
        }

        public bool IsDbSelectionRequired
        {
            get => _isDbSelectionRequired;
            private set
            {
                if (_isDbSelectionRequired != value)
                {
                    _isDbSelectionRequired = value;
                    OnPropertyChanged(nameof(IsDbSelectionRequired));
                }
            }
        }

        public ConfigDialog()
        {
            try
            {
                _dbConfigService = new DbConfigService();
                InitializeComponent();
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

        private void InitializeDbConfigs()
        {
            if (cmbDbConfigs != null)
            {
                _allDbConfigs = _dbConfigService.GetDbConfigurations();
                cmbDbConfigs.ItemsSource = _allDbConfigs;
                
                // 设置过滤器
                var view = CollectionViewSource.GetDefaultView(cmbDbConfigs.ItemsSource);
                view.Filter = item => FilterDbConfig(item as DbConfig);
                
                // 添加文本变更事件处理
                cmbDbConfigs.AddHandler(TextBox.TextChangedEvent, 
                    new TextChangedEventHandler(CmbDbConfigs_TextChanged));
            }
        }

        private bool FilterDbConfig(DbConfig item)
        {
            if (item == null) return false;
            if (string.IsNullOrEmpty(cmbDbConfigs.Text)) return true;

            return item.Name.Contains(cmbDbConfigs.Text, StringComparison.OrdinalIgnoreCase);
        }

        private void CmbDbConfigs_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var view = CollectionViewSource.GetDefaultView(comboBox.ItemsSource);
                view.Refresh();
                
                // 如果有文本且下拉框未打开，则打开它
                if (!string.IsNullOrEmpty(comboBox.Text) && !comboBox.IsDropDownOpen)
                {
                    comboBox.IsDropDownOpen = true;
                }
            }
        }

        public void SetFactorySelection(string factory)
        {
            if (string.IsNullOrEmpty(factory)) return;
            
            _selectedFactory = factory;
            IsFactoryNoteRequired = factory != "ALL";
            
            // 当Factory=ALL时，禁用Factory备注输入
            if (txtFactoryNote != null)
            {
                txtFactoryNote.IsEnabled = factory != "ALL";
                txtFactoryNote.Text = factory == "ALL" ? "" : txtFactoryNote.Text;
            }
            
            ValidateInputs();
        }

        public void SetDBSelection(string db)
        {
            if (string.IsNullOrEmpty(db)) return;
            
            _selectedDB = db;
            IsDbConfigRequired = db != "NULL";
            
            // 当DB=NULL时，重置并禁用所有DB相关控件
            if (!IsDbConfigRequired)
            {
                if (rbNeedDbNo != null && rbNeedDbYes != null)
                {
                    rbNeedDbNo.IsChecked = false;
                    rbNeedDbYes.IsChecked = false;
                    rbNeedDbNo.IsEnabled = false;
                    rbNeedDbYes.IsEnabled = false;
                }
                
                if (rbIsOnlineNo != null && rbIsOnlineYes != null)
                {
                    rbIsOnlineNo.IsChecked = false;
                    rbIsOnlineYes.IsChecked = false;
                }
                
                if (txtNoDbReason != null)
                {
                    txtNoDbReason.Text = "";
                }

                IsNoDbReasonRequired = false;
                IsOnlineStatusRequired = false;
                IsDbSelectionRequired = false;
            }
            else
            {
                // 启用所有DB配置相关控件
                if (rbNeedDbNo != null && rbNeedDbYes != null)
                {
                    rbNeedDbNo.IsEnabled = true;
                    rbNeedDbYes.IsEnabled = true;
                    rbNeedDbNo.IsChecked = true; // 默认选择"否"
                }

                UpdateRequiredFields();
            }
            
            UpdatePanelVisibility();
            ValidateInputs();
        }

        private void UpdateRequiredFields()
        {
            // 重置所有状态
            IsNoDbReasonRequired = false;
            IsOnlineStatusRequired = false;
            IsDbSelectionRequired = false;

            // 如果DB不为NULL，则"是否关联DB配置"为必填
            if (IsDbConfigRequired)
            {
                bool isNeedDb = rbNeedDbYes?.IsChecked == true;
                
                if (isNeedDb)
                {
                    // 选择"是"时，"是否已上线"为必填
                    IsOnlineStatusRequired = true;
                    
                    // 如果"是否已上线"选择"是"，则"DB配置选择"为必填
                    if (rbIsOnlineYes?.IsChecked == true)
                    {
                        IsDbSelectionRequired = true;
                    }
                }
                else
                {
                    // 选择"否"时，"不关联原因"为必填
                    IsNoDbReasonRequired = true;
                }
            }
        }

        private void UpdatePanelVisibility()
        {
            if (pnlNoDbReason != null && pnlDbConfig != null && pnlDbSelection != null)
            {
                // 如果DB为NULL，隐藏所有DB相关面板
                if (!IsDbConfigRequired)
                {
                    pnlNoDbReason.Visibility = Visibility.Collapsed;
                    pnlDbConfig.Visibility = Visibility.Collapsed;
                    pnlDbSelection.Visibility = Visibility.Collapsed;
                    return;
                }

                // 正常的DB配置逻辑
                bool isNeedDb = rbNeedDbYes?.IsChecked == true;
                pnlNoDbReason.Visibility = !isNeedDb ? Visibility.Visible : Visibility.Collapsed;
                pnlDbConfig.Visibility = isNeedDb ? Visibility.Visible : Visibility.Collapsed;

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

        private void RbNeedDb_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            
            try
            {
                UpdateRequiredFields();
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
                UpdateRequiredFields();
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

        private void CmbDbConfigs_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (!comboBox.IsDropDownOpen)
                {
                    e.Handled = true;
                    comboBox.IsDropDownOpen = true;

                    // 确保显示所有项目
                    var view = CollectionViewSource.GetDefaultView(comboBox.ItemsSource);
                    if (view != null)
                    {
                        view.Refresh();
                    }
                }
            }
        }

        private void ValidateInputs()
        {
            if (!_isInitialized || btnOk == null) return;

            try
            {
                bool isValid = true;

                // 验证Factory备注
                if (IsFactoryNoteRequired && string.IsNullOrWhiteSpace(txtFactoryNote?.Text))
                {
                    isValid = false;
                }

                // 只有在DB配置必填时才验证相关字段
                if (IsDbConfigRequired)
                {
                    if (rbNeedDbNo?.IsChecked == true && string.IsNullOrWhiteSpace(txtNoDbReason?.Text))
                    {
                        isValid = false;
                    }
                    else if (rbNeedDbYes?.IsChecked == true && rbIsOnlineYes?.IsChecked == true && cmbDbConfigs?.SelectedItem == null)
                    {
                        isValid = false;
                    }
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