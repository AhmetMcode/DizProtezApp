using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ServoViewModel : INotifyPropertyChanged
{
    private double _servo1ManualSpeed;

    private double _servo1Acc;
    private double _servo1Dcc;

    private double _servo1ForwardJogSpeed;
    private double _servo1ReverseJogSpeed;

    private double _servo1HomeFirstSpeed;
    private double _servo1HomeSecondSpeed;

    private double _servo1CurrentPosition;
    private double _servo1TargetPosition;

    private double _lc1;
    private double _kgset;
    private double _altkgset;

    public double kgset
    {
        get => _kgset;
        set
        {
            _kgset = value;
            OnPropertyChanged();
        }
    }

    public double altkgset
    {
        get => _altkgset;
        set
        {
            _altkgset = value;
            OnPropertyChanged();
        }
    }

    public double Servo1Acc
    {
        get => _servo1Acc;
        set
        {
            _servo1Acc = value;
            OnPropertyChanged();
        }
    }

    public double Servo1Dcc
    {
        get => _servo1Dcc;
        set
        {
            _servo1Dcc = value;
            OnPropertyChanged();
        }
    }


    public double Servo1CurrentPosition
    {
        get => _servo1CurrentPosition;
        set
        {
            _servo1CurrentPosition = value;
            OnPropertyChanged();
        }
    }

    public double lc1
    {
        get => _lc1;
        set
        {
            _lc1 = value;
            OnPropertyChanged();
        }
    }

    public double Servo1ManualSpeed
    {
        get => _servo1ManualSpeed;
        set
        {
            _servo1ManualSpeed = value;
            OnPropertyChanged();
        }
    }

    public double Servo1ForwardJogSpeed
    {
        get => _servo1ForwardJogSpeed;
        set
        {
            _servo1ForwardJogSpeed = value;
            OnPropertyChanged();
        }
    }

    public double Servo1ReverseJogSpeed
    {
        get => _servo1ReverseJogSpeed;
        set
        {
            _servo1ReverseJogSpeed = value;
            OnPropertyChanged();
        }
    }

    public double Servo1HomeFirstSpeed
    {
        get => _servo1HomeFirstSpeed;
        set
        {
            _servo1HomeFirstSpeed = value;
            OnPropertyChanged();
        }
    }

    public double Servo1HomeSecondSpeed
    {
        get => _servo1HomeSecondSpeed;
        set
        {
            _servo1HomeSecondSpeed = value;
            OnPropertyChanged();
        }
    }

    public double Servo1TargetPosition
    {
        get => _servo1TargetPosition;
        set
        {
            _servo1TargetPosition = value;
            OnPropertyChanged();
        }
    }



    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
