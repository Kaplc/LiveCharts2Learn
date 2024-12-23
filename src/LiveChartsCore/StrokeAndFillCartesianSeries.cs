﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;

namespace LiveChartsCore;

/// <summary>
/// Defines a data series with stroke and fill properties.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="StrokeAndFillCartesianSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
/// </remarks>
/// <param name="properties">The properties.</param>
/// <param name="values">The values.</param>
public abstract class StrokeAndFillCartesianSeries<TModel, TVisual, TLabel, TDrawingContext>(
    SeriesProperties properties,
    ICollection<TModel>? values)
        : CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>(properties, values)
            where TDrawingContext : DrawingContext
            where TVisual : class, IGeometry<TDrawingContext>, new()
            where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private IPaint<TDrawingContext>? _stroke = null;
    private IPaint<TDrawingContext>? _fill = null;

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    /// <value>
    /// The stroke.
    /// </value>
    public IPaint<TDrawingContext>? Stroke
    {
        get => _stroke;
        set => SetPaintProperty(ref _stroke, value, true);
    }

    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    /// <value>
    /// The fill.
    /// </value>
    public IPaint<TDrawingContext>? Fill
    {
        get => _fill;
        set => SetPaintProperty(ref _fill, value);
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.GetPaintTasks"/>
    protected internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return [_stroke, _fill, DataLabelsPaint];
    }

    /// <summary>
    /// Gets the fill paint for the miniature.
    /// </summary>
    /// <param name="point">the point/</param>
    /// <param name="zIndex">the x index.</param>
    /// <returns></returns>
    protected virtual IPaint<TDrawingContext>? GetMiniatureFill(ChartPoint? point, int zIndex)
    {
        var p = point is null ? null : ConvertToTypedChartPoint(point);
        var paint = p?.Visual?.Fill ?? Fill;

        return GetMiniaturePaint(paint, zIndex);
    }

    /// <summary>
    /// Gets the fill paint for the miniature.
    /// </summary>
    /// <param name="point">the point/</param>
    /// <param name="zIndex">the x index.</param>
    /// <returns></returns>
    protected virtual IPaint<TDrawingContext>? GetMiniatureStroke(ChartPoint? point, int zIndex)
    {
        var p = point is null ? null : ConvertToTypedChartPoint(point);
        var paint = p?.Visual?.Stroke ?? Stroke;

        return GetMiniaturePaint(paint, zIndex);
    }
}
