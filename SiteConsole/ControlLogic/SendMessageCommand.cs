using System;
using System.Windows.Input;

namespace Chat.Client.ControlLogic
{
    public class SendMessageCommand : ICommand
    {
        private readonly Action _commandHandler;
        private readonly Func<bool> _canExecuteHandler;

        public SendMessageCommand(Action commandHandler, Func<bool> canExecuteHandler)
        {
            _commandHandler = commandHandler;
            _canExecuteHandler = canExecuteHandler;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteHandler();
        }

        public void Execute(object parameter)
        {
            _commandHandler();
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (null != handler)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}
