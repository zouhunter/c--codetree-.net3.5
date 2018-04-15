//
// SeekableStreamReader.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using ICSharpCode.NRefactory.Editor;
using System.IO;
using System.Text;

namespace ICSharpCode.NRefactory.MonoCSharp
{
    /// <summary>
    ///   This is an arbitrarily seekable StreamReader wrapper.
    ///
    ///   It uses a self-tuning buffer to cache the seekable data,
    ///   but if the seek is too far, it may read the underly
    ///   stream all over from the beginning.
    /// </summary>
    public partial class SeekableStreamReader : IDisposable
    {
        public const int DefaultReadAheadSize =
            4096 / 2;

        StreamReader reader;
        Stream stream;
        readonly ITextSource textSource;
        char[] buffer;
        int read_ahead_length;  // the length of read buffer
        int buffer_start;       // in chars
        int char_count;         // count of filled characters in buffer[]
        int pos;                // index into buffer[]

        public SeekableStreamReader(Stream stream, Encoding encoding , char[] sharedBuffer = null) : this(new StringTextSource(GetAllText(stream, encoding)))
        {
            this.stream = stream;
            this.buffer = sharedBuffer;

            InitializeStream(DefaultReadAheadSize);
            reader = new StreamReader(stream, encoding, true);
        }
        static string GetAllText(Stream stream, Encoding encoding)
        {
            using (var rdr = new StreamReader(stream, encoding))
            {
                return rdr.ReadToEnd();
            }
        }

        public SeekableStreamReader(ITextSource source)
        {
            this.textSource = source;
        }

        public void Dispose()
        {
            // Needed to release stream reader buffers
            reader.Dispose();
        }

        void InitializeStream(int read_length_inc)
        {
            read_ahead_length += read_length_inc;

            int required_buffer_size = read_ahead_length * 2;

            if (buffer == null || buffer.Length < required_buffer_size)
                buffer = new char[required_buffer_size];

            stream.Position = 0;
            buffer_start = char_count = pos = 0;
        }

        /// <remarks>
        ///   This value corresponds to the current position in a stream of characters.
        ///   The StreamReader hides its manipulation of the underlying byte stream and all
        ///   character set/decoding issues.  Thus, we cannot use this position to guess at
        ///   the corresponding position in the underlying byte stream even though there is
        ///   a correlation between them.
        /// </remarks>
        public int Position
        {
            get
            {
                return buffer_start + pos;
            }

            set
            {
                //
                // If the lookahead was too small, re-read from the beginning. Increase the buffer size while we're at it
                // This should never happen until we are parsing some weird source code
                //
                if (value < buffer_start)
                {
                    InitializeStream(read_ahead_length);

                    //
                    // Discard buffer data after underlying stream changed position
                    // Cannot use handy reader.DiscardBufferedData () because it for
                    // some strange reason resets encoding as well
                    //
                    reader = new StreamReader(stream, reader.CurrentEncoding, true);
                }

                while (value > buffer_start + char_count)
                {
                    pos = char_count;
                    if (!ReadBuffer())
                        throw new InternalErrorException("Seek beyond end of file: " + (buffer_start + char_count - value));
                }

                pos = value - buffer_start;
            }
        }

        bool ReadBuffer()
        {
            int slack = buffer.Length - char_count;

            //
            // read_ahead_length is only half of the buffer to deal with
            // reads ahead and moves back without re-reading whole buffer
            //
            if (slack <= read_ahead_length)
            {
                //
                // shift the buffer to make room for read_ahead_length number of characters
                //
                int shift = read_ahead_length - slack;
                Array.Copy(buffer, shift, buffer, 0, char_count - shift);

                // Update all counters
                pos -= shift;
                char_count -= shift;
                buffer_start += shift;
                slack += shift;
            }

            char_count += reader.Read(buffer, char_count, slack);

            return pos < char_count;
        }

        public char GetChar(int position)
        {
            if (buffer_start <= position && position < buffer.Length)
                return buffer[position];
            return '\0';
        }

        public char[] ReadChars(int fromPosition, int toPosition)
        {
            char[] chars = new char[toPosition - fromPosition];
            if (buffer_start <= fromPosition && toPosition <= buffer_start + buffer.Length)
            {
                Array.Copy(buffer, fromPosition - buffer_start, chars, 0, chars.Length);
            }
            else
            {
                throw new NotImplementedException();
            }

            return chars;
        }

        public int Peek()
        {
            if ((pos >= char_count) && !ReadBuffer())
                return -1;

            return buffer[pos];
        }

        public int Read()
        {
            if ((pos >= char_count) && !ReadBuffer())
                return -1;

            return buffer[pos++];
        }
    }
 //   public partial class SeekableStreamReader : IDisposable
	//{
	//	public const int DefaultReadAheadSize = 2048;

	//	readonly ITextSource textSource;

	//	int pos;

	//	static string GetAllText(Stream stream, Encoding encoding) {
	//		using (var rdr = new StreamReader(stream, encoding)) {
	//			return rdr.ReadToEnd();
	//		}
	//	}

	//	public SeekableStreamReader (Stream stream, Encoding encoding, char[] sharedBuffer = null) : this(new StringTextSource(GetAllText(stream, encoding)))
	//	{
	//	}

	//	public SeekableStreamReader (ITextSource source)
	//	{
	//		this.textSource = source;
	//	}


	//	public void Dispose ()
	//	{
	//	}
		
	//	/// <remarks>
	//	///   This value corresponds to the current position in a stream of characters.
	//	///   The StreamReader hides its manipulation of the underlying byte stream and all
	//	///   character set/decoding issues.  Thus, we cannot use this position to guess at
	//	///   the corresponding position in the underlying byte stream even though there is
	//	///   a correlation between them.
	//	/// </remarks>
	//	public int Position {
	//		get {
	//			return pos;
	//		}
			
	//		set {
	//			pos = value;
	//		}
	//	}

	//	public char GetChar (int position)
	//	{
	//		return textSource.GetCharAt (position);
	//	}
		
	//	public char[] ReadChars (int fromPosition, int toPosition)
	//	{
	//		return textSource.GetText (fromPosition, toPosition - fromPosition).ToCharArray ();
	//	}
		
	//	public int Peek ()
	//	{
	//		if (pos >= textSource.TextLength)
	//			return -1;
	//		return textSource.GetCharAt (pos);
	//	}
		
	//	public int Read ()
	//	{
	//		if (pos >= textSource.TextLength)
	//			return -1;
	//		return textSource.GetCharAt (pos++);
	//	}
	//}
}

