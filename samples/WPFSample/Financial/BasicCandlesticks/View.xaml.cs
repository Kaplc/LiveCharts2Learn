using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;

namespace WPFSample.Financial.BasicCandlesticks;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        //MyChart.Tooltip = null;
        var y = MyChart.YAxes.FirstOrDefault();
        y.MaxLimit = 3000;
    }

    private Point? _lastMousePosition;

    private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _lastMousePosition = e.GetPosition((UIElement)sender);
        }
    }

    private void Chart_MouseMove(object sender, MouseEventArgs e)
    {
        if (_lastMousePosition.HasValue && e.LeftButton == MouseButtonState.Pressed)
        {
            var chart = sender as CartesianChart;
            if (chart == null) return;

            var currentMousePosition = e.GetPosition(chart);

            // 计算鼠标拖动偏移量
            var deltaX = currentMousePosition.X - _lastMousePosition.Value.X;
            var deltaY = currentMousePosition.Y - _lastMousePosition.Value.Y;

            // 获取轴
            var xAxis = chart.XAxes.FirstOrDefault();
            var yAxis = chart.YAxes.FirstOrDefault();

            if (xAxis != null)
            {
                var xRange = (xAxis.MaxLimit ?? 0) - (xAxis.MinLimit ?? 0);
                var xShift = deltaX * xRange / chart.ActualWidth;

                xAxis.MinLimit -= xShift;
                xAxis.MaxLimit -= xShift;
            }

            if (yAxis != null)
            {
                var yRange = (yAxis.MaxLimit ?? 0) - (yAxis.MinLimit ?? 0);
                var yShift = deltaY * yRange / chart.ActualHeight;

                yAxis.MinLimit += yShift;
                yAxis.MaxLimit += yShift;
            }

            _lastMousePosition = currentMousePosition;
        }
    }

    private void Chart_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _lastMousePosition = null;

        var chart = sender as CartesianChart;
        if (chart == null) return;

        // 锁定范围，防止回弹
        var xAxis = chart.XAxes.FirstOrDefault();
        var yAxis = chart.YAxes.FirstOrDefault();

        if (xAxis != null)
        {
            xAxis.MinLimit = xAxis.MinLimit; // 手动锁定当前范围
            xAxis.MaxLimit = xAxis.MaxLimit;
        }

        if (yAxis != null)
        {
            yAxis.MinLimit = yAxis.MinLimit;
            yAxis.MaxLimit = yAxis.MaxLimit;
        }
    }

    private void MyChart_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        // 获取 X 轴
        var xAxis = MyChart.XAxes.FirstOrDefault();
        if (xAxis == null) return;

        // 当前 X 轴的最小和最大范围
        double min = xAxis.MinLimit ?? 0;
        double max = xAxis.MaxLimit ?? 30; // 默认的最大范围

        // 滚轮缩放因子（每次缩放 10%）
        const double zoomFactor = 0.1;
        double range = max - min;

        if (e.Delta > 0) // 放大
        {
            min += range * zoomFactor;
            max -= range * zoomFactor;
        }
        else if (e.Delta < 0) // 缩小
        {
            min -= range * zoomFactor;
            max += range * zoomFactor;
        }

        // 防止范围过小或过大
        if (max - min < 1) return;

        // 更新 X 轴范围
        xAxis.MinLimit = min;
        xAxis.MaxLimit = max;
    }

}
