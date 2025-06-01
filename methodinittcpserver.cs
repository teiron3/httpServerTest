using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace tcptest
{
	public static partial class MethodClass
	{
		public static async Task inittcpserver(ViewModel vm, object parameter)
		{
			if (vm.tokensource != null)
			{
				vm.testmessage += "cancel exec->";
				await Task.Delay(500);
				vm.tokensource.Cancel();
			}
		}

		public static async Task sub1(ViewModel vm, object parameter)
		{
			if (vm.tokensource != null)
			{
				return;
			}
			vm.tokensource = new System.Threading.CancellationTokenSource();

			vm.testmessage = "sub1 =>";
			await sub2(vm, vm.tokensource.Token);

			vm.testmessage += "sub1 end";
			vm.tokensource.Dispose();
			vm.tokensource = null;
		}

		private static async Task sub2(ViewModel vm, CancellationToken token)
		{
			var _listener = new TcpListener(IPAddress.IPv6Any, 80);
			_listener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, true); // IPv6専用
			vm.teststatus = "";

			_listener.Start();

			vm.testmessage += "recv start->";
			while (!token.IsCancellationRequested)
			{
				try
				{
					Task<TcpClient> taskGetClient = _listener.AcceptTcpClientAsync();
					var anyTasks = await Task.WhenAny(taskGetClient, Task.Delay(-1, token));
					if (anyTasks == taskGetClient)
					{
						vm.testmessage += "client connected->";
						var client = taskGetClient.Result;
						await sub3(vm, client, token);
						client.Close();
					}
					else
					{
						vm.testmessage += "task cancelled->";
						_listener.Stop();
					}
					if (token.IsCancellationRequested) break;
				}
				catch (OperationCanceledException)
				{
					vm.testmessage += "cancelled ->";
					return;
				}
			}
			_listener.Stop();
			return;
		}

		private static async Task sub3(ViewModel vm, TcpClient client, CancellationToken token)
		{
			try
			{
				//現在時刻を取得し、vm.teststatusにyyyy/MM/dd HH:mm:ssで追記する
				string datetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ";

				vm.teststatus += datetime + "\n" + client.Client.RemoteEndPoint.ToString() + "\n";
				// remoteEndPoint.ToString()のUTF-8文字列を./log/accesslog.txtに追記する
				// ./log/accesslog.txtが存在しない場合は作成する
				if (!Directory.Exists(".\\log"))
				{
					MessageBox.Show("logフォルダが存在しません。作成します。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
					Directory.CreateDirectory(".\\log");
				}
				if (!File.Exists(".\\log\\accesslog.txt"))
				{
					File.Create(".\\log\\accesslog.txt").Close();
				}
				using (StreamWriter sw = new StreamWriter(".\\log\\accesslog.txt", true, Encoding.UTF8))
				{
					await sw.WriteLineAsync(datetime);
					await sw.WriteLineAsync(client.Client.RemoteEndPoint.ToString());
				}
				var stream = client.GetStream();
				int bytesRead;
				var buffer = new byte[1024];
				//1秒間リクエストデータが来なかった場合キャンセルする
				//キャンセル方法は新しいトークンを作成し、それをReadAsyncに渡すことで実現する
				//
				Task<int> readTask = stream.ReadAsync(buffer, 0, buffer.Length);
				var anyTasks = await Task.WhenAny(readTask, Task.Delay(3000));
				if (anyTasks == readTask)
				{
					bytesRead = readTask.Result;
				}
				else
				{
					throw new Exception("Read operation timed out.");
				}



				if (bytesRead > 0)
				{
					string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
					vm.teststatus += message + "\n";
					using (StreamWriter sw = new StreamWriter(".\\log\\accesslog.txt", true, Encoding.UTF8))
					{
						await sw.WriteLineAsync(message + "\n");
					}
				}
				else
				{
					throw new Exception("Client disconnected");
				}
				// responseDataBytesはHTMLのレスポンスデータ
				// responseHeaderBytesはHTTPヘッダー
				// responseDataBytesは ./response.html の内容を読み込む
				byte[] responseDataBytes = File.ReadAllBytes("./response.html");
				//byte[] responseDataBytes = Encoding.UTF8.GetBytes("<html><body><h1>Hello from TCP Server</h1></body></html>");
				byte[] responseHeaderBytes = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=UTF-8\r\nContent-Length: " + responseDataBytes.Length + "\r\n\r\n");

				var allBytes = new byte[responseHeaderBytes.Length + responseDataBytes.Length];
				Buffer.BlockCopy(responseHeaderBytes, 0, allBytes, 0, responseHeaderBytes.Length);
				Buffer.BlockCopy(responseDataBytes, 0, allBytes, responseHeaderBytes.Length, responseDataBytes.Length);


				await stream.WriteAsync(allBytes, 0, allBytes.Length, token);
			}
			catch (OperationCanceledException)
			{
				vm.teststatus += "Operation cancelled \n";
				client.Close();
			}
			catch (Exception ex)
			{
				vm.teststatus += "Error: " + ex.Message + "\n";
				client.Close();
			}
			finally
			{
			}
			return;
		}

	}
}

