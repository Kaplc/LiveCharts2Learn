using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Financial.BasicCandlesticks;

public class ViewModel
{
    public Axis[] XAxes { get; set; }
    public Axis[] YAxes { get; set; }

    public ISeries[] Series { get; set; }

    private int count = 3000;

    public ViewModel()
    {
        var data = new List<FinancialData>();
        var random = new Random();

        // 假设初始的开盘价
        double initialPrice = 500;

        // 设置波动性和趋势
        double volatility = 10; // 每天波动的幅度
        double drift = 0.0001d; // 趋势，比如每天涨0.05%

        // 用于生成随机波动的正态分布
        double mean = 0;  // 平均变化
        double stdDev = 1;  // 标准差

        double price = initialPrice;

        for (int i = 0; i < count; i++)
        {
            // 正态分布生成一个随机数，模拟股价的涨跌
            double randomFactor = mean + stdDev * GetGaussianRandom(random);
            double dailyChange = volatility * (double)randomFactor;

            // 计算当天的价格
            price += price * drift + dailyChange;

            var d = new FinancialData
            {
                High = price + random.Next(1, 5),  // 高点
                Low = price - random.Next(1, 5),   // 低点
                Open = price,                       // 开盘价
                Close = price + dailyChange        // 收盘价
            };

            data.Add(d);
        }

        // 创建金融数据点
        var financialPoints = data.Select(x => new FinancialPointI(x.High, x.Open, x.Close, x.Low)).ToArray();

        // 初始化图表的 Series
        Series = new ISeries[]
        {
            new CandlesticksSeries<FinancialPointI>
            {
                Values = financialPoints, // 确保所有点都被传递
            }
        };

        // 初始化 X 轴
        XAxes = new Axis[]
        {
            new Axis
            {
                LabelsRotation = 0,
                MinLimit = 0, // 初始 X 轴最小值
                MaxLimit = 500, // 初始 X 轴最大值
                MinStep = 1,
            }
        };

        // 初始化 Y 轴
        YAxes = new Axis[]
        {
            new Axis
            {
                Name = "Price", // 给 Y 轴加个名称
                TextSize = 12, // 调整文字大小，确保显示
                MinStep = 1,
            }
        };
    }

    // 生成符合正态分布的随机数（Box-Muller方法）
    private double GetGaussianRandom(Random random)
    {
        double u1 = random.NextDouble();
        double u2 = random.NextDouble();
        double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        return z0;
    }
}

public class FinancialData
{
    public DateTime Date { get; set; }
    public double High { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double Low { get; set; }
}
