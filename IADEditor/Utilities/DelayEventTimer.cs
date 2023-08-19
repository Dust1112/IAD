using System;
using System.Windows.Threading;

namespace IADEditor.Utilities;

public class DelayEventTimer
{
    private readonly DispatcherTimer _timer;
    private readonly TimeSpan _delay;
    private DateTime _lastEventTime = DateTime.Now;
    private object _data;

    public event EventHandler<DelayEventTimerArgs> Triggered;

    public DelayEventTimer(TimeSpan delay, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        _delay = delay;
        _timer = new DispatcherTimer(priority)
        {
            Interval = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 0.5)
        };
        _timer.Tick += OnTimerTick;
    }

    public void Trigger(object data = null)
    {
        _data = data;
        _lastEventTime = DateTime.Now;
        _timer.IsEnabled = true;
    }

    public void Disable()
    {
        _timer.IsEnabled = false;
    }
    
    private void OnTimerTick(object? sender, EventArgs e)
    {
        if ((DateTime.Now - _lastEventTime) < _delay){ return; }

        var eventArgs = new DelayEventTimerArgs(_data);
        Triggered?.Invoke(this, eventArgs);
        _timer.IsEnabled = eventArgs.RepeatEvent;
    }
}