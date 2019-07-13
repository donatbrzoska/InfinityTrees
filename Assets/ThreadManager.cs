using System;
using System.Collections.Generic;
using System.Threading;

public static class ThreadManager {

	static List<Thread> threads = new List<Thread>();

    //public ThreadManager() {
    //}

    public static void Add(Thread thread) {
        lock (threads) {
            thread.IsBackground = true;
            threads.Add(thread);
        }
    }

    public static void Reset() {
        lock (threads) {
            foreach (Thread t in threads) {
                t.Abort();
            }
            threads.Clear();
        }
	}

}