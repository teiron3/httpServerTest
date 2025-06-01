using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace tcptest{
    public partial class ViewModel : INotifyPropertyChanged{
        public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string info){
			if(PropertyChanged != null){
				PropertyChanged(this, new PropertyChangedEventArgs(info));

			}
        }
    }

    class MakeCommandClass : ICommand{
		ViewModel vm;
		Func<ViewModel, object, Task> execmd;
		public MakeCommandClass(ViewModel arg_vm, Func<ViewModel, object, Task> arg_cmd){
			vm = arg_vm;
			execmd = arg_cmd;
		}
		public event EventHandler CanExecuteChanged;
		public bool CanExecute(object parameter){return true;}
		public void Execute(object parameter){
            var task = execmd(vm, parameter);
            while(!task.IsCompleted){
            	Application.Current.Dispatcher.Invoke(
					new Action(() => {}),DispatcherPriority.Background, new object[]{}
				);
            };
        }
	}
}
