using System;
using System.IO;
using System.Threading;

namespace TayTail {
    class Tail {
        private string title = @"
  _______       _______    _ _ 
 |__   __|     |__   __|  (_) |
    | | __ _ _   _| | __ _ _| |
    | |/ _` | | | | |/ _` | | |
    | | (_| | |_| | | (_| | | |
    |_|\__,_|\__, |_|\__,_|_|_|
              __/ |            
             |___/       ";
        private string path;
        private AutoResetEvent autoReset;
        private FileSystemWatcher watcher;
        private FileStream fs;

        public static void Main(string[] args) {
            new Tail().InitLog();
            Console.Write("See you around!");
            Console.ReadKey();
        }

        private void InitLog() {
            Console.SetCursorPosition((Console.WindowWidth - 32) / 2, Console.CursorTop);
            Console.WriteLine(title);
            Console.Write("Please, introduce the logfile path: ");
            path = Console.ReadLine();

            Console.Clear();

            autoReset = new AutoResetEvent(false);

            if (!File.Exists(path)) {
                Console.WriteLine("The specified file does not exists");
                return;
            }

            watcher = new FileSystemWatcher() {
                Path = new FileInfo(path).DirectoryName,
                EnableRaisingEvents = true
            };

            watcher.Changed += (s, e) => autoReset.Set();

            fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            try {
                using (StreamReader sr = new StreamReader(fs)) {
                    string s = "";
                    while ((!Console.KeyAvailable || (Console.ReadKey(true)).Key != ConsoleKey.Q)) {
                        s = sr.ReadLine();
                        if (s != null)
                            Console.WriteLine(s);
                        else
                            autoReset.WaitOne(1000);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
            } finally {
                fs.Dispose();
                autoReset.Dispose();
                watcher.Dispose();
            }
        }
    }
}
