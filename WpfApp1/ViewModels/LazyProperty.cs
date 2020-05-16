using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class LazyProperty<T> : INotifyPropertyChanged
    {
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private readonly T _defaultValue;
        private readonly Func<CancellationToken, Task<T>> _retrievalFunc;

        private bool _isLoading;
        private bool _errorOnLoading;
        private T _value;

        private bool IsLoaded { get; set; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;

                _isLoading = value;

                OnPropertyChanged();
            }
        }

        public bool ErrorOnLoading
        {
            get => _errorOnLoading;
            set
            {
                if (_errorOnLoading == value) return;

                _errorOnLoading = value;

                OnPropertyChanged();
            }
        }

        public T Value
        {
            get
            {
                if (IsLoaded)
                    return _value;

                if (_isLoading) return _defaultValue;

                IsLoading = true;

                LoadValueAsync()
                    .ContinueWith((t) =>
                    {
                        if (t.IsCanceled) return;

                        if (t.IsFaulted)
                        {
                            _value = _defaultValue;
                            ErrorOnLoading = true;
                            IsLoaded = true;
                            IsLoading = false;

                            OnPropertyChanged(nameof(Value));
                        }
                        else
                        {
                            Value = t.Result;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                return _defaultValue;
            }
            set
            {
                if (_isLoading)
                    CancelLoading();

                if (EqualityComparer<T>.Default.Equals(_value, value)) return;

                _value = value;
                IsLoaded = true;
                IsLoading = false;
                ErrorOnLoading = false;

                OnPropertyChanged();
            }
        }

        private async Task<T> LoadValueAsync() => await _retrievalFunc(_cancelTokenSource.Token);

        public void CancelLoading() => _cancelTokenSource.Cancel();

        public LazyProperty(Func<CancellationToken, Task<T>> retrievalFunc, T defaultValue)
        {
            _retrievalFunc = retrievalFunc ?? throw new ArgumentNullException(nameof(retrievalFunc));
            _defaultValue = defaultValue;

            _value = default(T);
        }

        public static implicit operator T(LazyProperty<T> p) => p.Value;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}