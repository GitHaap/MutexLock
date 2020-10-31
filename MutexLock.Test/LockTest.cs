using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
	public class LockTest
	{
		[Fact]
		public void 単一スレッドからのミューテックス取得()
		{
			using (var mutex = new MutexLock("test1", 1000))
			{
				Assert.True(mutex.IsLocking);
				Assert.Equal("test1", mutex.MutexName);
			}
		}

		[Fact]
		public void マルチスレッドからのミューテックス取得()
		{
			int count = 0;

			// 裏で先にミューテックスを取得させておく
			Task.Run(() =>
			{
				using (var mutex = new MutexLock("test2", 2000))
				{
					Assert.True(mutex.IsLocking);

					count++;
					Assert.Equal(1, count);

					Thread.Sleep(1000);
				}
			});

			Thread.Sleep(10);

			using (var mutex = new MutexLock("test2", 2000))
			{
				Assert.True(mutex.IsLocking);

				count++;
				Assert.Equal(2, count);
			}
		}

		[Fact]
		public void マルチスレッドからのミューテックス取得_無限待機()
		{
			int count = 0;

			// 裏で先にミューテックスを取得させておく
			Task.Run(() =>
			{
				using (var mutex = new MutexLock("test2_inifini", 2000))
				{
					Assert.True(mutex.IsLocking);

					count++;
					Assert.Equal(1, count);

					Thread.Sleep(5000);
				}
			});

			Thread.Sleep(10);

			using (var mutex = new MutexLock("test2_inifini", -1))
			{
				Assert.True(mutex.IsLocking);

				count++;
				Assert.Equal(2, count);
			}
		}

		[Fact]
		public void ミューテックス待ちのタイムアウト()
		{
			// 裏で先にミューテックスを取得させておく
			Task.Run(() =>
			{
				using (var mutex = new MutexLock("test3", 1000))
				{
					Assert.True(mutex.IsLocking);

					Thread.Sleep(2000);
				}
			});

			Thread.Sleep(10);

			using (var mutex = new MutexLock("test3", 1000))
			{
				Assert.False(mutex.IsLocking);
			}
		}

		[Fact]
		public void ミューテックス取得時の引数間違いによるロック失敗()
		{
			// 裏で先にミューテックスを取得させておく
			Task.Run(() =>
			{
				// 引数指定ミスにより失敗させる
				using (var mutex = new MutexLock("test4", -7777))
				{
					Assert.False(mutex.IsLocking);

					Thread.Sleep(2000);
				}
			});

			Thread.Sleep(10);

			// ミューテックス取得可能
			using (var mutex = new MutexLock("test4", 1000))
			{
				Assert.True(mutex.IsLocking);
			}
		}
	}
}
