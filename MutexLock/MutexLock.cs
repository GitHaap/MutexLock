namespace System.Threading
{
	/// <summary>
	/// ミューテックスを用いた排他ロックを行います。
	/// <para>usingステートメントを使うことで、usingを抜けた時に自動的にミューテックスが解放されます。</para>
	/// </summary>
	public class MutexLock : IDisposable
	{
		private Mutex m_mutex;

		/// <summary>ロック中かどうかを取得します</summary>
		public bool IsLocking { get; private set; }

		/// <summary>ミューテックス名を取得します</summary>
		public string MutexName { get; private set; }

		/// <summary>
		/// 指定された名前のミューテックスをロックします。ロック結果はIsLockingプロパティを参照してください。
		/// </summary>
		/// <param name="mutexName">ミューテックス名</param>
		/// <param name="millisecondsTimeout">ロック待ちのタイムアウト(ms)</param>
		public MutexLock(string mutexName, int millisecondsTimeout)
		{
			IsLocking = false;
			MutexName = mutexName;

			try
			{
				m_mutex = new Mutex(false, mutexName);
				IsLocking = m_mutex.WaitOne(millisecondsTimeout, false);

				// ロックを取得できなければ終了（mutexの解放は不要）
				if (IsLocking == false)
				{
					return;
				}
			}
			catch (AbandonedMutexException)
			{
				IsLocking = true;
			}
		}

		/// <summary>
		/// ロック状態を解除します。
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (IsLocking)
				{
					m_mutex.ReleaseMutex();
					m_mutex.Close();
				}
			}
			catch { }
		}
	}
}
