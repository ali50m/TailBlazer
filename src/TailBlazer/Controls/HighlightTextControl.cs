﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DynamicData.Kernel;
using TailBlazer.Domain.Formatting;

namespace TailBlazer.Controls;

public class HighlightTextControl :Control
{
    public static readonly DependencyProperty HighlightForegroundBrushProperty = DependencyProperty.Register(
        "HighlightForegroundBrush",
        typeof(Brush),
        typeof(HighlightTextControl),
        new PropertyMetadata(default(Brush), UpdateControlCallBack));

    public static readonly DependencyProperty HighlightBackgroundBrushProperty = DependencyProperty.Register(
        "HighlightBackgroundBrush",
        typeof(Brush),
        typeof(HighlightTextControl),
        new PropertyMetadata(default(Brush), UpdateControlCallBack));

    public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register(nameof(FormattedText),
        typeof(IEnumerable<DisplayText>),
        typeof(HighlightTextControl),
        new PropertyMetadata(default(IEnumerable<DisplayText>), UpdateControlCallBack));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text),
        typeof(string),
        typeof(HighlightTextControl),
        new PropertyMetadata(default(string), UpdateControlCallBack));

    public static readonly DependencyProperty HighlightEnabledProperty =
        DependencyProperty.Register(nameof(HighlightEnabled), typeof(bool), typeof(HighlightTextControl),
            new PropertyMetadata(true, UpdateControlCallBack));

    private TextBlock _textBlock;

    public Brush HighlightBackgroundBrush
    {
        get => (Brush) GetValue(HighlightBackgroundBrushProperty);
        set => SetValue(HighlightBackgroundBrushProperty, value);
    }

    public Brush HighlightForegroundBrush
    {
        get => (Brush) GetValue(HighlightForegroundBrushProperty);
        set => SetValue(HighlightForegroundBrushProperty, value);
    }

    public IEnumerable<DisplayText> FormattedText
    {
        get => (IEnumerable<DisplayText>) GetValue(FormattedTextProperty);
        set => SetValue(FormattedTextProperty, value);
    }

    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool HighlightEnabled
    {
        get => (bool) GetValue(HighlightEnabledProperty);
        set => SetValue(HighlightEnabledProperty, value);
    }

    static HighlightTextControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightTextControl),
            new FrameworkPropertyMetadata(typeof(HighlightTextControl)));
    }

    private static void UpdateControlCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var obj = (HighlightTextControl) d;
        obj.InvalidateVisual();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _textBlock = (TextBlock) Template.FindName("PART_TEXT", this);

        //const string sample = "The quick brown fox jumps over the lazy dog";
        //var stringSize = this.MeasureString(sample);
        //var widthPerChar = stringSize.Width / sample.Length;

        ////6.5966
        //Console.WriteLine(widthPerChar);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        _textBlock.Inlines.Clear();
        if(FormattedText == null || !FormattedText.Any())
        {
            if(Text != null)
                _textBlock.Inlines.Add(Text);

            base.OnRender(drawingContext);
            return;
        }

        var formattedText = FormattedText.AsArray();
        if(formattedText.Length == 1)
        {
            var line = formattedText[0];
            _textBlock.Text = line.Text;
        }
        else
        {
            _textBlock.Inlines.AddRange(formattedText.Select(ft =>
            {
                var run = new Run(ft.Text);

                if(ft.Highlight && HighlightEnabled)
                {
                    if(ft.Hue == Hue.NotSpecified)
                    {
                        run.Background = HighlightBackgroundBrush;
                        run.Foreground = HighlightForegroundBrush;
                    }
                    else
                    {
                        run.Background = ft.Hue.BackgroundBrush;
                        run.Foreground = ft.Hue.ForegroundBrush;
                    }
                }

                return run;
            }));
        }

        base.OnRender(drawingContext);
    }
}