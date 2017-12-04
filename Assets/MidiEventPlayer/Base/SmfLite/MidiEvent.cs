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
	/// <summary>
	/// Midi Event.
	/// </summary>
	public struct MidiEvent
	{
		public byte status;
		public byte data1;
		public byte data2;

		public enum Type { NoteOff, NoteOn, Aftertouch, ControlChange, ProgramChange, ChannelPressure, PitchBend, Unknown }


		public MidiEvent( byte status, byte data1, byte data2 )
		{
			this.status = status;
			this.data1 = data1;
			this.data2 = data2;
		}


		public void UnpackStatus( out Type type, out int channelIndex )
		{
			// Info: 	https://ccrma.stanford.edu/~craig/articles/linuxmidi/misc/essenmidi.html
			//			http://www.skytopia.com/project/articles/midi.html

			// Get message type from higher four bits.
			byte typeByte = (byte) ( status & 0xF0 );

			// Get channel from lower four bits.
			channelIndex = status & 0x0F;

			type = Type.Unknown;
			switch( typeByte ){
			case 0x80: type = Type.NoteOff; break;
			case 0x90: type = Type.NoteOn; break;
			case 0xA0: type = Type.Aftertouch; break;
			case 0xB0: type = Type.ControlChange; break;
			case 0xC0: type = Type.ProgramChange; break;
			case 0xD0: type = Type.ChannelPressure; break;
			case 0xE0: type = Type.PitchBend; break;
			}
		}

		public override string ToString()
		{
			return "[" + status.ToString ("X") + "," + data1 + "," + data2 + "]";
		}
	}
}