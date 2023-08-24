using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using IADEditor.Utilities.Enums;

namespace IADEditor.Utilities;

public partial class RenderSurfaceView : UserControl, IDisposable
{
    private RenderSurfaceHost _host = null;
    private bool _disposedValue;
    private bool _canResize = true;
    private bool _moved = false;
    
    public RenderSurfaceView()
    {
        InitializeComponent();
        Loaded += OnRenderSurfaceViewLoad;
    }

    private void OnRenderSurfaceViewLoad(object sender, RoutedEventArgs e)
    {
        Loaded -= OnRenderSurfaceViewLoad;
        
        _host = new RenderSurfaceHost(ActualWidth, ActualHeight);
        _host.MessageHook += new HwndSourceHook(HostMsgFilter);
        Content = _host;

        var window = this.FindVisualParent<Window>();
        Debug.Assert(window != null);
        var helper = new WindowInteropHelper(window);

        if (helper.Handle != null)
        {
            HwndSource.FromHwnd(helper.Handle)?.AddHook(HwndMessageHook);
        }
    }

    private IntPtr HwndMessageHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        switch ((Win32Msg)msg)
        {
            case Win32Msg.WM_ENTERSIZEMOVE:
                _moved = true;
                break;
            case Win32Msg.WM_EXITSIZEMOVE:
                _canResize = true;
                if (!_moved) { _host.Resize(); }
                break;
            case Win32Msg.WM_SIZING:
                _moved = false;
                _canResize = false;
                break;
        }
        
        return IntPtr.Zero;
    }

    private IntPtr HostMsgFilter(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        switch ((Win32Msg)msg)
        {
            case Win32Msg.WM_SIZE:
                if (_canResize) { _host.Resize(); }
                break;
            case Win32Msg.WM_ENTERSIZEMOVE:
                throw new Exception();
            case Win32Msg.WM_EXITSIZEMOVE:
                throw new Exception();
            case Win32Msg.WM_SIZING:
                throw new Exception();
        }
        
        return IntPtr.Zero;
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