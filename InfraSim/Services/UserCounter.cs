using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserCounter : ISubject
{
    public int Counter { get; private set; }
    public event Action OnCounterChanged;
    private CancellationTokenSource _cts;
    private bool _isIncrementing;
    private bool Canceled;
    private List<IObserver> observers = new List<IObserver>();

    public void RegisterObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.Update(Counter);
        }
    }

    public async Task StartIncrementingAsync()
    {
        if (_isIncrementing)
            return;

        _isIncrementing = true;
        _cts = new CancellationTokenSource();
        Canceled = false;
        
        try
        {
            for (int i = 0; i < 200000 && !_cts.Token.IsCancellationRequested; i += 5000)
            {
                if (Canceled)
                    break;
                    
                Counter += 5000;
                OnCounterChanged?.Invoke();
                NotifyObservers();
                await Task.Delay(10, _cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Task was canceled - this is expected during reset
        }
        finally
        {
            _isIncrementing = false;
        }
    }
    
    public void Cancel()
    {
        Canceled = true;
    }
}
