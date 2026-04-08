using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Elysia
{
    public class MultiTextWriter : TextWriter
    {
        private readonly TextWriter _writer0;
        private readonly TextWriter _writer1;

        public override Encoding Encoding => throw new InvalidOperationException();
        public override IFormatProvider FormatProvider => throw new InvalidOperationException();
        public override string NewLine => throw new InvalidOperationException();

        public MultiTextWriter(TextWriter writer0, TextWriter writer1)
        {
            _writer0 = writer0;
            _writer1 = writer1;
        }

        public override void Close()
        {
            _writer0.Close();
            _writer1.Close();

            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _writer0.Dispose();
                _writer1.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void Flush()
        {
            _writer0.Flush();
            _writer1.Flush();
        }

        #region Write Sync

        public override void Write(bool value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(char value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(char[] buffer)
        {
            _writer0.Write(buffer);
            _writer1.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _writer0.Write(buffer, index, count);
            _writer1.Write(buffer, index, count);
        }

        public override void Write(ReadOnlySpan<char> buffer)
        {
            _writer0.Write(buffer);
            _writer1.Write(buffer);
        }

        public override void Write(decimal value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(double value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(int value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(long value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(object value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(float value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(string value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            _writer0.Write(format, arg0);
            _writer1.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            _writer0.Write(format, arg0, arg1);
            _writer1.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            _writer0.Write(format, arg0, arg1, arg2);
            _writer1.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            _writer0.Write(format, arg);
            _writer1.Write(format, arg);
        }

        public override void Write(uint value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        public override void Write(ulong value)
        {
            _writer0.Write(value);
            _writer1.Write(value);
        }

        #endregion

        #region WriteLine Sync

        public override void WriteLine()
        {
            _writer0.WriteLine();
            _writer1.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            _writer0.WriteLine(buffer);
            _writer1.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            _writer0.WriteLine(buffer, index, count);
            _writer1.WriteLine(buffer, index, count);
        }

        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            _writer0.WriteLine(buffer);
            _writer1.WriteLine(buffer);
        }

        public override void WriteLine(decimal value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            _writer0.WriteLine(format, arg0);
            _writer1.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            _writer0.WriteLine(format, arg0, arg1);
            _writer1.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            _writer0.WriteLine(format, arg0, arg1, arg2);
            _writer1.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            _writer0.WriteLine(format, arg);
            _writer1.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            _writer0.WriteLine(value);
            _writer1.WriteLine(value);
        }

        #endregion

        public override async ValueTask DisposeAsync()
        {
            Exception exception = null;

            try
            {
                await _writer0.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exception = e;
            }

            try
            {
                await _writer1.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exception = exception == null ? e : new AggregateException(exception, e);
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        public override Task FlushAsync()
        {
            return Task.WhenAll(_writer0.FlushAsync(), _writer1.FlushAsync());
        }

        #region Write Async

        public override Task WriteAsync(char value)
        {
            return Task.WhenAll(_writer0.WriteAsync(value), _writer1.WriteAsync(value));
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Task.WhenAll(_writer0.WriteAsync(buffer, index, count), _writer1.WriteAsync(buffer, index, count));
        }

        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.WhenAll(_writer0.WriteAsync(buffer, cancellationToken), _writer1.WriteAsync(buffer, cancellationToken));
        }

        public override Task WriteAsync(string value)
        {
            return Task.WhenAll(_writer0.WriteAsync(value), _writer1.WriteAsync(value));
        }

        #endregion

        #region WriteLine Async

        public override Task WriteLineAsync()
        {
            return Task.WhenAll(_writer0.WriteLineAsync(), _writer1.WriteLineAsync());
        }

        public override Task WriteLineAsync(char value)
        {
            return Task.WhenAll(_writer0.WriteLineAsync(value), _writer1.WriteLineAsync(value));
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Task.WhenAll(_writer0.WriteLineAsync(buffer, index, count), _writer1.WriteLineAsync(buffer, index, count));
        }

        public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.WhenAll(_writer0.WriteLineAsync(buffer, cancellationToken), _writer1.WriteLineAsync(buffer, cancellationToken));
        }

        public override Task WriteLineAsync(string value)
        {
            return Task.WhenAll(_writer0.WriteLineAsync(value), _writer1.WriteLineAsync(value));
        }

        #endregion
    }
}
