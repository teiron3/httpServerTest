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
    public partial class ViewModel{
        private ICommand _inittcpserver; public ICommand inittcpserver{
            get{
                if(_inittcpserver == null){
                    _inittcpserver = new MakeCommandClass(this, new Func<ViewModel, object, Task>(MethodClass.inittcpserver));
                }
                return _inittcpserver;
            }
        }         private ICommand _sub1; public ICommand sub1{
            get{
                if(_sub1 == null){
                    _sub1 = new MakeCommandClass(this, new Func<ViewModel, object, Task>(MethodClass.sub1));
                }
                return _sub1;
            }
        } 	}
}
