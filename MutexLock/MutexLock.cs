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
		/// <param name="millisecondsTimeout">ロック待ちのタイムアウト(ms) 無限に待機する場合はInfinite(-1)を指定</param>
		public MutexLock(string mutexName, int millisecondsTimeout)
		{
			IsLocking = false;
			MutexName = mutexName;

			try
			{
				m_mutex = new Mutex(false, mutexName);
				IsLocking = m_mutex.WaitOne(millisecondsTimeout, false);
			}
			catch (AbandonedMutexException)
			{
				// 前取得者がmutexを取得したまま落ちた場合に例外が出てしまうので抑止する
				// mutex自体は取得できるのでこのままIsLockingとする
				IsLocking = true;
			}
			catch
			{
				// それ以外の例外はミューテックス取得はできない
				IsLocking = false;
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
					IsLocking = false;
				}
			}
			catch { }
		}
	}
}
