using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
	public class LockTest
	{
		[Fact]
		public void �P��X���b�h����̃~���[�e�b�N�X�擾()
		{
			using (var mutex = new MutexLock("test1", 1000))
			{
				Assert.True(mutex.IsLocking);
				Assert.Equal("test1", mutex.MutexName);
			}
		}

		[Fact]
		public void �}���`�X���b�h����̃~���[�e�b�N�X�擾()
		{
			int count = 0;

			// ���Ő�Ƀ~���[�e�b�N�X���擾�����Ă���
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
		public void �~���[�e�b�N�X�҂��̃^�C���A�E�g()
		{
			// ���Ő�Ƀ~���[�e�b�N�X���擾�����Ă���
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
	}
}
