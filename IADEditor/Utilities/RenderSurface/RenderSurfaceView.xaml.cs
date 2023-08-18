using System;
using System.Windows;
using System.Windows.Controls;

namespace IADEditor.Utilities;

public partial class RenderSurfaceView : UserControl, IDisposable
{
    private RenderSurfaceHost _host = null;
    private bool _disposedValue;
    
    public RenderSurfaceView()
    {
        InitializeComponent();
        Loaded += OnRenderSurfaceViewLoad;
    }

    private void OnRenderSurfaceViewLoad(object sender, RoutedEventArgs e)
    {
        Loaded -= OnRenderSurfaceViewLoad;

        Dispatcher.BeginInvoke(new Action(() =>
        {
            _host = new RenderSurfaceHost(ActualWidth, ActualHeight);
            Content = _host;
        }));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _host.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}