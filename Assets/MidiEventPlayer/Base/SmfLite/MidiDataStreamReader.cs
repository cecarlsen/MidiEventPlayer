//
// SmfLite.cs - A minimal toolkit for reading standard MIDI files (SMF) in Unity
//
// Original work: Copyright (C) 2013 Keijiro Takahashi
//		https://github.com/keijiro/smflite
//
// Modified work: Copyright (C) 2017 Carl Emil Carlsen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using System.Collections.Generic;

namespace SmfLite
{
	//
	// Binary data stream reader (for internal use)
	//
	class MidiDataStreamReader
	{
		byte[] _data;
		int _offset;

		public int offset {
			get { return _offset; }
		}

		public MidiDataStreamReader( byte[] data )
		{
			_data = data;
		}

		public void Advance( int length )
		{
			_offset += length;
		}

		public byte PeekByte ()
		{
			return _data[_offset];
		}

		public byte ReadByte ()
		{
			return _data[_offset++];
		}

		public char[] ReadChars( int length )
		{
			var temp = new char[length];
			for( int i = 0; i < length; i++ ){
				temp [i] = (char)ReadByte ();
			}
			return temp;
		}

		public int ReadBEInt32()
		{
			int b1 = ReadByte();
			int b2 = ReadByte();
			int b3 = ReadByte();
			int b4 = ReadByte();
			return b4 + (b3 << 8) + (b2 << 16) + (b1 << 24);
		}

		public int ReadBEInt16()
		{
			int b1 = ReadByte();
			int b2 = ReadByte();
			return b2 + (b1 << 8);
		}

		public int ReadMultiByteValue()
		{
			int value = 0;
			while( true ){
				int b = ReadByte();
				value += b & 0x7f;
				if( b < 0x80 ) break;
				value <<= 7;
			}
			return value;
		}
	}
}