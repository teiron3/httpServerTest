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
public partial class MainWindow : Window
{
public MainWindow(){
InitializeComponent();

		this.DataContext = (vm = new ViewModel());
	
}

		private ViewModel vm;
	
}

}
