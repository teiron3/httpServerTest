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

 namespace tcptest
{ public partial class ViewModel{
 		private System.Threading.CancellationTokenSource _tokensource = null;
        public System.Threading.CancellationTokenSource tokensource{
            get{ return _tokensource;}
            set{
                if(_tokensource == value){
                    return;
                }else{
                    _tokensource = value;
                    NotifyPropertyChanged("tokensource");
                }
            }
        }
		private string _testmessage = "stand by";
        public string testmessage{
            get{ return _testmessage;}
            set{
                if(_testmessage == value){
                    return;
                }else{
                    _testmessage = value;
                    NotifyPropertyChanged("testmessage");
                }
            }
        }
		private string _teststatus = "";
        public string teststatus{
            get{ return _teststatus;}
            set{
                if(_teststatus == value){
                    return;
                }else{
                    _teststatus = value;
                    NotifyPropertyChanged("teststatus");
                }
            }
        }
	}
}
