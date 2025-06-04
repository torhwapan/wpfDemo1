# Configuration Manager

一个基于WPF的配置管理工具，用于管理工厂和数据库配置。

## 功能特点

- Factory配置管理
  - 支持选择A、B、ALL三种工厂类型
  - 当选择非ALL类型时，需要填写factory!=All的原因

- DB配置管理
  - 支持选择是否关联DB配置
  - 不关联时需要填写原因
  - 关联时可以选择是否已上线
  - 已上线时可以选择具体的DB配置

## 技术栈

- .NET 8.0
- WPF
- C#

## 运行要求

- Windows操作系统
- .NET 8.0 SDK或更高版本

## 如何运行

1. 克隆仓库
2. 使用Visual Studio或命令行打开解决方案
3. 运行项目

```bash
dotnet restore
dotnet build
dotnet run
```

## 界面预览

- 主界面包含Factory选择和配置按钮
- 配置对话框支持多种配置选项
- 所有必填项都有清晰的标记
- 界面布局合理，支持后续扩展 