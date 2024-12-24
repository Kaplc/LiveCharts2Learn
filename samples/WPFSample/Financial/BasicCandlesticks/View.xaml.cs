using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;

namespace WPFSample.Financial.BasicCandlesticks;

/// <summary>
/// Interaction logic for View.xaml
/// </summary>
public partial class View : UserControl
{
    private ICartesianAxis xAxis;
    private ICartesianAxis yAxis;
    private Point? _lastMousePosition;


    public View()
    {
        InitializeComponent();
        //MyChart.Tooltip = null;
        xAxis = MyChart.XAxes.FirstOrDefault();
        yAxis = MyChart.YAxes.FirstOrDefault();
        yAxis.MinLimit = 0;
        yAxis.MaxLimit = 3000;

        _ = Focus();
    }

    // 处理 KeyDown 事件
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        // o缩小
        if (e.Key == Key.O)
        {
            
        }
        else if (e.Key == Key.P)
        {
        }
    }

    private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Trace.WriteLine(Focus());
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

            Trace.WriteLine("ymin:" + yAxis.MinLimit);
            Trace.WriteLine("ymax:" + yAxis.MaxLimit);
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

    private void YAxisZoomIn(object sender, RoutedEventArgs e)
    {

        if (yAxis == null) return;

        double min = yAxis.MinLimit ?? 0;
        double max = yAxis.MaxLimit ?? 30; // 默认的最大范围

        // 缩放因子（每次缩放 10%）
        const double zoomFactor = 0.1;
        double range = max - min;

        min += range * zoomFactor;
        max -= range * zoomFactor;

        // 防止范围过小或过大
        if (max - min < 1) return;

        yAxis.MinLimit = min;
        yAxis.MaxLimit = max;
    }

    private void YAxisZoomOut(object sender, RoutedEventArgs e)
    {
        if (yAxis == null) return;

        double min = yAxis.MinLimit ?? 0;
        double max = yAxis.MaxLimit ?? 30; // 默认的最大范围

        // 缩放因子（每次缩放 10%）
        const double zoomFactor = 0.1;
        double range = max - min;

        min -= range * zoomFactor;
        max += range * zoomFactor;

        // 防止范围过小或过大
        if (max - min < 1) return;

        yAxis.MinLimit = min;
        yAxis.MaxLimit = max;
    }
}
